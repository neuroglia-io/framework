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

using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;

namespace Neuroglia.Data.Infrastructure.Redis.Services;

/// <summary>
/// Represents the Redis implementation of the <see cref="ITextDocumentRepository"/> interface
/// </summary>
/// <typeparam name="TKey">The type of key used to uniquely identify documents</typeparam>
public class RedisTextDocumentRepository<TKey>
    : ITextDocumentRepository<TKey>, IDisposable, IAsyncDisposable
    where TKey : IEquatable<TKey>
{

    /// <summary>
    /// Gets the name of the <see cref="RedisDatabase"/>'s connection string
    /// </summary>
    public const string ConnectionStringName = "redis";
    const string _ListKey = "text-documents-list";
    static readonly RedisChannel WatchEventChannel = new("text-document-watch-events", RedisChannel.PatternMode.Literal);

    bool _subscribed;
    bool _disposed;

    /// <summary>
    /// Initializes a new <see cref="RedisTextDocumentRepository{TKey}"/>
    /// </summary>
    /// <param name="loggerFactory">The service used to create <see cref="ILogger"/>s</param>
    /// <param name="redis">The current <see cref="IConnectionMultiplexer"/></param>
    /// <param name="jsonSerializer">The service used to serialize/deserialize data to/from JSON</param>
    public RedisTextDocumentRepository(ILoggerFactory loggerFactory, IConnectionMultiplexer redis, IJsonSerializer jsonSerializer)
    {
        this.Logger = loggerFactory.CreateLogger(this.GetType());
        this.Redis = redis;
        this.JsonSerializer = jsonSerializer;
        this.Database = redis.GetDatabase();
        this.Subscriber = redis.GetSubscriber();
    }

    /// <summary>
    /// Gets the service used to perform logging
    /// </summary>
    protected ILogger Logger { get; }

    /// <summary>
    /// Gets the current <see cref="IConnectionMultiplexer"/>
    /// </summary>
    protected IConnectionMultiplexer Redis { get; }

    /// <summary>
    /// Gets the service used to serialize/deserialize data to/from JSON
    /// </summary>
    protected IJsonSerializer JsonSerializer { get; }

    /// <summary>
    /// Gets the current <see cref="IDatabase"/>
    /// </summary>
    protected IDatabase Database { get; }

    /// <summary>
    /// Gets the current <see cref="ISubscriber"/>
    /// </summary>
    protected ISubscriber Subscriber { get; }

    /// <summary>
    /// Gets the <see cref="Subject{T}"/> used to observe <see cref="ITextDocument"/> watch events
    /// </summary>
    protected Subject<ITextDocumentWatchEvent> WatchEvents { get; } = new();

    /// <summary>
    /// Gets the <see cref="RedisTextDocumentRepository{TKey}"/>'s <see cref="System.Threading.CancellationTokenSource"/>
    /// </summary>
    protected CancellationTokenSource CancellationTokenSource { get; } = new();

    /// <inheritdoc/>
    public virtual async Task<ITextDocument<TKey>?> GetAsync(TKey key, CancellationToken cancellationToken = default)
    {
        var documentKey = this.BuildKey(key);
        if (!await this.Database.KeyExistsAsync(documentKey).ConfigureAwait(false)) return null;
        return await this.ReadDocumentMetadataAsync(documentKey, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public virtual async IAsyncEnumerable<ITextDocument<TKey>> GetAllAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        foreach(var key in await this.Database.ListRangeAsync(_ListKey).ConfigureAwait(false)) yield return await this.ReadDocumentMetadataAsync(key!, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public virtual async Task<ICollection<ITextDocument<TKey>>> ListAsync(int? max = null, int? skip = null, CancellationToken cancellationToken = default) => await this.GetAllAsync(cancellationToken).ToListAsync(cancellationToken).ConfigureAwait(false);

    /// <inheritdoc/>
    public virtual async Task<string> ReadToEndAsync(TKey key, CancellationToken cancellationToken = default) => (await this.Database.StringGetAsync(this.BuildKey(key)).ConfigureAwait(false))!;

    /// <inheritdoc/>
    public virtual async IAsyncEnumerable<string> ReadAsync(TKey key, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var text = await this.ReadToEndAsync(key, cancellationToken).ConfigureAwait(false);
        foreach(var line in text.Split('\n')) yield return line;
    }

    /// <inheritdoc/>
    public virtual async Task<ITextDocumentWatch> WatchAsync(TKey key, CancellationToken cancellationToken = default)
    {
        if (!this._subscribed)
        {
            await this.Subscriber.SubscribeAsync(WatchEventChannel, OnWatchEvent).ConfigureAwait(false);
            this._subscribed = true;
        }
        var observable = this.WatchEvents.Where(e => key.Equals(e.Key));
        return new TextDocumentWatch(observable, false);
    }

    /// <inheritdoc/>
    public virtual async Task AppendAsync(TKey key, string text, CancellationToken cancellationToken = default)
    {
        var documentKey = this.BuildKey(key);
        var watchEventType = await this.Database.KeyExistsAsync(documentKey).ConfigureAwait(false)
            ? TextDocumentWatchEventType.Appended 
            : TextDocumentWatchEventType.Created;
        var watchEvent = new TextDocumentWatchEvent(watchEventType, text);
        var json = this.JsonSerializer.SerializeToText(watchEvent);
        await this.Database.StringAppendAsync(documentKey, text).ConfigureAwait(false);
        await this.WriteDocumentMetadataAsync(documentKey, cancellationToken).ConfigureAwait(false);
        if (watchEventType == TextDocumentWatchEventType.Created) await this.Database.ListRightPushAsync(_ListKey, documentKey).ConfigureAwait(false);
        await this.Database.PublishAsync(WatchEventChannel, json).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public virtual async Task ReplaceAsync(TKey key, string text, CancellationToken cancellationToken = default)
    {
        var documentKey = this.BuildKey(key);
        var watchEventType = await this.Database.KeyExistsAsync(documentKey).ConfigureAwait(false)
           ? TextDocumentWatchEventType.Replaced
           : TextDocumentWatchEventType.Created;
        var watchEvent = new TextDocumentWatchEvent(watchEventType, text);
        var json = this.JsonSerializer.SerializeToText(watchEvent);
        await this.Database.StringSetAsync(documentKey, text).ConfigureAwait(false);
        await this.WriteDocumentMetadataAsync(documentKey, cancellationToken).ConfigureAwait(false);
        await this.Database.ListRightPushAsync(_ListKey, documentKey).ConfigureAwait(false);
        await this.Database.PublishAsync(WatchEventChannel, json).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public virtual async Task DeleteAsync(TKey key, CancellationToken cancellationToken = default)
    {
        var documentKey = this.BuildKey(key);
        if (!await this.Database.KeyExistsAsync(documentKey).ConfigureAwait(false)) throw new NullReferenceException($"Failed to find a document with the specified key '{documentKey}'");
        await this.Database.KeyDeleteAsync(documentKey).ConfigureAwait(false);
        await this.Database.ListRemoveAsync(_ListKey, documentKey).ConfigureAwait(false);
    }

    /// <summary>
    /// Writes the metadata of the <see cref="ITextDocument"/> with the specified key
    /// </summary>
    /// <param name="key">The key of the <see cref="ITextDocument"/> to write</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    protected virtual async Task WriteDocumentMetadataAsync(string key, CancellationToken cancellationToken = default)
    {
        var metadataKey = this.BuildDocumentMetadataKey(key);
        var metadata = await this.Database.KeyExistsAsync(metadataKey).ConfigureAwait(false)
            ? await this.ReadDocumentMetadataAsync(key, cancellationToken).ConfigureAwait(false)
            : new TextDocument<TKey>()
            {
                Key = key,
                CreatedAt = DateTimeOffset.Now,
                LastModified = DateTimeOffset.Now
            };
        metadata.LastModified = DateTimeOffset.Now;
        metadata.Length = await this.Database.StringLengthAsync(this.BuildDocumentKey(key)).ConfigureAwait(false);
        var json = this.JsonSerializer.SerializeToText(metadata);
        await this.Database.StringSetAsync(metadataKey, json).ConfigureAwait(false);
    }

    /// <summary>
    /// Reads the <see cref="ITextDocument"/> with the specified key, if any
    /// </summary>
    /// <param name="key">The key of the <see cref="ITextDocument"/> to get</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The <see cref="ITextDocument"/> with the specified key, if any</returns>
    protected virtual async Task<ITextDocument<TKey>> ReadDocumentMetadataAsync(string key, CancellationToken cancellationToken = default)
    {
        var metadataKey = this.BuildDocumentMetadataKey(key);
        var json = await this.Database.StringGetAsync(metadataKey).ConfigureAwait(false);
        return this.JsonSerializer.Deserialize<TextDocument<TKey>>(json!)!;
    }

    /// <summary>
    /// Builds a new key used to store the contents of the document with the specified key
    /// </summary>
    /// <param name="key">The key of the document to build a new key for</param>
    /// <returns>A new key used to store the contents of the document with the specified key</returns>
    protected virtual string BuildKey(TKey key) => $"{key}";

    /// <summary>
    /// Builds a new key used to store the metadata of the document with the specified key
    /// </summary>
    /// <param name="key">The key of the document to build a new metadata key for</param>
    /// <returns>A new key used to store the metadata of the document with the specified key</returns>
    protected virtual string BuildMetadataKey(TKey key) => $"{key}_metadata";

    /// <summary>
    /// Handles a watch event published on Redis
    /// </summary>
    /// <param name="channel">The channel the watch event has been published to</param>
    /// <param name="json">The <see cref="RedisValue"/> that wraps the JSON of the published watch event</param>
    protected virtual void OnWatchEvent(RedisChannel channel, RedisValue json)
    {
        if (channel != WatchEventChannel) return;
        var e = this.JsonSerializer.Deserialize<TextDocumentWatchEvent>(json.ToString())!;
        this.WatchEvents.OnNext(e);
    }

    async Task<ITextDocument?> ITextDocumentRepository.GetAsync(object key, CancellationToken cancellationToken) => await this.GetAsync((TKey)key, cancellationToken).ConfigureAwait(false);

    IAsyncEnumerable<ITextDocument> ITextDocumentRepository.GetAllAsync(CancellationToken cancellationToken) => this.GetAllAsync(cancellationToken);

    async Task<ICollection<ITextDocument>> ITextDocumentRepository.ListAsync(int? max, int? skip, CancellationToken cancellationToken) => (await this.ListAsync(max, skip, cancellationToken).ConfigureAwait(false)).OfType<ITextDocument>().ToList();

    IAsyncEnumerable<string> ITextDocumentRepository.ReadAsync(object key, CancellationToken cancellationToken) => this.ReadAsync((TKey)key, cancellationToken);

    Task<string> ITextDocumentRepository.ReadToEndAsync(object key, CancellationToken cancellationToken) => this.ReadToEndAsync((TKey)key, cancellationToken);

    Task<ITextDocumentWatch> ITextDocumentRepository.WatchAsync(object key, CancellationToken cancellationToken) => this.WatchAsync((TKey)key, cancellationToken);

    Task ITextDocumentRepository.AppendAsync(object key, string text, CancellationToken cancellationToken) => this.AppendAsync((TKey)key, text, cancellationToken);

    Task ITextDocumentRepository.ReplaceAsync(object key, string text, CancellationToken cancellationToken) => this.ReplaceAsync((TKey)key, text, cancellationToken);

    Task ITextDocumentRepository.DeleteAsync(object key, CancellationToken cancellationToken) => this.DeleteAsync((TKey)key, cancellationToken);

    /// <summary>
    /// Disposes of the <see cref="RedisTextDocumentRepository{TKey}"/>
    /// </summary>
    /// <param name="disposing">A boolean indicating whether or not to dispose of the <see cref="RedisTextDocumentRepository{TKey}"/></param>
    /// <returns>A new awaitable <see cref="ValueTask"/></returns>
    protected virtual async ValueTask DisposeAsync(bool disposing)
    {
        if (!this._disposed || !disposing) return;
        this.CancellationTokenSource?.Dispose();
        this.WatchEvents.Dispose();
        await this.Subscriber.UnsubscribeAsync(WatchEventChannel, this.OnWatchEvent).ConfigureAwait(false);
        this._disposed = true;
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        await this.DisposeAsync(true).ConfigureAwait(false);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes of the <see cref="RedisDatabase"/>
    /// </summary>
    /// <param name="disposing">A boolean indicating whether or not the <see cref="RedisDatabase"/> is being disposed of</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!this._disposed)
        {
            if (disposing)
            {
                this.CancellationTokenSource?.Dispose();
                this.WatchEvents.Dispose();
                this.Subscriber.Unsubscribe(WatchEventChannel, this.OnWatchEvent);
            }
            this._disposed = true;
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

}
