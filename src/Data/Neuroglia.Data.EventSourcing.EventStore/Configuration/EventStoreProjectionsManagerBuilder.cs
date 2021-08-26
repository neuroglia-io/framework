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

using EventStore.ClientAPI.Common.Log;
using EventStore.ClientAPI.Projections;
using System;
using System.Net;

namespace Neuroglia.Data.EventSourcing.EventStore.Configuration
{
    /// <summary>
    /// Represents the default implementation of the <see cref="IEventStoreProjectionsManagerBuilder"/> interface
    /// </summary>
    public class EventStoreProjectionsManagerBuilder
        : IEventStoreProjectionsManagerBuilder
    {

        /// <summary>
        /// Initializes a new <see cref="EventStoreProjectionsManagerBuilder"/>
        /// </summary>
        public EventStoreProjectionsManagerBuilder()
        {
            this.UseEndpoint(new IPEndPoint(IPAddress.Loopback, 2113));
            this.WithOperationTimeout(TimeSpan.FromMilliseconds(3000));
        }

        /// <summary>
        /// Gets/sets the <see cref="EndPoint"/> to use
        /// </summary>
        protected EndPoint Endpoint { get; set; }

        /// <summary>
        /// Gets/sets the operations timeout to use
        /// </summary>
        protected TimeSpan OperationTimeout { get; set; }

        /// <summary>
        /// Gets/sets the http scheme to use
        /// </summary>
        protected string HttpScheme { get; set; }

        /// <inheritdoc/>
        public IEventStoreProjectionsManagerBuilder UseEndpoint(EndPoint endPoint)
        {
            this.Endpoint = endPoint;
            this.HttpScheme = "http";
            return this;
        }

        /// <inheritdoc/>
        public IEventStoreProjectionsManagerBuilder UseSecuredEndpoint(EndPoint endPoint)
        {
            this.Endpoint = endPoint;
            this.HttpScheme = "https";
            return this;
        }

        /// <inheritdoc/>
        public IEventStoreProjectionsManagerBuilder WithOperationTimeout(TimeSpan timeout)
        {
            this.OperationTimeout = timeout;
            return this;
        }

        /// <inheritdoc/>
        public ProjectionsManager Build()
        {
            return new ProjectionsManager(new ConsoleLogger(), this.Endpoint, this.OperationTimeout, httpSchema: this.HttpScheme);
        }

    }

}
