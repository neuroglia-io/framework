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
using Simple.OData.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Neuroglia.Data.Services
{
    /// <summary>
    /// Represents an <see cref="ExpressionVisitor"/> used to translate an <see cref="Expression"/> into an OData query
    /// </summary>
    /// <typeparam name="T">The type of element to query</typeparam>
    public class ODataExpressionTranslator<T>
        : ExpressionVisitor
        where T : class
    {

        /// <summary>
        /// Initializes a new <see cref="ODataExpressionTranslator{T}"/>
        /// </summary>
        /// <param name="oDataClient">The service used to query ODATA endpoints</param>
        public ODataExpressionTranslator(IODataClient oDataClient)
        {
            ODataClient = oDataClient;
        }

        /// <summary>
        /// Gets the service used to query ODATA endpoints
        /// </summary>
        protected IODataClient ODataClient { get; }

        /// <summary>
        /// Gets a <see cref="List{T}"/> containing the <see cref="Action"/>s used to configure a translated OData query
        /// </summary>
        protected List<Action<IBoundClient<T>>> SetupPipeline { get; } = new();

        /// <summary>
        /// Translates the specified <see cref="Expression"/> into an OData query
        /// </summary>
        /// <param name="expression">The <see cref="Expression"/> to translate</param>
        /// <returns>A new <see cref="IBoundClient{T}"/> that represents the translated OData query</returns>
        public virtual IBoundClient<T> Translate(Expression expression)
        {
            IBoundClient<T> query = ODataClient.For<T>();
            Visit(expression);
            SetupPipeline.Reverse();
            foreach (Action<IBoundClient<T>> setup in SetupPipeline)
            {
                setup(query);
            }
            SetupPipeline.Clear();
            return query;
        }

        /// <inheritdoc/>
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Arguments.Count != 2)
                return base.VisitMethodCall(node);
            Action<IBoundClient<T>> setup;
            switch (node.Method.Name)
            {
                case nameof(Queryable.OrderBy):
                    UnaryExpression unary = (UnaryExpression)node.Arguments[1];
                    LambdaExpression lambda = (LambdaExpression)unary.Operand;
                    setup = query => query.OrderBy((Expression<Func<T, object>>)Expression.Lambda(Expression.Convert(lambda, typeof(object)), lambda.Parameters));
                    break;
                case nameof(Queryable.OrderByDescending):
                    unary = (UnaryExpression)node.Arguments[1];
                    lambda = (LambdaExpression)unary.Operand;
                    setup = query => query.OrderByDescending((Expression<Func<T, object>>)Expression.Lambda(Expression.Convert(lambda, typeof(object)), lambda.Parameters));
                    break;
                case nameof(Queryable.Where):
                    unary = (UnaryExpression)node.Arguments[1];
                    lambda = (LambdaExpression)unary.Operand;
                    setup = query => query.Filter((Expression<Func<T, bool>>)lambda);
                    break;
                case nameof(Queryable.Skip):
                    ConstantExpression constant = (ConstantExpression)node.Arguments[1];
                    setup = query => query.Skip((int)constant.Value);
                    break;
                case nameof(Queryable.Take):
                    constant = (ConstantExpression)node.Arguments[1];
                    setup = query => query.Top((int)constant.Value);
                    break;
                case nameof(string.ToLower):
                case nameof(string.ToLowerInvariant):
                case nameof(string.ToUpper):
                case nameof(string.ToUpperInvariant):
                    return base.VisitMethodCall(node);
                default:
                    throw new NotSupportedException($"The specified method '{node.Method.Name}' is not supported");
            }
            SetupPipeline.Add(setup);
            return base.VisitMethodCall(node);
        }

    }

}
