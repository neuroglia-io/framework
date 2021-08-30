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
using System.Threading;

namespace Neuroglia.Data.EventSourcing
{

    /// <summary>
    /// Represents the default implementation of the <see cref="IEventStream"/> interface
    /// </summary>
    public class EventStream
        : IEventStream
    {

        /// <summary>
        /// Initializes a new <see cref="EventStream"/>
        /// </summary>
        /// <param name="eventStore">The <see cref="IEventStore"/> used to read the <see cref="EventStream"/>'s <see cref="ISourcedEvent"/>s</param>
        /// <param name="id">The id of the described stream</param>
        /// <param name="length">The length of the stream</param>
        /// <param name="firstEventAt">The date and time at which the first event of the stream has been created</param>
        /// <param name="lastEventAt">The date and time at which the last event of the stream has been created</param>
        /// <param name="current">The <see cref="ISourcedEvent"/> at the current position</param>
        public EventStream(IEventStore eventStore, object id, long length, DateTime firstEventAt, DateTime lastEventAt, ISourcedEvent current)
        {
            this.EventStore = eventStore;
            this.Id = id;
            this.Length = length;
            this.FirstEventAt = firstEventAt;
            this.LastEventAt = lastEventAt;
            this.Current = current;
        }

        /// <summary>
        /// Gets the <see cref="IEventStore"/> used to read the <see cref="EventStream"/>'s <see cref="ISourcedEvent"/>s
        /// </summary>
        protected virtual IEventStore EventStore { get; }

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

        /// <inheritdoc/>
        public virtual ISourcedEvent Current { get; protected set; }

        /// <inheritdoc/>
        public virtual async IAsyncEnumerator<ISourcedEvent> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            do
            {
                yield return this.Current;
                this.Current = await this.EventStore.ReadSingleEventForwardAsync(this.Id.ToString(), this.Position + 1, cancellationToken);
                this.Position++;
            }
            while (this.Current != null);
        }

    }

}
