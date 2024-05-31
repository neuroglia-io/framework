// Copyright © 2021-Present Neuroglia SRL. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License"),
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Neuroglia.Data.Expressions.Services;
using Neuroglia.Serialization;
using Neuroglia.Serialization.Json;
using System.Collections;

namespace Neuroglia.Data.Expressions;

/// <summary>
/// Defines extensions for <see cref="IExpressionEvaluator"/>s
/// </summary>
public static partial class IExpressionEvaluatorExtensions
{

    /// <summary>
    /// Evaluates the specified runtime expression
    /// </summary>
    /// <typeparam name="TResult">The expected type of the evaluation's result</typeparam>
    /// <param name="evaluator">The extended <see cref="IExpressionEvaluator"/></param>
    /// <param name="expression">The runtime expression to evaluate</param>
    /// <param name="input">The data to evaluate the runtime expression against</param>
    /// <param name="arguments">A key/value mapping of the arguments used during evaluation, if any</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The evaluation's result</returns>
    public static async Task<TResult?> EvaluateAsync<TResult>(this IExpressionEvaluator evaluator, string expression, object input, IDictionary<string, object>? arguments = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(expression)) throw new ArgumentNullException(nameof(expression));
        return input == null
            ? throw new ArgumentNullException(nameof(input))
            : (TResult?)await evaluator.EvaluateAsync(expression, input, arguments, typeof(TResult), cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Evaluates the specified object by resolving all the runtime expressions it declares
    /// </summary>
    /// <param name="evaluator">The extended <see cref="IExpressionEvaluator"/></param>
    /// <param name="expression">The expression to evaluate, that is an object that declares - at any depth - runtime expressions to resolve against the specified data and arguments</param>
    /// <param name="input">The data to evaluate the runtime expression against</param>
    /// <param name="arguments">A key/value mapping of the arguments used during evaluation, if any</param>
    /// <param name="expectedType">The expected type of the mutated object</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The mutated object</returns>
    public static async Task<object?> EvaluateAsync(this IExpressionEvaluator evaluator, object expression, object input, IDictionary<string, object>? arguments = null, Type? expectedType = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(evaluator);
        if (expression.GetType().IsPrimitiveType())
        {
            if (expression is string expressionString && expressionString.IsRuntimeExpression()) return await evaluator.EvaluateAsync(expressionString, input, arguments, null, cancellationToken).ConfigureAwait(false);
            else return expression;
        }
        var expressionProperties = (IDictionary<string, object>)JsonSerializer.Default.Convert(expression, typeof(IDictionary<string, object>))!;
        var outputProperties = new Dictionary<string, object>();
        foreach (var property in expressionProperties)
        {
            var value = property.Value;
            if (property.Value is string expressionString && expressionString.IsRuntimeExpression()) value = await evaluator.EvaluateAsync(expressionString, input, arguments, null, cancellationToken).ConfigureAwait(false);
            else if (property.Value == null) outputProperties[property.Key] = null!;
            else if (!property.Value.GetType().IsPrimitiveType())
            {
                if (property.Value is IDictionary<string, object> expando)
                {
                    foreach (var kvp in expando.ToList()) expando[kvp.Key] = (await evaluator.EvaluateAsync(kvp.Value, input, arguments, null, cancellationToken).ConfigureAwait(false))!;
                }
                else if (property.Value is IEnumerable inputElements)
                {
                    var outputElements = new List<object>(inputElements.OfType<object>().Count());
                    foreach (var inputElement in inputElements)
                    {
                        var outputElement = await evaluator.EvaluateAsync(inputElement, input, arguments, null, cancellationToken).ConfigureAwait(false);
                        if (outputElement == null) continue;
                        outputElements.Add(outputElement);
                    }
                    value = outputElements;
                }
                else
                {
                    value = await evaluator.EvaluateAsync(property.Value, input, arguments, null, cancellationToken).ConfigureAwait(false);
                }
            }
            outputProperties[property.Key] = value!;
        }
        return expectedType == null || expectedType.IsAssignableFrom(typeof(Dictionary<string, object>)) ? outputProperties : Serialization.Json.JsonSerializer.Default.Convert(outputProperties, expectedType);
    }

    /// <summary>
    /// Evaluates the specified object by resolving all the runtime expressions it declares
    /// </summary>
    /// <typeparam name="TResult">The expected type of the evaluated object</typeparam>
    /// <param name="evaluator">The extended <see cref="IExpressionEvaluator"/></param>
    /// <param name="expression">The expression to evaluate, that is an object that declares - at any depth - runtime expressions to resolve against the specified data and arguments</param>
    /// <param name="input">The data to evaluate the runtime expression against</param>
    /// <param name="arguments">A key/value mapping of the arguments used during evaluation, if any</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The mutated object</returns>
    public static async Task<TResult?> EvaluateAsync<TResult>(this IExpressionEvaluator evaluator, object expression, object input, IDictionary<string, object>? arguments = null, CancellationToken cancellationToken = default) => (TResult?) await evaluator.EvaluateAsync(expression, input, arguments, typeof(TResult), cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Evaluates the specified runtime expression based condition
    /// </summary>
    /// <param name="evaluator">The extended <see cref="IExpressionEvaluator"/></param>
    /// <param name="expression">The runtime expression to evaluate</param>
    /// <param name="input">The data to evaluate the runtime expression against</param>
    /// <param name="arguments">A key/value mapping of the arguments used during evaluation, if any</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The evaluation's result</returns>
    public static async Task<bool> EvaluateConditionAsync(this IExpressionEvaluator evaluator, string expression, object input, IDictionary<string, object>? arguments = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(expression)) throw new ArgumentNullException(nameof(expression));
        ArgumentNullException.ThrowIfNull(input);
        return (bool?)await evaluator.EvaluateAsync(expression, input, arguments, typeof(bool), cancellationToken).ConfigureAwait(false) == true;
    }

}