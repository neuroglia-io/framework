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

namespace Neuroglia
{

    /// <summary>
    /// Defines the fundamentals of an object used to describe and manage an async duplex streaming connection
    /// </summary>
    /// <typeparam name="TOutbound">The type of outbound messages, meaning messages sent to the server</typeparam>
    /// <typeparam name="TInbound">The type of inbound messages, meaning messages received from the server</typeparam>
    public interface IAsyncDuplexStreamingConnection<in TOutbound, out TInbound>
    {

        /// <summary>
        /// Gets the service used to write messages to the outbound stream
        /// </summary>
        IAsyncStreamWriter<TOutbound> OutboundStream { get; }

        /// <summary>
        /// Gets the service used to read messages from the inbound stream
        /// </summary>
        IAsyncStreamReader<TInbound> InboundStream { get; }

    }

}
