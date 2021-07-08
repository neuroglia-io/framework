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

namespace Neuroglia.Data
{

    /// <summary>
    /// Represents the default implementation of the <see cref="IDomainEvent{TAggregate, TKey}"/> interface
    /// </summary>
    /// <typeparam name="TAggregate">The type of <see cref="IAggregateRoot"/> that has produced the <see cref="IDomainEvent"/></typeparam>
    /// <typeparam name="TKey">The type of key used to uniquely identify the <see cref="IAggregateRoot"/> that has produced the <see cref="IDomainEvent"/></typeparam>
    public abstract class DomainEvent<TAggregate, TKey>
        : IDomainEvent<TAggregate, TKey>
        where TAggregate : IAggregateRoot<TKey>
        where TKey : IEquatable<TKey>
    {

        /// <summary>
        /// Initializes a new <see cref="DomainEvent{TAggregate, TKey}"/>
        /// </summary>
        protected DomainEvent()
        {

        }

        /// <summary>
        /// Initializes a new <see cref="DomainEvent{TAggregate, TKey}"/>
        /// </summary>
        /// <param name="aggregateId">The id of the <see cref="IAggregateRoot"/> that has produced the <see cref="IDomainEvent"/></param>
        protected DomainEvent(TKey aggregateId)
        {
            this.AggregateId = aggregateId;
            this.CreatedAt = DateTimeOffset.Now;
        }

        /// <inheritdoc/>
        public virtual TKey AggregateId { get; protected set; }

        object IDomainEvent.AggregateId => this.AggregateId;

        /// <inheritdoc/>
        public virtual Type AggregateType => typeof(TAggregate);

        /// <inheritdoc/>
        public virtual DateTimeOffset CreatedAt { get; protected set; }

    }

}
