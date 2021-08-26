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
using System.Reflection;

namespace Neuroglia.Data.EventSourcing
{
    /// <summary>
    /// Represents the default implementation of the <see cref="IEventAggregator"/> interface
    /// </summary>
    /// <typeparam name="TAggregate">The type of aggregate to handle</typeparam>
    /// <typeparam name="TEvent">The type of <see cref="IDomainEvent"/>s to aggregate</typeparam>
    public class EventAggregator<TAggregate, TEvent>
       : IEventAggregator
       where TAggregate : class
       where TEvent : class, IDomainEvent
    {

        /// <summary>
        /// Initializes a new <see cref="EventAggregator{TAggregate, TEvent}"/>
        /// </summary>
        /// <param name="aggregationMethod">The aggregation <see cref="MethodInfo"/></param>
        public EventAggregator(MethodInfo aggregationMethod)
        {
            this.AggregationMethod = aggregationMethod;
        }

        /// <summary>
        /// Gets the aggregation <see cref="MethodInfo"/>
        /// </summary>
        protected MethodInfo AggregationMethod { get; }

        /// <inheritdoc/>
        public void Aggregate(TAggregate aggregate, TEvent e)
        {
            this.AggregationMethod.Invoke(aggregate, new object[] { e });
        }

        /// <inheritdoc/>
        void IEventAggregator.Aggregate(object aggregate, ISourcedEvent e)
        {
            this.Aggregate((TAggregate)aggregate, (TEvent)e);
        }

    }

}
