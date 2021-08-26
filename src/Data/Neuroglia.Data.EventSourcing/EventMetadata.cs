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
    /// Represents the default implementation of the <see cref="IEventMetadata"/> interface
    /// </summary>
    public class EventMetadata
        : IEventMetadata
    {

        /// <summary>
        /// Initializes a new <see cref="EventMetadata"/>
        /// </summary>
        /// <param name="id">The event's id</param>
        /// <param name="type">The event's type</param>
        /// <param name="data">The event's data</param>
        /// <param name="metadata">The event's metadata</param>
        public EventMetadata(Guid id, string type, object data, object metadata)
        {
            this.Id = id;
            this.Type = type;
            this.Data = data;
            this.Metadata = metadata;
        }

        /// <summary>
        /// Initializes a new <see cref="EventMetadata"/>
        /// </summary>
        /// <param name="id">The event's id</param>
        /// <param name="type">The event's type</param>
        /// <param name="data">The event's data</param>
        public EventMetadata(Guid id, string type, object data)
            : this(id, type, data, null)
        {

        }

        /// <summary>
        /// Initializes a new <see cref="EventMetadata"/>
        /// </summary>
        /// <param name="type">The event's type</param>
        /// <param name="data">The event's data</param>
        public EventMetadata(string type, object data)
            : this(Guid.NewGuid(), type, data)
        {

        }

        /// <summary>
        /// Initializes a new <see cref="EventMetadata"/>
        /// </summary>
        /// <param name="type">The event's type</param>
        /// <param name="data">The event's data</param>
        /// <param name="metadata">The event's metadata</param>
        public EventMetadata(string type, object data, object metadata)
            : this(Guid.NewGuid(), type, data, metadata)
        {

        }

        /// <inheritdoc/>
        public Guid Id { get; }

        /// <inheritdoc/>
        public string Type { get; }

        /// <inheritdoc/>
        public object Data { get; }

        /// <inheritdoc/>
        public object Metadata { get; }

    }

}
