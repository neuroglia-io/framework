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

using Jint;
using Jint.Runtime.Interop;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Neuroglia.Data.Expressions.JavaScript.Configuration;
using Neuroglia.Serialization;
using Newtonsoft.Json.Linq;

namespace Neuroglia.Data.Expressions.JavaScript
{

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
            this.JsonSerializer = (IJsonSerializer)serializerProvider.GetSerializer(options?.Value.SerializerType);
            if (this.JsonSerializer == null)
                throw new NullReferenceException($"Failed to find an {nameof(IJsonSerializer)} implementation of type '{options?.Value.SerializerType}'");
        }

        /// <summary>
        /// Gets the service used to perform logging
        /// </summary>
        protected ILogger Logger { get; }

        /// <summary>
        /// Gets the service used to serialize and deserialize json
        /// </summary>
        protected IJsonSerializer JsonSerializer { get; }

        /// <inheritdoc/>
        public virtual bool Supports(string language)
        {
            if (string.IsNullOrWhiteSpace(language))
                throw new ArgumentNullException(nameof(language));
            return language.Equals("javascript", StringComparison.OrdinalIgnoreCase);
        }

        /// <inheritdoc/>
        public virtual object? Evaluate(string expression, object data, Type expectedType, IDictionary<string, object>? args = null)
        {
            if (string.IsNullOrWhiteSpace(expression)) throw new ArgumentNullException(nameof(expression));
            if (data == null) throw new ArgumentNullException(nameof(data));
            if (expectedType == null) expectedType = typeof(object);
            expression = expression.Trim();
            if (expression.StartsWith("${"))
                expression = expression[2..^1].Trim();
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
                        .SetWrapObjectHandler((engine, target) =>
                        {
                            var instance = new ObjectWrapper(engine, target);
                            if (instance.IsArrayLike)
                            {
                                instance.SetPrototypeOf(engine.Realm.Intrinsics.Array.PrototypeObject);
                            }
                            return instance;
                        })
                    ;
                });;
            jsEngine.SetValue("input", data);
            if (args != null)
            {
                foreach (var argument in args)
                {
                    jsEngine.SetValue(argument.Key, argument.Value);
                }
            }
            var result = jsEngine.Evaluate(expression).UnwrapIfPromise().ToObject();
            if (expectedType == typeof(object))
                return result;
            return this.JsonSerializer.Deserialize(this.JsonSerializer.Serialize(result), expectedType);
        }

        /// <inheritdoc/>
        public virtual T? Evaluate<T>(string expression, object data, IDictionary<string, object>? args = null)
        {
            return (T?)this.Evaluate(expression, data, typeof(T), args);
        }

        /// <inheritdoc/>
        public virtual object? Evaluate(string expression, object data, IDictionary<string, object>? args = null)
        {
            var result = this.Evaluate(expression, data, typeof(object), args);
            if (result is JToken jtoken)
                result = jtoken.ToObject();
            return result;
        }

    }

}
