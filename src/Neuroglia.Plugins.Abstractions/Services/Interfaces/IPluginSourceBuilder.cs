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

using System.Reflection;

namespace Neuroglia.Plugins.Services;

/// <summary>
/// Defines the fundamentals of a service used to build an <see cref="IPluginSource"/> fluently
/// </summary>
public interface IPluginSourceBuilder
{

    /// <summary>
    /// Creates a new <see cref="IPluginSource"/> from the specified directory
    /// </summary>
    /// <param name="name">The name of the source to build, if any</param>
    /// <param name="filterSetup">An <see cref="Action{T}"/> used to configure the plugin type filter to use</param>
    /// <param name="directoryPath">The path to the directory to create a new <see cref="IPluginSource"/> for</param>
    /// <param name="searchPattern">The pattern used to find plugin assembly files</param>
    /// <param name="searchOption">Specifies whether to all directories or only top-level ones</param>
    /// <returns>A new <see cref="IPluginSource"/></returns>
    IPluginSourceFinalStageBuilder FromDirectory(string? name, Action<IPluginTypeFilterBuilder> filterSetup, string directoryPath, string searchPattern, SearchOption searchOption = SearchOption.TopDirectoryOnly);

    /// <summary>
    /// Creates a new <see cref="IPluginSource"/> from the specified <see cref="Assembly"/>
    /// </summary>
    /// <param name="name">The name of the source to build, if any</param>
    /// <param name="filterSetup">An <see cref="Action{T}"/> used to configure the plugin type filter to use</param>
    /// <param name="filePath">The path to the <see cref="Assembly"/> file to create a new <see cref="IPluginSource"/> for</param>
    /// <returns>A new <see cref="IPluginSource"/></returns>
    IPluginSourceFinalStageBuilder FromAssembly(string? name, Action<IPluginTypeFilterBuilder> filterSetup, string filePath);

    /// <summary>
    /// Creates a new <see cref="IPluginSource"/> from the specified Nuget package
    /// </summary>
    /// <param name="name">The name of the source to build, if any</param>
    /// <param name="filterSetup">An <see cref="Action{T}"/> used to configure the plugin type filter to use</param>
    /// <param name="packageId">The name of the Nuget package used to source plugins</param>
    /// <param name="packageVersion">The version of the Nuget package used to source plugins</param>
    /// <param name="packageSourceUri">The uri of the package source to get the specified Nuget package from</param>
    /// <param name="includePreRelease">A boolean indicating whether or not to include pre-release packages</param>
    /// <returns>A new <see cref="IPluginSource"/></returns>
    IPluginSourceFinalStageBuilder FromNugetPackage(string? name, Action<IPluginTypeFilterBuilder> filterSetup, string packageId, string packageVersion, Uri? packageSourceUri, bool includePreRelease = false);

}
