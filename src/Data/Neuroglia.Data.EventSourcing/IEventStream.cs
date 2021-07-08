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
    /// Defines the fundamentals of an object used to describe a stream of <see cref="IDomainEvent"/>s
    /// </summary>
    public interface IEventStream
        : IIdentifiable, IAsyncEnumerable<IEvent>
    {

        /// <summary>
        /// Gets the stream's length, or events count
        /// </summary>
        long Length { get; }

        /// <summary>
        /// Gets the current position in the stream
        /// </summary>
        long Position { get; }

        /// <summary>
        /// Gets the date and time at which the first event has been created
        /// </summary>
        DateTimeOffset FirstEventAt { get; }

        /// <summary>
        /// Gets the date and time at which the last event has been created
        /// </summary>
        DateTimeOffset LastEventAt { get; }

        /// <summary>
        /// Converts the <see cref="IEventStream"/> to a new <see cref="IEnumerable{T}"/> of <see cref="IEvent">events</see>
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A new <see cref="IEnumerable{T}"/> containing the <see cref="IEvent">events</see> the <see cref="IEventStream"/> is made out of</returns>
        Task<List<IEvent>> ToListAsync(CancellationToken cancellationToken = default);

    }

    /// <summary>
    /// Defines the fundamentals of an object used to describe a stream of <see cref="IDomainEvent"/>s
    /// </summary>
    public interface IEventStream<TKey>
        : IEventStream, IIdentifiable<TKey>, IAsyncEnumerable<IEvent<TKey>>
        where TKey : IEquatable<TKey>
    {

        /// <summary>
        /// Converts the <see cref="IEventStream{TKey}"/> to a new <see cref="IEnumerable{T}"/> of <see cref="IEvent{TKey}">events</see>
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A new <see cref="IEnumerable{T}"/> containing the <see cref="IEvent{TKey}">events</see> the <see cref="IEventStream{TKey}"/> is made out of</returns>
        new Task<List<IEvent<TKey>>> ToListAsync(CancellationToken cancellationToken = default);

    }

}
