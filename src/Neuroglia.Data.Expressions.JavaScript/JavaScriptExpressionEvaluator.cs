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

using Jint;
using Jint.Runtime.Interop;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Neuroglia.Data.Expressions.JavaScript.Configuration;
using Neuroglia.Data.Expressions.Services;
using Neuroglia.Serialization;
using System.Collections;

namespace Neuroglia.Data.Expressions.JavaScript;

/// <summary>
/// Represents the default, JQ implementation of the <see cref="IExpressionEvaluator"/> interface
/// </summary>
public class JavaScriptExpressionEvaluator
    : IExpressionEvaluator
{

    /// <summary>
    /// Initializes a new <see cref="JavaScriptExpressionEvaluator"/>
    /// </summary>
    /// <param name="logger">The service used to perform logging</param>
    /// <param name="options">The service used to access the current <see cref="JavaScriptExpressionEvaluatorOptions"/></param>
    /// <param name="serializerProvider">The service used to provide <see cref="ISerializer"/>s</param>
    public JavaScriptExpressionEvaluator(ILogger<JavaScriptExpressionEvaluator> logger, IOptions<JavaScriptExpressionEvaluatorOptions> options, ISerializerProvider serializerProvider)
    {
        this.Logger = logger;
        this.Options = options.Value;
        this.Serializer = serializerProvider.GetSerializers().OfType<IJsonSerializer>().First(s => this.Options.SerializerType == null || s.GetType() == this.Options.SerializerType);
    }

    /// <summary>
    /// Gets the service used to perform logging
    /// </summary>
    protected ILogger Logger { get; }

    /// <summary>
    /// Gets the service used to access the current <see cref="JavaScriptExpressionEvaluatorOptions"/>
    /// </summary>
    protected JavaScriptExpressionEvaluatorOptions Options { get; }

    /// <summary>
    /// Gets the service used to serialize and deserialize json
    /// </summary>
    protected IJsonSerializer Serializer { get; }

    /// <inheritdoc/>
    public virtual bool Supports(string language) => string.IsNullOrWhiteSpace(language) ? throw new ArgumentNullException(nameof(language)) : language.Equals("javascript", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Determines if the specified object is an array-like CLR collection.
    /// </summary>
    /// Notes: see https://github.com/sebastienros/jint/issues/1030#issuecomment-1919747531
    /// Code from https://github.com/elsa-workflows/elsa-core/blob/a1c30facbae44590ebd8d5662e052109950d535c/src/modules/Elsa.JavaScript/Helpers/ObjectArrayHelper.cs
    public static bool DetermineIfObjectIsArrayLikeClrCollection(Type type)
    {
        var isDictionary = typeof(IDictionary).IsAssignableFrom(type);

        if (isDictionary)
            return false;

        if (typeof(ICollection).IsAssignableFrom(type))
            return true;

        foreach (var interfaceType in type.GetInterfaces())
        {
            if (!interfaceType.IsGenericType)
            {
                continue;
            }

            if (interfaceType.GetGenericTypeDefinition() == typeof(IReadOnlyCollection<>)
                || interfaceType.GetGenericTypeDefinition() == typeof(ICollection<>))
            {
                return true;
            }
        }

        return false;
    }

    /// <inheritdoc/>
    public virtual Task<object?> EvaluateAsync(string expression, object data, IDictionary<string, object>? args = null, Type? expectedType = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(expression)) throw new ArgumentNullException(nameof(expression));
        if (data == null) throw new ArgumentNullException(nameof(data));
        if (expectedType == null) expectedType = typeof(object);
        return Task.Run(() =>
        {
            expression = expression.Trim();
            if (expression.StartsWith("${")) expression = expression[2..^1].Trim();
            if (string.IsNullOrWhiteSpace(expression)) throw new ArgumentNullException(nameof(expression));

            var jsEngine = new Engine(options =>
                {
                    // Limit memory allocations to 1MB
                    options.LimitMemory(1_000_000)
                        // Set a timeout to 500ms
                        .TimeoutInterval(TimeSpan.FromMilliseconds(500))
                        // Set limit of 500 executed statements
                        .MaxStatements(500)
                        // Set limit of 16 for recursive calls
                        .LimitRecursion(16)
                        // Use a cancellation token.
                        //.CancellationToken(cancellationToken)
                        // customizing object wrapping to set array prototype to objects
                        .SetWrapObjectHandler((engine, target, type) =>
                        {
                            var instance = new ObjectWrapper(engine, target);
                            if (DetermineIfObjectIsArrayLikeClrCollection(target.GetType()))
                            {
                                instance.Prototype = engine.Intrinsics.Array.PrototypeObject;
                            }
                            return instance;
                        })
                    ;
                }); ;
            jsEngine.SetValue("input", data);
            if (args != null) foreach (var arg in args) jsEngine.SetValue(arg.Key, arg.Value);
            var result = jsEngine.Evaluate(expression).UnwrapIfPromise().ToObject();
            if (expectedType == typeof(object)) return result;
            return this.Serializer.Deserialize(this.Serializer.SerializeToText(result), expectedType);
        }, cancellationToken);
    }

}
