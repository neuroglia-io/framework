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
using Neuroglia.Data;
using System.Linq.Expressions;
using System.Reflection;

namespace System.Linq
{

    /// <summary>
    /// Defines extensions for <see cref="IODataQueryable{T}"/> implementations
    /// </summary>
    public static class ODataQueryable
    {

        /// <summary>
        /// Specifies the related objects to expand in the query results
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source</typeparam>
        /// <typeparam name="TProperty"> The type of the property returned by the propertySelector</typeparam>
        /// <param name="source">The sequence for which to expand the specified property</param>
        /// <param name="propertySelector">A function to extract a property from an element</param>
        /// <returns>The configured <see cref="IODataQueryable{T}"/></returns>
        public static IODataQueryable<TSource> Expand<TSource, TProperty>(this IODataQueryable<TSource> source, Expression<Func<TSource, TProperty>> propertySelector)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (propertySelector == null)
                throw new ArgumentNullException(nameof(propertySelector));
            Expression expression = Expression.Call(null, GetMethodInfo(Expand, source, propertySelector), new Expression[] { source.Expression, Expression.Quote(propertySelector) });
            return (IODataQueryable<TSource>)source.Provider.CreateQuery<TSource>(expression);
        }

        private static MethodInfo GetMethodInfo<T1, T2, T3>(Func<T1, T2, T3> f, T1 unused1, T2 unused2)
        {
            return f.Method;
        }

    }

}
