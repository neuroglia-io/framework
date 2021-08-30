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
    /// Defines the fundamentals of an object used to describe a managed event
    /// </summary>
    public interface ISourcedEvent
    {

        /// <summary>
        /// Gets the id of the managed event
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Gets the sequence of the managed event
        /// </summary>
        long Sequence { get; }

        /// <summary>
        /// Gets the date and time at which the managed event has been created
        /// </summary>
        DateTimeOffset CreatedAt { get; }

        /// <summary>
        /// Gets the type of the managed event
        /// </summary>
        string Type { get; }

        /// <summary>
        /// Gets the data of the managed event
        /// </summary>
        object Data { get; }

        /// <summary>
        /// Gets the metadata of the managed event
        /// </summary>
        object Metadata { get; }

    }

}
