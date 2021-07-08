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
using System.Collections.Generic;

namespace Neuroglia.Data.EventSourcing
{

    /// <summary>
    /// Defines the fundamentals of a service used to aggregate <see cref="IDomainEvent"/>s
    /// </summary>
    public interface IAggregator
    {

        /// <summary>
        /// Aggregates the specified <see cref="IEvent"/>s
        /// </summary>
        /// <param name="events">An <see cref="IEnumerable{T}"/> containing the <see cref="IEvent"/>s to aggregate</param>
        /// <returns>The resulting aggregate</returns>
        object Aggregate(IEnumerable<IEvent> events);

        /// <summary>
        /// Aggregates the specified <see cref="IEvent"/>s
        /// </summary>
        /// <param name="state">The current state of the aggregate</param>
        /// <param name="events">An <see cref="IEnumerable{T}"/> containing the <see cref="IEvent"/>s to aggregate</param>
        /// <returns>The resulting aggregate</returns>
        object Aggregate(object state, IEnumerable<IEvent> events);

    }

    /// <summary>
    /// Defines the fundamentals of a service used to aggregate <see cref="IDomainEvent"/>s
    /// </summary>
    /// <typeparam name="TAggregate">The type of the expected aggregate</typeparam>
    public interface IAggregator<TAggregate>
        : IAggregator
        where TAggregate : class
    {

        /// <summary>
        /// Aggregates the specified <see cref="IEvent"/>s
        /// </summary>
        /// <param name="events">An <see cref="IEnumerable{T}"/> containing the <see cref="IEvent"/>s to aggregate</param>
        /// <returns>The resulting aggregate</returns>
        new TAggregate Aggregate(IEnumerable<IEvent> events);

        /// <summary>
        /// Aggregates the specified <see cref="IEvent"/>s
        /// </summary>
        /// <param name="state">The current state of the aggregate</param>
        /// <param name="events">An <see cref="IEnumerable{T}"/> containing the <see cref="IEvent"/>s to aggregate</param>
        /// <returns>The resulting aggregate</returns>
        TAggregate Aggregate(TAggregate state, IEnumerable<IEvent> events);

    }

}
