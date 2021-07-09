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
using System.Threading;
using System.Threading.Tasks;

namespace Neuroglia.Caching
{

    /// <summary>
    /// Defines extensions for <see cref="IDistributedCache"/>s
    /// </summary>
    public static class IDistributedCacheExtensions
    {

        /// <summary>
        /// Gets or creates a value of the specified type
        /// </summary>
        /// <typeparam name="T">The type of value to get or create</typeparam>
        /// <param name="cache">The extended <see cref="IDistributedCache"/></param>
        /// <param name="key">The key of the value to get or create</param>
        /// <param name="factory">A <see cref="Func{T, TResult}"/> used to congifure the <see cref="CacheEntryOptions"/> to use and to generate a value of the specified type</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>The value that has been retrieved or set</returns>
        public static async Task<T> GetOrCreateAsync1<T>(this IDistributedCache cache, string key, Func<CacheEntryOptions, Task<T>> factory, CancellationToken cancellationToken = default)
        {
            if (cache.TryGet(key, out T result))
                return result;
            CacheEntryOptions options = new();
            result = await factory(options);
            await cache.SetAsync(key, result, options, cancellationToken);
            return result;
        }

    }

}
