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
using NuGet.Common;
using NuGet.Packaging;
using NuGet.ProjectManagement;
using System.Xml.Linq;

namespace Neuroglia.Plugins.Services;

/// <summary>
/// Represents a plugin-specific implementation of the <see cref="INuGetProjectContext"/>
/// </summary>
public class PluginNugetProjectContext
    : INuGetProjectContext
{

    /// <summary>
    /// Initializes a new <see cref="PluginNugetProjectContext"/>
    /// </summary>
    /// <param name="loggerFactory">The service used to create <see cref="Microsoft.Extensions.Logging.ILogger"/>s</param>
    public PluginNugetProjectContext(ILoggerFactory loggerFactory) { this.Logger = loggerFactory.CreateLogger(this.GetType()); }

    /// <summary>
    /// Gets the service used to perform logging
    /// </summary>
    protected Microsoft.Extensions.Logging.ILogger Logger { get; }

    /// <inheritdoc/>
    public virtual PackageExtractionContext PackageExtractionContext { get; set; } = null!;

    /// <inheritdoc/>
    public virtual ISourceControlManagerProvider SourceControlManagerProvider { get; } = null!;

    /// <inheritdoc/>
    public virtual NuGet.ProjectManagement.ExecutionContext ExecutionContext { get; } = null!;

    /// <inheritdoc/>
    public virtual XDocument OriginalPackagesConfig { get; set; } = null!;

    /// <inheritdoc/>
    public virtual NuGetActionType ActionType { get; set; }

    /// <inheritdoc/>
    public virtual Guid OperationId { get; set; }

    /// <inheritdoc/>
    public virtual void Log(MessageLevel level, string message, params object[] args)
    {
#pragma warning disable CA2254 // Template should be a static expression
        switch (level)
        {
            case MessageLevel.Error: this.Logger.LogError(message, args); break;
            case MessageLevel.Info: this.Logger.LogInformation(message, args); break;
            case MessageLevel.Warning: this.Logger.LogWarning(message, args); break;
            default: this.Logger.LogDebug(message, args); break;
        };
#pragma warning restore CA2254 // Template should be a static expression
    }

    /// <inheritdoc/>
    public virtual void Log(ILogMessage message)
    {
#pragma warning disable CA2254 // Template should be a static expression
        switch (message.Level)
        {
            case NuGet.Common.LogLevel.Error: this.Logger.LogError(message.Message); break;
            case NuGet.Common.LogLevel.Information: this.Logger.LogInformation(message.Message); break;
            case NuGet.Common.LogLevel.Warning: this.Logger.LogWarning(message.Message); break;
            default: this.Logger.LogDebug(message.Message); break;
        };
#pragma warning restore CA2254 // Template should be a static expression
    }

    /// <inheritdoc/>
    public virtual void ReportError(string message) => this.Logger.LogError(message);

    /// <inheritdoc/>
    public virtual void ReportError(ILogMessage message) => this.Logger.LogError(message.Message);

    /// <inheritdoc/>
    public virtual FileConflictAction ResolveFileConflict(string message) => FileConflictAction.IgnoreAll;

}
