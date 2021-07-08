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

namespace Neuroglia.Data.EventSourcing
{

    /// <summary>
    /// Represents the default implementation of the <see cref="IEvent{TKey}"/> interface
    /// </summary>
    /// <typeparam name="TKey">The type of key used to uniquely identify the <see cref="IEvent{TKey}"/></typeparam>
    public class Event<TKey>
        : IEvent<TKey>
        where TKey : IEquatable<TKey>
    {

        /// <summary>
        /// Initializes a new <see cref="Event{TKey}"/>
        /// </summary>
        protected Event()
        {

        }

        /// <summary>
        /// Initializes a new <see cref="Event{TKey}"/>
        /// </summary>
        /// <param name="id">The id used to uniquely identity the <see cref="Event{TKey}"/></param>
        /// <param name="sequence">The <see cref="Event{TKey}"/>'s sequence</param>
        /// <param name="createdAt">The date and time at which the <see cref="Event{TKey}"/> has been created</param>
        /// <param name="type">The type of the described event</param>
        /// <param name="data">The data of the described event</param>
        /// <param name="metadata">The metadata of the described event</param>
        public Event(TKey id, long sequence, DateTimeOffset createdAt, string type, object data, object metadata)
        {
            this.Id = id;
            this.Sequence = sequence;
            this.CreatedAt = createdAt;
            this.Type = type;
            this.Data = data;
            this.Metadata = metadata;
        }

        /// <inheritdoc/>
        public virtual TKey Id { get; }

        object IEvent.Id => this.Id;

        /// <inheritdoc/>
        public virtual long Sequence { get; }

        /// <inheritdoc/>
        public virtual DateTimeOffset CreatedAt { get; }

        /// <inheritdoc/>
        public virtual string Type { get; }

        /// <inheritdoc/>
        public virtual object Data { get; }

        /// <inheritdoc/>
        public virtual object Metadata { get; }

    }

}
