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
    /// Defines the fundamentals of a cache entry
    /// </summary>
    public interface ICacheEntry
    {

        /// <summary>
        /// Gets the entry's key
        /// </summary>
        string Key { get; }

        /// <summary>
        /// Gets the entry's key
        /// </summary>
        object Value { get; }

        /// <summary>
        /// Gets the entry's absolute expiration
        /// </summary>
        DateTime? AbsoluteExpiration { get; }

        /// <summary>
        /// Gets the entry's sliding expiration
        /// </summary>
        TimeSpan? SlidingExpiration { get; }

        /// <summary>
        /// Gets the entry's concurrency token
        /// </summary>
        byte[] ConcurrencyToken { get; }

    }

    /// <summary>
    /// Defines the fundamentals of a generic cache entry
    /// </summary>
    public interface ICacheEntry<T>
        : ICacheEntry
    {

        /// <summary>
        /// Gets the entry's key
        /// </summary>
        new T Value { get; }

    }

}
