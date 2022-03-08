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

using System;

namespace Neuroglia
{
    /// <summary>
    /// Represents the default implementation of the <see cref="IAsyncDuplexStreamingConnection{TOutbound, TInbound}"/> interface
    /// </summary>
    /// <typeparam name="TOutbound">The type of outbound messages, meaning messages sent to the server</typeparam>
    /// <typeparam name="TInbound">The type of inbound messages, meaning messages received from the server</typeparam>
    public class AsyncDuplexStreamingConnection<TOutbound, TInbound>
        : IAsyncDuplexStreamingConnection<TOutbound, TInbound>
    {

        /// <summary>
        /// Initializes a new <see cref="AsyncDuplexStreamingConnection{TOutbound, TInbound}"/>
        /// </summary>
        /// <param name="outboundStream">The service used to write messages to the outbound stream</param>
        /// <param name="inboundStream">The service used to read messages from the inbound stream</param>
        public AsyncDuplexStreamingConnection(IAsyncStreamWriter<TOutbound> outboundStream, IAsyncStreamReader<TInbound> inboundStream)
        {
            this.OutboundStream = outboundStream ?? throw new ArgumentNullException(nameof(outboundStream));
            this.InboundStream = inboundStream ?? throw new ArgumentNullException(nameof(inboundStream));
        }

        /// <inheritdoc/>
        public IAsyncStreamWriter<TOutbound> OutboundStream { get; }

        /// <inheritdoc/>
        public IAsyncStreamReader<TInbound> InboundStream { get; }

    }

}
