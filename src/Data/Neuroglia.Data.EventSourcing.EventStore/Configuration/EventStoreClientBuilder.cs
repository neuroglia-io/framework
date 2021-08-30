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
    /// Represents the default implementation of the <see cref="IEventStoreClientBuilder"/>
    /// </summary>
    public class EventStoreClientBuilder
        : IEventStoreClientBuilder
    {

        /// <summary>
        /// Initializes a new <see cref="EventStoreClientBuilder"/>
        /// </summary>
        public EventStoreClientBuilder()
        {
            this.ConnectionString = "ConnectTo=tcp://admin:changeit@localhost:1113; DefaultUserCredentials=admin:changeit;";
        }

        /// <summary>
        /// Gets/sets the name of the <see cref="EventStoreClient"/> to create
        /// </summary>
        protected string Name { get; set; }

        /// <summary>
        /// Gets/sets the connection string of the <see cref="EventStoreClient"/> to create
        /// </summary>
        protected string ConnectionString { get; set; }

        /// <inheritdoc/>
        public IEventStoreClientBuilder WithName(string name)
        {
            this.Name = name;
            return this;
        }

        /// <inheritdoc/>
        public IEventStoreClientBuilder UseConnectionString(string connectionString)
        {
            this.ConnectionString = connectionString;
            return this;
        }

        /// <inheritdoc/>
        public EventStoreClient Build()
        {
            EventStoreClientSettings settings = EventStoreClientSettings.Create(this.ConnectionString);
            EventStoreClient client = new(settings);
            return client;
        }

    }

}
