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
using Simple.OData.Client;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Neuroglia.Data.Services
{
    /// <summary>
    /// Represents an OData <see cref="IQueryProvider"/>
    /// </summary>
    /// <typeparam name="T">The type of element to query</typeparam>
    public class ODataQueryProvider<T>
        : IAsyncQueryProvider
        where T : class
    {

        /// <summary>
        /// Initializes a new <see cref="ODataQueryProvider{T}"/>
        /// </summary>
        /// <param name="serviceProvider">The current <see cref="IServiceProvider"/></param>
        public ODataQueryProvider(IServiceProvider serviceProvider)
        {
            ExpressionTranslator = ActivatorUtilities.CreateInstance<ODataExpressionTranslator<T>>(serviceProvider);
        }

        /// <summary>
        /// Gets the service used to translate <see cref="Expression"/>s to OData queries
        /// </summary>
        protected ODataExpressionTranslator<T> ExpressionTranslator { get; }

        /// <inheritdoc/>
        public IQueryable CreateQuery(Expression expression)
        {
            Type elementType = expression.Type.GetEnumerableElementType();
            Type listType = typeof(ODataList<>).MakeGenericType(elementType);
            try
            {
                return (IQueryable)Activator.CreateInstance(listType, new object[] { this, expression });
            }
            catch (TargetInvocationException e)
            {
                throw e.InnerException;
            }
        }

        /// <inheritdoc/>
        public virtual IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return (IQueryable<TElement>)new ODataList<T>(this, expression);
        }

        /// <inheritdoc/>
        public virtual object Execute(Expression expression)
        {
            IBoundClient<T> boundClient = ExpressionTranslator.Translate(expression);
            Task<object> task;
            if (expression.Type.IsEnumerable())
                task = Task.Run(async () => (object)await boundClient.FindEntriesAsync().ConfigureAwait(false));
            else
                task = Task.Run(async () => (object)await boundClient.FindEntryAsync().ConfigureAwait(false));
            task.Wait();
            return task.Result;
        }

        /// <inheritdoc/>
        public virtual async Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
        {
            IBoundClient<T> boundClient = ExpressionTranslator.Translate(expression);
            if (expression.Type.IsEnumerable())
                return (TResult)await boundClient.FindEntriesAsync(cancellationToken).ConfigureAwait(false);
            else
                return (TResult)(object)await boundClient.FindEntryAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        TResult IQueryProvider.Execute<TResult>(Expression expression)
        {
            return (TResult)Execute(expression);
        }

    }

}
