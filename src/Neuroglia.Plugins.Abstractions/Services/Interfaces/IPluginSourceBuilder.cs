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
    /// <param name="packagesDirectory">The directory to output the nuget packages to, if any</param>
    /// <returns>A new <see cref="IPluginSource"/></returns>
    IPluginSourceFinalStageBuilder FromNugetPackage(string? name, Action<IPluginTypeFilterBuilder> filterSetup, string packageId, string packageVersion, Uri? packageSourceUri, bool includePreRelease = false, string? packagesDirectory = null);

}
