﻿// Copyright © 2021-Present Neuroglia SRL. All rights reserved.
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

using Microsoft.Extensions.Caching.Memory;
using Neuroglia.Data.Infrastructure.EventSourcing.Services;
using Neuroglia.Plugins;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;

namespace Neuroglia.Data.Infrastructure.EventSourcing.DistributedCache.Services;

/// <summary>
/// Represents an <see cref="IEventStore"/> implementation relying on an <see cref="IMemoryCache"/>
/// </summary>
/// <remarks>Should not be used in production</remarks>
/// <remarks>
/// Initializes a new <see cref="MemoryEventStore"/>
/// </remarks>
/// <param name="cache">The cache to stream events to</param>
[Plugin(Tags = ["event-store"]), Factory(typeof(MemoryCacheEventStoreFactory))]
public class MemoryEventStore(IMemoryCache cache)
    : IEventStore, IDisposable
{

    bool _disposed;

    /// <summary>
    /// Gets the cache to stream events to
    /// </summary>
    protected IMemoryCache Cache { get; } = cache;

    /// <summary>
    /// Gets the <see cref="ConcurrentDictionary{TKey, TValue}"/> containing all published <see cref="IEventRecord"/>s
    /// </summary>
    protected ConcurrentDictionary<ulong, IEventRecord> Stream { get; } = [];

    /// <summary>
    /// Gets the <see cref="ISubject{T}"/> used to stream <see cref="IEventRecord"/>s
    /// </summary>
    protected Subject<IEventRecord> Subject { get; } = new();

    /// <inheritdoc/>
    public virtual Task<ulong> AppendAsync(string streamId, IEnumerable<IEventDescriptor> events, long? expectedVersion = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(streamId)) throw new ArgumentNullException(nameof(streamId));
        if (events == null || !events.Any()) throw new ArgumentNullException(nameof(events));
        if (expectedVersion < StreamPosition.EndOfStream) throw new ArgumentOutOfRangeException(nameof(expectedVersion));

        this.Cache.TryGetValue<ObservableCollection<IEventRecord>>(streamId, out var stream);
        var actualVersion = stream == null ? (long?)null : (long)stream.Last().Offset;

        if (expectedVersion.HasValue)
        {
            if(expectedVersion.Value == StreamPosition.EndOfStream)
            {
                if (actualVersion != null) throw new OptimisticConcurrencyException(expectedVersion, actualVersion);
            }
            else if(actualVersion == null || actualVersion != expectedVersion) throw new OptimisticConcurrencyException(expectedVersion, actualVersion);
        }

        stream ??= [];
        ulong offset = actualVersion.HasValue ? (ulong)actualVersion.Value + 1 : StreamPosition.StartOfStream;
        foreach(var e in events)
        {
            var record = new EventRecord(streamId, Guid.NewGuid().ToString(), offset, (ulong)this.Stream.Count, DateTimeOffset.Now, e.Type, e.Data, e.Metadata);
            stream.Add(record);
            this.Stream.AddOrUpdate((ulong)this.Stream.Count + 1, record, (key, value) => record);
            this.Subject.OnNext(record);
            offset++;
        }
        offset--;
        this.Cache.Set(streamId, stream);
        return Task.FromResult(offset);
    }

    /// <inheritdoc/>
    public virtual Task<IEventStreamDescriptor> GetAsync(string streamId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(streamId)) throw new ArgumentNullException(nameof(streamId));

        if (!this.Cache.TryGetValue<ObservableCollection<IEventRecord>>(streamId, out var stream) || stream == null) throw new StreamNotFoundException(streamId);

        return Task.FromResult((IEventStreamDescriptor)new EventStreamDescriptor(streamId, stream.Count, stream.FirstOrDefault()?.Timestamp, stream.LastOrDefault()?.Timestamp));
    }

    /// <inheritdoc/>
    public virtual IAsyncEnumerable<IEventRecord> ReadAsync(string? streamId, StreamReadDirection readDirection, long offset, ulong? length = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(streamId)) return this.ReadAllAsync(readDirection, offset, length, cancellationToken);
        else return this.ReadFromStreamAsync(streamId, readDirection, offset, length, cancellationToken);
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

        if (!this.Cache.TryGetValue<ObservableCollection<IEventRecord>>(streamId, out var stream) || stream == null) throw new StreamNotFoundException(streamId);

        var events = stream.ToList();
        IEventRecord? firstEvent;
        switch (readDirection)
        {
            case StreamReadDirection.Forwards:
                if (offset < StreamPosition.StartOfStream) yield break;
                firstEvent = events.FirstOrDefault(e => e.Offset == (ulong)offset);
                if (firstEvent == null) yield break;
                events = events.Skip(events.IndexOf(firstEvent)).ToList();
                break;
            case StreamReadDirection.Backwards:
                if (offset <= StreamPosition.StartOfStream && offset != StreamPosition.EndOfStream) yield break;
                events.Reverse();
                if (offset != StreamPosition.EndOfStream)
                {
                    firstEvent = events.FirstOrDefault(e => e.Offset == (ulong)offset);
                    if (firstEvent == null) yield break;
                    events = events.Skip(events.IndexOf(firstEvent)).ToList();
                }
                break;
            default: throw new NotSupportedException($"The specified {nameof(StreamReadDirection)} '{readDirection}' is not supported");
        }

        if (length.HasValue) events = events.Take((int)length.Value).ToList();

        foreach (var e in events) yield return e;

        await Task.CompletedTask;
    }

    /// <summary>
    /// Reads recorded events across all streams
    /// </summary>
    /// <param name="readDirection">The direction in which to read events</param>
    /// <param name="offset">The offset starting from which to read events</param>
    /// <param name="length">The amount of events to read</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="IAsyncEnumerable{T}"/> containing the events read from the store</returns>
    protected virtual IAsyncEnumerable<IEventRecord> ReadAllAsync(StreamReadDirection readDirection, long offset, ulong? length = null, CancellationToken cancellationToken = default)
    {
        var entries = readDirection == StreamReadDirection.Backwards ? this.Stream.Reverse().ToAsyncEnumerable() : this.Stream.ToAsyncEnumerable();
        var records = entries.SkipWhile(kvp => readDirection == StreamReadDirection.Forwards ? kvp.Key <= (ulong)offset : kvp.Key > (ulong)(offset == StreamPosition.EndOfStream || offset == StreamPosition.StartOfStream ? offset : offset + 1)).Select(kvp => kvp.Value);
        if (length.HasValue) records = records.Take((int)length.Value);
        return records;
    }

    /// <inheritdoc/>
    public virtual Task<IObservable<IEventRecord>> ObserveAsync(string? streamId, long offset = StreamPosition.EndOfStream, string? consumerGroup = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(streamId)) return this.ObserveAllAsync(offset, consumerGroup, cancellationToken);
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

        if (!this.Cache.TryGetValue<ObservableCollection<IEventRecord>>(streamId, out var stream) || stream == null) throw new StreamNotFoundException(streamId);

        var storedOffset = string.IsNullOrWhiteSpace(consumerGroup) ? offset : await this.GetOffsetAsync(consumerGroup, streamId, cancellationToken).ConfigureAwait(false) ?? offset;
        var events = storedOffset == StreamPosition.EndOfStream ? [] : await (this.ReadAsync(streamId, StreamReadDirection.Forwards, storedOffset, cancellationToken: cancellationToken)).ToListAsync(cancellationToken).ConfigureAwait(false);
        var subject = new Subject<IEventRecord>();
        
        var checkpointedPosition = (ulong?)null;
        if(!string.IsNullOrWhiteSpace(consumerGroup)) checkpointedPosition = await this.GetConsumerCheckpointedPositionAsync(consumerGroup, streamId, cancellationToken).ConfigureAwait(false);

        var subscription = Observable.StartWith(Observable.FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(h => stream.CollectionChanged += h, h => stream.CollectionChanged -= h)
            .Where(e => e.EventArgs.Action == NotifyCollectionChangedAction.Add)
            .SelectMany(e => e.EventArgs.NewItems!.Cast<IEventRecord>()!), events)
            .Subscribe(e => this.OnEventConsumed(subject, e, streamId, consumerGroup, checkpointedPosition.HasValue ? checkpointedPosition.Value > e.Offset : string.IsNullOrWhiteSpace(consumerGroup) ? null : false));
        var observable = Observable.Using(() => subscription, _ => subject);
        return Observable.StartWith(observable, events);
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

        var storedOffset = string.IsNullOrWhiteSpace(consumerGroup) ? offset : await this.GetOffsetAsync(consumerGroup, cancellationToken: cancellationToken).ConfigureAwait(false) ?? offset;
        var events = storedOffset == StreamPosition.EndOfStream ? [] : await (this.ReadAsync(null, StreamReadDirection.Forwards, storedOffset, cancellationToken: cancellationToken)).ToListAsync(cancellationToken).ConfigureAwait(false);
        var subject = new ReplaySubject<IEventRecord>();

        var checkpointedPosition = (ulong?)null;
        if (!string.IsNullOrWhiteSpace(consumerGroup)) checkpointedPosition = await this.GetConsumerCheckpointedPositionAsync(consumerGroup, null, cancellationToken).ConfigureAwait(false);

        var subscription = Observable.StartWith(this.Subject, events).Subscribe(e => this.OnEventConsumed(subject, e, null, consumerGroup, checkpointedPosition.HasValue ? checkpointedPosition.Value >= this.Stream.First(kvp => kvp.Value == e).Key : string.IsNullOrWhiteSpace(consumerGroup) ? null : false));
        return Observable.Using(() => subscription, _ => subject);
    }

    /// <summary>
    /// Gets the offset persisted for the specified consumer group and stream
    /// </summary>
    /// <param name="consumerGroup">The consumer group to the stored offset for</param>
    /// <param name="streamId">The id of the stream, if any, to get the stored offset for</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    protected virtual Task<long?> GetOffsetAsync(string consumerGroup, string? streamId = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(consumerGroup)) throw new ArgumentNullException(nameof(consumerGroup));
        this.Cache.TryGetValue(this.GetConsumerGroupCacheKey(consumerGroup, streamId), out long? offset);
        return Task.FromResult(offset);
    }

    /// <inheritdoc/>
    public virtual async Task SetOffsetAsync(string consumerGroup, long offset, string? streamId = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(consumerGroup)) throw new ArgumentNullException(nameof(consumerGroup));
        ArgumentOutOfRangeException.ThrowIfLessThan(offset, StreamPosition.EndOfStream);

        var checkpointedPosition = (ulong?)await this.GetOffsetAsync(consumerGroup, streamId, cancellationToken).ConfigureAwait(false);
        if (checkpointedPosition.HasValue) await this.SetConsumerCheckpointPositionAsync(consumerGroup, streamId, checkpointedPosition.Value, cancellationToken).ConfigureAwait(false);

        this.Cache.Set(this.GetConsumerGroupCacheKey(consumerGroup, streamId), offset);
    }

    /// <summary>
    /// Gets the cache key for the specified consumer group and stream
    /// </summary>
    /// <param name="consumerGroup">The consumer group to the cache key for</param>
    /// <param name="streamId">The id of the stream, if any, to get the cache key for</param>
    /// <returns>The cache key for the specified consumer group and stream</returns>
    protected virtual string GetConsumerGroupCacheKey(string consumerGroup, string? streamId = null) => string.IsNullOrWhiteSpace(streamId) ? $"cs:{consumerGroup}-$all" : $"cs:{consumerGroup}-{streamId}";

    /// <summary>
    /// Gets the last checkpointed position, if any, of the specified consumer group
    /// </summary>
    /// <param name="consumerGroup">The consumer group to get the highest checkpointed position for</param>
    /// <param name="streamId">The id of the stream, if any, to get the consumer group's checkpointed position for</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    protected virtual Task<ulong?> GetConsumerCheckpointedPositionAsync(string consumerGroup, string? streamId = null, CancellationToken cancellationToken = default)
    {
        return Task.Run(() =>
        {
            if (this.Cache.TryGetValue(this.GetConsumerCheckpointCacheKey(consumerGroup, streamId), out ulong? checkpointPosition)) return checkpointPosition;
            else return null;
        }, cancellationToken);
    }

    /// <summary>
    /// Sets the last checkpointed position of the specified consumer group
    /// </summary>
    /// <param name="consumerGroup">The consumer group to set the last checkpointed position for</param>
    /// <param name="streamId">The id of the stream, if any, to get the consumer group's checkpointed position for</param>
    /// <param name="position">The last checkpointed position</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    protected virtual Task SetConsumerCheckpointPositionAsync(string consumerGroup, string? streamId, ulong position, CancellationToken cancellationToken = default) => Task.Run(() => this.Cache.Set(this.GetConsumerCheckpointCacheKey(consumerGroup, streamId), position), cancellationToken);

    /// <summary>
    /// Gets the cache key used to store the checkpoints of the specified consumer group, and optionally stream
    /// </summary>
    /// <param name="consumerGroup">The consumer group to get the checkpoint cache key for</param>
    /// <param name="streamId">The id of the stream, if any, to get the consumer group's checkpoint cache key for</param>
    /// <returns></returns>
    protected virtual string GetConsumerCheckpointCacheKey(string consumerGroup, string? streamId) => $"${consumerGroup}:{streamId ?? "$all"}_checkpoints";

    /// <inheritdoc/>
    public virtual Task TruncateAsync(string streamId, ulong? beforeVersion = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(streamId)) throw new ArgumentNullException(nameof(streamId));

        if (!this.Cache.TryGetValue<ObservableCollection<IEventRecord>>(streamId, out var stream) || stream == null) throw new StreamNotFoundException(streamId);

        var e = stream.FirstOrDefault();
        beforeVersion ??= stream.Last().Offset + 1;

        while(e != null && e.Offset < beforeVersion)
        {
            stream.Remove(e);
            e = stream.FirstOrDefault();
        }

        this.Cache.Set(streamId, stream);

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public virtual Task DeleteAsync(string streamId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(streamId)) throw new ArgumentNullException(nameof(streamId));

        if (!this.Cache.TryGetValue<ObservableCollection<IEventRecord>>(streamId, out var stream) || stream == null) throw new StreamNotFoundException(streamId);

        this.Cache.Remove(streamId);

        return Task.CompletedTask;
    }

    /// <summary>
    /// Handles the consumption of a <see cref="IEventRecord"/> on a subscription
    /// </summary>
    /// <param name="subject">The <see cref="ISubject{T}"/> used to stream <see cref="IEventRecord"/>s</param>
    /// <param name="streamId">The id of the stream <see cref="IEventRecord"/> belongs to</param>
    /// <param name="e">The <see cref="IEventRecord"/> to handle</param>
    /// <param name="replayed">A boolean indicating whether or not the <see cref="IEventRecord"/> is being replayed to its subscriber</param>
    /// <param name="consumerGroup">The name of the group, if any, that consumes the <see cref="IEventRecord"/></param>
    protected void OnEventConsumed(ISubject<IEventRecord> subject, IEventRecord e, string? streamId = null, string? consumerGroup = null, bool? replayed = null)
    {
        var ackDelegate = () => string.IsNullOrWhiteSpace(consumerGroup) ? null : this.SetOffsetAsync(consumerGroup, string.IsNullOrWhiteSpace(streamId) ? (long)this.Stream.First(kvp => kvp.Value == e).Key : (long)e.Offset, streamId);
        var nackDelegate = (string? reason) => string.IsNullOrWhiteSpace(consumerGroup) ? null : Task.Run(() => this.OnEventConsumed(subject, e, streamId, consumerGroup));
        var record = string.IsNullOrEmpty(consumerGroup) ? e : new AckableEventRecord(e.StreamId, e.Id, e.Offset, e.Position, e.Timestamp, e.Type, e.Data, e.Metadata, replayed, ackDelegate, nackDelegate);
        subject.OnNext(record);
    }

    /// <summary>
    /// Disposes of the <see cref="MemoryEventStore"/>
    /// </summary>
    /// <param name="disposing">A boolean indicating whether or not the <see cref="MemoryEventStore"/> is being disposed of</param>
    protected virtual void Dispose(bool disposing)
    {
        if (this._disposed) return;
        if (disposing) this.Subject.Dispose();
        this._disposed = true;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

}
