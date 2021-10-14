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
using Microsoft.Extensions.DependencyInjection;
using Neuroglia.Data.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;

namespace Neuroglia.Data
{

    /// <summary>
    /// Represents an <see cref="IQueryable{T}"/> used to wrap an OData set
    /// </summary>
    /// <typeparam name="T">The type of the elements the <see cref="ODataQueryable{T}"/> is made out of</typeparam>
    public class ODataQueryable<T>
        : IODataQueryable<T>
        where T : class
    {

        /// <summary>
        /// Initializes a new <see cref="ODataQueryable{T}"/>
        /// </summary>
        /// <param name="provider">The <see cref="IQueryProvider"/> associated with the data source</param>
        /// <param name="expression">The <see cref="System.Linq.Expressions.Expression"/> associated with this <see cref="ODataQueryable{T}"/> instance</param>
        public ODataQueryable(IAsyncQueryProvider provider, Expression expression)
        {
            this.Provider = provider;
            this.Expression = expression ?? Expression.Constant(this);
        }

        /// <summary>
        /// Initializes a new <see cref="ODataQueryable{T}"/>
        /// </summary>
        /// <param name="serviceProvider">The current <see cref="IServiceProvider"/></param>
        public ODataQueryable(IServiceProvider serviceProvider)
            : this(ActivatorUtilities.CreateInstance<ODataQueryProvider<T>>(serviceProvider), null)
        {

        }

        /// <inheritdoc/>
        public Type ElementType => typeof(T);

        /// <inheritdoc/>
        public Expression Expression { get; }

        /// <inheritdoc/>
        public IAsyncQueryProvider Provider { get; }

        IQueryProvider IQueryable.Provider => this.Provider;

        /// <inheritdoc/>
        public virtual IEnumerator<T> GetEnumerator()
        {
            return this.Provider.Execute<IEnumerable<T>>(this.Expression).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <inheritdoc/>
        public async IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            foreach (T elem in await this.Provider.ExecuteAsync<IEnumerable<T>>(this.Expression, cancellationToken))
            {
                yield return elem;
            }
        }

    }

}
