/*
 * Copyright © 2021 Neuroglia SPRL. All rights reserved.
 * <p>
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * <p>
 * http://www.apache.org/licenses/LICENSE-2.0
 * <p>
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 */
using Microsoft.Extensions.Options;
using Neuroglia.Serialization;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core.Configuration;
using StackExchange.Redis.Extensions.Core.ServerIteration;
using StackExchange.Redis.KeyspaceIsolation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Neuroglia.Caching
{

    /// <summary>
    /// Represents the StackExchange.Redis implementation of the <see cref="IDistributedCache"/> interface
    /// </summary>
    public class RedisCache
        : IDistributedCache
    {

        /// <summary>
        /// Gets the name of the hash field used to store a cache entry's value
        /// </summary>
        private const string ValueField = "value";
        /// <summary>
        /// Gets the name of the hash field used to store a cache entry's concurrency token
        /// </summary>
        private const string ConcurrencyTokenField = "token";
        /// <summary>
        /// Gets the name of the hash field used to store a cache entry's absolute expiration
        /// </summary>
        private const string AbsoluteExpirationField = "expiration_absolute";
        /// <summary>
        /// Gets the name of the hash field used to store a cache entry's sliding expiration
        /// </summary>
        private const string SlidingExpirationField = "expiration_sliding";

        /// <summary>
        /// Initializes a new <see cref="RedisCache"/>
        /// </summary>
        /// <param name="options">The service used to access the current <see cref="RedisCacheOptions"/></param>
        /// <param name="connection">The connection used to connect to the Redis cache</param>
        /// <param name="serializer">The service used to serialize and deserialize cache entries</param>
        public RedisCache(IOptions<RedisCacheOptions> options, IConnectionMultiplexer connection, ISerializer serializer)
        {
            this.Options = options.Value;
            this.Connection = connection;
            this.Serializer = serializer;
            this.Cache = this.Connection.GetDatabase();
            if (!string.IsNullOrWhiteSpace(this.KeyPrefix))
                this.Cache = this.Cache.WithKeyPrefix(this.KeyPrefix);
        }

        /// <summary>
        /// Gets the options used to configure the <see cref="RedisCache"/>
        /// </summary>
        protected RedisCacheOptions Options { get; }

        /// <summary>
        /// Gets the connection used to connect to the Redis cache
        /// </summary>
        protected IConnectionMultiplexer Connection { get; }

        /// <summary>
        /// Gets the service used to serialize and deserialize cache entries
        /// </summary>
        protected ISerializer Serializer { get; }

        /// <summary>
        /// Gets the Redis cache database
        /// </summary>
        protected IDatabase Cache { get; }

        /// <summary>
        /// Gets the key prefix used by the <see cref="IConnectionMultiplexer"/> to isolate the cache
        /// </summary>
        protected string KeyPrefix
        {
            get
            {
                return this.Options.InstanceName;
            }
        }

        /// <summary>
        /// Gets a boolean indicating whether or not the cache contains the specified key
        /// </summary>
        /// <param name="key">The key to check</param>
        /// <returns>A boolean indicating whether or not the cache contains the specified key</returns>
        public virtual bool ContainsKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));
            return this.Cache.KeyExists(key);
        }

        /// <summary>
        /// Gets a boolean indicating whether or not the cache contains the specified key
        /// </summary>
        /// <param name="key">The key to check</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A boolean indicating whether or not the cache contains the specified key</returns>
        public virtual async Task<bool> ContainsKeyAsync(string key, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));
            return await this.Cache.KeyExistsAsync(key);
        }

        /// <summary>
        /// Determines whether the specified list contains an element with the specified key
        /// </summary>
        /// <param name="key">The key of the list to check</param>
        /// <param name="elemKey">The key of the element to check for existence</param>
        /// <returns>A boolean indicating whether or not the specified list contains an element with the specified key</returns>
        public virtual bool ContainsListElement(string key, string elemKey)
        {
            return this.Cache.HashExists(key, elemKey);
        }

        /// <summary>
        /// Determines whether the specified list contains an element with the specified key
        /// </summary>
        /// <param name="key">The key of the list to check</param>
        /// <param name="elemKey">The key of the element to check for existence</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A boolean indicating whether or not the specified list contains an element with the specified key</returns>
        public virtual async Task<bool> ContainsListElementAsync(string key, string elemKey, CancellationToken cancellationToken = default)
        {
            return await this.Cache.HashExistsAsync(key, elemKey);
        }

        /// <summary>
        /// Finds all keys that match the specified pattern
        /// </summary>
        /// <param name="pattern">The pattern to match</param>
        /// <returns>A new <see cref="IEnumerable{T}"/> containing all the keys that match the specified pattern</returns>
        public virtual IEnumerable<string> FindKeys(string pattern)
        {
            if (string.IsNullOrWhiteSpace(pattern))
                throw new ArgumentNullException(nameof(pattern));
            pattern = $"{this.KeyPrefix}{pattern}";
            HashSet<string> keys = new();
            IServer[] servers = ServerIteratorFactory.GetServers(this.Connection, this.Options.ServerEnumerationStrategy ?? new ServerEnumerationStrategy()).ToArray();
            if (!servers.Any())
                throw new Exception("No server found to serve the KEYS command");
            foreach (var server in servers)
            {
                int nextCursor = 0;
                do
                {
                    RedisResult redisResult = this.Cache.Execute("SCAN", nextCursor.ToString(), "MATCH", pattern, "COUNT", "1000");
                    RedisResult[] innerResult = (RedisResult[])redisResult;
                    nextCursor = int.Parse((string)innerResult[0]);
                    List<string> resultLines = ((string[])innerResult[1]).ToList();
                    keys.UnionWith(resultLines);
                }
                while (nextCursor != 0);
            }
            return !string.IsNullOrEmpty(this.KeyPrefix) ? keys.Select(k => k[this.KeyPrefix.Length..]) : keys;
        }

        /// <summary>
        /// Finds all keys that match the specified pattern
        /// </summary>
        /// <param name="pattern">The pattern to match</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A new <see cref="IEnumerable{T}"/> containing all the keys that match the specified pattern</returns>
        public virtual async Task<IEnumerable<string>> FindKeysAsync(string pattern, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(pattern))
                throw new ArgumentNullException(nameof(pattern));
            if (string.IsNullOrWhiteSpace(pattern))
                throw new ArgumentNullException(nameof(pattern));
            pattern = $"{this.KeyPrefix}{pattern}";
            HashSet<string> keys = new();
            IServer[] servers = ServerIteratorFactory.GetServers(this.Connection, this.Options.ServerEnumerationStrategy ?? new ServerEnumerationStrategy()).ToArray();
            if (!servers.Any())
                throw new Exception("No server found to serve the KEYS command");
            foreach (var server in servers)
            {
                int nextCursor = 0;
                do
                {
                    RedisResult redisResult = await this.Cache.ExecuteAsync("SCAN", nextCursor.ToString(), "MATCH", pattern, "COUNT", "1000");
                    RedisResult[] innerResult = (RedisResult[])redisResult;
                    nextCursor = int.Parse((string)innerResult[0]);
                    List<string> resultLines = ((string[])innerResult[1]).ToList();
                    keys.UnionWith(resultLines);
                }
                while (nextCursor != 0);
            }
            return !string.IsNullOrEmpty(this.KeyPrefix) ? keys.Select(k => k[this.KeyPrefix.Length..]) : keys;
        }

        /// <summary>
        /// Retrieves all the element keys of the list with the specified key
        /// </summary>
        /// <param name="key">The key of the list for which to retrieve all the element keys</param>
        /// <returns>A new <see cref="IEnumerable{T}"/> containing all the element keys of the list with the specified key</returns>
        public virtual IEnumerable<string> GetListKeys(string key)
        {
            return this.Cache.HashKeys(key).Select(hf => hf.ToString());
        }

        /// <summary>
        /// Retrieves all the element keys of the list with the specified key
        /// </summary>
        /// <param name="key">The key of the list for which to retrieve all the element keys</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A new <see cref="IEnumerable{T}"/> containing all the element keys of the list with the specified key</returns>
        public virtual async Task<IEnumerable<string>> GetListKeysAsync(string key, CancellationToken cancellationToken = default)
        {
            return (await this.Cache.HashKeysAsync(key)).Select(hf => hf.ToString());
        }

        /// <summary>
        /// Locks access to the specified key
        /// </summary>
        /// <param name="key">The key to lock</param>
        /// <param name="token">The lock's token</param>
        /// <param name="expiresIn">The time in which the lock expires</param>
        /// <returns></returns>
        public virtual IDisposable Lock(string key, string token, TimeSpan expiresIn)
        {
            if (!this.Cache.LockTake($"{key}_lock", token, expiresIn))
                throw new PessimisticConcurrencyException($"A pessimistic concurrency exception has occured because the specified key '{key}' is already locked");
            return new CacheLock(this, key, token, expiresIn);
        }

        /// <summary>
        /// Releases the lock on the specified key
        /// </summary>
        /// <param name="key">The key to release the lock of</param>
        /// <param name="token">The token of the lock to release</param>
        public virtual void ReleaseLock(string key, string token)
        {
            this.Cache.LockRelease($"{key}_lock", token);
        }

        /// <summary>
        /// Gets the <see cref="ICacheEntry{T}"/> that matches the specified key
        /// </summary>
        /// <typeparam name="T">The type of data cached by the entry</typeparam>
        /// <param name="key">The key of the <see cref="ICacheEntry{T}"/> to retrieve</param>
        /// <returns>The <see cref="ICacheEntry{T}"/> that matches the specified key</returns>
        public virtual ICacheEntry<T> GetEntry<T>(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));
            ICacheEntry<T> entry = this.MapToCacheEntry<T>(key);
            if (entry != null
                && entry.SlidingExpiration.HasValue)
            {
                DateTime absoluteExpiration = DateTime.UtcNow.Add(entry.SlidingExpiration.Value);
                StackExchange.Redis.ITransaction transaction = this.Cache.CreateTransaction();
                transaction.AddCondition(Condition.HashEqual(key, RedisCache.ConcurrencyTokenField, entry.ConcurrencyToken));
                transaction.HashSetAsync(key, RedisCache.AbsoluteExpirationField, absoluteExpiration.Ticks);
                transaction.KeyExpireAsync(key, absoluteExpiration);
                transaction.Execute();
            }
            return entry;
        }

        /// <summary>
        /// Gets the <see cref="ICacheEntry{T}"/> that matches the specified key
        /// </summary>
        /// <typeparam name="T">The type of data cached by the entry</typeparam>
        /// <param name="key">The key of the <see cref="ICacheEntry{T}"/> to retrieve</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>The <see cref="ICacheEntry{T}"/> that matches the specified key</returns>
        public virtual async Task<ICacheEntry<T>> GetEntryAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));
            ICacheEntry<T> entry = await this.MapToCacheEntryAsync<T>(key);
            if (entry != null
                && entry.SlidingExpiration.HasValue)
            {
                DateTime absoluteExpiration = DateTime.UtcNow.Add(entry.SlidingExpiration.Value);
                StackExchange.Redis.ITransaction transaction = this.Cache.CreateTransaction();
                transaction.AddCondition(Condition.HashEqual(key, RedisCache.ConcurrencyTokenField, entry.ConcurrencyToken));
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                transaction.HashSetAsync(key, RedisCache.AbsoluteExpirationField, absoluteExpiration.Ticks);
                transaction.KeyExpireAsync(key, absoluteExpiration);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                await transaction.ExecuteAsync();
            }
            return entry;
        }

        /// <summary>
        /// Gets the value cached with the specified key
        /// </summary>
        /// <typeparam name="T">The type of the cached value</typeparam>
        /// <param name="key">The key of the value to retrieve</param>
        /// <returns>The value cached with the specified key</returns>
        public virtual T Get<T>(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));
            ICacheEntry<T> cacheEntry = this.GetEntry<T>(key);
            if (cacheEntry == null)
                return default;
            return cacheEntry.Value;
        }

        /// <summary>
        /// Gets the value cached with the specified key
        /// </summary>
        /// <typeparam name="T">The type of the cached value</typeparam>
        /// <param name="key">The key of the value to retrieve</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>The value cached with the specified key</returns>
        public virtual async Task<T> GetAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));
            ICacheEntry<T> cacheEntry = await this.GetEntryAsync<T>(key, cancellationToken);
            if (cacheEntry == null)
                return default;
            return cacheEntry.Value;
        }

        /// <summary>
        /// Sets the value of the specified key
        /// </summary>
        /// <typeparam name="T">The type of value to cache</typeparam>
        /// <param name="key">The key of the value to cache</param>
        /// <param name="value">The value to cache</param>
        public virtual void Set<T>(string key, T value)
        {
            this.SetInternal(key, value, new CacheEntryOptions());
        }

        /// <summary>
        /// Sets the value of the specified key
        /// </summary>
        /// <typeparam name="T">The type of value to cache</typeparam>
        /// <param name="key">The key of the value to cache</param>
        /// <param name="value">The value to cache</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        public virtual async Task SetAsync<T>(string key, T value, CancellationToken cancellationToken = default)
        {
            await this.SetInternalAsync(key, value, new CacheEntryOptions());
        }

        /// <summary>
        /// Sets the value of the specified key
        /// </summary>
        /// <typeparam name="T">The type of value to cache</typeparam>
        /// <param name="key">The key of the value to cache</param>
        /// <param name="value">The value to cache</param>
        /// <param name="options">The options configuring how to cache the value</param>
        public virtual void Set<T>(string key, T value, CacheEntryOptions options)
        {
            this.SetInternal(key, value, options);
        }

        /// <summary>
        /// Sets the value of the specified key
        /// </summary>
        /// <typeparam name="T">The type of value to cache</typeparam>
        /// <param name="key">The key of the value to cache</param>
        /// <param name="value">The value to cache</param>
        /// <param name="options">The options configuring how to cache the value</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        public virtual async Task SetAsync<T>(string key, T value, CacheEntryOptions options, CancellationToken cancellationToken = default)
        {
            await this.SetInternalAsync(key, value, options);
        }

        /// <summary>
        /// Sets the value of the specified key
        /// </summary>
        /// <typeparam name="T">The type of value to cache</typeparam>
        /// <param name="key">The key of the value to cache</param>
        /// <param name="value">The value to cache</param>
        /// <param name="concurrencyToken">The concurrency token used for optimistic persistency</param>
        public virtual void Set<T>(string key, T value, byte[] concurrencyToken)
        {
            this.SetInternal(key, value, concurrencyToken, new CacheEntryOptions());
        }

        /// <summary>
        /// Sets the value of the specified key
        /// </summary>
        /// <typeparam name="T">The type of value to cache</typeparam>
        /// <param name="key">The key of the value to cache</param>
        /// <param name="value">The value to cache</param>
        /// <param name="concurrencyToken">The concurrency token used for optimistic persistency</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        public virtual async Task SetAsync<T>(string key, T value, byte[] concurrencyToken, CancellationToken cancellationToken = default)
        {
            await this.SetInternalAsync(key, value, concurrencyToken, new CacheEntryOptions());
        }

        /// <summary>
        /// Sets the value of the specified key
        /// </summary>
        /// <typeparam name="T">The type of value to cache</typeparam>
        /// <param name="key">The key of the value to cache</param>
        /// <param name="value">The value to cache</param>
        /// <param name="concurrencyToken">The concurrency token used for optimistic persistency</param>
        /// <param name="options">The options configuring how to cache the value</param>
        public virtual void Set<T>(string key, T value, byte[] concurrencyToken, CacheEntryOptions options)
        {
            this.SetInternal(key, value, concurrencyToken, options);
        }

        /// <summary>
        /// Sets the value of the specified key
        /// </summary>
        /// <typeparam name="T">The type of value to cache</typeparam>
        /// <param name="key">The key of the value to cache</param>
        /// <param name="value">The value to cache</param>
        /// <param name="concurrencyToken">The concurrency token used for optimistic persistency</param>
        /// <param name="options">The options configuring how to cache the value</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        public virtual async Task SetAsync<T>(string key, T value, byte[] concurrencyToken, CacheEntryOptions options, CancellationToken cancellationToken = default)
        {
            await this.SetInternalAsync(key, value, concurrencyToken, options);
        }

        /// <summary>
        /// Sets the value of the specified key
        /// </summary>
        /// <typeparam name="T">The type of value to cache</typeparam>
        /// <param name="key">The key of the value to cache</param>
        /// <param name="value">The value to cache</param>
        /// <param name="expiresAt">The date and time at which the entry expires</param>
        public virtual void Set<T>(string key, T value, DateTimeOffset expiresAt)
        {
            this.SetInternal(key, value, new CacheEntryOptions() { AbsoluteExpiration = expiresAt.UtcDateTime });
        }

        /// <summary>
        /// Sets the value of the specified key
        /// </summary>
        /// <typeparam name="T">The type of value to cache</typeparam>
        /// <param name="key">The key of the value to cache</param>
        /// <param name="value">The value to cache</param>
        /// <param name="expiresAt">The date and time at which the entry expires</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        public virtual async Task SetAsync<T>(string key, T value, DateTimeOffset expiresAt, CancellationToken cancellationToken = default)
        {
            await this.SetInternalAsync(key, value, new CacheEntryOptions() { AbsoluteExpiration = expiresAt.UtcDateTime });
        }

        /// <summary>
        /// Sets the value of the specified key
        /// </summary>
        /// <typeparam name="T">The type of value to cache</typeparam>
        /// <param name="key">The key of the value to cache</param>
        /// <param name="value">The value to cache</param>
        /// <param name="expiresAt">The date and time at which the entry expires</param>
        /// <param name="concurrencyToken">The concurrency token used for optimistic persistency</param>
        public virtual void Set<T>(string key, T value, DateTimeOffset expiresAt, byte[] concurrencyToken)
        {
            this.SetInternal(key, value, concurrencyToken, new CacheEntryOptions() { AbsoluteExpiration = expiresAt.UtcDateTime });
        }

        /// <summary>
        /// Sets the value of the specified key
        /// </summary>
        /// <typeparam name="T">The type of value to cache</typeparam>
        /// <param name="key">The key of the value to cache</param>
        /// <param name="value">The value to cache</param>
        /// <param name="expiresAt">The date and time at which the entry expires</param>
        /// <param name="concurrencyToken">The concurrency token used for optimistic persistency</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        public virtual async Task SetAsync<T>(string key, T value, DateTimeOffset expiresAt, byte[] concurrencyToken, CancellationToken cancellationToken = default)
        {
            await this.SetInternalAsync(key, value, concurrencyToken, new CacheEntryOptions() { AbsoluteExpiration = expiresAt.UtcDateTime });
        }

        /// <summary>
        /// Sets the value of the specified key
        /// </summary>
        /// <typeparam name="T">The type of value to cache</typeparam>
        /// <param name="key">The key of the value to cache</param>
        /// <param name="value">The value to cache</param>
        /// <param name="expiresIn">The time in which the entry expires</param>
        public virtual void Set<T>(string key, T value, TimeSpan expiresIn)
        {
            this.SetInternal(key, value, new CacheEntryOptions() { RelativeExpiration = expiresIn });
        }

        /// <summary>
        /// Sets the value of the specified key
        /// </summary>
        /// <typeparam name="T">The type of value to cache</typeparam>
        /// <param name="key">The key of the value to cache</param>
        /// <param name="value">The value to cache</param>
        /// <param name="expiresIn">The time in which the entry expires</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        public virtual async Task SetAsync<T>(string key, T value, TimeSpan expiresIn, CancellationToken cancellationToken = default)
        {
            await this.SetInternalAsync(key, value, new CacheEntryOptions() { RelativeExpiration = expiresIn });
        }

        /// <summary>
        /// Sets the value of the specified key
        /// </summary>
        /// <typeparam name="T">The type of value to cache</typeparam>
        /// <param name="key">The key of the value to cache</param>
        /// <param name="value">The value to cache</param>
        /// <param name="expiresIn">The time in which the entry expires</param>
        /// <param name="concurrencyToken">The concurrency token used for optimistic persistency</param>
        public virtual void Set<T>(string key, T value, TimeSpan expiresIn, byte[] concurrencyToken)
        {
            this.SetInternal(key, value, concurrencyToken, new CacheEntryOptions() { RelativeExpiration = expiresIn });
        }

        /// <summary>
        /// Sets the value of the specified key
        /// </summary>
        /// <typeparam name="T">The type of value to cache</typeparam>
        /// <param name="key">The key of the value to cache</param>
        /// <param name="value">The value to cache</param>
        /// <param name="expiresIn">The time in which the entry expires</param>
        /// <param name="concurrencyToken">The concurrency token used for optimistic persistency</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        public virtual async Task SetAsync<T>(string key, T value, TimeSpan expiresIn, byte[] concurrencyToken, CancellationToken cancellationToken = default)
        {
            await this.SetInternalAsync(key, value, concurrencyToken, new CacheEntryOptions() { RelativeExpiration = expiresIn });
        }

        /// <summary>
        /// Removes the specified key
        /// </summary>
        /// <param name="key">The key to remove</param>
        /// <returns>A boolean indicating whether or not the key could be removed</returns>
        public virtual bool Remove(string key)
        {
            return this.Cache.KeyDelete(key);
        }

        /// <summary>
        /// Removes the specified key
        /// </summary>
        /// <param name="key">The key to remove</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A boolean indicating whether or not the key could be removed</returns>
        public virtual async Task<bool> RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            return await this.Cache.KeyDeleteAsync(key);
        }

        /// <summary>
        /// Gets the list stored at the specified key
        /// </summary>
        /// <typeparam name="T">The type of elements held in the list</typeparam>
        /// <param name="key">The key of the list to retrieve</param>
        /// <returns>The list store at the specified key</returns>
        public virtual List<T> GetList<T>(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));
            List<T> list = new();
            foreach (HashEntry hashField in this.Cache.HashGetAll(key))
            {
                list.Add(this.DeserializeEntry<T>(hashField.Value));
            }
            return list;
        }

        /// <summary>
        /// Gets the list stored at the specified key
        /// </summary>
        /// <typeparam name="T">The type of elements held in the list</typeparam>
        /// <param name="key">The key of the list to retrieve</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>The list store at the specified key</returns>
        public virtual async Task<List<T>> GetListAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));
            List<T> list = new();
            foreach (HashEntry hashField in await this.Cache.HashGetAllAsync(key))
            {
                list.Add(this.DeserializeEntry<T>(hashField.Value));
            }
            return list;
        }

        /// <summary>
        /// Gets the specified element from the list stored at the specified key
        /// </summary>
        /// <typeparam name="T">The type of elements held in the list</typeparam>
        /// <param name="key">The key of the list to retrieve an element from</param>
        /// <param name="elemKey">The key of the element to retrieve</param>
        /// <returns>The element with the specified key, from the list stored at the specified key</returns>
        public virtual T GetListElement<T>(string key, string elemKey)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));
            if (string.IsNullOrWhiteSpace(elemKey))
                throw new ArgumentNullException(nameof(elemKey));
            string json = this.Cache.HashGet(key, elemKey);
            if (string.IsNullOrWhiteSpace(json))
                return default;
            else
                return this.DeserializeEntry<T>(json);
        }

        /// <summary>
        /// Gets the specified element from the list stored at the specified key
        /// </summary>
        /// <typeparam name="T">The type of elements held in the list</typeparam>
        /// <param name="key">The key of the list to retrieve an element from</param>
        /// <param name="elemKey">The key of the element to retrieve</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>The element with the specified key, from the list stored at the specified key</returns>
        public virtual async Task<T> GetListElementAsync<T>(string key, string elemKey, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));
            if (string.IsNullOrWhiteSpace(elemKey))
                throw new ArgumentNullException(nameof(elemKey));
            string json = await this.Cache.HashGetAsync(key, elemKey);
            if (string.IsNullOrWhiteSpace(json))
                return default;
            else
                return this.DeserializeEntry<T>(json);
        }

        /// <summary>
        /// Adds a new element to the list stored at the specified key
        /// </summary>
        /// <typeparam name="T">The type of elements held in the list</typeparam>
        /// <param name="key">The key of the list to add a new element to</param>
        /// <param name="elemKey">The key of the element to add</param>
        /// <param name="elem">The element to add</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        public virtual void AddToList<T>(string key, string elemKey, T elem)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));
            if (string.IsNullOrWhiteSpace(elemKey))
                throw new ArgumentNullException(nameof(elemKey));
            string json;
            if (elem == null)
                json = null;
            else
                json = this.SerializeEntry(elem);
            string lockToken = Guid.NewGuid().ToString();
            using IDisposable cacheLock = this.Lock(key, lockToken, TimeSpan.FromMilliseconds(500));
            this.Cache.HashSet(key, new HashEntry[] { new HashEntry(elemKey, json) });
        }

        /// <summary>
        /// Adds a new element to the list stored at the specified key
        /// </summary>
        /// <typeparam name="T">The type of elements held in the list</typeparam>
        /// <param name="key">The key of the list to add a new element to</param>
        /// <param name="elemKey">The key of the element to add</param>
        /// <param name="elem">The element to add</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        public virtual async Task AddToListAsync<T>(string key, string elemKey, T elem, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));
            if (string.IsNullOrWhiteSpace(elemKey))
                throw new ArgumentNullException(nameof(elemKey));
            string json;
            if (elem == null)
                json = null;
            else
                json = this.SerializeEntry(elem);
            string lockToken = Guid.NewGuid().ToString();
            using IDisposable cacheLock = this.Lock(key, lockToken, TimeSpan.FromMilliseconds(500));
            await this.Cache.HashSetAsync(key, new HashEntry[] { new HashEntry(elemKey, json) });
        }

        /// <summary>
        /// Adds a new element to the list stored at the specified key
        /// </summary>
        /// <typeparam name="T">The type of elements held in the list</typeparam>
        /// <param name="key">The key of the list to add a new element to</param>
        /// <param name="elemKey">The key of the element to add</param>
        /// <param name="elem">The element to add</param>
        /// <param name="expiresAt">The UTC date and time at which the cached list should expire at</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        public virtual void AddToList<T>(string key, string elemKey, T elem, DateTime expiresAt)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));
            if (string.IsNullOrWhiteSpace(elemKey))
                throw new ArgumentNullException(nameof(elemKey));
            string json;
            if (elem == null)
                json = null;
            else
                json = this.SerializeEntry(elem);
            string lockToken = Guid.NewGuid().ToString();
            using IDisposable cacheLock = this.Lock(key, lockToken, TimeSpan.FromMilliseconds(500));
            this.Cache.HashSet(key, new HashEntry[] { new HashEntry(elemKey, json) });
            this.Cache.KeyExpire(key, expiresAt);
        }

        /// <summary>
        /// Adds a new element to the list stored at the specified key
        /// </summary>
        /// <typeparam name="T">The type of elements held in the list</typeparam>
        /// <param name="key">The key of the list to add a new element to</param>
        /// <param name="elemKey">The key of the element to add</param>
        /// <param name="elem">The element to add</param>
        /// <param name="expiresAt">The UTC date and time at which the cached list should expire at</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        public virtual async Task AddToListAsync<T>(string key, string elemKey, T elem, DateTime expiresAt, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));
            if (string.IsNullOrWhiteSpace(elemKey))
                throw new ArgumentNullException(nameof(elemKey));
            string json;
            if (elem == null)
                json = null;
            else
                json = this.SerializeEntry(elem);
            string lockToken = Guid.NewGuid().ToString();
            using IDisposable cacheLock = this.Lock(key, lockToken, TimeSpan.FromMilliseconds(500));
            await this.Cache.HashSetAsync(key, new HashEntry[] { new HashEntry(elemKey, json) });
            await this.Cache.KeyExpireAsync(key, expiresAt);
        }

        /// <summary>
        /// Adds a new element to the list stored at the specified key
        /// </summary>
        /// <typeparam name="T">The type of elements held in the list</typeparam>
        /// <param name="key">The key of the list to add a new element to</param>
        /// <param name="elemKey">The key of the element to add</param>
        /// <param name="elem">The element to add</param>
        /// <param name="expiresIn">A <see cref="TimeSpan"/> that represents the time in which the list should expire</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        public virtual void AddToList<T>(string key, string elemKey, T elem, TimeSpan expiresIn)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));
            if (string.IsNullOrWhiteSpace(elemKey))
                throw new ArgumentNullException(nameof(elemKey));
            string json;
            if (elem == null)
                json = null;
            else
                json = this.SerializeEntry(elem);
            string lockToken = Guid.NewGuid().ToString();
            using IDisposable cacheLock = this.Lock(key, lockToken, TimeSpan.FromMilliseconds(500));
            this.Cache.HashSet(key, new HashEntry[] { new HashEntry(elemKey, json) });
            this.Cache.KeyExpire(key, expiresIn);
        }

        /// <summary>
        /// Adds a new element to the list stored at the specified key
        /// </summary>
        /// <typeparam name="T">The type of elements held in the list</typeparam>
        /// <param name="key">The key of the list to add a new element to</param>
        /// <param name="elemKey">The key of the element to add</param>
        /// <param name="elem">The element to add</param>
        /// <param name="expiresIn">A <see cref="TimeSpan"/> that represents the time in which the list should expire</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        public virtual async Task AddToListAsync<T>(string key, string elemKey, T elem, TimeSpan expiresIn, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));
            if (string.IsNullOrWhiteSpace(elemKey))
                throw new ArgumentNullException(nameof(elemKey));
            string json;
            if (elem == null)
                json = null;
            else
                json = this.SerializeEntry(elem);
            string lockToken = Guid.NewGuid().ToString();
            using IDisposable cacheLock = this.Lock(key, lockToken, TimeSpan.FromMilliseconds(500));
            await this.Cache.HashSetAsync(key, new HashEntry[] { new HashEntry(elemKey, json) });
            await this.Cache.KeyExpireAsync(key, expiresIn);
        }

        /// <summary>
        /// Updates an element of the list stored at the specified key
        /// </summary>
        /// <typeparam name="T">The type of elements held in the list</typeparam>
        /// <param name="key">The key of the list to update an element of</param>
        /// <param name="elemKey">The key of the element to update</param>
        /// <param name="elem">The updated element</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        public virtual void UpdateList<T>(string key, string elemKey, T elem)
        {
            string json;
            if (elem == null)
                json = null;
            else
                json = this.SerializeEntry(elem);
            string lockToken = Guid.NewGuid().ToString();
            using IDisposable cacheLock = this.Lock(key, lockToken, TimeSpan.FromMilliseconds(500));
            this.Cache.HashSet(key, new HashEntry[] { new HashEntry(elemKey, json) });
        }

        /// <summary>
        /// Updates an element of the list stored at the specified key
        /// </summary>
        /// <typeparam name="T">The type of elements held in the list</typeparam>
        /// <param name="key">The key of the list to update an element of</param>
        /// <param name="elemKey">The key of the element to update</param>
        /// <param name="elem">The updated element</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        public virtual async Task UpdateListAsync<T>(string key, string elemKey, T elem, CancellationToken cancellationToken = default)
        {
            string json;
            if (elem == null)
                json = null;
            else
                json = this.SerializeEntry(elem);
            string lockToken = Guid.NewGuid().ToString();
            using IDisposable cacheLock = this.Lock(key, lockToken, TimeSpan.FromMilliseconds(500));
            await this.Cache.HashSetAsync(key, new HashEntry[] { new HashEntry(elemKey, json) });
        }

        /// <summary>
        /// Removes an element from the list stored at the specified key
        /// </summary>
        /// <param name="key">The key of the list to remove an element from</param>
        /// <param name="elemKey">The key of the element to remove</param>
        /// <returns>A boolean indicating whether or not the element could be removed from the list</returns>
        public virtual bool RemoveFromList(string key, string elemKey)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));
            if (string.IsNullOrWhiteSpace(elemKey))
                throw new ArgumentNullException(nameof(elemKey));
            string lockToken = Guid.NewGuid().ToString();
            using IDisposable cacheLock = this.Lock(key, lockToken, TimeSpan.FromMilliseconds(500));
            bool removed = this.Cache.HashDelete(key, elemKey);
            return removed;
        }

        /// <summary>
        /// Removes an element from the list stored at the specified key
        /// </summary>
        /// <param name="key">The key of the list to remove an element from</param>
        /// <param name="elemKey">The key of the element to remove</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A boolean indicating whether or not the element could be removed from the list</returns>
        public virtual async Task<bool> RemoveFromListAsync(string key, string elemKey, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));
            if (string.IsNullOrWhiteSpace(elemKey))
                throw new ArgumentNullException(nameof(elemKey));
            string lockToken = Guid.NewGuid().ToString();
            using IDisposable cacheLock = this.Lock(key, lockToken, TimeSpan.FromMilliseconds(500));
            bool removed = await this.Cache.HashDeleteAsync(key, elemKey);
            return removed;
        }

        /// <summary>
        /// Attempts to retrieve the value with the specified key
        /// </summary>
        /// <typeparam name="T">The type of the value to retrieve</typeparam>
        /// <param name="key">The key of the value to retrieve</param>
        /// <param name="value">The resulting value, of any</param>
        /// <returns>A boolean indicating whether or not the value with the specified key exists and could be retrieved</returns>
        public virtual bool TryGet<T>(string key, out T value)
        {
            if (this.ContainsKey(key))
            {
                value = this.Get<T>(key);
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }

        /// <summary>
        /// Attempts to retrieve a list value with the specified key
        /// </summary>
        /// <typeparam name="T">The type of the value to retrieve</typeparam>
        /// <param name="key">The key of the list to retrieve the value from</param>
        /// <param name="elemKey">The key of the list element to retrieve</param>
        /// <param name="value">The resulting value, of any</param>
        /// <returns>A boolean indicating whether or not the value with the specified key exists and could be retrieved</returns>
        public virtual bool TryGetListElement<T>(string key, string elemKey, out T value)
        {
            if (this.ContainsListElement(key, elemKey))
            {
                value = this.GetListElement<T>(key, elemKey);
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }

        /// <inheritdoc/>
        public virtual void Clear()
        {
            foreach (EndPoint endpoint in this.Connection.GetEndPoints())
            {
                IServer server = this.Connection.GetServer(endpoint);
                server.FlushAllDatabases();
            }
        }

        /// <inheritdoc/>
        public virtual async Task ClearAsync(CancellationToken cancellationToken = default)
        {
            foreach (EndPoint endpoint in this.Connection.GetEndPoints())
            {
                IServer server = this.Connection.GetServer(endpoint);
                await server.FlushAllDatabasesAsync();
            }
        }

        /// <summary>
        /// Gets the concurrency token for the specified value
        /// </summary>
        /// <param name="value">The value to get the concurrency token for</param>
        /// <returns>An array of byte that represents the concurrency token for the specified value</returns>
        protected virtual byte[] GetConcurrencyToken(string value)
        {
            return MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(value));
        }

        /// <summary>
        /// Maps the hash fields for the specified hash key to an <see cref="ICacheEntry"/>
        /// </summary>
        /// <typeparam name="T">The type of value held by the key</typeparam>
        /// <param name="key">The hash key of the fields to map to an <see cref="ICacheEntry"/></param>
        /// <returns>A new <see cref="ICacheEntry"/> based on the hash fields located at the specified hash key</returns>
        private ICacheEntry<T> MapToCacheEntry<T>(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));
            HashEntry[] hashFields = this.Cache.HashGetAll(key);
            if (hashFields == null || !hashFields.Any())
                return new CacheEntry<T>(key, default, null);
            string rawValue = hashFields.First(f => f.Name == RedisCache.ValueField).Value;
            T value = default;
            if (!string.IsNullOrWhiteSpace(rawValue))
                value = this.DeserializeEntry<T>(rawValue);
            byte[] concurrencyToken = hashFields.First(f => f.Name == RedisCache.ConcurrencyTokenField).Value;
            HashEntry absoluteExpirationField = hashFields.FirstOrDefault(f => f.Name == RedisCache.AbsoluteExpirationField);
            DateTime? absoluteExpiration = null;
            if (absoluteExpirationField != default
                && !string.IsNullOrWhiteSpace(absoluteExpirationField.Value))
                absoluteExpiration = new DateTime(long.Parse(absoluteExpirationField.Value));
            HashEntry slidingExpirationField = hashFields.FirstOrDefault(f => f.Name == RedisCache.SlidingExpirationField);
            TimeSpan? slidingExpiration = null;
            if (slidingExpirationField != default
                && !string.IsNullOrWhiteSpace(slidingExpirationField.Value))
                slidingExpiration = TimeSpan.FromMilliseconds(long.Parse(slidingExpirationField.Value));
            return new CacheEntry<T>(key, value, concurrencyToken, absoluteExpiration, slidingExpiration);
        }

        /// <summary>
        /// Maps the hash fields for the specified hash key to an <see cref="ICacheEntry"/>
        /// </summary>
        /// <typeparam name="T">The type of value held by the key</typeparam>
        /// <param name="key">The hash key of the fields to map to an <see cref="ICacheEntry"/></param>
        /// <returns>A new <see cref="ICacheEntry"/> based on the hash fields located at the specified hash key</returns>
        private async Task<ICacheEntry<T>> MapToCacheEntryAsync<T>(string key)
        {
            HashEntry[] hashFields = await this.Cache.HashGetAllAsync(key);
            if (!hashFields.Any())
                return new CacheEntry<T>(key, default, null);
            string rawValue = hashFields.First(f => f.Name == RedisCache.ValueField).Value;
            T value = default;
            if (!string.IsNullOrWhiteSpace(rawValue))
                value = this.DeserializeEntry<T>(rawValue);
            byte[] concurrencyToken = hashFields.First(f => f.Name == RedisCache.ConcurrencyTokenField).Value;
            HashEntry absoluteExpirationField = hashFields.FirstOrDefault(f => f.Name == RedisCache.AbsoluteExpirationField);
            DateTime? absoluteExpiration = null;
            if (absoluteExpirationField != default
                && !string.IsNullOrWhiteSpace(absoluteExpirationField.Value))
                absoluteExpiration = new DateTime(long.Parse(absoluteExpirationField.Value));
            HashEntry slidingExpirationField = hashFields.FirstOrDefault(f => f.Name == RedisCache.SlidingExpirationField);
            TimeSpan? slidingExpiration = null;
            if (slidingExpirationField != default
                && !string.IsNullOrWhiteSpace(slidingExpirationField.Value))
                slidingExpiration = TimeSpan.FromMilliseconds(long.Parse(slidingExpirationField.Value));
            return new CacheEntry<T>(key, value, concurrencyToken, absoluteExpiration, slidingExpiration);
        }

        private void SetInternal<T>(string key, T value, byte[] concurrencyToken, CacheEntryOptions options)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));
            string json = this.SerializeEntry(value);
            DateTime? absoluteExpiration = null;
            TimeSpan? slidingExpiration = null;
            if (options.SlidingExpiration.HasValue)
            {
                absoluteExpiration = DateTime.UtcNow.Add(options.SlidingExpiration.Value);
                slidingExpiration = options.SlidingExpiration;
            }
            else if (options.AbsoluteExpiration.HasValue)
            {
                absoluteExpiration = options.AbsoluteExpiration.Value.UtcDateTime;
            }
            else if (options.RelativeExpiration.HasValue)
            {
                absoluteExpiration = DateTime.UtcNow.Add(options.RelativeExpiration.Value);
            }
            StackExchange.Redis.ITransaction transaction = this.Cache.CreateTransaction();
            transaction.AddCondition(Condition.HashEqual(key, RedisCache.ConcurrencyTokenField, concurrencyToken));
            transaction.HashSetAsync(key, RedisCache.ValueField, json);
            transaction.HashSetAsync(key, RedisCache.ConcurrencyTokenField, this.GetConcurrencyToken(json));
            transaction.HashSetAsync(key, RedisCache.AbsoluteExpirationField, absoluteExpiration.HasValue ? absoluteExpiration.Value.Ticks : (long?)null);
            transaction.HashSetAsync(key, RedisCache.SlidingExpirationField, slidingExpiration.HasValue ? slidingExpiration.Value.Milliseconds : (int?)null);
            if (absoluteExpiration.HasValue)
                transaction.KeyExpireAsync(key, absoluteExpiration.Value);
            if (!transaction.Execute())
                throw new OptimisticConcurrencyException($"An optimistic concurrency exception occured while setting the value of the distributed cache entry with key '{key}'. Optimistic concurrency exception occur when a key has been updated before it could be written to");
        }

        private async Task SetInternalAsync<T>(string key, T value, byte[] concurrencyToken, CacheEntryOptions options)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));
            string json = this.SerializeEntry(value);
            DateTime? absoluteExpiration = null;
            TimeSpan? slidingExpiration = null;
            if (options.SlidingExpiration.HasValue)
            {
                absoluteExpiration = DateTime.UtcNow.Add(options.SlidingExpiration.Value);
                slidingExpiration = options.SlidingExpiration;
            }
            else if (options.AbsoluteExpiration.HasValue)
            {
                absoluteExpiration = options.AbsoluteExpiration.Value.UtcDateTime;
            }
            else if (options.RelativeExpiration.HasValue)
            {
                absoluteExpiration = DateTime.UtcNow.Add(options.RelativeExpiration.Value);
            }
            StackExchange.Redis.ITransaction transaction = this.Cache.CreateTransaction();
            transaction.AddCondition(Condition.HashEqual(key, RedisCache.ConcurrencyTokenField, concurrencyToken));
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            transaction.HashSetAsync(key, RedisCache.ValueField, json);
            transaction.HashSetAsync(key, RedisCache.ConcurrencyTokenField, this.GetConcurrencyToken(json));
            transaction.HashSetAsync(key, RedisCache.AbsoluteExpirationField, absoluteExpiration.HasValue ? absoluteExpiration.Value.Ticks : (long?)null);
            transaction.HashSetAsync(key, RedisCache.SlidingExpirationField, slidingExpiration.HasValue ? slidingExpiration.Value.Milliseconds : (int?)null);
            if (absoluteExpiration.HasValue)
                transaction.KeyExpireAsync(key, absoluteExpiration.Value);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            if (!await transaction.ExecuteAsync())
                throw new OptimisticConcurrencyException($"An optimistic concurrency exception occured while setting the value of the distributed cache entry with key '{key}'. Optimistic concurrency exception occur when a key has been updated before it could be written to");
        }

        private void SetInternal<T>(string key, T value, CacheEntryOptions options)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));
            string json = this.SerializeEntry(value);
            DateTime? absoluteExpiration = null;
            TimeSpan? slidingExpiration = null;
            if (options.SlidingExpiration.HasValue)
            {
                absoluteExpiration = DateTime.UtcNow.Add(options.SlidingExpiration.Value);
                slidingExpiration = options.SlidingExpiration;
            }
            else if (options.AbsoluteExpiration.HasValue)
            {
                absoluteExpiration = options.AbsoluteExpiration.Value.UtcDateTime;
            }
            else if (options.RelativeExpiration.HasValue)
            {
                absoluteExpiration = DateTime.UtcNow.Add(options.RelativeExpiration.Value);
            }
            StackExchange.Redis.ITransaction transaction = this.Cache.CreateTransaction();
            transaction.HashSetAsync(key, RedisCache.ValueField, json);
            transaction.HashSetAsync(key, RedisCache.ConcurrencyTokenField, this.GetConcurrencyToken(json));
            transaction.HashSetAsync(key, RedisCache.AbsoluteExpirationField, absoluteExpiration.HasValue ? absoluteExpiration.Value.Ticks : (long?)null);
            transaction.HashSetAsync(key, RedisCache.SlidingExpirationField, slidingExpiration.HasValue ? slidingExpiration.Value.Milliseconds : (int?)null);
            if (absoluteExpiration.HasValue)
                transaction.KeyExpireAsync(key, absoluteExpiration.Value);
            if (!transaction.Execute())
                throw new RedisException($"Failed to set the value of key '{key}'");
        }

        private async Task SetInternalAsync<T>(string key, T value, CacheEntryOptions options)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));
            string json = this.SerializeEntry(value);
            DateTime? absoluteExpiration = null;
            TimeSpan? slidingExpiration = null;
            if (options.SlidingExpiration.HasValue)
            {
                absoluteExpiration = DateTime.UtcNow.Add(options.SlidingExpiration.Value);
                slidingExpiration = options.SlidingExpiration;
            }
            else if (options.AbsoluteExpiration.HasValue)
            {
                absoluteExpiration = options.AbsoluteExpiration.Value.UtcDateTime;
            }
            else if (options.RelativeExpiration.HasValue)
            {
                absoluteExpiration = DateTime.UtcNow.Add(options.RelativeExpiration.Value);
            }
            StackExchange.Redis.ITransaction transaction = this.Cache.CreateTransaction();
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            transaction.HashSetAsync(key, RedisCache.ValueField, json);
            transaction.HashSetAsync(key, RedisCache.ConcurrencyTokenField, this.GetConcurrencyToken(json));
            transaction.HashSetAsync(key, RedisCache.AbsoluteExpirationField, absoluteExpiration.HasValue ? absoluteExpiration.Value.Ticks : (long?)null);
            transaction.HashSetAsync(key, RedisCache.SlidingExpirationField, slidingExpiration.HasValue ? slidingExpiration.Value.Milliseconds : (int?)null);
            if (absoluteExpiration.HasValue)
                transaction.KeyExpireAsync(key, absoluteExpiration.Value);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            if (!await transaction.ExecuteAsync())
                throw new RedisException($"Failed to set the value of key '{key}'");
        }

        /// <summary>
        /// Serializes the specified cache entry value
        /// </summary>
        /// <param name="value">The value of the cache entry to serialize</param>
        /// <returns>The serialized cache entry value</returns>
        protected virtual string SerializeEntry(object value)
        {
            return Encoding.UTF8.GetString(this.Serializer.Serialize(value));
        }

        /// <summary>
        /// Deserializes the specified cache entry
        /// </summary>
        /// <typeparam name="T">The expected type of the cache entry value to deserialize</typeparam>
        /// <param name="value">The cache entry value to deserialize</param>
        /// <returns>The deserialize cache entry value</returns>
        protected virtual T DeserializeEntry<T>(string value)
        {
            return this.Serializer.Deserialize<T>(Encoding.UTF8.GetBytes(value));
        }

    }

}
