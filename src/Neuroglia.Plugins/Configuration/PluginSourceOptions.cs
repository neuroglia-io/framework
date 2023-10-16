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

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Neuroglia.Plugins.Services;
using System.ComponentModel.DataAnnotations;

namespace Neuroglia.Plugins.Configuration;

/// <summary>
/// Represents an object used to configure a plugin source
/// </summary>
public class PluginSourceOptions
{

    /// <summary>
    /// Gets/sets the type of the source to configure
    /// </summary>
    [Required]
    public PluginSourceType Type { get; set; }

    /// <summary>
    /// Gets/sets the name of the configured source
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets/sets the plugin type filter to use
    /// </summary>
    [Required]
    public PluginTypeFilter Filter { get; set; } = null!;

    /// <summary>
    /// Gets/sets a key/value mapping of the plugin's implementation-specific properties, if any
    /// </summary>
    public Dictionary<string, object> Properties { get; set; } = null!;

    /// <summary>
    /// Builds the <see cref="IPluginSource"/> configured by the <see cref="PluginSourceOptions"/>
    /// </summary>
    /// <param name="serviceProvider">The current <see cref="IServiceProvider"/></param>
    /// <returns>A new <see cref="IPluginSource"/></returns>
    public virtual IPluginSource BuildSource(IServiceProvider serviceProvider)
    {
        var properties = this.Properties ==null ? new Dictionary<string, object>() : new Dictionary<string, object>(this.Properties, StringComparer.OrdinalIgnoreCase);
        switch (this.Type)
        {
            case PluginSourceType.Assembly:
                if(!properties.TryGetValue("path", out var path)) throw new Exception($"Invalid configuration: 'path' property must be set");
                return new AssemblyPluginSource(this.Name, this, path.ToString()!);
            case PluginSourceType.Directory:
                properties.TryGetValue("path", out path);
                properties.TryGetValue("searchPattern", out var searchPattern);
                properties.TryGetValue("searchOption", out var searchOption);
                return new DirectoryPluginSource(this.Name, this, path?.ToString()!, searchPattern?.ToString()!, searchOption == default ? default : EnumHelper.Parse< SearchOption>(searchOption.ToString()!));
            case PluginSourceType.Nuget:
                if (!properties.TryGetValue("packageId", out var packageId)) throw new Exception($"Invalid configuration: 'packageId' property must be set");
                properties.TryGetValue("packageVersion", out var packageVersion);
                properties.TryGetValue("packageSourceUri", out var packageSourceUri);
                properties.TryGetValue("includePreRelease", out var includePreRelease);
                properties.TryGetValue("packagesDirectory", out var packagesDirectory);
                return new NugetPackagePluginSource(serviceProvider.GetRequiredService<ILoggerFactory>(), this.Name, this, (string)packageId, packageVersion?.ToString()!, packageSourceUri == null ? null! : new Uri(packageSourceUri.ToString()!), includePreRelease != null && bool.Parse(includePreRelease.ToString()!), packagesDirectory?.ToString()!);
            default: throw new NotSupportedException($"The specified {nameof(PluginSourceType)} '{EnumHelper.Stringify(this.Type)}' is not supported");
        }
    }

}
