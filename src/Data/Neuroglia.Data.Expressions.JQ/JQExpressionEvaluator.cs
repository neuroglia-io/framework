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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

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
        public virtual object? Evaluate(string expression, object data, Type expectedType, IDictionary<string, object>? args = null)
        {
            if (string.IsNullOrWhiteSpace(expression))
                throw new ArgumentNullException(nameof(expression));
            expression = expression.Trim();
            if (expression.StartsWith("${"))
                expression = expression[2..^1].Trim();
            if (string.IsNullOrWhiteSpace(expression))
                throw new ArgumentNullException(nameof(expression));
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            var serializerSettings = new JsonSerializerSettings() { ContractResolver = new DefaultContractResolver(), NullValueHandling = NullValueHandling.Ignore };
            var input = JsonConvert.SerializeObject(data, serializerSettings);
            var serializedArgs = args?
                .ToDictionary(a => a.Key, a => JsonConvert.SerializeObject(JToken.FromObject(a.Value), Formatting.None, serializerSettings))
                .Aggregate(
                    new StringBuilder(),
                    (accumulator, source) => accumulator.Append(@$"--argjson ""{source.Key}"" ""{this.EscapeArgs(source.Value)}"" "),
                    sb => sb.ToString()
                );
            var processArguments = @$"""{this.EscapeArgs(expression)}"" {serializedArgs} -c";
            var files = new List<string>();
            var maxLength = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? 8000 : 32699; // https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.processstartinfo.arguments?view=net-7.0#remarks
            if (processArguments.Length >= maxLength)
            {
                var filterFile = Path.GetTempFileName();
                File.WriteAllText(filterFile, expression);
                files.Add(filterFile);
                processArguments = $"-f {filterFile}";
                if (args != null && args.Any())
                {
                    foreach (var kvp in args)
                    {
                        var argFile = Path.GetTempFileName();
                        File.WriteAllText(argFile, JsonConvert.SerializeObject(JToken.FromObject(kvp.Value), Formatting.None, serializerSettings));
                        files.Add(argFile);
                        processArguments += @$" --argfile ""{kvp.Key}"" ""{argFile}""";
                    }
                }
                processArguments += " -c";
            }
            using Process process = new();
            
            process.StartInfo.FileName = "jq";
            process.StartInfo.Arguments = processArguments;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.Start();
            process.StandardInput.Write(input);
            process.StandardInput.Close();
            var output = process.StandardOutput.ReadToEnd();
            var error = process.StandardError.ReadToEnd();
            process.WaitForExit();
            foreach (var file in files)
            {
                try { File.Delete(file); } catch { }
            }
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

        /// <summary>
        /// Escapes double quotes (") or backslashes (\) in the provided string
        /// 
        /// Ignores \( and \\$
        /// </summary>
        /// <param name="input">The string to escape</param>
        /// <returns>The escaped string</returns>
        protected virtual string EscapeArgs(string input) => Regex.Replace(input, @"(\\(?!(\(|\$|\\\$))|"")", @"\$1", RegexOptions.Compiled);


    }

}
