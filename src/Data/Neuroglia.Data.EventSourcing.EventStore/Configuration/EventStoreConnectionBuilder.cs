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

using EventStore.ClientAPI;
using System;

namespace Neuroglia.Data.EventSourcing.EventStore.Configuration
{
    /// <summary>
    /// Represents the default implementation of the <see cref="IEventStoreConnectionBuilder"/>
    /// </summary>
    public class EventStoreConnectionBuilder
        : IEventStoreConnectionBuilder
    {

        /// <summary>
        /// Initializes a new <see cref="EventStoreConnectionBuilder"/>
        /// </summary>
        public EventStoreConnectionBuilder()
        {
            this.ConnectionString = "ConnectTo=tcp://admin:changeit@localhost:1113; DefaultUserCredentials=admin:changeit;";
            this.SettingsConfigurationAction = settings => settings.UseConsoleLogger();
        }

        /// <summary>
        /// Gets/sets the name of the <see cref="IEventStoreConnection"/> to create
        /// </summary>
        protected string Name { get; set; }

        /// <summary>
        /// Gets/sets the connection string of the <see cref="IEventStoreConnection"/> to create
        /// </summary>
        protected string ConnectionString { get; set; }

        /// <summary>
        /// Gets/sets an <see cref="Action{T}"/> used to configure the <see cref="ConnectionSettings"/> of the <see cref="IEventStoreConnection"/> to create
        /// </summary>
        protected Action<ConnectionSettingsBuilder> SettingsConfigurationAction { get; set; }

        /// <inheritdoc/>
        public IEventStoreConnectionBuilder WithName(string name)
        {
            this.Name = name;
            return this;
        }

        /// <inheritdoc/>
        public IEventStoreConnectionBuilder UseConnectionString(string connectionString)
        {
            this.ConnectionString = connectionString;
            return this;
        }

        /// <inheritdoc/>
        public IEventStoreConnectionBuilder ConfigureSettings(Action<ConnectionSettingsBuilder> configurationAction)
        {
            this.SettingsConfigurationAction = configurationAction;
            return this;
        }

        /// <inheritdoc/>
        public IEventStoreConnection Build()
        {
            ConnectionSettingsBuilder settingsBuilder = ConnectionSettings.Create();
            this.SettingsConfigurationAction?.Invoke(settingsBuilder);
            IEventStoreConnection connection = EventStoreConnection.Create(this.ConnectionString, settingsBuilder, this.Name);
            connection.ConnectAsync().GetAwaiter().GetResult();
            return connection;
        }

    }

}
