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
using Neuroglia.Data.EventSourcing;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Neuroglia.Data
{

    /// <summary>
    /// Defines the fundamentals of an <see cref="IRepository{TEntity, TKey}"/> that uses event sourcing to store the <see cref="IDomainEvent"/>s of the specified <see cref="IAggregateRoot"/> type
    /// </summary>
    /// <typeparam name="TAggregate">The type of <see cref="IAggregateRoot"/> managed by the <see cref="IEventSourcingRepository{TAggregate, TKey}"/></typeparam>
    public interface IEventSourcingRepository<TAggregate>
        : IRepository<TAggregate>
        where TAggregate : class, IAggregateRoot
    {

        /// <summary>
        /// Gets the specified <see cref="IAggregateRoot"/>'s <see cref="IEventStream"/>
        /// </summary>
        /// <param name="streamId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<IEventStream> GetStreamAsync(object streamId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Finds the <see cref="IAggregateRoot"/> with the specified key and version
        /// </summary>
        /// <param name="streamId">The key of the <see cref="IAggregateRoot"/> to find</param>
        /// <param name="version">The version of the <see cref="IAggregateRoot"/> to find</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>The <see cref="IAggregateRoot"/> with the specified key and version, if any</returns>
        Task<TAggregate> FindAsync(object streamId, long version, CancellationToken cancellationToken = default);

    }

    /// <summary>
    /// Defines the fundamentals of an <see cref="IRepository{TEntity, TKey}"/> that uses event sourcing to store the <see cref="IDomainEvent"/>s of the specified <see cref="IAggregateRoot"/> type
    /// </summary>
    /// <typeparam name="TAggregate">The type of <see cref="IAggregateRoot"/> managed by the <see cref="IEventSourcingRepository{TAggregate, TKey}"/></typeparam>
    /// <typeparam name="TKey">The type of key used to uniquely identify the <see cref="IAggregateRoot"/>s managed by the <see cref="IEventSourcingRepository{TAggregate, TKey}"/></typeparam>
    public interface IEventSourcingRepository<TAggregate, TKey>
        : IEventSourcingRepository<TAggregate>, IRepository<TAggregate, TKey>
        where TAggregate : class, IAggregateRoot<TKey>
        where TKey : IEquatable<TKey>
    {

        /// <summary>
        /// Gets the specified <see cref="IAggregateRoot"/>'s <see cref="IEventStream"/>
        /// </summary>
        /// <param name="streamId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<IEventStream<TKey>> GetStreamAsync(TKey streamId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Finds the <see cref="IAggregateRoot"/> with the specified key and version
        /// </summary>
        /// <param name="key">The key of the <see cref="IAggregateRoot"/> to find</param>
        /// <param name="version">The version of the <see cref="IAggregateRoot"/> to find</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>The <see cref="IAggregateRoot"/> with the specified key and version, if any</returns>
        Task<TAggregate> FindAsync(TKey key, long version, CancellationToken cancellationToken = default);

    }

}
