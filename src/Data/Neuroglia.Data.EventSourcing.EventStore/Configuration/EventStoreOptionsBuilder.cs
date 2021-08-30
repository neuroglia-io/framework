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
using Neuroglia.Serialization;
using System;

namespace Neuroglia.Data.EventSourcing.EventStore.Configuration
{

    /// <summary>
    /// Represents the default implementation of the <see cref="IEventStoreOptionsBuilder"/> interface
    /// </summary>
    public class EventStoreOptionsBuilder
        : IEventStoreOptionsBuilder
    {

        /// <summary>
        /// Initializes a new <see cref="EventStoreOptionsBuilder"/>
        /// </summary>
        /// <param name="options">The <see cref="EventStoreOptions"/> to configure</param>
        public EventStoreOptionsBuilder(EventStoreOptions options)
        {
            this.Options = options;
        }

        /// <summary>
        /// Initializes a new <see cref="EventStoreOptionsBuilder"/>
        /// </summary>
        public EventStoreOptionsBuilder()
            : this(new EventStoreOptions())
        {

        }

        /// <summary>
        /// Gets the <see cref="EventStoreOptions"/> to configure
        /// </summary>
        protected EventStoreOptions Options { get; }

        /// <inheritdoc/>
        public IEventStoreOptionsBuilder UseAggregatorFactory<TFactory>()
            where TFactory : class, IAggregatorFactory
        {
            this.Options.AggregatorFactoryType = typeof(TFactory);
            return this;
        }

        /// <inheritdoc/>
        public IEventStoreOptionsBuilder UseSerializer<TSerializer>()
            where TSerializer : class, ISerializer
        {
            this.Options.SerializerType = typeof(TSerializer);
            return this;
        }

        /// <inheritdoc/>
        public IEventStoreOptionsBuilder ConfigureClient(Action<IEventStoreClientBuilder> setupAction)
        {
            this.Options.ClientSetupAction = setupAction;
            return this;
        }

        /// <inheritdoc/>
        public IEventStoreOptionsBuilder UseDefaultCredentials(UserCredentials credentials)
        {
            this.Options.DefaultCredentials = credentials;
            return this;
        }

        /// <inheritdoc/>
        public EventStoreOptions Build()
        {
            return this.Options;
        }

    }

}
