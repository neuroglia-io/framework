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
    public virtual IPluginSourceFinalStageBuilder FromDirectory(Action<IPluginTypeFilterBuilder> filterSetup, string directoryPath, string searchPattern, SearchOption searchOption = SearchOption.TopDirectoryOnly)
    {
        var filterBuilder = new PluginTypeFilterBuilder();
        filterSetup.Invoke(filterBuilder);
        var options = new PluginSourceOptions() { TypeFilter = filterBuilder.Build() };
        this.Source = new DirectoryPluginSource(options, directoryPath, searchPattern, searchOption);
        return this;
    }

    /// <inheritdoc/>
    public virtual IPluginSourceFinalStageBuilder FromAssembly(Action<IPluginTypeFilterBuilder> filterSetup, string filePath)
    {
        var filterBuilder = new PluginTypeFilterBuilder();
        filterSetup.Invoke(filterBuilder);
        var options = new PluginSourceOptions() { TypeFilter = filterBuilder.Build() };
        this.Source = new AssemblyPluginSource(options, filePath);
        return this;
    }

    /// <inheritdoc/>
    public virtual IPluginSourceFinalStageBuilder FromNugetPackage(Action<IPluginTypeFilterBuilder> filterSetup, string name, string version, Uri? sourceUri)
    {
        throw new NotImplementedException(); //todo
    }

    /// <inheritdoc/>
    public virtual IPluginSource Build() => this.Source ?? throw new InvalidOperationException();

}