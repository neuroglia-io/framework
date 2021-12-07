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

namespace Neuroglia.Data.Expressions
{

    /// <summary>
    /// Defines extensions for <see cref="IExpressionEvaluator"/>s
    /// </summary>
    public static class IExpressionEvaluatorProviderExtensions
    {

        /// <summary>
        /// Evaluates an object against the specified data
        /// </summary>
        /// <param name="evaluator">The <see cref="IExpressionEvaluator"/> to use</param>
        /// <param name="obj">The object to evaluate</param>
        /// <param name="data">The data to evaluate the specified object against</param>
        /// <returns>The result of the input object's evaluation</returns>
        public static object? Evaluate(this IExpressionEvaluator evaluator, object obj, object data)
        {
            if(evaluator == null)
                throw new ArgumentNullException(nameof(evaluator));
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

    }

}
