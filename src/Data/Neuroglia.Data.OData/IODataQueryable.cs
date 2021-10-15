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
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Neuroglia.Data
{

    /// <summary>
    /// Defines the fundamentals of an OData <see cref="IAsyncQueryable{T}"/>
    /// </summary>
    public interface IODataQueryable
        : IOrderedQueryable
    {

        /// <summary>
        /// Counts asynchronously the elements the <see cref="IODataQueryable"/> is made out of
        /// </summary>
        /// <param name="predicate">The <see cref="LambdaExpression"/> of the predicate used to filter the elements to count</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>The count of elements the <see cref="IODataQueryable"/> is made out of</returns>
        Task<int> CountAsync(LambdaExpression predicate, CancellationToken cancellationToken = default);

    }

    /// <summary>
    /// Defines the fundamentals of an OData <see cref="IAsyncQueryable{T}"/>
    /// </summary>
    /// <typeparam name="T">The type of element to query</typeparam>
    public interface IODataQueryable<T>
        : IODataQueryable, IAsyncQueryable<T>
    {

    }

}
