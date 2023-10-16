// Copyright © 2021-Present Neuroglia SRL. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License"),
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using EventStore.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Neuroglia.Data.Infrastructure.EventSourcing.Configuration;
using Neuroglia.Data.Infrastructure.EventSourcing.EventStore;
using Neuroglia.Data.Infrastructure.EventSourcing.Services;
using Neuroglia.Serialization;
using System.Collections.Concurrent;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using ESStreamPosition = EventStore.Client.StreamPosition;

namespace Neuroglia.Data.Infrastructure.EventSourcing;

/// <summary>
/// Represents the default <see href="https://www.eventstore.com/">Event Store</see> implementation of the <see cref="IEventStore"/> interface
/// </summary>
public class ESEventStore
    : IEventStore
{

    /// <summary>
    /// Initializes a new <see cref="ESEventStore"/>
    /// </summary>
    /// <param name="serviceProvider">The current <see cref="IServiceProvider"/></param>
    /// <param name="logger">The service used to perform logging</param>
    /// <param name="options">The options used to configure the <see cref="ESEventStore"/></param>
    /// <param name="serializerProvider">The service used to provide <see cref="ISerializer"/>s</param>
    /// <param name="eventStoreClient">The service used to interact with the remove <see href="https://www.eventstore.com/">Event Store</see> service</param>
    public ESEventStore(IServiceProvider serviceProvider, ILogger<ESEventStore> logger, IOptions<EventStoreOptions> options, ISerializerProvider serializerProvider, EventStoreClient eventStoreClient)
    {
        this.ServiceProvider = serviceProvider;
        this.Logger = logger;
        this.Options = options.Value;
        this.Serializer = serializerProvider.GetSerializers().First(s => this.Options.SerializerType == null || s.GetType() == this.Options.SerializerType);
        this.EventStoreClient = eventStoreClient;
    }

    /// <summary>
    /// Gets the current <see cref="IServiceProvider"/>
    /// </summary>
    protected virtual IServiceProvider ServiceProvider { get; }

    /// <summary>
    /// Gets the service used to perform logging
    /// </summary>
    protected virtual ILogger Logger { get; }

    /// <summary>
    /// Gets the options used to configure the <see cref="ESEventStore"/>
    /// </summary>
    protected virtual EventStoreOptions Options { get; }

    /// <summary>
    /// Gets the service used to interact with the remove <see href="https://www.eventstore.com/">Event Store</see> service
    /// </summary>
    protected virtual EventStoreClient EventStoreClient { get; }

    /// <summary>
    /// Gets the service used to serialize and deserialize <see cref="IEventRecord"/>s
    /// </summary>
    protected virtual ISerializer Serializer { get; }

    /// <summary>
    /// Gets a <see cref="ConcurrentDictionary{TKey, TValue}"/> containing all active <see cref="EventStoreSubscription"/>s
    /// </summary>
    protected virtual ConcurrentDictionary<string, EventStoreSubscription> Subscriptions { get; } = new();

    /// <inheritdoc/>
    public virtual async Task<bool> StreamExistsAsync(string streamId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(streamId)) throw new ArgumentNullException(nameof(streamId));
        return (await this.GetAsync(streamId, cancellationToken).ConfigureAwait(false)) != null;
    }

    /// <inheritdoc/>
    public virtual async Task<IEventStreamDescriptor> GetAsync(string streamId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(streamId)) throw new ArgumentNullException(nameof(streamId));

        var streamMetadataResult = await this.EventStoreClient.GetStreamMetadataAsync(streamId, cancellationToken: cancellationToken).ConfigureAwait(false);
        if (streamMetadataResult.StreamDeleted) throw new StreamNotFoundException(streamId);
        var offset = streamMetadataResult.Metadata.TruncateBefore ?? StreamPosition.StartOfStream;

        var readResult = this.EventStoreClient.ReadStreamAsync(Direction.Forwards, streamId, offset, 1, cancellationToken: cancellationToken);
        ReadState? readState;

        try { readState = await readResult.ReadState.ConfigureAwait(false); }
        catch { throw new StreamNotFoundException(streamId); }
        if (readState == ReadState.StreamNotFound) throw new StreamNotFoundException(streamId);

        var firstEvent = await readResult.FirstAsync(cancellationToken).ConfigureAwait(false);
        readResult = this.EventStoreClient.ReadStreamAsync(Direction.Backwards, streamId, ESStreamPosition.End, 1, cancellationToken: cancellationToken);
        var lastEvent = await readResult.FirstAsync(cancellationToken).ConfigureAwait(false);

        return new EventStreamDescriptor(streamId, lastEvent.Event.EventNumber.ToInt64() + 1 - offset.ToInt64(), firstEvent.Event.Created, lastEvent.Event.Created);
    }

    /// <inheritdoc/>
    public virtual async Task AppendAsync(string streamId, IEnumerable<IEventDescriptor> events, long? expectedVersion = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(streamId)) throw new ArgumentNullException(nameof(streamId));
        if (events == null || !events.Any()) throw new ArgumentNullException(nameof(events));
        
        var readResult = this.EventStoreClient.ReadStreamAsync(Direction.Backwards, streamId, ESStreamPosition.End, 1, cancellationToken: cancellationToken);
        var shouldThrowIfNotExists = expectedVersion.HasValue && expectedVersion != StreamPosition.StartOfStream && expectedVersion != StreamPosition.EndOfStream;
        try { if (await readResult.ReadState.ConfigureAwait(false) == ReadState.StreamNotFound && shouldThrowIfNotExists) throw new OptimisticConcurrencyException(expectedVersion, null); }
        catch (StreamDeletedException) { if(shouldThrowIfNotExists) throw new OptimisticConcurrencyException(expectedVersion, null); }

        var eventsData = events.Select(e => 
        {
            var metadata = e.Metadata ?? new Dictionary<string, object>();
            metadata[EventRecordMetadata.ClrTypeName] = e.Data?.GetType().AssemblyQualifiedName!;
            return new EventData(Uuid.NewUuid(), e.Type, this.Serializer.SerializeToByteArray(e.Data), this.Serializer.SerializeToByteArray(metadata));
        });
        if (expectedVersion.HasValue) await this.EventStoreClient.AppendToStreamAsync(streamId, StreamRevision.FromInt64(expectedVersion.Value), eventsData, cancellationToken: cancellationToken).ConfigureAwait(false);
        else await this.EventStoreClient.AppendToStreamAsync(streamId, StreamState.Any, eventsData, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public virtual async IAsyncEnumerable<IEventRecord> ReadAsync(string streamId, StreamReadDirection readDirection, long offset, ulong? length = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var direction = readDirection switch
        {
            StreamReadDirection.Backwards => Direction.Backwards,
            StreamReadDirection.Forwards => Direction.Forwards,
            _ => throw new NotSupportedException($"The specified {nameof(StreamReadDirection)} '{readDirection}' is not supported")
        };

        var streamMetadataResult = await this.EventStoreClient.GetStreamMetadataAsync(streamId, cancellationToken: cancellationToken).ConfigureAwait(false);
        if (streamMetadataResult.StreamDeleted) throw new StreamNotFoundException(streamId);
        if (streamMetadataResult.Metadata.TruncateBefore.HasValue && offset != StreamPosition.EndOfStream && offset < streamMetadataResult.Metadata.TruncateBefore.Value.ToInt64()) offset = streamMetadataResult.Metadata.TruncateBefore.Value.ToInt64();

        if (readDirection == StreamReadDirection.Forwards && offset == StreamPosition.EndOfStream) yield break;
        else if(readDirection == StreamReadDirection.Backwards && offset == StreamPosition.StartOfStream) yield break;

        var readResult = this.EventStoreClient.ReadStreamAsync(direction, streamId, ESStreamPosition.FromInt64(offset), length.HasValue ? (long)length.Value : long.MaxValue, cancellationToken: cancellationToken);
        try { if (await readResult.ReadState.ConfigureAwait(false) == ReadState.StreamNotFound) throw new StreamNotFoundException(streamId); }
        catch (StreamDeletedException) { throw new StreamNotFoundException(streamId); }

        await foreach (var e in readResult) yield return this.DeserializeEventRecord(e);
    }

    /// <inheritdoc/>
    public virtual async Task<IObservable<IEventRecord>> SubscribeAsync(string streamId, long offset = StreamPosition.EndOfStream, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(streamId)) throw new ArgumentNullException(nameof(streamId));
        if (!await this.StreamExistsAsync(streamId, cancellationToken).ConfigureAwait(false)) throw new StreamNotFoundException(streamId);

        var position = offset == StreamPosition.EndOfStream ? FromStream.End : FromStream.After(ESStreamPosition.FromInt64(offset));
        var records = new List<IEventRecord>();
        if (position != FromStream.End) records = await this.ReadAsync(streamId, StreamReadDirection.Forwards, offset, cancellationToken: cancellationToken).ToListAsync(cancellationToken).ConfigureAwait(false);
        var subject = new Subject<IEventRecord>();
        var subscription = await this.EventStoreClient.SubscribeToStreamAsync(streamId, FromStream.End, (sub, e, cancellation) => Task.Run(() => subject.OnNext(this.DeserializeEventRecord(e)), cancellation), cancellationToken: cancellationToken).ConfigureAwait(false);
        return Observable.StartWith(Observable.Using
        (
            () => subscription,
            watch => subject
        ), records);
    }

    /// <inheritdoc/>
    public virtual async Task TruncateAsync(string streamId, ulong? beforeVersion = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(streamId)) throw new ArgumentNullException(nameof(streamId));
        if (!await this.StreamExistsAsync(streamId, cancellationToken).ConfigureAwait(false)) throw new StreamNotFoundException(streamId);

        var truncateBefore = beforeVersion.HasValue ? ESStreamPosition.FromInt64((long)beforeVersion.Value) : ESStreamPosition.End;
        await this.EventStoreClient.SetStreamMetadataAsync(streamId, StreamState.Any, new StreamMetadata(truncateBefore: truncateBefore), cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public virtual async Task DeleteAsync(string streamId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(streamId)) throw new ArgumentNullException(nameof(streamId));
        if (!await this.StreamExistsAsync(streamId, cancellationToken).ConfigureAwait(false)) throw new StreamNotFoundException(streamId);

        await this.EventStoreClient.DeleteAsync(streamId, StreamState.Any, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Deserializes the specified <see cref="ResolvedEvent"/> into a new <see cref="IEventRecord"/>
    /// </summary>
    /// <param name="e">The <see cref="ResolvedEvent"/> to deserialize</param>
    /// <returns>The deserialized <see cref="IEventRecord"/></returns>
    protected virtual IEventRecord DeserializeEventRecord(ResolvedEvent e)
    {
        var metadata = this.Serializer.Deserialize<IDictionary<string, object>>(e.Event.Metadata.ToArray());
        var clrTypeName = metadata![EventRecordMetadata.ClrTypeName].ToString()!;
        var clrType = Type.GetType(clrTypeName) ?? throw new Exception();
        var data = this.Serializer.Deserialize(e.Event.Data.ToArray(), clrType);
        metadata.Remove(EventRecordMetadata.ClrTypeName);
        if (!metadata.Any()) metadata = null;
        return new EventRecord(e.Event.EventId.ToString(), e.Event.EventNumber.ToUInt64(), e.Event.Created, e.Event.EventType, data, metadata);
    }

    /// <summary>
    /// Exposes constants about event related metadata used by the <see cref="ESEventStore"/>
    /// </summary>
    protected static class EventRecordMetadata
    {

        /// <summary>
        /// Gets the name of the event record metadata used to store the event CLR type's assembly qualified name
        /// </summary>
        public const string ClrTypeName = "clr-type";

    }
}
