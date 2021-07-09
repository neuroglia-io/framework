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
    /// Represents a cache lock
    /// </summary>
    public class CacheLock
        : IDisposable
    {

        /// <summary>
        /// Initializes a new <see cref="CacheLock"/>
        /// </summary>
        /// <param name="distributedCache">The <see cref="IDistributedCache"/> to create the lock for</param>
        /// <param name="key">The key to lock</param>
        /// <param name="token">The lock's token</param>
        /// <param name="expiresIn">The time in which the <see cref="CacheLock"/> expires</param>
        public CacheLock(IDistributedCache distributedCache, string key, string token, TimeSpan expiresIn)
        {
            this.DisitributedCache = distributedCache;
            this.Key = key;
            this.Token = token;
            this.ExpiresIn = expiresIn;
        }

        /// <summary>
        /// Gets the <see cref="IDistributedCache"/> to create the lock for
        /// </summary>
        public IDistributedCache DisitributedCache { get; private set; }

        /// <summary>
        /// Gets the locked key
        /// </summary>
        public string Key { get; private set; }

        /// <summary>
        /// Gets the lock's token
        /// </summary>
        public string Token { get; private set; }

        /// <summary>
        /// Gets the date and time at which the token has been acquired
        /// </summary>
        public DateTime AcquiredAt { get; private set; }

        /// <summary>
        /// Gets the time in which the <see cref="CacheLock"/> expires
        /// </summary>
        public TimeSpan ExpiresIn { get; private set; }

        /// <summary>
        /// Gets a boolean indicating whether or not the token has expired
        /// </summary>
        public bool HasExpired
        {
            get
            {
                return DateTime.UtcNow > this.AcquiredAt.Add(this.ExpiresIn);
            }
        }

        /// <summary>
        /// Disposes of the <see cref="CacheLock"/>
        /// </summary>
        public void Dispose()
        {
            this.DisitributedCache.ReleaseLock(this.Key, this.Token);
            GC.SuppressFinalize(this);
        }

    }

}
