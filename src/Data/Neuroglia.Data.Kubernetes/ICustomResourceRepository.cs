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
using Neuroglia.K8s;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Neuroglia.Data
{
    /// <summary>
    /// Defines the fundamentals of an <see cref="IRepository{TEntity, TKey}"/> used to manage <see cref="ICustomResource"/>s
    /// </summary>
    /// <typeparam name="TResource">The type of <see cref="ICustomResource"/> to manage</typeparam>
    public interface ICustomResourceRepository<TResource>
        : IRepository<TResource, string>
        where TResource : class, ICustomResource, IIdentifiable<string>, new()
    {

        /// <summary>
        /// Filters cluster <see cref="ICustomResource"/>s
        /// </summary>
        /// <param name="labelSelector">The <see cref="ICustomResource"/>'s label selector</param>
        /// <param name="fieldSelector">The <see cref="ICustomResource"/>'s field selector</param>
        /// <param name="namespace">The <see cref="ICustomResource"/>'s namespace</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A new <see cref="IEnumerable{T}"/> containing all filtered <see cref="ICustomResource"/>s</returns>
        Task<IEnumerable<TResource>> FilterAsync(string labelSelector = null, string fieldSelector = null, string @namespace = null, CancellationToken cancellationToken = default);

    }

}
