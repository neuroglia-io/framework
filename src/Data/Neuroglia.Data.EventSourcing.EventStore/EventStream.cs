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

namespace Neuroglia.Data.EventSourcing.EventStore
{

    /// <summary>
    /// Represents the <see href="https://www.eventstore.com/">Event Store</see> implementation of the <see cref="IEventStream"/> interface
    /// </summary>
    public class EventStream
        : IEventStream
    {

        /// <summary>
        /// Initializes a new <see cref="EventStream"/>
        /// </summary>
        /// <param name="streamId">The id of the described stream</param>
        /// <param name="length">The length of the stream</param>
        /// <param name="firstEventAt">The date and time at which the first event of the stream has been created</param>
        /// <param name="lastEventAt">The date and time at which the last event of the stream has been created</param>
        public EventStream(object streamId, long length, DateTime firstEventAt, DateTime lastEventAt)
        {
            this.Id = streamId;
            this.Length = length;
            this.FirstEventAt = firstEventAt;
            this.LastEventAt = lastEventAt;
        }

        /// <inheritdoc/>
        public virtual object Id { get; }

        /// <inheritdoc/>
        public virtual long Length { get; }

        /// <inheritdoc/>
        public virtual DateTimeOffset FirstEventAt { get; }

        /// <inheritdoc/>
        public virtual DateTimeOffset LastEventAt { get; }

        /// <inheritdoc/>
        public virtual long Position { get; protected set; }

    }

}
