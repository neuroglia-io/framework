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

using EventStore.ClientAPI.Projections;
using System;
using System.Net;

namespace Neuroglia.Data.EventSourcing.EventStore.Configuration
{
    /// <summary>
    /// Defines the fundamentals of a service used to build a <see cref="ProjectionsManager"/>
    /// </summary>
    public interface IEventStoreProjectionsManagerBuilder
    {

        /// <summary>
        /// Uses the specified <see cref="EndPoint"/>
        /// </summary>
        /// <param name="endPoint">The <see cref="EndPoint"/> to use</param>
        /// <returns>The configured <see cref="IEventStoreProjectionsManagerBuilder"/></returns>
        IEventStoreProjectionsManagerBuilder UseEndpoint(EndPoint endPoint);

        /// <summary>
        /// Uses the specified secured <see cref="EndPoint"/>
        /// </summary>
        /// <param name="endPoint">The secured <see cref="EndPoint"/> to use</param>
        /// <returns>The configured <see cref="IEventStoreProjectionsManagerBuilder"/></returns>
        IEventStoreProjectionsManagerBuilder UseSecuredEndpoint(EndPoint endPoint);

        /// <summary>
        /// Configures the <see cref="ProjectionsManager"/> to use the specify operations timeout
        /// </summary>
        /// <param name="timeout">A <see cref="TimeSpan"/> representing the timeout to use when executing operations</param>
        /// <returns>The configured <see cref="IEventStoreProjectionsManagerBuilder"/></returns>
        IEventStoreProjectionsManagerBuilder WithOperationTimeout(TimeSpan timeout);

        /// <summary>
        /// Builds the <see cref="ProjectionsManager"/>
        /// </summary>
        /// <returns>A new <see cref="ProjectionsManager"/></returns>
        ProjectionsManager Build();

    }

}
