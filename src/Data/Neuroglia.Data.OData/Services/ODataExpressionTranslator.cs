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
        /// <param name="pluralizer">The service used to pluralize words</param>
        public ODataExpressionTranslator(IODataClient oDataClient, IPluralizer pluralizer)
        {
            this.ODataClient = oDataClient;
            this.Pluralizer = pluralizer;
        }

        /// <summary>
        /// Gets the service used to query ODATA endpoints
        /// </summary>
        protected IODataClient ODataClient { get; }

        /// <summary>
        /// Gets the service used to pluralize words
        /// </summary>
        protected IPluralizer Pluralizer { get; }

        private string _CollectionName;
        /// <summary>
        /// Gets the name of the OData collection the queried entities belong to
        /// </summary>
        protected string CollectionName
        {
            get
            {
                if(string.IsNullOrWhiteSpace(this._CollectionName))
                {
                    if (typeof(T).TryGetCustomAttribute(out ODataEntityAttribute oDataEntityAttribute)
                        && !string.IsNullOrWhiteSpace(oDataEntityAttribute.Collection))
                        this._CollectionName = oDataEntityAttribute.Collection;
                    else
                        this._CollectionName = this.Pluralizer.Pluralize(typeof(T).Name.Replace("Dto", ""));
                }
                return this._CollectionName;
            }
        }

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
            IBoundClient<T> query = this.ODataClient.For<T>(this.CollectionName);
            this.Visit(expression);
            this.SetupPipeline.Reverse();
            foreach (Action<IBoundClient<T>> setup in SetupPipeline)
            {
                setup(query);
            }
            this.SetupPipeline.Clear();
            return query;
        }

        /// <inheritdoc/>
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Arguments.Count != 2)
                return base.VisitMethodCall(node);
            Action<IBoundClient<T>> setup;
            UnaryExpression unary;
            LambdaExpression lambda;
            ConstantExpression constant;
            switch (node.Method.Name)
            {
                case nameof(Queryable.Count):
                    unary = (UnaryExpression)node.Arguments[1];
                    lambda = (LambdaExpression)unary.Operand;
                    setup = query => query.Filter((Expression<Func<T, bool>>)lambda).Count();
                    break;
                case nameof(ODataQueryable.Expand):
                    unary = (UnaryExpression)node.Arguments[1];
                    lambda = (LambdaExpression)unary.Operand;
                    setup = query => query.Expand((Expression<Func<T, object>>)Expression.Lambda(Expression.Convert(lambda, typeof(object)), lambda.Parameters));
                    break;
                case nameof(Queryable.OrderBy):
                    unary = (UnaryExpression)node.Arguments[1];
                    lambda = (LambdaExpression)unary.Operand;
                    setup = query => query.OrderBy((Expression<Func<T, object>>)Expression.Lambda(Expression.Convert(lambda, typeof(object)), lambda.Parameters));
                    break;
                case nameof(Queryable.OrderByDescending):
                    unary = (UnaryExpression)node.Arguments[1];
                    lambda = (LambdaExpression)unary.Operand;
                    setup = query => query.OrderByDescending((Expression<Func<T, object>>)Expression.Lambda(Expression.Convert(lambda, typeof(object)), lambda.Parameters));
                    break;
                case nameof(Queryable.Skip):
                    constant = (ConstantExpression)node.Arguments[1];
                    setup = query => query.Skip((int)constant.Value);
                    break;
                case nameof(Queryable.Take):
                    constant = (ConstantExpression)node.Arguments[1];
                    setup = query => query.Top((int)constant.Value);
                    break;
                case nameof(Queryable.Where):
                    unary = (UnaryExpression)node.Arguments[1];
                    lambda = (LambdaExpression)unary.Operand;
                    setup = query => query.Filter((Expression<Func<T, bool>>)lambda);
                    break;
                case nameof(string.Contains):
                case nameof(string.ToLower):
                case nameof(string.ToLowerInvariant):
                case nameof(string.ToUpper):
                case nameof(string.ToUpperInvariant):
                    return base.VisitMethodCall(node);
                default:
                    throw new NotSupportedException($"The specified method '{node.Method.Name}' is not supported");
            }
            this.SetupPipeline.Add(setup);
            return base.VisitMethodCall(node);
        }

    }

}
