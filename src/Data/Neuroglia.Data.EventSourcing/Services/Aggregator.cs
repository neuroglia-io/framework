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
using System.Linq;
using System.Reflection;

namespace Neuroglia.Data.EventSourcing
{

    /// <summary>
    /// Represents the default implementation of the <see cref="IAggregator{TAggregate}"/> interface
    /// </summary>
    /// <typeparam name="TAggregate">The type of aggregates produced by the <see cref="Aggregator{TAggregate}"/></typeparam>
    public class Aggregator<TAggregate>
        : IAggregator<TAggregate>
        where TAggregate : class
    {

        /// <summary>
        /// Gets the default name of aggregation methods
        /// </summary>
        public const string DefaultAggregationMethodName = "On";

        /// <summary>
        /// Initializes a new <see cref="Aggregator{TAggregate}"/>
        /// </summary>
        /// <param name="aggregationMethodName">The name of aggregation methods</param>
        public Aggregator(string aggregationMethodName)
        {
            if (string.IsNullOrWhiteSpace(aggregationMethodName))
                throw new ArgumentNullException(nameof(aggregationMethodName));
            IEnumerable<MethodInfo> aggregationMethods = typeof(TAggregate).GetMethods(BindingFlags.Default | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                  .Where(m => m.Name == aggregationMethodName && m.GetParameters().Length == 1);
            Dictionary<Type, IEventAggregator> aggregations = new();
            aggregationMethods.ToList().ForEach(m =>
            {
                Type eventType = m.GetParameters().First().ParameterType;
                Type eventAggregatorType = typeof(EventAggregator<,>).MakeGenericType(typeof(TAggregate), eventType);
                IEventAggregator eventAggregator = (IEventAggregator)Activator.CreateInstance(eventAggregatorType, m);
                aggregations.Add(eventType, eventAggregator);
            });
            this.Aggregators = aggregations;
        }

        /// <summary>
        /// Initializes a new <see cref="Aggregator{TAggregate}"/> using <see cref="DefaultAggregationMethodName"/>
        /// </summary>
        public Aggregator()
            : this(DefaultAggregationMethodName)
        {

        }

        /// <summary>
        /// Gets an <see cref="IReadOnlyDictionary{TKey, TValue}"/> containing all available <see cref="IEventAggregator"/>s
        /// </summary>
        protected IReadOnlyDictionary<Type, IEventAggregator> Aggregators { get; }

        /// <inheritdoc/>
        public virtual TAggregate Aggregate(IEnumerable<IDomainEvent> events)
        {
            return this.Aggregate(null, events);
        }

        object IAggregator.Aggregate(IEnumerable<IDomainEvent> events)
        {
            return this.Aggregate(events);
        }

        /// <inheritdoc/>
        public virtual TAggregate Aggregate(TAggregate aggregate, IEnumerable<IDomainEvent> events)
        {
            if (aggregate == null)
                aggregate = this.CreateAggregateInstance();
            foreach (ISourcedEvent e in events)
            {
                if (!this.Aggregators.TryGetValue(e.GetType(), out IEventAggregator aggregator))
                    continue;
                aggregator.Aggregate(aggregate, e);
            }
            return aggregate;
        }

        object IAggregator.Aggregate(object aggregate, IEnumerable<IDomainEvent> events)
        {
            return this.Aggregate((TAggregate)aggregate, events);
        }

        /// <summary>
        /// Creates a new aggregate instance
        /// </summary>
        /// <returns>A new aggregate instance</returns>
        protected virtual TAggregate CreateAggregateInstance()
        {
            return (TAggregate)Activator.CreateInstance(typeof(TAggregate), true);
        }

    }

}
