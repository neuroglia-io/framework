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
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Neuroglia.Caching
{

    /// <summary>
    /// Defines the fundamentals of a distributed cache
    /// </summary>
    public interface IDistributedCache
    {

        /// <summary>
        /// Gets a boolean indicating whether or not the cache contains the specified key
        /// </summary>
        /// <param name="key">The key to check</param>
        /// <returns>A boolean indicating whether or not the cache contains the specified key</returns>
        bool ContainsKey(string key);

        /// <summary>
        /// Gets a boolean indicating whether or not the cache contains the specified key
        /// </summary>
        /// <param name="key">The key to check</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A boolean indicating whether or not the cache contains the specified key</returns>
        Task<bool> ContainsKeyAsync(string key, CancellationToken cancellationToken = default);

        /// <summary>
        /// Determines whether the specified list contains an element with the specified key
        /// </summary>
        /// <param name="key">The key of the list to check</param>
        /// <param name="elemKey">The key of the element to check for existence</param>
        /// <returns>A boolean indicating whether or not the specified list contains an element with the specified key</returns>
        bool ContainsListElement(string key, string elemKey);

        /// <summary>
        /// Determines whether the specified list contains an element with the specified key
        /// </summary>
        /// <param name="key">The key of the list to check</param>
        /// <param name="elemKey">The key of the element to check for existence</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A boolean indicating whether or not the specified list contains an element with the specified key</returns>
        Task<bool> ContainsListElementAsync(string key, string elemKey, CancellationToken cancellationToken = default);

        /// <summary>
        /// Finds all keys that match the specified pattern
        /// </summary>
        /// <param name="pattern">The pattern to match</param>
        /// <returns>A new <see cref="IEnumerable{T}"/> containing all the keys that match the specified pattern</returns>
        IEnumerable<string> FindKeys(string pattern);

        /// <summary>
        /// Finds all keys that match the specified pattern
        /// </summary>
        /// <param name="pattern">The pattern to match</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A new <see cref="IEnumerable{T}"/> containing all the keys that match the specified pattern</returns>
        Task<IEnumerable<string>> FindKeysAsync(string pattern, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves all the element keys of the list with the specified key
        /// </summary>
        /// <param name="key">The key of the list for which to retrieve all the element keys</param>
        /// <returns>A new <see cref="IEnumerable{T}"/> containing all the element keys of the list with the specified key</returns>
        IEnumerable<string> GetListKeys(string key);

        /// <summary>
        /// Retrieves all the element keys of the list with the specified key
        /// </summary>
        /// <param name="key">The key of the list for which to retrieve all the element keys</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A new <see cref="IEnumerable{T}"/> containing all the element keys of the list with the specified key</returns>
        Task<IEnumerable<string>> GetListKeysAsync(string key, CancellationToken cancellationToken = default);

        /// <summary>
        /// Locks access to the specified key
        /// </summary>
        /// <param name="key">The key to lock</param>
        /// <param name="token">The lock's token</param>
        /// <param name="expiresIn">The time in which the lock expires</param>
        /// <returns></returns>
        IDisposable Lock(string key, string token, TimeSpan expiresIn);

        /// <summary>
        /// Releases the lock on the specified key
        /// </summary>
        /// <param name="key">The key to release the lock of</param>
        /// <param name="token">The token of the lock to release</param>
        void ReleaseLock(string key, string token);

        /// <summary>
        /// Gets the <see cref="ICacheEntry{T}"/> that matches the specified key
        /// </summary>
        /// <typeparam name="T">The type of data cached by the entry</typeparam>
        /// <param name="key">The key of the <see cref="ICacheEntry{T}"/> to retrieve</param>
        /// <returns>The <see cref="ICacheEntry{T}"/> that matches the specified key</returns>
        ICacheEntry<T> GetEntry<T>(string key);

        /// <summary>
        /// Gets the <see cref="ICacheEntry{T}"/> that matches the specified key
        /// </summary>
        /// <typeparam name="T">The type of data cached by the entry</typeparam>
        /// <param name="key">The key of the <see cref="ICacheEntry{T}"/> to retrieve</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>The <see cref="ICacheEntry{T}"/> that matches the specified key</returns>
        Task<ICacheEntry<T>> GetEntryAsync<T>(string key, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the value cached with the specified key
        /// </summary>
        /// <typeparam name="T">The type of the cached value</typeparam>
        /// <param name="key">The key of the value to retrieve</param>
        /// <returns>The value cached with the specified key</returns>
        T Get<T>(string key);

        /// <summary>
        /// Gets the value cached with the specified key
        /// </summary>
        /// <typeparam name="T">The type of the cached value</typeparam>
        /// <param name="key">The key of the value to retrieve</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>The value cached with the specified key</returns>
        Task<T> GetAsync<T>(string key, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sets the value of the specified key
        /// </summary>
        /// <typeparam name="T">The type of value to cache</typeparam>
        /// <param name="key">The key of the value to cache</param>
        /// <param name="value">The value to cache</param>
        void Set<T>(string key, T value);

        /// <summary>
        /// Sets the value of the specified key
        /// </summary>
        /// <typeparam name="T">The type of value to cache</typeparam>
        /// <param name="key">The key of the value to cache</param>
        /// <param name="value">The value to cache</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        Task SetAsync<T>(string key, T value, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sets the value of the specified key
        /// </summary>
        /// <typeparam name="T">The type of value to cache</typeparam>
        /// <param name="key">The key of the value to cache</param>
        /// <param name="value">The value to cache</param>
        /// <param name="options">The options configuring how to cache the value</param>
        void Set<T>(string key, T value, CacheEntryOptions options);

        /// <summary>
        /// Sets the value of the specified key
        /// </summary>
        /// <typeparam name="T">The type of value to cache</typeparam>
        /// <param name="key">The key of the value to cache</param>
        /// <param name="value">The value to cache</param>
        /// <param name="options">The options configuring how to cache the value</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        Task SetAsync<T>(string key, T value, CacheEntryOptions options, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sets the value of the specified key
        /// </summary>
        /// <typeparam name="T">The type of value to cache</typeparam>
        /// <param name="key">The key of the value to cache</param>
        /// <param name="value">The value to cache</param>
        /// <param name="concurrencyToken">The concurrency token used for optimistic persistency</param>
        void Set<T>(string key, T value, byte[] concurrencyToken);

        /// <summary>
        /// Sets the value of the specified key
        /// </summary>
        /// <typeparam name="T">The type of value to cache</typeparam>
        /// <param name="key">The key of the value to cache</param>
        /// <param name="value">The value to cache</param>
        /// <param name="concurrencyToken">The concurrency token used for optimistic persistency</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        Task SetAsync<T>(string key, T value, byte[] concurrencyToken, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sets the value of the specified key
        /// </summary>
        /// <typeparam name="T">The type of value to cache</typeparam>
        /// <param name="key">The key of the value to cache</param>
        /// <param name="value">The value to cache</param>
        /// <param name="concurrencyToken">The concurrency token used for optimistic persistency</param>
        /// <param name="options">The options configuring how to cache the value</param>
        void Set<T>(string key, T value, byte[] concurrencyToken, CacheEntryOptions options);

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
        Task SetAsync<T>(string key, T value, byte[] concurrencyToken, CacheEntryOptions options, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sets the value of the specified key
        /// </summary>
        /// <typeparam name="T">The type of value to cache</typeparam>
        /// <param name="key">The key of the value to cache</param>
        /// <param name="value">The value to cache</param>
        /// <param name="expiresAt">The date and time at which the entry expires</param>
        void Set<T>(string key, T value, DateTimeOffset expiresAt);

        /// <summary>
        /// Sets the value of the specified key
        /// </summary>
        /// <typeparam name="T">The type of value to cache</typeparam>
        /// <param name="key">The key of the value to cache</param>
        /// <param name="value">The value to cache</param>
        /// <param name="expiresAt">The date and time at which the entry expires</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        Task SetAsync<T>(string key, T value, DateTimeOffset expiresAt, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sets the value of the specified key
        /// </summary>
        /// <typeparam name="T">The type of value to cache</typeparam>
        /// <param name="key">The key of the value to cache</param>
        /// <param name="value">The value to cache</param>
        /// <param name="expiresAt">The date and time at which the entry expires</param>
        /// <param name="concurrencyToken">The concurrency token used for optimistic persistency</param>
        void Set<T>(string key, T value, DateTimeOffset expiresAt, byte[] concurrencyToken);

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
        Task SetAsync<T>(string key, T value, DateTimeOffset expiresAt, byte[] concurrencyToken, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sets the value of the specified key
        /// </summary>
        /// <typeparam name="T">The type of value to cache</typeparam>
        /// <param name="key">The key of the value to cache</param>
        /// <param name="value">The value to cache</param>
        /// <param name="expiresIn">The time in which the entry expires</param>
        void Set<T>(string key, T value, TimeSpan expiresIn);

        /// <summary>
        /// Sets the value of the specified key
        /// </summary>
        /// <typeparam name="T">The type of value to cache</typeparam>
        /// <param name="key">The key of the value to cache</param>
        /// <param name="value">The value to cache</param>
        /// <param name="expiresIn">The time in which the entry expires</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        Task SetAsync<T>(string key, T value, TimeSpan expiresIn, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sets the value of the specified key
        /// </summary>
        /// <typeparam name="T">The type of value to cache</typeparam>
        /// <param name="key">The key of the value to cache</param>
        /// <param name="value">The value to cache</param>
        /// <param name="expiresIn">The time in which the entry expires</param>
        /// <param name="concurrencyToken">The concurrency token used for optimistic persistency</param>
        void Set<T>(string key, T value, TimeSpan expiresIn, byte[] concurrencyToken);

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
        Task SetAsync<T>(string key, T value, TimeSpan expiresIn, byte[] concurrencyToken, CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes the specified key
        /// </summary>
        /// <param name="key">The key to remove</param>
        /// <returns>A boolean indicating whether or not the key could be removed</returns>
        bool Remove(string key);

        /// <summary>
        /// Removes the specified key
        /// </summary>
        /// <param name="key">The key to remove</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A boolean indicating whether or not the key could be removed</returns>
        Task<bool> RemoveAsync(string key, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the list stored at the specified key
        /// </summary>
        /// <typeparam name="T">The type of elements held in the list</typeparam>
        /// <param name="key">The key of the list to retrieve</param>
        /// <returns>The list store at the specified key</returns>
        List<T> GetList<T>(string key);

        /// <summary>
        /// Gets the list stored at the specified key
        /// </summary>
        /// <typeparam name="T">The type of elements held in the list</typeparam>
        /// <param name="key">The key of the list to retrieve</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>The list store at the specified key</returns>
        Task<List<T>> GetListAsync<T>(string key, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the specified element from the list stored at the specified key
        /// </summary>
        /// <typeparam name="T">The type of elements held in the list</typeparam>
        /// <param name="key">The key of the list to retrieve an element from</param>
        /// <param name="elemKey">The key of the element to retrieve</param>
        /// <returns>The element with the specified key, from the list stored at the specified key</returns>
        T GetListElement<T>(string key, string elemKey);

        /// <summary>
        /// Gets the specified element from the list stored at the specified key
        /// </summary>
        /// <typeparam name="T">The type of elements held in the list</typeparam>
        /// <param name="key">The key of the list to retrieve an element from</param>
        /// <param name="elemKey">The key of the element to retrieve</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>The element with the specified key, from the list stored at the specified key</returns>
        Task<T> GetListElementAsync<T>(string key, string elemKey, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds a new element to the list stored at the specified key
        /// </summary>
        /// <typeparam name="T">The type of elements held in the list</typeparam>
        /// <param name="key">The key of the list to add a new element to</param>
        /// <param name="elemKey">The key of the element to add</param>
        /// <param name="elem">The element to add</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        void AddToList<T>(string key, string elemKey, T elem);

        /// <summary>
        /// Adds a new element to the list stored at the specified key
        /// </summary>
        /// <typeparam name="T">The type of elements held in the list</typeparam>
        /// <param name="key">The key of the list to add a new element to</param>
        /// <param name="elemKey">The key of the element to add</param>
        /// <param name="elem">The element to add</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        Task AddToListAsync<T>(string key, string elemKey, T elem, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds a new element to the list stored at the specified key
        /// </summary>
        /// <typeparam name="T">The type of elements held in the list</typeparam>
        /// <param name="key">The key of the list to add a new element to</param>
        /// <param name="elemKey">The key of the element to add</param>
        /// <param name="elem">The element to add</param>
        /// <param name="expiresAt">The UTC date and time at which the cached list should expire at</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        void AddToList<T>(string key, string elemKey, T elem, DateTime expiresAt);

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
        Task AddToListAsync<T>(string key, string elemKey, T elem, DateTime expiresAt, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds a new element to the list stored at the specified key
        /// </summary>
        /// <typeparam name="T">The type of elements held in the list</typeparam>
        /// <param name="key">The key of the list to add a new element to</param>
        /// <param name="elemKey">The key of the element to add</param>
        /// <param name="elem">The element to add</param>
        /// <param name="expiresIn">A <see cref="TimeSpan"/> that represents the time in which the list should expire</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        void AddToList<T>(string key, string elemKey, T elem, TimeSpan expiresIn);

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
        Task AddToListAsync<T>(string key, string elemKey, T elem, TimeSpan expiresIn, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an element of the list stored at the specified key
        /// </summary>
        /// <typeparam name="T">The type of elements held in the list</typeparam>
        /// <param name="key">The key of the list to update an element of</param>
        /// <param name="elemKey">The key of the element to update</param>
        /// <param name="elem">The updated element</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        void UpdateList<T>(string key, string elemKey, T elem);

        /// <summary>
        /// Updates an element of the list stored at the specified key
        /// </summary>
        /// <typeparam name="T">The type of elements held in the list</typeparam>
        /// <param name="key">The key of the list to update an element of</param>
        /// <param name="elemKey">The key of the element to update</param>
        /// <param name="elem">The updated element</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        Task UpdateListAsync<T>(string key, string elemKey, T elem, CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes an element from the list stored at the specified key
        /// </summary>
        /// <param name="key">The key of the list to remove an element from</param>
        /// <param name="elemKey">The key of the element to remove</param>
        /// <returns>A boolean indicating whether or not the element could be removed from the list</returns>
        bool RemoveFromList(string key, string elemKey);

        /// <summary>
        /// Removes an element from the list stored at the specified key
        /// </summary>
        /// <param name="key">The key of the list to remove an element from</param>
        /// <param name="elemKey">The key of the element to remove</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A boolean indicating whether or not the element could be removed from the list</returns>
        Task<bool> RemoveFromListAsync(string key, string elemKey, CancellationToken cancellationToken = default);

        /// <summary>
        /// Attempts to retrieve the value with the specified key
        /// </summary>
        /// <typeparam name="T">The type of the value to retrieve</typeparam>
        /// <param name="key">The key of the value to retrieve</param>
        /// <param name="value">The resulting value, of any</param>
        /// <returns>A boolean indicating whether or not the value with the specified key exists and could be retrieved</returns>
        bool TryGet<T>(string key, out T value);

        /// <summary>
        /// Attempts to retrieve a list value with the specified key
        /// </summary>
        /// <typeparam name="T">The type of the value to retrieve</typeparam>
        /// <param name="key">The key of the list to retrieve the value from</param>
        /// <param name="elemKey">The key of the list element to retrieve</param>
        /// <param name="value">The resulting value, of any</param>
        /// <returns>A boolean indicating whether or not the value with the specified key exists and could be retrieved</returns>
        bool TryGetListElement<T>(string key, string elemKey, out T value);

        /// <summary>
        /// Clears the cache
        /// </summary>
        void Clear();

        /// <summary>
        /// Clears the cache
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        Task ClearAsync(CancellationToken cancellationToken = default);

    }

}
