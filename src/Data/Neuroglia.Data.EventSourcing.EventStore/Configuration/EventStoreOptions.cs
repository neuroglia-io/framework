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
using EventStore.ClientAPI.Projections;
using EventStore.ClientAPI.SystemData;
using Neuroglia.Serialization;
using System;

namespace Neuroglia.Data.EventSourcing.EventStore.Configuration
{

    /// <summary>
    /// Represents the options used to configure an <see cref="ESEventStore"/>
    /// </summary>
    public class EventStoreOptions
    {

        /// <summary>
        /// Initializes a new <see cref="EventStoreOptions"/>
        /// </summary>
        public EventStoreOptions()
        {
            this.SerializerType = typeof(NewtonsoftJsonSerializer);
            this.AggregatorFactoryType = typeof(AggregatorFactory);
            this.ConnectionConfigurationAction = builder => { };
        }

        /// <summary>
        /// Gets/sets the maximum length for an <see cref="ISourcedEvent"/> slice. Defaults to 100.
        /// </summary>
        public virtual int MaxSliceLength { get; set; } = 100;

        /// <summary>
        /// Gets/sets the type of <see cref="ISerializer"/> to use to serialize and deserialize events
        /// </summary>
        public Type SerializerType { get; set; }

        /// <summary>
        /// Gets/sets the type of <see cref="IAggregatorFactory"/> to use to create <see cref="IAggregator"/>s
        /// </summary>
        public Type AggregatorFactoryType { get; set; }

        /// <summary>
        /// Gets/sets an <see cref="Action{T}"/> used to configure the <see cref="IEventStoreConnection"/> to use
        /// </summary>
        public Action<IEventStoreConnectionBuilder> ConnectionConfigurationAction { get; set; }

        /// <summary>
        /// Gets/sets an <see cref="Action{T}"/> used to configure the <see cref="ProjectionsManager"/> to use
        /// </summary>
        public Action<IEventStoreProjectionsManagerBuilder> ProjectionsManagerConfigurationAction { get; set; }

        /// <summary>
        /// Gets/sets the default <see cref="UserCredentials"/> to use to connect to the remote EventStore server
        /// </summary>
        public UserCredentials DefaultCredentials { get; set; }

    }

}
