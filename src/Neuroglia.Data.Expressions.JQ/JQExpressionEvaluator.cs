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

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Neuroglia.Data.Expressions.JQ.Configuration;
using Neuroglia.Data.Expressions.Services;
using Neuroglia.Serialization;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Neuroglia.Data.Expressions.JQ;

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
        this.Serializer = serializerProvider.GetSerializers().OfType<IJsonSerializer>().First(s => this.Options.SerializerType == null || s.GetType() == this.Options.SerializerType);
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
    protected IJsonSerializer Serializer { get; }

    /// <inheritdoc/>
    public virtual bool Supports(string language)
    {
        if (string.IsNullOrWhiteSpace(language)) throw new ArgumentNullException(nameof(language));
        return language.Equals("jq", StringComparison.OrdinalIgnoreCase);
    }

    /// <inheritdoc/>
    public virtual async Task<object?> EvaluateAsync(string expression, object input, IDictionary<string, object>? args = null, Type? expectedType = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(expression)) throw new ArgumentNullException(nameof(expression));
        if (input == null) throw new ArgumentNullException(nameof(input));
        if (expectedType == null) expectedType = typeof(object);

        expression = expression.Trim();
        if (expression.StartsWith("${")) expression = expression[2..^1].Trim();
        if (string.IsNullOrWhiteSpace(expression)) throw new ArgumentNullException(nameof(expression));

        var startInfo = new ProcessStartInfo()
        {
            FileName = "jq",
            UseShellExecute = false,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };
        startInfo.ArgumentList.Add(expression);
        if (args != null)
        {
            foreach (var kvp in args.ToDictionary(a => a.Key, a => this.Serializer.SerializeToText(a.Value)))
            {
                startInfo.ArgumentList.Add("--argjson");
                startInfo.ArgumentList.Add(kvp.Key);
                startInfo.ArgumentList.Add(kvp.Value);
            }
        }
        var files = new List<string>();
        var maxLength = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? 8000 : 32699;
        if (startInfo.ArgumentList.Any(a => a.Length >= maxLength))
        {
            startInfo.ArgumentList.Clear();
            var filterFile = Path.GetTempFileName();
            File.WriteAllText(filterFile, expression);
            files.Add(filterFile);
            startInfo.ArgumentList.Add("-f");
            startInfo.ArgumentList.Add(filterFile);
            if (args?.Any() == true)
            {
                foreach (var kvp in args)
                {
                    var argFile = Path.GetTempFileName();
                    File.WriteAllText(argFile, this.Serializer.SerializeToText(kvp.Value));
                    files.Add(argFile);
                    startInfo.ArgumentList.Add("--argfile");
                    startInfo.ArgumentList.Add(kvp.Key);
                    startInfo.ArgumentList.Add(argFile);
                }
            }
        }
        startInfo.ArgumentList.Add("-c");

        using var process = new Process() { StartInfo = startInfo };
        var cancellationRegistration = cancellationToken.Register(() => 
        {
            try
            {
                process.Kill();
            }
            catch { }
        });
        process.Start();
        process.StandardInput.Write(this.Serializer.SerializeToText(input));
        process.StandardInput.Close();
        var output = process.StandardOutput.ReadToEnd();
        var error = process.StandardError.ReadToEnd();
        await process.WaitForExitAsync(cancellationToken).ConfigureAwait(false);
        cancellationRegistration.Unregister();
        cancellationRegistration.Dispose();

        foreach (var file in files)
        {
            try { File.Delete(file); } catch { }
        }

        if (process.ExitCode != 0) throw new Exception($"An error occured while evaluting the specified expression: {error}");
        if (string.IsNullOrWhiteSpace(output)) return null;
        return this.Serializer.Deserialize(output, expectedType);
    }

}
