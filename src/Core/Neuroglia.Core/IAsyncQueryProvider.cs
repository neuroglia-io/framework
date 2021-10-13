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
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace System.Linq
{

    /// <summary>
    /// Defines the fundamentals of an async <see cref="IQueryProvider"/>
    /// </summary>
    public interface IAsyncQueryProvider
        : IQueryProvider
    {

        /// <summary>
        /// Executes the query represented by a specified expression tree
        /// </summary>
        /// <typeparam name="TResult">The type of the value that results from executing the query</typeparam>
        /// <param name="expression">An <see cref="Expression"/> tree that represents a LINQ query</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>The value that results from executing the specified query</returns>
        Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default);

    }

}
