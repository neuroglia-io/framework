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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Neuroglia.Data.EventSourcing
{
    /// <summary>
    /// Defines extensions for <see cref="IEventStore"/>s
    /// </summary>
    public static class IEventStoreExtensions
    {

        /// <summary>
        /// Reads a single event in a forward direction
        /// </summary>
        /// <param name="eventStore">The extended <see cref="IEventStore"/></param>
        /// <param name="streamId">The id of the stream to read</param>
        /// <param name="offset">The offset at which to read the <see cref="ISourcedEvent"/> from</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>The <see cref="ISourcedEvent"/> at the specified offset</returns>
        public static async Task<ISourcedEvent> ReadSingleEventForwardAsync(this IEventStore eventStore, string streamId, long offset, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(streamId))
                throw new ArgumentNullException(nameof(streamId));
            IEnumerable<ISourcedEvent> sourcedEvents = await eventStore.ReadEventsForwardAsync(streamId, offset, 1, cancellationToken);
            if (sourcedEvents == null)
                return null;
            return sourcedEvents.FirstOrDefault();
        }

        /// <summary>
        /// Reads a single event in a backward direction
        /// </summary>
        /// <param name="eventStore">The extended <see cref="IEventStore"/></param>
        /// <param name="streamId">The id of the stream to read</param>
        /// <param name="offset">The offset at which to read the <see cref="ISourcedEvent"/> from</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>The <see cref="ISourcedEvent"/> at the specified offset</returns>
        public static async Task<ISourcedEvent> ReadSingleEventBackwardAsync(this IEventStore eventStore, string streamId, long offset, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(streamId))
                throw new ArgumentNullException(nameof(streamId));
            IEnumerable<ISourcedEvent> sourcedEvents = await eventStore.ReadEventsBackwardAsync(streamId, offset, 1, cancellationToken);
            if (sourcedEvents == null)
                return null;
            return sourcedEvents.FirstOrDefault();
        }

    }

}
