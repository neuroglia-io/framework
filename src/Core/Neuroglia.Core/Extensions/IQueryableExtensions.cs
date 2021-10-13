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
using System.Linq.Expressions;
using System.Reflection;

namespace System.Linq
{

    /// <summary>
    /// Defines extensions for <see cref="IQueryable"/>s
    /// </summary>
    public static class IQueryableExtensions
    {

        private static readonly MethodInfo OrderByMethod = typeof(Queryable).GetMethods().Single(m => m.Name == nameof(Queryable.OrderBy) && m.GetParameters().Length == 2);
        private static readonly MethodInfo OrderByDescendingMethod = typeof(Queryable).GetMethods().Single(m => m.Name == nameof(Queryable.OrderByDescending) && m.GetParameters().Length == 2);
        private static readonly MethodInfo WhereMethod = typeof(Queryable).GetMethods().First(m => m.Name == nameof(Queryable.Where) && m.GetParameters().Length == 2);

        /// <summary>
        /// Orders the <see cref="IQueryable"/> by the specified <see cref="PropertyPath"/>
        /// </summary>
        /// <param name="queryable">The <see cref="IQueryable"/> to order</param>
        /// <param name="propertyPath">The <see cref="PropertyPath"/> to order the <see cref="IQueryable"/> by</param>
        /// <returns>A new <see cref="IQueryable"/></returns>
        public static IQueryable OrderBy(this IQueryable queryable, PropertyPath propertyPath)
        {
            ParameterExpression parameter = Expression.Parameter(queryable.ElementType);
            Expression body = propertyPath.ToExpression(parameter);
            LambdaExpression lambda = Expression.Lambda(typeof(Func<,>).MakeGenericType(queryable.ElementType, body.Type), body, parameter);
            MethodInfo method = OrderByMethod.MakeGenericMethod(queryable.ElementType, body.Type);
            return (IQueryable)method.Invoke(null, new object[] { queryable, lambda });
        }

        /// <summary>
        /// Orders the <see cref="IQueryable"/> by the specified <see cref="PropertyPath"/>, in a descending fashion
        /// </summary>
        /// <param name="queryable">The <see cref="IQueryable"/> to order</param>
        /// <param name="propertyPath">The <see cref="PropertyPath"/> to order the <see cref="IQueryable"/> by</param>
        /// <returns>A new <see cref="IQueryable"/></returns>
        public static IQueryable OrderByDescending(this IQueryable queryable, PropertyPath propertyPath)
        {
            ParameterExpression parameter = Expression.Parameter(queryable.ElementType);
            Expression body = propertyPath.ToExpression(parameter);
            LambdaExpression lambda = Expression.Lambda(typeof(Func<,>).MakeGenericType(queryable.ElementType, body.Type), body, parameter);
            MethodInfo method = OrderByDescendingMethod.MakeGenericMethod(queryable.ElementType, body.Type);
            return (IQueryable)method.Invoke(null, new object[] { queryable, lambda });
        }

    }

}
