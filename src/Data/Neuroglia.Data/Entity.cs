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
    /// Represents the default implementation of the <see cref="IEntity"/> interface
    /// </summary>
    /// <typeparam name="TKey">The type of key used to uniquely identify the <see cref="IEntity"/></typeparam>
    public abstract class Entity<TKey>
        : IEntity<TKey>
        where TKey : IEquatable<TKey>
    {

        /// <summary>
        /// Initializes a new <see cref="Entity{TKey}"/>
        /// </summary>
        /// <param name="id">The <see cref="IEntity"/>'s unique key</param>
        protected Entity(TKey id)
        {
            this.Id = id;
        }

        /// <inheritdoc/>
        public virtual TKey Id { get; protected set; }

        /// <inheritdoc/>
        public virtual DateTimeOffset CreatedAt { get; protected set; }

        /// <inheritdoc/>
        public virtual DateTimeOffset LastModified { get; protected set; }

        /// <inheritdoc/>
        public virtual int Version { get; protected set; }

        object IIdentifiable.Id => this.Id;

        void IEntity.SetVersion(int version)
        {
            if (this.Version < 0)
                throw new ArgumentOutOfRangeException(nameof(version));
            this.Version = version;
        }

    }

}
