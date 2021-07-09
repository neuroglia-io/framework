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

namespace Neuroglia.Caching
{

    /// <summary>
    /// Represents the default implementation of the <see cref="ICacheEntry{T}"/> interface
    /// </summary>
    /// <typeparam name="T">The type of the entry</typeparam>
    public class CacheEntry<T>
        : ICacheEntry<T>
    {

        /// <summary>
        /// Initializes a new <see cref="CacheEntry{T}"/>
        /// </summary>
        protected CacheEntry()
        {

        }

        /// <summary>
        /// Initializes a new <see cref="CacheEntry{T}"/>
        /// </summary>
        /// <param name="key">The entry's key</param>
        /// <param name="value">The entry's value</param>
        /// <param name="concurrencyToken">The entry's concurrency token</param>
        /// <param name="absoluteExpiration">The entry's absolute expiration</param>
        /// <param name="slidingExpiration">The entry's sliding expiration</param>
        public CacheEntry(string key, T value, byte[] concurrencyToken = null, DateTime? absoluteExpiration = null, TimeSpan? slidingExpiration = null)
        {
            this.Key = key;
            this.Value = value;
            this.ConcurrencyToken = concurrencyToken;
            this.AbsoluteExpiration = absoluteExpiration;
            this.SlidingExpiration = slidingExpiration;
        }

        /// <summary>
        /// Gets the entry's key
        /// </summary>
        public string Key { get; protected set; }

        /// <summary>
        /// Gets the entry's value
        /// </summary>
        public T Value { get; protected set; }

        /// <summary>
        /// Gets the entry's concurrency token
        /// </summary>
        public byte[] ConcurrencyToken { get; protected set; }

        object ICacheEntry.Value
        {
            get
            {
                return this.Value;
            }
        }

        /// <summary>
        /// Gets the entry's absolute expiration
        /// </summary>
        public DateTime? AbsoluteExpiration { get; protected set; }

        /// <summary>
        /// Gets the entry's sliding expiration
        /// </summary>
        public TimeSpan? SlidingExpiration { get; protected set; }

    }


}
