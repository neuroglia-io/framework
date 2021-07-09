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
using Neuroglia.Serialization;
using StackExchange.Redis.Extensions.Core.Configuration;
using System;

namespace Neuroglia.Caching
{

    /// <summary>
    /// Represents the options used to configure a <see cref="RedisCache"/>
    /// </summary>
    public class RedisCacheOptions
    {

        /// <summary>
        /// Initializes a new <see cref="RedisCacheOptions"/>
        /// </summary>
        public RedisCacheOptions()
        {
            this.EntrySerializerType = typeof(NewtonsoftJsonSerializer);
        }

        /// <summary>
        /// Gets/sets the cache's configuration string
        /// </summary>
        public string Configuration { get; set; }

        /// <summary>
        /// Gets/sets the cache's instance name
        /// </summary>
        public string InstanceName { get; set; }

        /// <summary>
        /// Gets/sets the cache's <see cref="StackExchange.Redis.Extensions.Core.Configuration.ServerEnumerationStrategy"/>
        /// </summary>
        public ServerEnumerationStrategy ServerEnumerationStrategy { get; set; }

        /// <summary>
        /// Gets the type of <see cref="ISerializer"/> used to serialize and deserialize cache entries
        /// </summary>
        public Type EntrySerializerType { get; set; }

    }

}
