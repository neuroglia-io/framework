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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Neuroglia.Data.Expressions.JQ
{

    /// <summary>
    /// Represents the default, JQ implementation of the <see cref="IExpressionEvaluator"/> interface
    /// </summary>
    public class JQExpressionEvaluator
        : IExpressionEvaluator
    {

        /// <inheritdoc/>
        public virtual bool Supports(string language)
        {
            if(string.IsNullOrWhiteSpace(language))
                throw new ArgumentNullException(nameof(language));
            return language.Equals("jq", StringComparison.OrdinalIgnoreCase);
        }

        /// <inheritdoc/>
        public virtual JToken? Evaluate(string expression, JToken data)
        {
            expression = expression.Trim();
            if (expression.StartsWith("${"))
                expression = expression[2..^1].Trim();
            string fileName;
            string args;
            using Process process = new();
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                fileName = "cmd.exe";
                args = @$"/c echo {data.ToString(Formatting.None)} | jq ""{this.EscapeJson(expression)}""";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                fileName = "bash";
                args = @$"-c ""echo '{this.EscapeJson(data.ToString(Formatting.None))}' | jq '{this.EscapeJson(expression)}'""";
            }
            else
            {
                throw new PlatformNotSupportedException();
            }
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
                throw new Exception($"An error occured while evaluting the specified expression:{Environment.NewLine}Details: {error}");
            if (string.IsNullOrWhiteSpace(output))
                return null;
            else
                return JToken.Parse(output);
        }

        /// <summary>
        /// Escapes doubles quotes in the supplied json
        /// </summary>
        /// <param name="json">The json string to escape</param>
        /// <returns>The escaped json</returns>
        protected virtual string EscapeJson(string json)
        {
            if (!json.Contains(@"\"""))
                json = json.Replace("\"", @"\""");
            if (!json.Contains("^&"))
                json = json.Replace("&", "^&");
            return json;
        }

    }

}
