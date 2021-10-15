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
using Neuroglia;
using Neuroglia.Data;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace System.Linq
{
    /// <summary>
    /// Defines extensions for <see cref="IQueryable"/>s
    /// </summary>
    public static class IQueryableExtensions
    {

        private static readonly MethodInfo CountAsyncMethod = typeof(AsyncEnumerable).GetMethods().First(m => m.Name == nameof(AsyncEnumerable.CountAsync) && m.GetParameters().Length == 2);

        /// <summary>
        /// Counts asynchronously the elements the <see cref="IQueryable"/> is made out of
        /// </summary>
        /// <param name="queryable">The <see cref="IQueryable"/> to count the elements of</param>
        /// <param name="predicate">The <see cref="LambdaExpression"/> of the predicate used to filter the elements to count</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>The count of elements in the <see cref="IQueryable"/></returns>
        public static async Task<int> CountAsync(this IQueryable queryable, LambdaExpression predicate, CancellationToken cancellationToken = default)
        {
            if (queryable == null)
                throw new ArgumentNullException(nameof(queryable));
            MethodInfo method;
            if (queryable is IODataQueryable oDataQueryable)
                return await oDataQueryable.CountAsync(predicate, cancellationToken);
            method = CountAsyncMethod.MakeGenericMethod(queryable.ElementType);
            return (int)await method.InvokeAsync(null, new object[] { queryable, cancellationToken });
        }

    }

}
