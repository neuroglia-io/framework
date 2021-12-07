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
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Neuroglia.Data.Expressions
{

    /// <summary>
    /// Defines extensions for <see cref="IServiceCollection"/>s
    /// </summary>
    public static class IServiceCollectionExtensions
    {

        /// <summary>
        /// Adds and configures an <see cref="IExpressionEvaluator"/> of the specified type
        /// </summary>
        /// <typeparam name="TEvaluator">The type of <see cref="IExpressionEvaluator"/> to register</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
        /// <param name="lifetime">The <see cref="ServiceLifetime"/>. Defaults to <see cref="ServiceLifetime.Transient"/></param>
        /// <returns>The configured <see cref="IServiceCollection"/></returns>
        public static IServiceCollection AddExpressionEvaluator<TEvaluator>(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Transient)
            where TEvaluator : class, IExpressionEvaluator
        {
            services.TryAddSingleton<IExpressionEvaluatorProvider, ExpressionEvaluatorProvider>();
            services.TryAdd(new ServiceDescriptor(typeof(TEvaluator), typeof(TEvaluator), lifetime));
            services.Add(new ServiceDescriptor(typeof(IExpressionEvaluator), provider => provider.GetRequiredService<TEvaluator>(), lifetime));
            return services;
        }

    }

    public static class IExpressionEvaluatorProviderExtensions
    {

        public static object? Evaluate(this IExpressionEvaluator evaluator, object obj, object data, Type expectedType)
        {
            var inputProperties = obj.ToDictionary();
            var outputProperties = new Dictionary<string, object>();
            foreach (var property in inputProperties)
            {
                var value = property.Value;
                if (property.Value is string expression
                    && expression.IsRuntimeExpression())
                    value = evaluator.Evaluate(expression, data);
                else if (!property.Value.GetType().IsValueType())
                    value = evaluator.Evaluate(property.Value, data);
                outputProperties.Add(property.Key, value!);
            }
            return outputProperties.ToExpandoObject();
        }

        public static T? Evaluate<T>(this IExpressionEvaluator evaluator, object obj, object data)
        {
            return (T?)evaluator.Evaluate(obj, data, typeof(T));
        }

        public static object? Evaluate(this IExpressionEvaluator evaluator, object obj, object data)
        {
            return evaluator.Evaluate(obj, data, typeof(object));
        }

    }

}
