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
    /// Represents the default implementation of the <see cref="ISubscriptionOptionsBuilder"/> interface
    /// </summary>
    public class SubscriptionOptionsBuilder
        : ISubscriptionOptionsBuilder
    {

        /// <summary>
        /// Initializes a new <see cref="SubscriptionOptionsBuilder"/>
        /// </summary>
        /// <param name="options">The <see cref="SubscriptionOptions"/> to configure</param>
        public SubscriptionOptionsBuilder(SubscriptionOptions options)
        {
            this.Options = options;
        }

        /// <summary>
        /// Initializes a new <see cref="SubscriptionOptionsBuilder"/>
        /// </summary>
        public SubscriptionOptionsBuilder()
            : this(new SubscriptionOptions())
        {

        }

        /// <summary>
        /// Gets the <see cref="SubscriptionOptions"/> to configure
        /// </summary>
        protected SubscriptionOptions Options { get; }

        /// <inheritdoc/>
        public virtual ISubscriptionOptionsBuilder AsDurable(string durableName)
        {
            this.Options.DurableName = durableName;
            return this;
        }

        /// <inheritdoc/>
        public virtual ISubscriptionOptionsBuilder StartFrom(long position)
        {
            this.Options.StreamPosition = EventStreamPosition.Custom;
            this.Options.StartFrom = position;
            return this;
        }

        /// <inheritdoc/>
        public virtual ISubscriptionOptionsBuilder StartFromBegining()
        {
            this.Options.StreamPosition = EventStreamPosition.Start;
            return this;
        }

        /// <inheritdoc/>
        public virtual ISubscriptionOptionsBuilder StartFromCurrent()
        {
            this.Options.StreamPosition = EventStreamPosition.Current;
            return this;
        }

        /// <inheritdoc/>
        public ISubscriptionOptionsBuilder ResolveLinks(bool resolve)
        {
            this.Options.ResolveLinks = resolve;
            return this;
        }

        /// <inheritdoc/>
        public ISubscriptionOptionsBuilder WithMaxSubscribers(int subscribers)
        {
            this.Options.MaxSubscribers = subscribers;
            return this;
        }

        /// <inheritdoc/>
        public ISubscriptionOptionsBuilder WithMinEventsBeforeCheckpoint(int min)
        {
            this.Options.MinEventsBeforeCheckpoint = min;
            return this;
        }

        /// <inheritdoc/>
        public ISubscriptionOptionsBuilder WithMaxEventsBeforeCheckpoint(int max)
        {
            this.Options.MaxEventsBeforeCheckpoint = max;
            return this;
        }

        /// <inheritdoc/>
        public ISubscriptionOptionsBuilder WithAckMode(EventAckMode mode)
        {
            this.Options.AckMode = mode;
            return this;
        }

        /// <inheritdoc/>
        public virtual ISubscriptionOptions Build()
        {
            return this.Options;
        }

    }

}
