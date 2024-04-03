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
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Neuroglia.Data.Infrastructure.EventSourcing.Configuration;
using Neuroglia.Data.Infrastructure.EventSourcing.EventStore.Services;
using Neuroglia.Data.Infrastructure.EventSourcing.Services;
using Neuroglia.Plugins;
using Neuroglia.Serialization;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using ESStreamPosition = EventStore.Client.StreamPosition;

namespace Neuroglia.Data.Infrastructure.EventSourcing;

/// <summary>
/// Represents the default <see href="https://www.eventstore.com/">Event Store</see> implementation of the <see cref="IEventStore"/> interface
/// </summary>
[Plugin(Tags = ["event-store"]), Factory(typeof(ESEventStoreFactory))]
public class ESEventStore
    : IEventStore
{

    /// <summary>
    /// Initializes a new <see cref="ESEventStore"/>
    /// </summary>
    /// <param name="logger">The service used to perform logging</param>
    /// <param name="options">The options used to configure the <see cref="ESEventStore"/></param>
    /// <param name="serializerProvider">The service used to provide <see cref="ISerializer"/>s</param>
    /// <param name="eventStoreClient">The service used to interact with the remove <see href="https://www.eventstore.com/">Event Store</see> service</param>
    /// <param name="eventStorePersistentSubscriptionsClient">The service used to interact with the remove <see href="https://www.eventstore.com/">Event Store</see> service, exclusively for persistent subscriptions</param>
    public ESEventStore(ILogger<ESEventStore> logger, IOptions<EventStoreOptions> options, ISerializerProvider serializerProvider, EventStoreClient eventStoreClient, EventStorePersistentSubscriptionsClient eventStorePersistentSubscriptionsClient)
    {
        this.Logger = logger;
        this.Options = options.Value;
        this.Serializer = serializerProvider.GetSerializers().First(s => this.Options.SerializerType == null || s.GetType() == this.Options.SerializerType);
        this.EventStoreClient = eventStoreClient;
        this.EventStorePersistentSubscriptionsClient = eventStorePersistentSubscriptionsClient;
    }

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
    /// Gets the service used to interact with the remove <see href="https://www.eventstore.com/">Event Store</see> service, exclusively for persistent subscriptions
    /// </summary>
    protected virtual EventStorePersistentSubscriptionsClient EventStorePersistentSubscriptionsClient { get; }

    /// <summary>
    /// Gets the service used to serialize and deserialize <see cref="IEventRecord"/>s
    /// </summary>
    protected virtual ISerializer Serializer { get; }

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
        var qualifiedStreamId = this.GetQualifiedStreamId(streamId);

        var streamMetadataResult = await this.EventStoreClient.GetStreamMetadataAsync(qualifiedStreamId, cancellationToken: cancellationToken).ConfigureAwait(false);
        if (streamMetadataResult.StreamDeleted) throw new StreamNotFoundException(streamId);
        var offset = streamMetadataResult.Metadata.TruncateBefore ?? StreamPosition.StartOfStream;

        var readResult = this.EventStoreClient.ReadStreamAsync(Direction.Forwards, qualifiedStreamId, offset, 1, cancellationToken: cancellationToken);
        ReadState? readState;

        try { readState = await readResult.ReadState.ConfigureAwait(false); }
        catch { throw new StreamNotFoundException(streamId); }
        if (readState == ReadState.StreamNotFound)
        {
            if (streamId.StartsWith("$ce-")) return new EventStreamDescriptor(streamId, 0, null, null);
            else throw new StreamNotFoundException(streamId);
        }
        var firstEvent = await readResult.FirstAsync(cancellationToken).ConfigureAwait(false);
        readResult = this.EventStoreClient.ReadStreamAsync(Direction.Backwards, qualifiedStreamId, ESStreamPosition.End, 1, cancellationToken: cancellationToken);
        var lastEvent = await readResult.FirstAsync(cancellationToken).ConfigureAwait(false);

        return new EventStreamDescriptor(streamId, lastEvent.Event.EventNumber.ToInt64() + 1 - offset.ToInt64(), firstEvent.Event.Created, lastEvent.Event.Created);
    }

    /// <inheritdoc/>
    public virtual async Task AppendAsync(string streamId, IEnumerable<IEventDescriptor> events, long? expectedVersion = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(streamId)) throw new ArgumentNullException(nameof(streamId));
        if (events == null || !events.Any()) throw new ArgumentNullException(nameof(events));
        if (expectedVersion < StreamPosition.EndOfStream) throw new ArgumentOutOfRangeException(nameof(expectedVersion));
        streamId = this.GetQualifiedStreamId(streamId);

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
    public virtual IAsyncEnumerable<IEventRecord> ReadAsync(string? streamId, StreamReadDirection readDirection, long offset, ulong? length = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(streamId))
        {
            if(string.IsNullOrWhiteSpace(this.Options.DatabaseName)) return this.ReadFromAllAsync(readDirection, offset, length, cancellationToken);
            else return this.ReadFromStreamAsync(this.GetDatabaseStreamId()!, readDirection, offset, length, cancellationToken);
        }
        else return this.ReadFromStreamAsync(this.GetQualifiedStreamId(streamId), readDirection, offset, length, cancellationToken);
    }

    /// <summary>
    /// Reads events recorded on the specified stream
    /// </summary>
    /// <param name="streamId">The id of the stream to read events from</param>
    /// <param name="readDirection">The direction in which to read the stream</param>
    /// <param name="offset">The offset starting from which to read events</param>
    /// <param name="length">The amount of events to read</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="IAsyncEnumerable{T}"/> containing the events read from the store</returns>
    protected virtual async IAsyncEnumerable<IEventRecord> ReadFromStreamAsync(string streamId, StreamReadDirection readDirection, long offset, ulong? length = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(streamId)) throw new ArgumentNullException(nameof(streamId));

        ArgumentOutOfRangeException.ThrowIfLessThan(offset, StreamPosition.EndOfStream);

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
        else if (readDirection == StreamReadDirection.Backwards && offset == StreamPosition.StartOfStream) yield break;

        var readResult = this.EventStoreClient.ReadStreamAsync(direction, streamId, ESStreamPosition.FromInt64(offset), length.HasValue ? (long)length.Value : long.MaxValue, true, cancellationToken: cancellationToken);
        try { if (await readResult.ReadState.ConfigureAwait(false) == ReadState.StreamNotFound) throw new StreamNotFoundException(streamId); }
        catch (StreamDeletedException) { throw new StreamNotFoundException(streamId); }

        await foreach (var e in readResult) yield return this.DeserializeEventRecord(e);
    }

    /// <summary>
    /// Reads recorded events across all streams
    /// </summary>
    /// <param name="readDirection">The direction in which to read events</param>
    /// <param name="offset">The offset starting from which to read events</param>
    /// <param name="length">The amount of events to read</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="IAsyncEnumerable{T}"/> containing the events read from the store</returns>
    protected virtual async IAsyncEnumerable<IEventRecord> ReadFromAllAsync(StreamReadDirection readDirection, long offset, ulong? length = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var direction = readDirection switch
        {
            StreamReadDirection.Backwards => Direction.Backwards,
            StreamReadDirection.Forwards => Direction.Forwards,
            _ => throw new NotSupportedException($"The specified {nameof(StreamReadDirection)} '{readDirection}' is not supported")
        };

        if (readDirection == StreamReadDirection.Forwards && offset == StreamPosition.EndOfStream) yield break;
        else if (readDirection == StreamReadDirection.Backwards && offset == StreamPosition.StartOfStream) yield break;

        var position = offset switch
        {
            StreamPosition.StartOfStream => Position.Start,
            StreamPosition.EndOfStream => Position.End,
            _ => readDirection == StreamReadDirection.Backwards ? Position.End : Position.Start
        };
        var events = this.EventStoreClient.ReadAllAsync(direction, position, length.HasValue ? (long)length.Value : long.MaxValue, cancellationToken: cancellationToken);
        var streamOffset = 0;
        await foreach (var e in events.Where(e => !e.Event.EventType.StartsWith('$')))
        {
            if (readDirection == StreamReadDirection.Forwards ? streamOffset >= offset : streamOffset < (offset == StreamPosition.EndOfStream ? int.MaxValue : offset + 1)) yield return this.DeserializeEventRecord(e);
            streamOffset++;
        }
    }

    /// <inheritdoc/>
    public virtual Task<IObservable<IEventRecord>> ObserveAsync(string? streamId, long offset = StreamPosition.EndOfStream, string? consumerGroup = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(streamId))
        {
            if (string.IsNullOrWhiteSpace(this.Options.DatabaseName)) return this.ObserveAllAsync(offset, consumerGroup, cancellationToken);
            else return this.ObserveStreamAsync(this.GetDatabaseStreamId()!, offset, consumerGroup, cancellationToken);
        }
        else return this.ObserveStreamAsync(streamId, offset, consumerGroup, cancellationToken);
    }

    /// <summary>
    /// Subscribes to events of the specified stream
    /// </summary>
    /// <param name="streamId">The id of the stream, if any, to subscribe to. If not set, subscribes to all events</param>
    /// <param name="offset">The offset starting from which to receive events. Defaults to <see cref="StreamPosition.EndOfStream"/></param>
    /// <param name="consumerGroup">The name of the consumer group, if any, in case the subscription is persistent</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="IObservable{T}"/> used to observe events</returns>
    protected virtual async Task<IObservable<IEventRecord>> ObserveStreamAsync(string streamId, long offset = StreamPosition.EndOfStream, string? consumerGroup = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(streamId)) throw new ArgumentNullException(nameof(streamId));
        ArgumentOutOfRangeException.ThrowIfLessThan(offset, StreamPosition.EndOfStream);
        if (!await this.StreamExistsAsync(streamId, cancellationToken).ConfigureAwait(false)) throw new StreamNotFoundException(streamId);
        var qualifiedStreamId = this.GetQualifiedStreamId(streamId);

        var subject = new Subject<IEventRecord>();
        if (string.IsNullOrWhiteSpace(consumerGroup))
        {
            var position = offset == StreamPosition.EndOfStream ? FromStream.End : FromStream.After(ESStreamPosition.FromInt64(offset));
            var records = new List<IEventRecord>();
            if (position != FromStream.End) records = await this.ReadAsync(streamId, StreamReadDirection.Forwards, offset, cancellationToken: cancellationToken).ToListAsync(cancellationToken).ConfigureAwait(false);
            var subscription = await this.EventStoreClient.SubscribeToStreamAsync(qualifiedStreamId, FromStream.End, (sub, e, token) => this.OnEventConsumedAsync(subject, sub, e, token), true, (sub, reason, ex) => this.OnSubscriptionDropped(subject, sub, reason, ex), cancellationToken: cancellationToken).ConfigureAwait(false);
            return Observable.StartWith(Observable.Using(() => subscription, watch => subject), records);
        }
        else
        {
            var position = offset == StreamPosition.EndOfStream ? ESStreamPosition.End : ESStreamPosition.FromInt64(offset);
            var settings = new PersistentSubscriptionSettings(true, position, checkPointLowerBound: 1, checkPointUpperBound: 1);
            try { await this.EventStorePersistentSubscriptionsClient.CreateToStreamAsync(streamId, consumerGroup, settings, cancellationToken: cancellationToken).ConfigureAwait(false); }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.AlreadyExists) { }
            var checkpointedPosition = await this.GetConsumerCheckpointedPositionAsync(consumerGroup, streamId, cancellationToken).ConfigureAwait(false);
            var persistentSubscription = await this.EventStorePersistentSubscriptionsClient.SubscribeToStreamAsync(qualifiedStreamId, consumerGroup, (sub, e, retry, token) => this.OnEventConsumedAsync(subject, streamId, sub, e, retry, checkpointedPosition, token), (sub, reason, ex) => this.OnSubscriptionDropped(subject, sub, reason, ex), cancellationToken: cancellationToken).ConfigureAwait(false);
            return Observable.Using(() => persistentSubscription, watch => subject);
        }
    }

    /// <summary>
    /// Subscribes to all events
    /// </summary>
    /// <param name="offset">The offset starting from which to receive events. Defaults to <see cref="StreamPosition.EndOfStream"/></param>
    /// <param name="consumerGroup">The name of the consumer group, if any, in case the subscription is persistent</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="IObservable{T}"/> used to observe events</returns>
    protected virtual async Task<IObservable<IEventRecord>> ObserveAllAsync(long offset = StreamPosition.EndOfStream, string? consumerGroup = null, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(offset, StreamPosition.EndOfStream);

        var subject = new ReplaySubject<IEventRecord>();
        if (string.IsNullOrWhiteSpace(consumerGroup))
        {
            var position = offset == StreamPosition.EndOfStream ? FromAll.End : FromAll.Start;
            var filterOptions = new SubscriptionFilterOptions(EventTypeFilter.ExcludeSystemEvents());
            var subscription = await this.EventStoreClient.SubscribeToAllAsync(position, (sub, e, token) => this.OnEventConsumedAsync(subject, sub, e, token), true, (sub, reason, ex) => this.OnSubscriptionDropped(subject, sub, reason, ex), filterOptions: filterOptions, cancellationToken: cancellationToken).ConfigureAwait(false);
            var observable = Observable.Using(() => subscription, _ => subject);
            var streamOffset = 0;
            if (offset != StreamPosition.StartOfStream && offset != StreamPosition.EndOfStream) observable = observable.SkipWhile(e => 
            {
                var skip = streamOffset < offset;
                streamOffset++;
                return skip;
            });
            return observable;
        }
        else
        {
            var position = offset == StreamPosition.EndOfStream ? Position.End : Position.Start;
            var filter = EventTypeFilter.ExcludeSystemEvents();
            var settings = new PersistentSubscriptionSettings(true, position, checkPointLowerBound: 1, checkPointUpperBound: 1);
            try { await this.EventStorePersistentSubscriptionsClient.CreateToAllAsync(consumerGroup, filter, settings, cancellationToken: cancellationToken).ConfigureAwait(false); }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.AlreadyExists) { }
            var checkpointedPosition = await this.GetConsumerCheckpointedPositionAsync(consumerGroup, cancellationToken: cancellationToken).ConfigureAwait(false);
            var persistentSubscription = await this.EventStorePersistentSubscriptionsClient.SubscribeToAllAsync(consumerGroup, (sub, e, retry, token) => this.OnEventConsumedAsync(subject, null, sub, e, retry, checkpointedPosition, token), (sub, reason, ex) => this.OnSubscriptionDropped(subject, sub, reason, ex), cancellationToken: cancellationToken);
            return Observable.Using(() => persistentSubscription, watch => subject);
        }
    }

    /// <inheritdoc/>
    public virtual async Task SetOffsetAsync(string consumerGroup, long offset, string? streamId = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(consumerGroup)) throw new ArgumentNullException(nameof(consumerGroup));
        ArgumentOutOfRangeException.ThrowIfLessThan(offset, StreamPosition.EndOfStream);

        IPosition position = string.IsNullOrWhiteSpace(streamId) ? offset == StreamPosition.EndOfStream ? ESStreamPosition.End : ESStreamPosition.Start : offset == StreamPosition.EndOfStream ? ESStreamPosition.End : ESStreamPosition.FromInt64(offset);
        var settings = new PersistentSubscriptionSettings(true, position, checkPointLowerBound: 1, checkPointUpperBound: 1);
        PersistentSubscriptionInfo subscription;
        streamId = string.IsNullOrWhiteSpace(streamId) ? string.IsNullOrWhiteSpace(this.Options.DatabaseName) ? null : this.GetDatabaseStreamId()! : this.GetQualifiedStreamId(streamId);
        if (string.IsNullOrWhiteSpace(streamId))
        {
            try { subscription = await this.EventStorePersistentSubscriptionsClient.GetInfoToAllAsync(consumerGroup, cancellationToken: cancellationToken).ConfigureAwait(false); }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound) { throw new StreamNotFoundException(); }
            if (subscription.Stats.LastCheckpointedEventPosition != null) await this.SetConsumerCheckpointPositionAsync(consumerGroup, streamId, subscription.Stats.LastCheckpointedEventPosition, cancellationToken).ConfigureAwait(false);
           
            await this.EventStorePersistentSubscriptionsClient.DeleteToAllAsync(consumerGroup, cancellationToken: cancellationToken).ConfigureAwait(false);
            try { await this.EventStorePersistentSubscriptionsClient.CreateToAllAsync(consumerGroup, settings, cancellationToken: cancellationToken).ConfigureAwait(false); } //it occurred in tests that EventStore would only eventually delete the subscription, resulting in caught exception, thus the need for the try/catch block
            catch (RpcException ex) when (ex.StatusCode == StatusCode.AlreadyExists) { await this.EventStorePersistentSubscriptionsClient.UpdateToAllAsync(consumerGroup, settings, cancellationToken: cancellationToken).ConfigureAwait(false); }
        }
        else
        {
            try { subscription = await this.EventStorePersistentSubscriptionsClient.GetInfoToStreamAsync(streamId, consumerGroup, cancellationToken: cancellationToken).ConfigureAwait(false); }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound) { throw new StreamNotFoundException(); }
            if (subscription.Stats.LastCheckpointedEventPosition != null) await this.SetConsumerCheckpointPositionAsync(consumerGroup, streamId, subscription.Stats.LastCheckpointedEventPosition, cancellationToken).ConfigureAwait(false);

            await this.EventStorePersistentSubscriptionsClient.DeleteToStreamAsync(streamId, consumerGroup, cancellationToken: cancellationToken).ConfigureAwait(false);
            try { await this.EventStorePersistentSubscriptionsClient.CreateToStreamAsync(streamId, consumerGroup, settings, cancellationToken: cancellationToken).ConfigureAwait(false); } //it occurred in tests that EventStore would only eventually delete the subscription, resulting in caught exception, thus the need for the try/catch block
            catch (RpcException ex) when (ex.StatusCode == StatusCode.AlreadyExists) { await this.EventStorePersistentSubscriptionsClient.UpdateToStreamAsync(streamId, consumerGroup, settings, cancellationToken: cancellationToken).ConfigureAwait(false); }
        }
    }

    /// <inheritdoc/>
    public virtual async Task TruncateAsync(string streamId, ulong? beforeVersion = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(streamId)) throw new ArgumentNullException(nameof(streamId));
        if (!await this.StreamExistsAsync(streamId, cancellationToken).ConfigureAwait(false)) throw new StreamNotFoundException(streamId);

        var truncateBefore = beforeVersion.HasValue ? ESStreamPosition.FromInt64((long)beforeVersion.Value) : ESStreamPosition.End;
        await this.EventStoreClient.SetStreamMetadataAsync(this.GetQualifiedStreamId(streamId), StreamState.Any, new StreamMetadata(truncateBefore: truncateBefore), cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public virtual async Task DeleteAsync(string streamId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(streamId)) throw new ArgumentNullException(nameof(streamId));
        if (!await this.StreamExistsAsync(streamId, cancellationToken).ConfigureAwait(false)) throw new StreamNotFoundException(streamId);

        await this.EventStoreClient.DeleteAsync(this.GetQualifiedStreamId(streamId), StreamState.Any, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Converts the specified stream id to a qualified stream id, which is prefixed with the current database name, if any
    /// </summary>
    /// <param name="streamId">The stream id to convert</param>
    /// <returns>The qualified id of the specified stream id</returns>
    protected virtual string GetQualifiedStreamId(string streamId) => string.IsNullOrWhiteSpace(this.Options.DatabaseName) || streamId.StartsWith($"$ce-") ? streamId : $"{this.Options.DatabaseName}-{streamId}";

    /// <summary>
    /// Gets the id, if any, of the stream that contains references to all events in the database
    /// </summary>
    /// <returns>The id, if any, of the stream that contains references to all events in the database</returns>
    protected virtual string? GetDatabaseStreamId() => string.IsNullOrWhiteSpace(this.Options.DatabaseName) ? null : $"$ce-{this.Options.DatabaseName}";

    /// <summary>
    /// Deserializes the specified <see cref="ResolvedEvent"/> into a new <see cref="IEventRecord"/>
    /// </summary>
    /// <param name="e">The <see cref="ResolvedEvent"/> to deserialize</param>
    /// <param name="subscription">The <see cref="PersistentSubscription"/> the <see cref="ResolvedEvent"/> has been produced by, if any</param>
    /// <param name="subject">The <see cref="ISubject{T}"/> to stream <see cref="IEventRecord"/>s to</param>
    /// <param name="replayed">A boolean indicating whether or not the <see cref="ResolvedEvent"/> is being replayed to its consumer. Ignore if 'subscription' is null</param>
    /// <returns>The deserialized <see cref="IEventRecord"/></returns>
    protected virtual IEventRecord DeserializeEventRecord(ResolvedEvent e, PersistentSubscription? subscription = null, ISubject<IEventRecord>? subject = null, bool? replayed = null)
    {
        var metadata = this.Serializer.Deserialize<IDictionary<string, object>>(e.Event.Metadata.ToArray());
        var clrTypeName = metadata![EventRecordMetadata.ClrTypeName].ToString()!;
        var clrType = Type.GetType(clrTypeName) ?? throw new Exception();
        var data = this.Serializer.Deserialize(e.Event.Data.ToArray(), clrType);
        metadata.Remove(EventRecordMetadata.ClrTypeName);
        if (!metadata.Any()) metadata = null;
        if (subscription == null) return new EventRecord(e.OriginalStreamId, e.Event.EventId.ToString(), e.Event.EventNumber.ToUInt64(), e.Event.Position.CommitPosition, e.Event.Created, e.Event.EventType, data, metadata);
        else return new AckableEventRecord(e.OriginalStreamId, e.Event.EventId.ToString(), e.Event.EventNumber.ToUInt64(), e.Event.Position.CommitPosition, e.Event.Created, e.Event.EventType, data, metadata, replayed, () => this.OnAckEventAsync(subject!, subscription, e), reason => this.OnNackEventAsync(subject!, subscription, e, reason));
    }

    /// <summary>
    /// Gets the last checkpointed position, if any, of the specified consumer group
    /// </summary>
    /// <param name="consumerGroup">The consumer group to get the highest checkpointed position for</param>
    /// <param name="streamId">The id of the stream, if any, to get the consumer group's checkpointed position for</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    protected virtual async Task<ulong?> GetConsumerCheckpointedPositionAsync(string consumerGroup, string? streamId = null, CancellationToken cancellationToken = default)
    {
        try
        {
            return await this.ReadAsync(this.GetConsumerCheckpointStreamId(consumerGroup, streamId), StreamReadDirection.Forwards, StreamPosition.StartOfStream, cancellationToken: cancellationToken)
                .Select(e => e.Data)
                .OfType<ulong>()
                .OrderByDescending(u => u)
                .FirstOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);
        }
        catch (StreamNotFoundException) { return null; }
    }

    /// <summary>
    /// Sets the last checkpointed position of the specified consumer group
    /// </summary>
    /// <param name="consumerGroup">The consumer group to set the last checkpointed position for</param>
    /// <param name="streamId">The id of the stream, if any, to get the consumer group's checkpointed position for</param>
    /// <param name="position">The last checkpointed position</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    protected virtual async Task SetConsumerCheckpointPositionAsync(string consumerGroup, string? streamId, IPosition position, CancellationToken cancellationToken = default)
    {
        var data = position switch
        {
            Position pos => pos.CommitPosition,
            ESStreamPosition spos => (ulong)spos.ToInt64(),
            _ => throw new NotSupportedException($"The position type '{position.GetType()}' is not supported in this context")
        };
        await this.AppendAsync(this.GetConsumerCheckpointStreamId(consumerGroup, streamId), new EventDescriptor[] { new("$checkpoint", data) }, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets the id of the stream used to store the checkpoints of the specified consumer group, and optionally stream
    /// </summary>
    /// <param name="consumerGroup">The consumer group to get the checkpoint stream id for</param>
    /// <param name="streamId">The id of the stream, if any, to get the consumer group's checkpoint stream for</param>
    /// <returns></returns>
    protected virtual string GetConsumerCheckpointStreamId(string consumerGroup, string? streamId) => $"${consumerGroup}:{streamId ?? "$all"}_checkpoints";

    /// <summary>
    /// Handles the consumption of a <see cref="ResolvedEvent"/> on a <see cref="PersistentSubscription"/>
    /// </summary>
    /// <param name="subject">The <see cref="ISubject{T}"/> to stream <see cref="IEventRecord"/>s to</param>
    /// <param name="subscription">The <see cref="PersistentSubscription"/> the <see cref="ResolvedEvent"/> has been received by</param>
    /// <param name="e">The <see cref="ResolvedEvent"/> to handle</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    protected virtual Task OnEventConsumedAsync(ISubject<IEventRecord> subject, StreamSubscription subscription, ResolvedEvent e, CancellationToken cancellationToken) => Task.Run(() => subject.OnNext(this.DeserializeEventRecord(e)), cancellationToken);

    /// <summary>
    /// Handles the consumption of a <see cref="ResolvedEvent"/> on a <see cref="PersistentSubscription"/>
    /// </summary>
    /// <param name="subject">The <see cref="ISubject{T}"/> to stream <see cref="IEventRecord"/>s to</param>
    /// <param name="streamId">The id of the stream, if any, to consume <see cref="IEventRecord"/>s from</param>
    /// <param name="subscription">The <see cref="PersistentSubscription"/> the <see cref="ResolvedEvent"/> has been received by</param>
    /// <param name="e">The <see cref="ResolvedEvent"/> to handle</param>
    /// <param name="retryCount">The retry count, if any</param>
    /// <param name="checkpointedPosition">The highest position ever checkpointed by the consumer group</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    protected virtual Task OnEventConsumedAsync(ISubject<IEventRecord> subject, string? streamId, PersistentSubscription subscription, ResolvedEvent e, int? retryCount, ulong? checkpointedPosition, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(this.Options.DatabaseName) || !e.OriginalStreamId.StartsWith(this.GetDatabaseStreamId()!)) if (e.OriginalStreamId.StartsWith('$') || e.Event.Metadata.Length < 1) return subscription.Ack(e);
            return Task.Run(() => subject.OnNext(this.DeserializeEventRecord(e, subscription, subject, checkpointedPosition > (string.IsNullOrWhiteSpace(streamId) ? e.Event.Position.CommitPosition : e.Event.EventNumber.ToUInt64()))), cancellationToken);
        }
        catch (Exception ex)
        {
            subject.OnError(ex);
            return Task.CompletedTask;
        }
    }

    /// <summary>
    /// Acks the specified <see cref="ResolvedEvent"/>
    /// </summary>
    /// <param name="subject">The <see cref="ISubject{T}"/> to stream <see cref="IEventRecord"/>s to</param>
    /// <param name="subscription">The <see cref="PersistentSubscription"/> the <see cref="ResolvedEvent"/> to ack has been received by</param>
    /// <param name="e">The <see cref="ResolvedEvent"/> to ack</param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    protected virtual async Task OnAckEventAsync(ISubject<IEventRecord> subject, PersistentSubscription subscription, ResolvedEvent e)
    {
        try { await subscription.Ack(e.OriginalEvent.EventId).ConfigureAwait(false); }
        catch (ObjectDisposedException ex) { subject.OnError(ex); }
    }

    /// <summary>
    /// Nacks the specified <see cref="ResolvedEvent"/>
    /// </summary>
    /// <param name="subject">The <see cref="ISubject{T}"/> to stream <see cref="IEventRecord"/>s to</param>
    /// <param name="subscription">The <see cref="PersistentSubscription"/> the <see cref="ResolvedEvent"/> to nack has been received by</param>
    /// <param name="e">The <see cref="ResolvedEvent"/> to nack</param>
    /// <param name="reason">The reason why to nack the <see cref="ResolvedEvent"/></param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    protected virtual async Task OnNackEventAsync(ISubject<IEventRecord> subject, PersistentSubscription subscription, ResolvedEvent e, string? reason)
    {
        try { await subscription.Nack(PersistentSubscriptionNakEventAction.Retry, reason ?? "Unknown", e.OriginalEvent.EventId).ConfigureAwait(false); }
        catch (ObjectDisposedException ex) { subject.OnError(ex); }
    }

    /// <summary>
    /// Handles the specified <see cref="StreamSubscription"/> being dropped
    /// </summary>
    /// <param name="subject">The <see cref="ISubject{T}"/> to stream <see cref="IEventRecord"/>s to</param>
    /// <param name="subscription">The <see cref="StreamSubscription"/> the <see cref="ResolvedEvent"/> has been received by</param>
    /// <param name="reason">The reason why to drop the <see cref="StreamSubscription"/></param>
    /// <param name="ex">The <see cref="Exception"/> that occurred, if any</param>
    protected virtual void OnSubscriptionDropped(ISubject<IEventRecord> subject, StreamSubscription subscription, SubscriptionDroppedReason reason, Exception? ex)
    {
        switch (reason)
        {
            case SubscriptionDroppedReason.Disposed:
                subject.OnCompleted();
                break;
            case SubscriptionDroppedReason.SubscriberError:
            case SubscriptionDroppedReason.ServerError:
                subject.OnError(ex ?? new Exception());
                break;
        }
    }

    /// <summary>
    /// Handles the specified <see cref="PersistentSubscription"/> being dropped
    /// </summary>
    /// <param name="subject">The <see cref="ISubject{T}"/> to stream <see cref="IEventRecord"/>s to</param>
    /// <param name="subscription">The <see cref="PersistentSubscription"/> the <see cref="ResolvedEvent"/> has been received by</param>
    /// <param name="reason">The reason why to drop the <see cref="PersistentSubscription"/></param>
    /// <param name="ex">The <see cref="Exception"/> that occurred, if any</param>
    protected virtual void OnSubscriptionDropped(ISubject<IEventRecord> subject, PersistentSubscription subscription, SubscriptionDroppedReason reason, Exception? ex)
    {
        switch (reason)
        {
            case SubscriptionDroppedReason.Disposed:
                subject.OnCompleted();
                break;
            case SubscriptionDroppedReason.SubscriberError:
            case SubscriptionDroppedReason.ServerError:
                subject.OnError(ex ?? new Exception());
                break;
        }
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
