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
    /// Defines the fundamentals of a service used to build <see cref="EventStoreOptions"/>
    /// </summary>
    public interface IEventStoreOptionsBuilder
    {

        /// <summary>
        /// Uses the specified <see cref="ISerializer"/> to serialize and deserialize events
        /// </summary>
        /// <typeparam name="TSerializer">The type of <see cref="ISerializer"/> to use to serialize and deserialize events</typeparam>
        /// <returns>The configured <see cref="IEventStoreOptionsBuilder"/></returns>
        IEventStoreOptionsBuilder UseSerializer<TSerializer>()
            where TSerializer : class, ISerializer;

        /// <summary>
        /// Uses the specified <see cref="IAggregatorFactory"/> to create <see cref="IAggregator"/>s
        /// </summary>
        /// <typeparam name="TFactory">The type of <see cref="IAggregatorFactory"/> to use to create <see cref="IAggregator"/>s</typeparam>
        /// <returns>The configured <see cref="IEventStoreOptionsBuilder"/></returns>
        IEventStoreOptionsBuilder UseAggregatorFactory<TFactory>()
            where TFactory : class, IAggregatorFactory;

        /// <summary>
        /// Configures the <see cref="IEventStoreConnection"/> to use
        /// </summary>
        /// <param name="configurationAction">An <see cref="Action{T}"/> used to configure the <see cref="IEventStoreConnection"/> to use</param>
        /// <returns>The configured <see cref="IEventStoreOptionsBuilder"/></returns>
        IEventStoreOptionsBuilder UseConnection(Action<IEventStoreConnectionBuilder> configurationAction);

        /// <summary>
        /// Configures the <see cref="ProjectionsManager"/> to use
        /// </summary>
        /// <param name="configurationAction">An <see cref="Action{T}"/> used to configure the <see cref="ProjectionsManager"/> to use</param>
        /// <returns>The configured <see cref="IEventStoreOptionsBuilder"/></returns>
        IEventStoreOptionsBuilder UseProjectionsManager(Action<IEventStoreProjectionsManagerBuilder> configurationAction);

        /// <summary>
        /// Uses the specified <see cref="UserCredentials"/> when contacting the remote server
        /// </summary>
        /// <param name="credentials">The default <see cref="UserCredentials"/> to use</param>
        /// <returns>The configured <see cref="IEventStoreOptionsBuilder"/></returns>
        IEventStoreOptionsBuilder UseDefaultCredentials(UserCredentials credentials);

        /// <summary>
        /// Builds the <see cref="EventStoreOptions"/>
        /// </summary>
        /// <returns>A new <see cref="EventStoreOptions"/></returns>
        EventStoreOptions Build();

    }

}
