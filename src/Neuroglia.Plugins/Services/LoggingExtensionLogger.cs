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

namespace Neuroglia.Plugins.Services;

/// <summary>
/// Represents a <see cref="NuGet.Common.ILogger"/> that leverages <see cref="ILogger"/>s
/// </summary>
public class LoggingExtensionLogger
    : NuGet.Common.LoggerBase
{

    /// <summary>
    /// Initializes a new <see cref="LoggingExtensionLogger"/>
    /// </summary>
    /// <param name="logger">The underlying <see cref="ILogger"/></param>
    public LoggingExtensionLogger(ILogger logger) { this.Logger = logger; }

    /// <summary>
    /// Gets the service used to perform logging
    /// </summary>
    protected ILogger Logger { get; }

    /// <inheritdoc/>
    public override void Log(NuGet.Common.ILogMessage message)
    {
        switch (message.Level)
        {
#pragma warning disable CA2254 // Template should be a static expression
            case NuGet.Common.LogLevel.Error: this.Logger.LogError(message.Message); break;
            case NuGet.Common.LogLevel.Information: this.Logger.LogInformation(message.Message); break;
            case NuGet.Common.LogLevel.Warning: this.Logger.LogWarning(message.Message); break;
            default: this.Logger.LogDebug(message.Message); break;
#pragma warning restore CA2254 // Template should be a static expression
        };
    }

    /// <inheritdoc/>
    public override Task LogAsync(NuGet.Common.ILogMessage message) => Task.Run(() => this.Log(message));

}