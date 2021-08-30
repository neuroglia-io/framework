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
    /// Defines the fundamentals of a service used to build <see cref="ISubscriptionOptions"/>
    /// </summary>
    public interface ISubscriptionOptionsBuilder
    {

        /// <summary>
        /// Configures the subscription to be durable
        /// </summary>
        /// <param name="durableName">The subscription's durable name</param>
        /// <returns>The configured <see cref="ISubscriptionOptionsBuilder"/></returns>
        ISubscriptionOptionsBuilder AsDurable(string durableName);

        /// <summary>
        /// Starts from the specified position in the event stream
        /// </summary>
        /// <param name="position">The event stream position to start from</param>
        /// <returns>The configured <see cref="ISubscriptionOptionsBuilder"/></returns>
        ISubscriptionOptionsBuilder StartFrom(long position);

        /// <summary>
        /// Starts from the begining of the event stream
        /// </summary>
        /// <returns>The configured <see cref="ISubscriptionOptionsBuilder"/></returns>
        ISubscriptionOptionsBuilder StartFromBegining();

        /// <summary>
        /// Starts from the current position in the event stream
        /// </summary>
        /// <returns>The configured <see cref="ISubscriptionOptionsBuilder"/></returns>
        ISubscriptionOptionsBuilder StartFromCurrent();

        /// <summary>
        /// Configures the subscription to resolve event links
        /// </summary>
        /// <param name="resolve">A boolean indicating whether or not to resolve event links</param>
        /// <returns>The configured <see cref="ISubscriptionOptionsBuilder"/></returns>
        ISubscriptionOptionsBuilder ResolveLinks(bool resolve);

        /// <summary>
        /// Configures the subscription to have a maximum amount of subscribers
        /// </summary>
        /// <param name="subscribers">The maximum amount of subscribers</param>
        /// <returns>The configured <see cref="ISubscriptionOptionsBuilder"/></returns>
        ISubscriptionOptionsBuilder WithMaxSubscribers(int subscribers);

        /// <summary>
        /// Configures the minimum event count before the subscription creates a checkpoint
        /// </summary>
        /// <param name="min">The minimum event count before the subscription creates a checkpoint</param>
        /// <returns>The configured <see cref="ISubscriptionOptionsBuilder"/></returns>
        ISubscriptionOptionsBuilder WithMinEventsBeforeCheckpoint(int min);

        /// <summary>
        /// Configures the maximum event count before the subscription creates a checkpoint
        /// </summary>
        /// <param name="max">The maximum event count before the subscription creates a checkpoint</param>
        /// <returns>The configured <see cref="ISubscriptionOptionsBuilder"/></returns>
        ISubscriptionOptionsBuilder WithMaxEventsBeforeCheckpoint(int max);

        /// <summary>
        /// Configures the subscription to use the specified <see cref="EventAckMode"/>
        /// </summary>
        /// <param name="mode">The <see cref="EventAckMode"/> to use</param>
        /// <returns>The configured <see cref="ISubscriptionOptionsBuilder"/></returns>
        ISubscriptionOptionsBuilder WithAckMode(EventAckMode mode);

        /// <summary>
        /// Builds the <see cref="ISubscriptionOptions"/>
        /// </summary>
        /// <returns>A new <see cref="ISubscriptionOptions"/></returns>
        ISubscriptionOptions Build();

    }

}
