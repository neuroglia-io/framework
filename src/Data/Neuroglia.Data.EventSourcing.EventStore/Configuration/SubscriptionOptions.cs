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

namespace Neuroglia.Data.EventSourcing.EventStore.Configuration
{
    /// <summary>
    /// Represents the default implementation of the <see cref="ISubscriptionOptions"/> interface
    /// </summary>
    public class SubscriptionOptions
        : ISubscriptionOptions
    {

        /// <summary>
        /// Initializes a new <see cref="SubscriptionOptions"/>
        /// </summary>
        public SubscriptionOptions()
        {
            this.MaxEventsBeforeCheckpoint = 10;
        }


        /// <inheritdoc/>
        public string DurableName { get; set; }

        /// <inheritdoc/>
        public bool IsDurable
        {
            get
            {
                return !string.IsNullOrWhiteSpace(this.DurableName);
            }
        }

        /// <inheritdoc/>
        public bool ResolveLinks { get; set; }

        /// <inheritdoc/>
        public EventStreamPosition StreamPosition { get; set; }

        /// <inheritdoc/>
        public long? StartFrom { get; set; }

        /// <inheritdoc/>
        public int MinEventsBeforeCheckpoint { get; set; }

        /// <inheritdoc/>
        public int MaxEventsBeforeCheckpoint { get; set; }

        /// <inheritdoc/>
        public int MaxSubscribers { get; set; }

        /// <inheritdoc/>
        public EventAckMode AckMode { get; set; }

    }

}
