using Microsoft.Extensions.Options;
using Neuroglia.Data.Infrastructure.EventSourcing;
using Neuroglia.Data.Infrastructure.EventSourcing.Configuration;
using Neuroglia.Data.Infrastructure.EventSourcing.Services;
using Neuroglia.Serialization;
using StackExchange.Redis;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;

namespace Neuroglia.Data.Infrastructure.EventSourcing.Services;

/// <summary>
/// Represents a <see href="https://stackexchange.github.io/StackExchange.Redis/">StackExchange Redis</see> implementation of the <see cref="IEventStore"/> interface
/// </summary>
public class RedisEventStore
    : IEventStore
{

    /// <summary>
    /// Initializes a new <see cref="RedisEventStore"/>
    /// </summary>
    /// <param name="options">The current <see cref="EventStoreOptions"/></param>
    /// <param name="redis">The service used to connect to the Redis Server</param>
    /// <param name="serializerProvider">The service used to provide <see cref="ISerializer"/>s</param>
    public RedisEventStore(IOptions<EventStoreOptions> options, IConnectionMultiplexer redis, ISerializerProvider serializerProvider)
    {
        this.Options = options.Value;
        this.Serializer = serializerProvider.GetSerializers().First(s => this.Options.SerializerType == null || s.GetType() == this.Options.SerializerType);
        this.Redis = redis;
        this.Database = redis.GetDatabase();
        this.Subscriber = redis.GetSubscriber();
    }

    /// <summary>
    /// Gets the current <see cref="EventStoreOptions"/>
    /// </summary>
    protected EventStoreOptions Options { get; }

    /// <summary>
    /// Gets the service used to serialize and deserialize stored events
    /// </summary>
    protected ISerializer Serializer { get; }

    /// <summary>
    /// Gets the service used to connect to the Redis Server
    /// </summary>
    protected IConnectionMultiplexer Redis { get; }

    /// <summary>
    /// Gets the Redis database to store events into
    /// </summary>
    protected IDatabase Database { get; }

    /// <summary>
    /// Gets the service used to subscribe to a redis pub/sub channel
    /// </summary>
    protected ISubscriber Subscriber { get; }

    /// <inheritdoc/>
    public virtual async Task AppendAsync(string streamId, IEnumerable<IEventDescriptor> events, long? expectedVersion = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(streamId)) throw new ArgumentNullException(nameof(streamId));
        if (events == null || !events.Any()) throw new ArgumentNullException(nameof(events));

        var keys = (await this.Database.HashKeysAsync(streamId).ConfigureAwait(false)).Order().ToList();
        var actualversion = keys == null || !keys.Any() ? (long?)null : (long)keys.Order().LastOrDefault();

        if (expectedVersion.HasValue)
        {
            if (expectedVersion.Value == Infrastructure.EventSourcing.StreamPosition.EndOfStream)
            {
                if (actualversion != null) throw new OptimisticConcurrencyException(expectedVersion, actualversion);
            }
            else if (actualversion == null || actualversion != expectedVersion) throw new OptimisticConcurrencyException(expectedVersion, actualversion);
        }

        ulong offset = actualversion.HasValue ? (ulong)actualversion.Value + 1 : Infrastructure.EventSourcing.StreamPosition.StartOfStream;
        foreach (var e in events)
        {
            var record = new EventRecord(Guid.NewGuid().ToShortString(), offset, DateTimeOffset.Now, e.Type, e.Data, e.Metadata);
            record.Metadata ??= new Dictionary<string, object>();
            record.Metadata[EventRecordMetadata.ClrTypeName] = e.Data?.GetType().AssemblyQualifiedName!;
            var entryValue = this.Serializer.SerializeToByteArray(record);
            await this.Database.HashSetAsync(streamId, new HashEntry[] { new(offset, entryValue) }).ConfigureAwait(false);
            await this.Database.PublishAsync(this.GetRedisChannelFor(streamId), entryValue).ConfigureAwait(false);
            offset++;
        }

    }

    /// <inheritdoc/>
    public virtual async Task<IEventStreamDescriptor> GetAsync(string streamId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(streamId)) throw new ArgumentNullException(nameof(streamId));
        if (!await this.Database.KeyExistsAsync(streamId).ConfigureAwait(false)) throw new StreamNotFoundException(streamId);

        var keys = (await this.Database.HashKeysAsync(streamId).ConfigureAwait(false)).Order().ToList();
        DateTimeOffset? firstEventAt = null;
        DateTimeOffset? lastEventAt = null;
        if(keys.Any())
        {
            firstEventAt = this.Serializer.Deserialize<EventRecord>((byte[])(await this.Database.HashGetAsync(streamId, keys.First()).ConfigureAwait(false))!)!.Timestamp;
            lastEventAt = this.Serializer.Deserialize<EventRecord>((byte[])(await this.Database.HashGetAsync(streamId, keys.Last()).ConfigureAwait(false))!)!.Timestamp;
        }

        return new EventStreamDescriptor(streamId, keys.Count, firstEventAt, lastEventAt);
    }

    /// <inheritdoc/>
    public virtual async IAsyncEnumerable<IEventRecord> ReadAsync(string streamId, StreamReadDirection readDirection, long offset, ulong? length = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(streamId)) throw new ArgumentNullException(nameof(streamId));
        if (!await this.Database.KeyExistsAsync(streamId).ConfigureAwait(false)) throw new StreamNotFoundException(streamId);

        var hashKeys = (await this.Database.HashKeysAsync(streamId).ConfigureAwait(false)).Order().ToList();

        int skip;
        switch (readDirection)
        {
            case StreamReadDirection.Forwards:
                if (offset < Infrastructure.EventSourcing.StreamPosition.StartOfStream) yield break;
                skip = hashKeys.IndexOf(offset);
                if (skip < 0) yield break;
                hashKeys = hashKeys.Skip(skip).ToList();
                break;
            case StreamReadDirection.Backwards:
                if (offset <= Infrastructure.EventSourcing.StreamPosition.StartOfStream && offset != Infrastructure.EventSourcing.StreamPosition.EndOfStream) yield break;
                hashKeys.Reverse();
                if (offset != Infrastructure.EventSourcing.StreamPosition.EndOfStream)
                {
                    skip = hashKeys.IndexOf(offset);
                    if (skip < 0) yield break;
                    hashKeys = hashKeys.Skip(skip).ToList();
                }
                break;
            default: throw new NotSupportedException($"The specified {nameof(StreamReadDirection)} '{readDirection}' is not supported");
        }

        if (length.HasValue) hashKeys = hashKeys.Take((int)length.Value).ToList();

        foreach(var hashKey in hashKeys) yield return this.DeserializeEventRecord((byte[])(await this.Database.HashGetAsync(streamId, hashKey).ConfigureAwait(false))!);

    }

    /// <inheritdoc/>
    public virtual async Task<IObservable<IEventRecord>> SubscribeAsync(string streamId, long offset = Infrastructure.EventSourcing.StreamPosition.EndOfStream, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(streamId)) throw new ArgumentNullException(nameof(streamId));
        if (!await this.Database.KeyExistsAsync(streamId).ConfigureAwait(false)) throw new StreamNotFoundException(streamId);

        var events = offset == Infrastructure.EventSourcing.StreamPosition.EndOfStream ? Array.Empty<IEventRecord>().ToList() : await (this.ReadAsync(streamId, StreamReadDirection.Forwards, offset, cancellationToken: cancellationToken)).ToListAsync(cancellationToken).ConfigureAwait(false);
        var messageQueue = await this.Subscriber.SubscribeAsync(this.GetRedisChannelFor(streamId));
        var observable = Observable.Using(() => new RedisSubscription(messageQueue), _ => messageQueue.Select(m => this.DeserializeEventRecord(m.Message)).ToObservable());

        return Observable.StartWith(observable, events);
    }

    /// <inheritdoc/>
    public virtual async Task TruncateAsync(string streamId, ulong? beforeVersion = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(streamId)) throw new ArgumentNullException(nameof(streamId));
        if (!await this.Database.KeyExistsAsync(streamId).ConfigureAwait(false)) throw new StreamNotFoundException(streamId);

        var hashKeys = (await this.Database.HashKeysAsync(streamId).ConfigureAwait(false)).Order().ToList();
        if (!hashKeys.Any()) return;

        var beforeElement = hashKeys.Select(k => (ulong?)k).FirstOrDefault(o => o >= beforeVersion);
        if (beforeElement != null)
        {
            var index = hashKeys.Select(k => (ulong?)k).ToList().IndexOf(beforeElement);
            hashKeys = hashKeys.Take(index).ToList();
        }
        await this.Database.HashDeleteAsync(streamId, hashKeys.ToArray()).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public virtual async Task DeleteAsync(string streamId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(streamId)) throw new ArgumentNullException(nameof(streamId));
        if (!await this.Database.KeyExistsAsync(streamId).ConfigureAwait(false)) throw new StreamNotFoundException(streamId);

        await this.Database.KeyDeleteAsync(streamId).ConfigureAwait(false);
    }

    /// <summary>
    /// Deserializes the specified <see cref="RedisValue"/> into a new <see cref="IEventRecord"/>
    /// </summary>
    /// <param name="value">The <see cref="RedisValue"/> to deserialize</param>
    /// <returns>The deserialized <see cref="IEventRecord"/></returns>
    protected virtual IEventRecord DeserializeEventRecord(RedisValue value)
    {
        var byteArray = (byte[])value!;
        var record = this.Serializer.Deserialize<EventRecord>(byteArray)!;
        var clrTypeName = record.Metadata![EventRecordMetadata.ClrTypeName].ToString()!;
        var clrType = Type.GetType(clrTypeName) ?? throw new Exception();
        record.Data = this.Serializer.Convert(record.Data, clrType);
        record.Metadata.Remove(EventRecordMetadata.ClrTypeName);
        if (!record.Metadata.Any()) record.Metadata = null;
        return record;
    }

    /// <summary>
    /// Gets the <see cref="RedisChannel"/> for the specified stream id
    /// </summary>
    /// <param name="streamId">The id of the stream to get the <see cref="RedisChannel"/> for</param>
    /// <returns>The <see cref="RedisChannel"/> for the specified stream id</returns>
    protected virtual RedisChannel GetRedisChannelFor(string streamId) => new($"{streamId}_rxs", RedisChannel.PatternMode.Literal);

    /// <summary>
    /// Exposes constants about event related metadata used by the <see cref="RedisEventStore"/>
    /// </summary>
    protected static class EventRecordMetadata
    {

        /// <summary>
        /// Gets the name of the event record metadata used to store the event CLR type's assembly qualified name
        /// </summary>
        public const string ClrTypeName = "clr-type";

    }

    class RedisSubscription
        : IDisposable
    {

        private readonly ChannelMessageQueue _queue;

        public RedisSubscription(ChannelMessageQueue queue)
        {
            this._queue = queue;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this._queue.Unsubscribe();
            GC.SuppressFinalize(this);
        }

    }

}
