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
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Neuroglia.Data.EventSourcing
{

    /// <summary>
    /// Defines the fundamentals of a service used to store <see cref="ISourcedEvent"/>s
    /// </summary>
    public interface IEventStore
    {

        /// <summary>
        /// Gets the specified <see cref="IAggregateRoot"/>'s <see cref="IEventStream"/>
        /// </summary>
        /// <param name="streamId">The id of the <see cref="IEventStream"/> to fetch</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>The <see cref="IEventStream"/> with the specified key</returns>
        Task<IEventStream> GetStreamAsync(string streamId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Appends events to the specified stream
        /// </summary>
        /// <param name="streamId">The id of the stream to append events to</param>
        /// <param name="events">An <see cref="IEnumerable{T}"/> containing the events to append to the specified stream</param>
        /// <param name="expectedVersion">The expected version of the stream to append events to</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        Task AppendToStreamAsync(string streamId, IEnumerable<IEventMetadata> events, long expectedVersion, CancellationToken cancellationToken = default);

        /// <summary>
        /// Appends events to the specified stream
        /// </summary>
        /// <param name="streamId">The id of the stream to append events to</param>
        /// <param name="events">An <see cref="IEnumerable{T}"/> containing the events to append to the specified stream</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        Task AppendToStreamAsync(string streamId, IEnumerable<IEventMetadata> events, CancellationToken cancellationToken = default);

        /// <summary>
        /// Reads the <see cref="ISourcedEvent"/>s of the specified <see cref="IEventStream"/> in a forward fashion
        /// </summary>
        /// <param name="streamId">The id of the <see cref="IEventStream"/> to get the <see cref="ISourcedEvent"/>s of</param>
        /// <param name="offset">The number of the <see cref="ISourcedEvent"/> to start reading the <see cref="IEventStream"/> from</param>
        /// <param name="length">The amount of <see cref="ISourcedEvent"/>s to read the <see cref="IEventStream"/> to</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A new <see cref="IEnumerable{T}"/> containing the <see cref="ISourcedEvent"/>s the <see cref="IEventStream"/> is made out of</returns>
        Task<IEnumerable<ISourcedEvent>> ReadEventsForwardAsync(string streamId, long offset, long length, CancellationToken cancellationToken = default);

        /// <summary>
        /// Reads the <see cref="ISourcedEvent"/>s of the specified <see cref="IEventStream"/> in a forward fashion
        /// </summary>
        /// <param name="streamId">The id of the <see cref="IEventStream"/> to get the <see cref="ISourcedEvent"/>s of</param>
        /// <param name="offset">The number of the <see cref="ISourcedEvent"/> to start reading the <see cref="IEventStream"/> from</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A new <see cref="IEnumerable{T}"/> containing the <see cref="ISourcedEvent"/>s the <see cref="IEventStream"/> is made out of</returns>
        Task<IEnumerable<ISourcedEvent>> ReadEventsForwardAsync(string streamId, long offset, CancellationToken cancellationToken = default);

        /// <summary>
        /// Reads all the <see cref="ISourcedEvent"/>s of the specified <see cref="IEventStream"/> in a forward fashion
        /// </summary>
        /// <param name="streamId">The id of the <see cref="IEventStream"/> to get the <see cref="ISourcedEvent"/>s of</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A new <see cref="IEnumerable{T}"/> containing all the <see cref="ISourcedEvent"/>s the <see cref="IEventStream"/> is made out of</returns>
        Task<IEnumerable<ISourcedEvent>> ReadAllEventsForwardAsync(string streamId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Reads the <see cref="ISourcedEvent"/>s of the specified <see cref="IEventStream"/> in a backward fashion
        /// </summary>
        /// <param name="streamId">The id of the <see cref="IEventStream"/> to get the <see cref="ISourcedEvent"/>s of</param>
        /// <param name="offset">The number of the <see cref="ISourcedEvent"/> to start reading the <see cref="IEventStream"/> from</param>
        /// <param name="length">The amount of <see cref="ISourcedEvent"/>s to read the <see cref="IEventStream"/> to</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A new <see cref="IEnumerable{T}"/> containing the <see cref="ISourcedEvent"/>s the <see cref="IEventStream"/> is made out of</returns>
        Task<IEnumerable<ISourcedEvent>> ReadEventsBackwardAsync(string streamId, long offset, long length, CancellationToken cancellationToken = default);

        /// <summary>
        /// Reads the <see cref="ISourcedEvent"/>s of the specified <see cref="IEventStream"/> in a backward fashion
        /// </summary>
        /// <param name="streamId">The id of the <see cref="IEventStream"/> to get the <see cref="ISourcedEvent"/>s of</param>
        /// <param name="offset">The number of the <see cref="ISourcedEvent"/> to start reading the <see cref="IEventStream"/> from</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A new <see cref="IEnumerable{T}"/> containing the <see cref="ISourcedEvent"/>s the <see cref="IEventStream"/> is made out of</returns>
        Task<IEnumerable<ISourcedEvent>> ReadEventsBackwardAsync(string streamId, long offset, CancellationToken cancellationToken = default);

        /// <summary>
        /// Reads all the <see cref="ISourcedEvent"/>s of the specified <see cref="IEventStream"/> in a backward fashion
        /// </summary>
        /// <param name="streamId">The id of the <see cref="IEventStream"/> to get the <see cref="ISourcedEvent"/>s of</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A new <see cref="IEnumerable{T}"/> containing all the <see cref="ISourcedEvent"/>s the <see cref="IEventStream"/> is made out of</returns>
        Task<IEnumerable<ISourcedEvent>> ReadAllEventsBackwardAsync(string streamId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Subscribes to the specified event stream
        /// </summary>
        /// <param name="streamId">The id of the stream to subscribe to</param>
        /// <param name="handler">A <see cref="Func{T1, T2, TResult}"/> used to handle the subscription</param>
        /// <param name="setup">An <see cref="Action{T}"/> used to setup the subscription options</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>The id of the resulting subscription</returns>
        Task<string> SubscribeToStreamAsync(string streamId, Action<ISubscriptionOptionsBuilder> setup, Func<IServiceProvider, ISourcedEvent, Task> handler, CancellationToken cancellationToken = default);

        /// <summary>
        /// Subscribes to the specified event stream
        /// </summary>
        /// <param name="streamId">The id of the stream to subscribe to</param>
        /// <param name="handler">A <see cref="Func{T1, T2, TResult}"/> used to handle the subscription</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>The id of the resulting subscription</returns>
        Task<string> SubscribeToStreamAsync(string streamId, Func<IServiceProvider, ISourcedEvent, Task> handler, CancellationToken cancellationToken = default);

        /// <summary>
        /// Unsubscribes from the specified subscription
        /// </summary>
        /// <param name="subscriptionId">The id of the subscription to unsubscribe from</param>
        void UnsubscribeFrom(string subscriptionId);

        /// <summary>
        /// Truncates the specified stream
        /// </summary>
        /// <param name="streamId">The id of the stream to truncate</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        Task TruncateStreamAsync(string streamId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Truncates the specified stream
        /// </summary>
        /// <param name="streamId">The id of the stream to truncate</param>
        /// <param name="beforeVersion">The version before which to truncate the stream</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        Task TruncateStreamAsync(string streamId, long beforeVersion, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes the specified stream
        /// </summary>
        /// <param name="streamId">The id of the stream to delete</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        Task DeleteStreamAsync(string streamId, CancellationToken cancellationToken = default);

    }

}
