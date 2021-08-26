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

namespace Neuroglia.Data.EventSourcing.EventStore.Subscriptions
{

    /// <summary>
    /// Represents a catch-up <see cref="EventStoreSubscription"/>
    /// </summary>
    public class CatchUpSubscription
        : EventStoreSubscription
    {

        /// <summary>
        /// Initializes a new <see cref="CatchUpSubscription"/>
        /// </summary>
        /// <param name="id">The <see cref="CatchUpSubscription"/>'s id</param>
        /// <param name="source">The underlying <see cref="EventStoreCatchUpSubscription"/></param>
        public CatchUpSubscription(string id, EventStoreCatchUpSubscription source)
            : base(id, source)
        {

        }

        /// <summary>
        /// Gets the underlying <see cref="EventStoreCatchUpSubscription"/>
        /// </summary>
        protected new EventStoreCatchUpSubscription Source
        {
            get
            {
                return (EventStoreCatchUpSubscription)base.Source;
            }
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Source?.Stop();
                base.Source = null;
            }
            base.Dispose(disposing);
        }

    }

}
