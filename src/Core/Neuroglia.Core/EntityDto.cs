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

namespace Neuroglia
{

    /// <summary>
    /// Describes an entity Data Transfer Object (DTO)
    /// </summary>
    /// <typeparam name="TKey">The type of key the descibed entity is uniquely identified by</typeparam>
    public abstract class EntityDto<TKey>
        : DataTransferObject, IIdentifiable<TKey>
        where TKey : IEquatable<TKey>
    {

        /// <summary>
        /// Gets/sets the entity's id
        /// </summary>
        public virtual TKey Id { get; set; }

        object IIdentifiable.Id
        {
            get
            {
                return this.Id;
            }
        }

        /// <summary>
        /// Gets/sets the date and time the entity has been created at
        /// </summary>
        public virtual DateTimeOffset CreatedAt { get; set; }

    }


}
