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

namespace Neuroglia.Data.EventSourcing
{

    /// <summary>
    /// Defines the fundamentals of a service used to create <see cref="IAggregator"/>s
    /// </summary>
    public interface IAggregatorFactory
    {

        /// <summary>
        /// Creates a new <see cref="IAggregator"/>
        /// </summary>
        /// <typeparam name="TAggregate">The type of aggregates produced by the resulting <see cref="IAggregator"/></typeparam>
        /// <returns>A new <see cref="IAggregator"/></returns>
        IAggregator<TAggregate> CreateAggregator<TAggregate>()
            where TAggregate : class;

        /// <summary>
        /// Creates a new <see cref="IAggregator"/>
        /// </summary>
        /// <param name="aggregateType">The type of aggregates produced by the resulting <see cref="IAggregator"/></param>
        /// <returns>A new <see cref="IAggregator"/></returns>
        IAggregator CreateAggregator(Type aggregateType);

    }

}
