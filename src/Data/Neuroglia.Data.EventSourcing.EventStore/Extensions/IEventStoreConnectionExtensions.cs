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
using System.Threading.Tasks;

namespace Neuroglia.Data.EventSourcing
{

    /// <summary>
    /// Defines extensions for <see cref="IEventStoreConnection"/>s
    /// </summary>
    public static class IEventStoreConnectionExtensions
    {

        /// <summary>
        /// Reads events from the specified stream
        /// </summary>
        /// <param name="connection">The extended <see cref="IEventStoreConnection"/></param>
        /// <param name="streamId">The id of the stream to read</param>
        /// <param name="position">The position starting from which to read events</param>
        /// <param name="maxSliceLength">The maximum length of the <see cref="StreamEventsSlice"/>s used to page events</param>
        /// <param name="resolveLinkTos">Whether to resolve LinkTo events automatically</param>
        /// <returns>A new <see cref="List{T}"/> containing the events read from the specified stream</returns>
        public static async Task<List<ResolvedEvent>> ReadStreamEventsForwardAsync(this IEventStoreConnection connection, string streamId, long position = StreamPosition.Start, long maxSliceLength = 100, bool resolveLinkTos = false)
        {
            if (string.IsNullOrWhiteSpace(streamId))
                throw new ArgumentNullException(nameof(streamId));
            if (position < StreamPosition.End)
                throw new ArgumentOutOfRangeException(nameof(position));
            if(maxSliceLength < 1)
                throw new ArgumentOutOfRangeException(nameof(maxSliceLength));
            EventReadResult lastEvent = await connection.ReadEventAsync(streamId, StreamPosition.End, false);
            List<ResolvedEvent> streamEvents = new();
            StreamEventsSlice currentSlice;
            long nextSliceStart = position;
            long sliceLength = lastEvent.EventNumber - position;
            if (sliceLength > maxSliceLength)
                sliceLength = maxSliceLength;
            do
            {
                currentSlice = await connection.ReadStreamEventsForwardAsync(streamId, nextSliceStart, (int)sliceLength, resolveLinkTos);
                nextSliceStart = currentSlice.NextEventNumber;
                streamEvents.AddRange(currentSlice.Events);
            }
            while (!currentSlice.IsEndOfStream);
            return streamEvents;
        }

        /// <summary>
        /// Reads and abstracts events from the specified stream
        /// </summary>
        /// <param name="connection">The extended <see cref="IEventStoreConnection"/></param>
        /// <param name="streamId">The id of the stream to read</param>
        /// <param name="position">The position starting from which to read events</param>
        /// <param name="maxSliceLength">The maximum length of the <see cref="StreamEventsSlice"/>s used to page events</param>
        /// <param name="resolveLinkTos">Whether to resolve LinkTo events automatically</param>
        /// <returns>A new <see cref="List{T}"/> containing the events read from the specified stream</returns>
        public static async Task<List<ISourcedEvent<TKey>>> ReadAndAbstractStreamEventsForwardAsync<TKey>(this IEventStoreConnection connection, string streamId, long position = StreamPosition.Start, long maxSliceLength = 100, bool resolveLinkTos = false)
            where TKey : IEquatable<TKey>
        {
            List<ResolvedEvent> events = await connection.ReadStreamEventsForwardAsync(streamId, position, maxSliceLength, resolveLinkTos);
            return events.Select(e => e.AsAbstraction<TKey>()).ToList(); 
        }

    }

}
