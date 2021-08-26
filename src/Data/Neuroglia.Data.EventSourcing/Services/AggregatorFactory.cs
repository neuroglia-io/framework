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
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace Neuroglia.Data.EventSourcing
{

    /// <summary>
    /// Represents the default implementation of the <see cref="IAggregatorFactory"/> interface
    /// </summary>
    public class AggregatorFactory
        : IAggregatorFactory
    {

        private static readonly MethodInfo GenericFactoryMethod = typeof(AggregatorFactory).GetMethods().Single(m => m.Name == nameof(CreateAggregator) && m.IsGenericMethod);

        /// <summary>
        /// Initializes a new <see cref="AggregatorFactory"/>
        /// </summary>
        public AggregatorFactory()
        {
            this.Aggregators = new ConcurrentDictionary<Type, IAggregator>();
        }

        /// <summary>
        /// Gets a <see cref="ConcurrentDictionary{TKey, TValue}"/> containing all available <see cref="IAggregator"/>s
        /// </summary>
        protected ConcurrentDictionary<Type, IAggregator> Aggregators { get; }

        /// <inheritdoc/>
        public IAggregator<TAggregate> CreateAggregator<TAggregate>()
            where TAggregate : class
        {
            if (!this.Aggregators.TryGetValue(typeof(TAggregate), out IAggregator aggregator))
            {
                aggregator = new Aggregator<TAggregate>();
                this.Aggregators.TryAdd(typeof(TAggregate), aggregator);
            }
            return aggregator as IAggregator<TAggregate>;
        }

        /// <inheritdoc/>
        public IAggregator CreateAggregator(Type aggregateType)
        {
            return (IAggregator)GenericFactoryMethod.MakeGenericMethod(aggregateType).Invoke(this, Array.Empty<object>());
        }

    }

}
