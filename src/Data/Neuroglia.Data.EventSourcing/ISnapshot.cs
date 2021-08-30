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

namespace Neuroglia.Data.EventSourcing
{

    /// <summary>
    /// Defines the fundamentals of an <see cref="IAggregateRoot"/> snapshot
    /// </summary>
    public interface ISnapshot
    {

        /// <summary>
        /// Gets the version of the <see cref="ISnapshot"/>'s <see cref="IAggregateRoot"/>
        /// </summary>
        long Version { get; }

        /// <summary>
        /// Gets the <see cref="ISnapshot"/>'s <see cref="IAggregateRoot"/>
        /// </summary>
        IAggregateRoot Data { get; }

        /// <summary>
        /// Gets the <see cref="ISnapshot"/>'s metadata
        /// </summary>
        object Metadata { get; }

    }

    /// <summary>
    /// Defines the fundamentals of an <see cref="IAggregateRoot"/> snapshot
    /// </summary>
    /// <typeparam name="TAggregate">The type of the snapshot <see cref="IAggregateRoot"/></typeparam>
    public interface ISnapshot<TAggregate>
        : ISnapshot
        where TAggregate : class, IAggregateRoot
    {

        /// <summary>
        /// Gets the <see cref="ISnapshot"/>'s <see cref="IAggregateRoot"/>
        /// </summary>
        new TAggregate Data { get; }

    }

}
