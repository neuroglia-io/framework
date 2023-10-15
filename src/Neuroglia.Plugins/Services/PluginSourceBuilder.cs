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
    public virtual IPluginSourceFinalStageBuilder FromNugetPackage(string? name, Action<IPluginTypeFilterBuilder> filterSetup, string packageId, string packageVersion, Uri? packageSourceUri, bool includePreRelease = false, string? packagesDirectory = null)
    {
        var filterBuilder = new PluginTypeFilterBuilder();
        filterSetup.Invoke(filterBuilder);
        var options = new PluginSourceOptions() { Filter = filterBuilder.Build() };
        this.Source = new NugetPackagePluginSource(new NullLoggerFactory(), name, options, packageId, packageVersion, packageSourceUri, includePreRelease, packagesDirectory);
        return this;
    }

    /// <inheritdoc/>
    public virtual IPluginSource Build() => this.Source ?? throw new InvalidOperationException();

}