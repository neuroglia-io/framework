using Microsoft.Extensions.Caching.Memory;
using Neuroglia.Data.Infrastructure.EventSourcing;
using Neuroglia.Data.Infrastructure.EventSourcing.Services;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;

namespace Neuroglia.Data.EventSourcing.DistributedCache.Services;

/// <summary>
/// Represents an <see cref="IEventStore"/> implementation relying on an <see cref="IMemoryCache"/>
/// </summary>
public class MemoryCacheEventStore
    : IEventStore
{

    /// <summary>
    /// Initializes a new <see cref="MemoryCacheEventStore"/>
    /// </summary>
    /// <param name="cache">The cache to stream events to</param>
    public MemoryCacheEventStore(IMemoryCache cache)
    {
        this.Cache = cache;
    }

    /// <summary>
    /// Gets the cache to stream events to
    /// </summary>
    protected IMemoryCache Cache { get; }

    /// <inheritdoc/>
    public virtual Task AppendAsync(string streamId, IEnumerable<IEventDescriptor> events, long? expectedVersion = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(streamId)) throw new ArgumentNullException(nameof(streamId));
        if (events == null || !events.Any()) throw new ArgumentNullException(nameof(events));

        this.Cache.TryGetValue<ObservableCollection<IEventRecord>>(streamId, out var stream);
        var actualversion = stream == null ? (long?)null : stream.Count - 1;

        if (expectedVersion.HasValue)
        {
            if(expectedVersion.Value == StreamPosition.EndOfStream)
            {
                if (actualversion != null) throw new OptimisticConcurrencyException(expectedVersion, actualversion);
            }
            else if(actualversion == null || actualversion != expectedVersion) throw new OptimisticConcurrencyException(expectedVersion, actualversion);
        }

        stream ??= new();
        ulong offset = actualversion.HasValue ? (ulong)actualversion.Value + 1 : StreamPosition.StartOfStream;
        foreach(var e in events)
        {
            stream.Add(new EventRecord(Guid.NewGuid().ToString(), offset, DateTimeOffset.Now, e.Type, e.Data, e.Metadata));
            offset++;
        }

        this.Cache.Set(streamId, stream);

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public virtual Task<IEventStreamDescriptor> GetAsync(string streamId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(streamId)) throw new ArgumentNullException(nameof(streamId));

        if (!this.Cache.TryGetValue<ObservableCollection<IEventRecord>>(streamId, out var stream) || stream == null) throw new StreamNotFoundException(streamId);

        return Task.FromResult((IEventStreamDescriptor)new EventStreamDescriptor(streamId, stream.LongCount(), stream.FirstOrDefault()?.Timestamp, stream.LastOrDefault()?.Timestamp));
    }

    /// <inheritdoc/>
    public virtual async IAsyncEnumerable<IEventRecord> ReadAsync(string streamId, StreamReadDirection readDirection, long offset, ulong? length = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(streamId)) throw new ArgumentNullException(nameof(streamId));

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
                if(offset != StreamPosition.EndOfStream)
                {
                    firstEvent = events.FirstOrDefault(e => e.Offset == (ulong)offset);
                    if (firstEvent == null) yield break;
                    events = events.Skip(events.IndexOf(firstEvent) + 1).ToList();
                }
                break;
            default: throw new NotSupportedException($"The specified {nameof(StreamReadDirection)} '{readDirection}' is not supported");
        }

        foreach (var e in events) yield return e;

        await Task.CompletedTask;
    }

    /// <inheritdoc/>
    public virtual async Task<IObservable<IEventRecord>> SubscribeAsync(string streamId, long offset = StreamPosition.EndOfStream, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(streamId)) throw new ArgumentNullException(nameof(streamId));

        if (!this.Cache.TryGetValue<ObservableCollection<IEventRecord>>(streamId, out var stream) || stream == null) throw new StreamNotFoundException(streamId);

        var events = offset == StreamPosition.EndOfStream ? Array.Empty<IEventRecord>().ToList() : await (this.ReadAsync(streamId, StreamReadDirection.Forwards, offset, cancellationToken: cancellationToken)).ToListAsync(cancellationToken).ConfigureAwait(false);

        return Observable.StartWith(Observable.FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(h => stream.CollectionChanged += h, h => stream.CollectionChanged -= h)
           .Where(e => e.EventArgs.Action == NotifyCollectionChangedAction.Add)
           .SelectMany(e => e.EventArgs.NewItems!.Cast<IEventRecord>()!), events);
    }

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

}
