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

namespace Neuroglia.Data.EventSourcing
{

    /// <summary>
    /// Defines the fundamentals of an object used to describe a stream of <see cref="IDomainEvent"/>s
    /// </summary>
    public interface IEventStream
        : IIdentifiable, IAsyncEnumerable<ISourcedEvent>
    {

        /// <summary>
        /// Gets the stream's length, or events count
        /// </summary>
        long Length { get; }

        /// <summary>
        /// Gets the current position in the stream
        /// </summary>
        long Position { get; }

        /// <summary>
        /// Gets the date and time at which the first event has been created
        /// </summary>
        DateTimeOffset FirstEventAt { get; }

        /// <summary>
        /// Gets the date and time at which the last event has been created
        /// </summary>
        DateTimeOffset LastEventAt { get; }

        /// <summary>
        /// Gets the <see cref="ISourcedEvent"/> at the current position
        /// </summary>
        ISourcedEvent Current { get; }

    }

}
