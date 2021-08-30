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

using EventStore.Client;

namespace Neuroglia.Data.EventSourcing.EventStore.Configuration
{
    /// <summary>
    /// Defines the fundamentals of a service used to build an <see cref="EventStoreClient"/>
    /// </summary>
    public interface IEventStoreClientBuilder
    {

        /// <summary>
        /// Sets the <see cref="EventStoreClient"/>'s name
        /// </summary>
        /// <param name="name">The name of the <see cref="EventStoreClient"/> to create</param>
        /// <returns>The configured <see cref="IEventStoreClientBuilder"/></returns>
        IEventStoreClientBuilder WithName(string name);

        /// <summary>
        /// Uses the specified connection string to connect to the remote server
        /// </summary>
        /// <param name="connectionString">The connection string to connect to the remote server</param>
        /// <returns>The configured <see cref="IEventStoreClientBuilder"/></returns>
        IEventStoreClientBuilder UseConnectionString(string connectionString);

        /// <summary>
        /// Builds the <see cref="EventStoreClient"/>
        /// </summary>
        /// <returns>A new <see cref="EventStoreClient"/></returns>
        EventStoreClient Build();

    }

}
