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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Neuroglia.Serialization;
using System;

namespace Neuroglia.Caching
{

    /// <summary>
    /// Represents the service used to create <see cref="RedisCache"/> instances
    /// </summary>
    public class RedisCacheFactory
    {

        /// <summary>
        /// Initializes a new <see cref="RedisCacheFactory"/>
        /// </summary>
        /// <param name="serviceProvider">The current <see cref="IServiceProvider"/></param>
        /// <param name="options">The service used to access the current <see cref="RedisCacheOptions"/></param>
        public RedisCacheFactory(IServiceProvider serviceProvider, IOptions<RedisCacheOptions> options)
        {
            this.ServiceProvider = serviceProvider;
            this.Options = options.Value;
        }

        /// <summary>
        /// Gets the current <see cref="IServiceProvider"/>
        /// </summary>
        protected IServiceProvider ServiceProvider { get; }

        /// <summary>
        /// Gets the current <see cref="RedisCacheOptions"/>
        /// </summary>
        protected RedisCacheOptions Options { get; }

        /// <summary>
        /// Creates a new <see cref="RedisCache"/> instance
        /// </summary>
        /// <returns>A new <see cref="RedisCache"/> instance</returns>
        public RedisCache Create()
        {
            ISerializer serializer = (ISerializer)this.ServiceProvider.GetRequiredService(this.Options.EntrySerializerType);
            return ActivatorUtilities.CreateInstance<RedisCache>(this.ServiceProvider, serializer);
        }

    }

}
