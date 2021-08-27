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
using EventStore.Client;

namespace Neuroglia.Data.EventSourcing.EventStore.Subscriptions
{

    /// <summary>
    /// Represents a durable <see cref="EventStoreSubscription"/>
    /// </summary>
    public class DurableSubscription
        : EventStoreSubscription
    {

        /// <summary>
        /// Initializes a new <see cref="DurableSubscription"/>
        /// </summary>
        /// <param name="id">The <see cref="DurableSubscription"/>'s id</param>
        /// <param name="source">The underlying <see cref="PersistentSubscription"/></param>
        public DurableSubscription(string id, PersistentSubscription source)
            : base(id, source)
        {

        }

        /// <summary>
        /// Gets the id of the stream to subscribe to
        /// </summary>
        public string StreamId { get; }

        /// <summary>
        /// Gets the <see cref="DurableSubscription"/>'s name
        /// </summary>
        public string DurableName { get; }

        /// <summary>
        /// Gets the underlying <see cref="PersistentSubscription"/>
        /// </summary>
        protected new PersistentSubscription Source
        {
            get
            {
                return (PersistentSubscription)base.Source;
            }
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Source?.Dispose();
                base.Source = null;
            }
            base.Dispose(disposing);
        }

    }

}
