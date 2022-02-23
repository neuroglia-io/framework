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
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Neuroglia.Data.Expressions.JQ.Configuration;
using Neuroglia.Serialization;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Diagnostics;
using System.Dynamic;
using System.Runtime.InteropServices;

namespace Neuroglia.Data.Expressions.JQ
{

    /// <summary>
    /// Represents the default, JQ implementation of the <see cref="IExpressionEvaluator"/> interface
    /// </summary>
    public class JQExpressionEvaluator
        : IExpressionEvaluator
    {

        /// <summary>
        /// Initializes a new <see cref="JQExpressionEvaluator"/>
        /// </summary>
        /// <param name="logger">The service used to perform logging</param>
        /// <param name="options">The service used to access the current <see cref="JQExpressionEvaluatorOptions"/></param>
        /// <param name="serializerProvider">The service used to provide <see cref="ISerializer"/>s</param>
        public JQExpressionEvaluator(ILogger<JQExpressionEvaluator> logger, IOptions<JQExpressionEvaluatorOptions> options, ISerializerProvider serializerProvider)
        {
            this.Logger = logger;
            this.Options = options.Value;
            this.JsonSerializer = (IJsonSerializer)serializerProvider.GetSerializer(this.Options.SerializerType);
            if (this.JsonSerializer == null)
                throw new NullReferenceException($"Failed to find an {nameof(IJsonSerializer)} implementation of type '{this.Options.SerializerType}'");
        }

        /// <summary>
        /// Gets the service used to perform logging
        /// </summary>
        protected ILogger Logger { get; }

        /// <summary>
        /// Gets the service used to access the current <see cref="JQExpressionEvaluatorOptions"/>
        /// </summary>
        protected JQExpressionEvaluatorOptions Options { get; }

        /// <summary>
        /// Gets the service used to serialize and deserialize json
        /// </summary>
        protected IJsonSerializer JsonSerializer { get; }

        /// <inheritdoc/>
        public virtual bool Supports(string language)
        {
            if (string.IsNullOrWhiteSpace(language))
                throw new ArgumentNullException(nameof(language));
            return language.Equals("jq", StringComparison.OrdinalIgnoreCase);
        }

        /// <inheritdoc/>
        public virtual object? Evaluate(string expression, object data, Type expectedType)
        {
            if (string.IsNullOrWhiteSpace(expression))
                throw new ArgumentNullException(nameof(expression));
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            string json = this.JsonSerializer.Serialize(data);
            string jqExpression = this.BuildJQExpression(expression);
            string fileName;
            string args;
            using Process process = new();
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                fileName = "cmd.exe";
                args = @$"/c echo {json} | jq.exe ""{jqExpression}""";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                fileName = "bash";
                args = @$"-c ""echo '{this.EscapeDoubleQuotes(json)}' | jq '{jqExpression}'""";
            }
            else
                throw new PlatformNotSupportedException();
            process.StartInfo.FileName = fileName;
            process.StartInfo.Arguments = args;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();
            if (process.ExitCode != 0)
            {
                this.Logger.LogError("An error occured while evaluting the specified expression: {error}", error);
                throw new Exception($"An error occured while evaluting the specified expression: {error}");
            }
            if (string.IsNullOrWhiteSpace(output))
                return null;
            else
                return this.JsonSerializer.Deserialize(output, expectedType);
        }

        /// <inheritdoc/>
        public virtual T? Evaluate<T>(string expression, object data)
        {
            return (T?)this.Evaluate(expression, data, typeof(T));
        }

        /// <inheritdoc/>
        public virtual object? Evaluate(string expression, object data)
        {
            var result = this.Evaluate(expression, data, typeof(object));
            if (result is JToken jtoken)
                result =  jtoken.ToObject();
            return result;
        }

        /// <summary>
        /// Builds a jq compliant expression from the specified expression
        /// </summary>
        /// <param name="expression">The expression to build a jq compliant expression for</param>
        /// <returns>A new jq compliant expression built from the specified expression</returns>
        protected virtual string BuildJQExpression(string expression)
        {
            string jqExpression = expression.Trim();
            if (jqExpression.StartsWith("${"))
                jqExpression = jqExpression[2..^1].Trim();
            if (!jqExpression.Contains(@"\"""))
                jqExpression = jqExpression.Replace("\"", @"\""");
            if (!jqExpression.Contains("^&"))
                jqExpression = jqExpression.Replace("&", "^&");
            return jqExpression;
        }

        /// <summary>
        /// Escapes double quotes in the specified string
        /// </summary>
        /// <param name="input">The string for which to escape double quotes</param>
        /// <returns>The string with escaped double quotes</returns>
        protected virtual string EscapeDoubleQuotes(string input)
        {
            if (!input.Contains(@"\"""))
                input = input.Replace("\"", @"\""");
            return input;
        }

    }

}
