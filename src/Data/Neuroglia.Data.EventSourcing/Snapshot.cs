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
    /// Represents the snapshot envelope of an <see cref="IAggregateRoot"/>
    /// </summary>
    public abstract class Snapshot
        : ISnapshot
    {

        /// <summary>
        /// Initializes a new <see cref="Snapshot{TKey}"/>
        /// </summary>
        protected Snapshot()
        {

        }

        /// <summary>
        /// Initializes a new <see cref="Snapshot{TKey}"/>
        /// </summary>
        /// <param name="data">The <see cref="IAggregateRoot"/> to snapshot</param>
        /// <param name="metadata">The metadata of the <see cref="Snapshot"/> to create</param>
        protected Snapshot(IAggregateRoot data, object metadata)
        {
            this.Version = data.Version;
            this.Data = data;
            this.Metadata = metadata;
        }

        /// <summary>
        /// Initializes a new <see cref="Snapshot{TKey}"/>
        /// </summary>
        /// <param name="data">The <see cref="IAggregateRoot"/> to snapshot</param>
        protected Snapshot(IAggregateRoot data)
            : this(data, null)
        {
   
        }

        /// <inheritdoc/>
        public virtual long Version { get; protected set; }

        /// <inheritdoc/>
        public virtual IAggregateRoot Data { get; protected set; }

        /// <inheritdoc/>
        public virtual object Metadata { get; protected set; }

        /// <summary>
        /// Creates a new <see cref="Snapshot{TAggregate}"/> for the specified <see cref="IAggregateRoot"/>
        /// </summary>
        /// <param name="aggregate">The <see cref="IAggregateRoot"/> to create a new <see cref="Snapshot{TAggregate}"/> for</param>
        /// <returns>A new <see cref="Snapshot{TAggregate}"/> of the specified <see cref="IAggregateRoot"/></returns>
        public static Snapshot<TAggregate> CreateFor<TAggregate>(TAggregate aggregate)
            where TAggregate : class, IAggregateRoot
        {
            return new(aggregate);
        }

    }

    /// <summary>
    /// Represents the snapshot envelope of an <see cref="IAggregateRoot"/>
    /// </summary>
    /// <typeparam name="TAggregate">The type of the snapshot <see cref="IAggregateRoot"/></typeparam>
    public class Snapshot<TAggregate>
        : Snapshot, ISnapshot<TAggregate>
        where TAggregate : class, IAggregateRoot
    {

        /// <summary>
        /// Initializes a new <see cref="Snapshot{TKey}"/>
        /// </summary>
        protected Snapshot()
        {

        }

        /// <summary>
        /// Initializes a new <see cref="Snapshot{TKey}"/>
        /// </summary>
        /// <param name="data">The <see cref="IAggregateRoot"/> to snapshot</param>
        /// <param name="metadata">The metadata of the <see cref="Snapshot"/> to create</param>
        public Snapshot(TAggregate data, object metadata)
            : base(data, metadata)
        {

        }

        /// <summary>
        /// Initializes a new <see cref="Snapshot{TKey}"/>
        /// </summary>
        public Snapshot(TAggregate data)
            : base(data, null)
        {

        }

        /// <inheritdoc/>
        public new virtual TAggregate Data
        {
            get
            {
                return (TAggregate)base.Data;
            }
            protected set
            {
                base.Data = value;
            }
        }

    }

}
