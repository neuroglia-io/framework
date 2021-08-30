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

namespace Neuroglia.Data.EventSourcing
{

    /// <summary>
    /// Defines the fundamentals of an event stream subscription options
    /// </summary>
    public interface ISubscriptionOptions
    {

        /// <summary>
        /// Gets the durable name of the subscription
        /// </summary>
        string DurableName { get; }

        /// <summary>
        /// Gets a boolean indicating whether or not the subscription is durable
        /// </summary>
        bool IsDurable { get; }

        /// <summary>
        /// Gets a boolean indicating whether or not to resolve event links
        /// </summary>
        bool ResolveLinks { get; }

        /// <summary>
        /// Gets the initial event stream position
        /// </summary>
        EventStreamPosition StreamPosition { get; }

        /// <summary>
        /// Gets the event stream position to start from
        /// </summary>
        long? StartFrom { get; }

        /// <summary>
        /// Gets the minimum events before a checkpoint
        /// </summary>
        int MinEventsBeforeCheckpoint { get; }

        /// <summary>
        /// Gets the maximum events before a checkpoint
        /// </summary>
        int MaxEventsBeforeCheckpoint { get; }

        /// <summary>
        /// Gets the maximum amount of subscribers
        /// </summary>
        int MaxSubscribers { get; }

        /// <summary>
        /// Gets the event ack mode
        /// </summary>
        EventAckMode AckMode { get; }

    }

}
