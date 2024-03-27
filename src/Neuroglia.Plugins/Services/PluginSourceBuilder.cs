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

using Microsoft.Extensions.Logging.Abstractions;
using Neuroglia.Plugins.Configuration;

namespace Neuroglia.Plugins.Services;

/// <summary>
/// Represents the default implementation of the <see cref="IPluginSourceBuilder"/> interface
/// </summary>
public class PluginSourceBuilder
    : IPluginSourceBuilder, IPluginSourceFinalStageBuilder
{

    /// <summary>
    /// Gets/sets the <see cref="IPluginSource"/> to build
    /// </summary>
    protected IPluginSource? Source { get; set; }

    /// <inheritdoc/>
    public virtual IPluginSourceFinalStageBuilder FromDirectory(string? name, Action<IPluginTypeFilterBuilder> filterSetup, string directoryPath, string searchPattern, SearchOption searchOption = SearchOption.TopDirectoryOnly)
    {
        var filterBuilder = new PluginTypeFilterBuilder();
        filterSetup.Invoke(filterBuilder);
        var options = new PluginSourceOptions() { Filter = filterBuilder.Build() };
        this.Source = new DirectoryPluginSource(name, options, directoryPath, searchPattern, searchOption);
        return this;
    }

    /// <inheritdoc/>
    public virtual IPluginSourceFinalStageBuilder FromAssembly(string? name, Action<IPluginTypeFilterBuilder> filterSetup, string filePath)
    {
        var filterBuilder = new PluginTypeFilterBuilder();
        filterSetup.Invoke(filterBuilder);
        var options = new PluginSourceOptions() { Filter = filterBuilder.Build() };
        this.Source = new AssemblyPluginSource(name, options, filePath);
        return this;
    }

    /// <inheritdoc/>
    public virtual IPluginSourceFinalStageBuilder FromNugetPackage(string? name, Action<IPluginTypeFilterBuilder> filterSetup, string packageId, string packageVersion, Uri? packageSourceUri, bool includePreRelease = false)
    {
        var filterBuilder = new PluginTypeFilterBuilder();
        filterSetup.Invoke(filterBuilder);
        var options = new PluginSourceOptions() { Filter = filterBuilder.Build() };
        this.Source = new NugetPackagePluginSource(new NullLoggerFactory(), name, options, packageId, packageVersion, packageSourceUri, includePreRelease);
        return this;
    }

    /// <inheritdoc/>
    public virtual IPluginSource Build() => this.Source ?? throw new InvalidOperationException();

}