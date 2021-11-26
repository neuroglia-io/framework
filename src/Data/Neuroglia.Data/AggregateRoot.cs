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

namespace Neuroglia.Data
{
    /// <summary>
    /// Represents the default implementation of the <see cref="IAggregateRoot"/> interface
    /// </summary>
    /// <typeparam name="TKey">The type of key used to uniquely identify the <see cref="IAggregateRoot"/></typeparam>
    public abstract class AggregateRoot<TKey>
        : Entity<TKey>, IAggregateRoot<TKey>
        where TKey : IEquatable<TKey>
    {

        /// <summary>
        /// Initializes a new <see cref="AggregateRoot{TKey}"/>
        /// </summary>
        /// <param name="id">The <see cref="AggregateRoot{TKey}"/>'s unique identifier</param>
        protected AggregateRoot(TKey id) 
            : base(id)
        {

        }

        private readonly List<IDomainEvent> _PendingEvents = new();
        /// <inheritdoc/>
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual IReadOnlyCollection<IDomainEvent> PendingEvents => this._PendingEvents;

        /// <summary>
        /// Registers the specified <see cref="IDomainEvent"/>
        /// </summary>
        /// <typeparam name="TEvent">The type of <see cref="IDomainEvent"/> to register</typeparam>
        /// <param name="e">The <see cref="IDomainEvent"/> to register</param>
        /// <returns>The registered <see cref="IDomainEvent"/></returns>
        protected virtual TEvent RegisterEvent<TEvent>(TEvent e)
            where TEvent : IDomainEvent
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));
            this._PendingEvents.Add(e);
            return e;
        }

        /// <inheritdoc/>
        public virtual void ClearPendingEvents()
        {
            this._PendingEvents.Clear();
        }
    
    }

}
