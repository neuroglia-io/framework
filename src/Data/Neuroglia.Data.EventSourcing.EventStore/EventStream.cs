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
using EventStore.ClientAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Neuroglia.Data.EventSourcing.EventStore
{

    /// <summary>
    /// Represents the <see href="https://www.eventstore.com/">Event Store</see> implementation of the <see cref="IEventStream{TKey}"/> interface
    /// </summary>
    /// <typeparam name="TKey">The type of key used to uniquely identify the <see cref="EventStream{TKey}"/></typeparam>
    public class EventStream<TKey>
        : IEventStream<TKey>
        where TKey : IEquatable<TKey>
    {

        /// <summary>
        /// Initializes a new <see cref="EventStream"/>
        /// </summary>
        /// <param name="connection">The service used to interact with the <see href="https://www.eventstore.com/">Event Store</see> API</param>
        /// <param name="streamId">The id of the described stream</param>
        /// <param name="length">The length of the stream</param>
        /// <param name="firstEventAt">The date and time at which the first event of the stream has been created</param>
        /// <param name="lastEventAt">The date and time at which the last event of the stream has been created</param>
        public EventStream(IEventStoreConnection connection, TKey streamId, long length, DateTime firstEventAt, DateTime lastEventAt)
        {
            this.Connection = connection;
            this.Id = streamId;
            this.Length = length;
            this.FirstEventAt = firstEventAt;
            this.LastEventAt = lastEventAt;
        }

        /// <summary>
        /// Gets the service used to interact with the <see href="https://www.eventstore.com/">Event Store</see> API
        /// </summary>
        protected IEventStoreConnection Connection { get; }

        /// <inheritdoc/>
        public virtual TKey Id { get; }

        object IIdentifiable.Id => this.Id;

        /// <inheritdoc/>
        public virtual long Length { get; }

        /// <inheritdoc/>
        public virtual DateTimeOffset FirstEventAt { get; }

        /// <inheritdoc/>
        public virtual DateTimeOffset LastEventAt { get; }

        /// <inheritdoc/>
        public virtual long Position { get; protected set; }

        /// <inheritdoc/>
        public virtual async IAsyncEnumerator<IEvent<TKey>> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            StreamEventsSlice slice;
            do
            {
                slice = await this.Connection.ReadStreamEventsForwardAsync(this.Id.ToString(), this.Position, 1, true);
                yield return slice.Events.First().AsAbstraction<TKey>();
                this.Position++;
            }
            while (!cancellationToken.IsCancellationRequested
                &&!slice.IsEndOfStream);
        }

        IAsyncEnumerator<IEvent> IAsyncEnumerable<IEvent>.GetAsyncEnumerator(CancellationToken cancellationToken)
        {
            return this.GetAsyncEnumerator(cancellationToken);
        }

        /// <inheritdoc/>
        public virtual async Task<List<IEvent<TKey>>> ToListAsync(CancellationToken cancellationToken = default)
        {
            List<ResolvedEvent> resolvedEvents = await this.Connection.ReadStreamEventsForwardAsync(this.Id.ToString(), this.Position);
            return resolvedEvents.Select(e => e.AsAbstraction<TKey>()).ToList();
        }

        async Task<List<IEvent>> IEventStream.ToListAsync(CancellationToken cancellationToken)
        {
            return (await this.ToListAsync(cancellationToken)).OfType<IEvent>().ToList();
        }

    }

}
