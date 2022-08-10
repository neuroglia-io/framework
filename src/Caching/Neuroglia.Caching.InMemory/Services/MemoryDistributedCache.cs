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
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Neuroglia.Caching
{

    /// <summary>
    /// Represents the in-memory implementation of the <see cref="IDistributedCache"/> interface
    /// </summary>
    public class MemoryDistributedCache
        : IDistributedCache
    {

        /// <summary>
        /// Gets the 'EntriesCollection' property of the <see cref="MemoryCache"/> type
        /// </summary>
        protected static readonly PropertyInfo EntriesCollectionProperty = typeof(MemoryCache).GetProperty("EntriesCollection", BindingFlags.Default | BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);

        /// <summary>
        /// Initializes a new <see cref="MemoryDistributedCache"/>
        /// </summary>
        /// <param name="cache">The underlying <see cref="IMemoryCache"/></param>
        public MemoryDistributedCache(IMemoryCache cache)
        {
            this.Cache = cache;
        }

        /// <summary>
        /// Gets the underlying <see cref="IMemoryCache"/>
        /// </summary>
        protected IMemoryCache Cache { get; private set; }

        /// <summary>
        /// Gets the <see cref="IDictionary"/> containing the entries of the underlying <see cref="IMemoryCache"/> 
        /// </summary>
        protected IDictionary EntriesCollection
        {
            get
            {
                return ((IDictionary)EntriesCollectionProperty.GetValue(this.Cache));
            }
        }

        /// <summary>
        /// Gets an <see cref="IEnumerable"/> containing the <see cref="Microsoft.Extensions.Caching.Memory.ICacheEntry"/> instances of the underlying <see cref="IMemoryCache"/>
        /// </summary>
        protected IEnumerable<Microsoft.Extensions.Caching.Memory.ICacheEntry> Entries
        {
            get
            {
                return this.EntriesCollection.Values.OfType<Microsoft.Extensions.Caching.Memory.ICacheEntry>();
            }
        }

        /// <summary>
        /// Gets an <see cref="IEnumerable"/> containing the keys of the underlying <see cref="IMemoryCache"/>
        /// </summary>
        protected IEnumerable<string> Keys
        {
            get
            {
                return this.EntriesCollection.Keys.OfType<string>().ToList();
            }
        }

        /// <summary>
        /// Gets a boolean indicating whether or not the cache contains the specified key
        /// </summary>
        /// <param name="key">The key to check</param>
        /// <returns>A boolean indicating whether or not the cache contains the specified key</returns>
        public virtual bool ContainsKey(string key)
        {
            return this.Keys.Contains(key);
        }

        /// <summary>
        /// Gets a boolean indicating whether or not the cache contains the specified key
        /// </summary>
        /// <param name="key">The key to check</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A boolean indicating whether or not the cache contains the specified key</returns>
        public virtual async Task<bool> ContainsKeyAsync(string key, CancellationToken cancellationToken = default)
        {
            return await Task.Run(() => this.ContainsKey(key), cancellationToken);
        }

        /// <summary>
        /// Determines whether the specified list contains an element with the specified key
        /// </summary>
        /// <param name="key">The key of the list to check</param>
        /// <param name="elemKey">The key of the element to check for existence</param>
        /// <returns>A boolean indicating whether or not the specified list contains an element with the specified key</returns>
        public virtual bool ContainsListElement(string key, string elemKey)
        {
            List<string> listKeys = this.Get<List<string>>(key);
            if (listKeys == null)
                return false;
            return listKeys.Contains(elemKey);
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
            List<string> listKeys = await this.GetAsync<List<string>>(key, cancellationToken);
            if (listKeys == null)
                return false;
            return listKeys.Contains(elemKey);
        }

        /// <summary>
        /// Finds all keys that match the specified pattern
        /// </summary>
        /// <param name="pattern">The pattern to match</param>
        /// <returns>A new <see cref="IEnumerable{T}"/> containing all the keys that match the specified pattern</returns>
        public virtual IEnumerable<string> FindKeys(string pattern)
        {
            Regex regex = new(pattern.Replace("*", ".*"));
            List<string> matches = new();
            foreach (string key in this.Keys)
            {
                if (regex.Match(key).Success)
                    matches.Add(key);
            }
            return matches;
        }

        /// <summary>
        /// Finds all keys that match the specified pattern
        /// </summary>
        /// <param name="pattern">The pattern to match</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A new <see cref="IEnumerable{T}"/> containing all the keys that match the specified pattern</returns>
        public virtual async Task<IEnumerable<string>> FindKeysAsync(string pattern, CancellationToken cancellationToken = default)
        {
            return await Task.Run(() => this.FindKeys(pattern), cancellationToken);
        }

        /// <summary>
        /// Retrieves all the element keys of the list with the specified key
        /// </summary>
        /// <param name="key">The key of the list for which to retrieve all the element keys</param>
        /// <returns>A new <see cref="IEnumerable{T}"/> containing all the element keys of the list with the specified key</returns>
        public virtual IEnumerable<string> GetListKeys(string key)
        {
            return this.Get<List<string>>(key);
        }

        /// <summary>
        /// Retrieves all the element keys of the list with the specified key
        /// </summary>
        /// <param name="key">The key of the list for which to retrieve all the element keys</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A new <see cref="IEnumerable{T}"/> containing all the element keys of the list with the specified key</returns>
        public virtual async Task<IEnumerable<string>> GetListKeysAsync(string key, CancellationToken cancellationToken = default)
        {
            return await this.GetAsync<List<string>>(key, cancellationToken);
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
            return new CacheLock(this, key, token, expiresIn);
        }

        /// <summary>
        /// Releases the lock on the specified key
        /// </summary>
        /// <param name="key">The key to release the lock of</param>
        /// <param name="token">The token of the lock to release</param>
        public virtual void ReleaseLock(string key, string token)
        {

        }

        /// <summary>
        /// Gets the value cached with the specified key
        /// </summary>
        /// <typeparam name="T">The type of the cached value</typeparam>
        /// <param name="key">The key of the value to retrieve</param>
        /// <returns>The value cached with the specified key</returns>
        public virtual T Get<T>(string key)
        {
            return this.Cache.Get<T>(key);
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
            return await Task.Run(() => this.Get<T>(key), cancellationToken);
        }

        /// <summary>
        /// Gets the <see cref="ICacheEntry{T}"/> that matches the specified key
        /// </summary>
        /// <typeparam name="T">The type of data cached by the entry</typeparam>
        /// <param name="key">The key of the <see cref="ICacheEntry{T}"/> to retrieve</param>
        /// <returns>The <see cref="ICacheEntry{T}"/> that matches the specified key</returns>
        public virtual ICacheEntry<T> GetEntry<T>(string key)
        {
            Microsoft.Extensions.Caching.Memory.ICacheEntry entry = (Microsoft.Extensions.Caching.Memory.ICacheEntry)this.EntriesCollection[key];
            return new CacheEntry<T>((string)entry.Key, (T)entry.Value, null, entry.AbsoluteExpiration.HasValue ? entry.AbsoluteExpiration.Value.UtcDateTime : (DateTime?)null, entry.SlidingExpiration);
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
            return await Task.Run(() => this.GetEntry<T>(key), cancellationToken);
        }

        /// <summary>
        /// Sets the value of the specified key
        /// </summary>
        /// <typeparam name="T">The type of value to cache</typeparam>
        /// <param name="key">The key of the value to cache</param>
        /// <param name="value">The value to cache</param>
        public virtual void Set<T>(string key, T value)
        {
            this.Cache.Set(key, value);
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
            await Task.Run(() => this.Set<T>(key, value), cancellationToken);
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
            this.Cache.Set(key, value, new MemoryCacheEntryOptions()
            {
                AbsoluteExpiration = options.AbsoluteExpiration,
                AbsoluteExpirationRelativeToNow = options.RelativeExpiration,
                SlidingExpiration = options.SlidingExpiration
            });
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
            await Task.Run(() => this.Set<T>(key, value, options), cancellationToken);
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
            this.Cache.Set(key, value);
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
            await Task.Run(() => this.Set<T>(key, value, concurrencyToken), cancellationToken);
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
            this.Cache.Set(key, value, new MemoryCacheEntryOptions()
            {
                AbsoluteExpiration = options.AbsoluteExpiration,
                AbsoluteExpirationRelativeToNow = options.RelativeExpiration,
                SlidingExpiration = options.SlidingExpiration
            });
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
            await Task.Run(() => this.Set<T>(key, value, concurrencyToken, options), cancellationToken);
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
            this.Cache.Set(key, value, expiresAt);
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
            await Task.Run(() => this.Set(key, value, expiresAt), cancellationToken);
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
            this.Cache.Set(key, value, expiresAt);
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
            await Task.Run(() => this.Set(key, value, expiresAt, concurrencyToken), cancellationToken);
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
            this.Cache.Set(key, value, expiresIn);
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
            await Task.Run(() => this.Set(key, value, expiresIn), cancellationToken);
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
            this.Cache.Set(key, value, expiresIn);
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
            await Task.Run(() => this.Set(key, value, expiresIn, concurrencyToken), cancellationToken);
        }

        /// <summary>
        /// Removes the specified key
        /// </summary>
        /// <param name="key">The key to remove</param>
        /// <returns>A boolean indicating whether or not the key could be removed</returns>
        public virtual bool Remove(string key)
        {
            if (!this.EntriesCollection.Contains(key))
                return false;
            this.EntriesCollection.Remove(key);
            return true;
        }

        /// <summary>
        /// Removes the specified key
        /// </summary>
        /// <param name="key">The key to remove</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A boolean indicating whether or not the key could be removed</returns>
        public virtual async Task<bool> RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            return await Task.Run(() => this.Remove(key), cancellationToken);
        }

        /// <summary>
        /// Gets the list stored at the specified key
        /// </summary>
        /// <typeparam name="T">The type of elements held in the list</typeparam>
        /// <param name="key">The key of the list to retrieve</param>
        /// <returns>The list store at the specified key</returns>
        public virtual List<T> GetList<T>(string key)
        {
            List<T> results = new();
            List<string> keyList = this.Get<List<string>>(key);
            if (keyList != null)
            {
                foreach (string elemKey in keyList.ToList())
                {
                    results.Add(this.Get<T>(GetListElementKey(key, elemKey)));
                }
            }
            return results;
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
            return await Task.Run(() => this.GetList<T>(key), cancellationToken);
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
            return this.Get<T>(GetListElementKey(key, elemKey));
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
            return await Task.Run(() => this.GetListElement<T>(key, elemKey), cancellationToken);
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
            List<string> listKeys = this.Get<List<string>>(key);
            if (listKeys == null)
                listKeys = new List<string>();
            if (!listKeys.Contains(elemKey))
                listKeys.Add(elemKey);
            this.Cache.Set(key, listKeys);
            this.Cache.Set(GetListElementKey(key, elemKey), elem);
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
            await Task.Run(() => this.AddToList(key, elemKey, elem), cancellationToken);
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
            List<string> listKeys = this.Get<List<string>>(key);
            if (listKeys == null)
                listKeys = new List<string>();
            listKeys.Add(elemKey);
            this.Cache.Set(key, listKeys, expiresAt);
            this.Cache.Set(GetListElementKey(key, elemKey), elem, expiresAt);
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
            await Task.Run(() => this.AddToList(key, elemKey, elem, expiresAt), cancellationToken);
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
            List<string> listKeys = this.Get<List<string>>(key);
            if (listKeys == null)
                listKeys = new List<string>();
            listKeys.Add(elemKey);
            this.Cache.Set(key, listKeys, expiresIn);
            this.Cache.Set(GetListElementKey(key, elemKey), elem, expiresIn);
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
            await Task.Run(() => this.AddToList(key, elemKey, elem, expiresIn), cancellationToken);
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
            this.Cache.Set(GetListElementKey(key, elemKey), elem);
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
            await Task.Run(() => this.UpdateList(key, elemKey, elem), cancellationToken);
        }

        /// <summary>
        /// Removes an element from the list stored at the specified key
        /// </summary>
        /// <param name="key">The key of the list to remove an element from</param>
        /// <param name="elemKey">The key of the element to remove</param>
        /// <returns>A boolean indicating whether or not the element could be removed from the list</returns>
        public virtual bool RemoveFromList(string key, string elemKey)
        {
            List<string> listKeys = this.Get<List<string>>(key);
            if (listKeys == null
                || !listKeys.Contains(elemKey))
                return false;
            listKeys.Remove(elemKey);
            this.Remove(GetListElementKey(key, elemKey));
            return true;
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
            return await Task.Run(() => this.RemoveFromList(key, elemKey), cancellationToken);
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
            return this.Cache.TryGetValue(key, out value);
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
            this.EntriesCollection.Clear();
        }

        /// <inheritdoc/>
        public virtual async Task ClearAsync(CancellationToken cancellationToken = default)
        {
            this.EntriesCollection.Clear();
            await Task.CompletedTask;
        }

        private static string GetListElementKey(string key, string elemKey)
        {
            return $"{key}_{elemKey}";
        }

    }

}
