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

namespace Neuroglia.Data.EventSourcing
{
    /// <summary>
    /// Defines extensions for <see cref="ISubscriptionOptions"/>
    /// </summary>
    public static class ISubscriptionOptionsExtensions
    {

        /// <summary>
        /// Transforms the <see cref="ISubscriptionOptions"/> into a new <see cref="PersistentSubscriptionSettings"/>
        /// </summary>
        /// <param name="subscriptionOptions">The <see cref="ISubscriptionOptions"/> to transform</param>
        /// <returns>A new <see cref="PersistentSubscriptionSettings"/></returns>
        public static PersistentSubscriptionSettings ToPersistentSubscriptionSettings(this ISubscriptionOptions subscriptionOptions)
        {
            StreamPosition? startFrom = null;
            //todo: 
            //switch (subscriptionOptions.StreamPosition)
            //{
            //    case EventStreamPosition.Custom:
            //        if (subscriptionOptions.StartFrom.HasValue)
            //            startFrom = StreamPosition.FromInt64(subscriptionOptions.StartFrom.Value);
            //        break;
            //    case EventStreamPosition.Start:
            //        startFrom = StreamPosition.Start;
            //        break;
            //    case EventStreamPosition.End:
            //        startFrom = StreamPosition.End;
            //        break;
            //}
            PersistentSubscriptionSettings settings = new(subscriptionOptions.ResolveLinks, startFrom, maxSubscriberCount: subscriptionOptions.MaxSubscribers, minCheckPointCount: subscriptionOptions.MinEventsBeforeCheckpoint, maxCheckPointCount: subscriptionOptions.MaxEventsBeforeCheckpoint);
            return settings;
        }

    }

}
