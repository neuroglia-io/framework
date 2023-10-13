using Neuroglia.Plugins.Configuration;
using System.Reflection;

namespace Neuroglia.Plugins.Services;

/// <summary>
/// Represents an <see cref="IPluginSource"/> implementation that retrieves plugins from a specific <see cref="Assembly"/>
/// </summary>
public class AssemblyPluginSource
    : IPluginSource
{

    readonly List<IPlugin> _plugins = new();

    /// <summary>
    /// Initializes a new <see cref="AssemblyPluginSource"/>
    /// </summary>
    /// <param name="path">The path to the <see cref="Assembly"/> file used to source <see cref="IPlugin"/>s</param>
    /// <param name="searchPattern">The search pattern to use to find plugin assembly files</param>
    /// <param name="searchOption">A value indicating whether to search all directories or only the top level ones</param>
    /// <param name="options">The source's options</param>
    public AssemblyPluginSource(PluginSourceOptions options, string path)
    {
        if(string.IsNullOrWhiteSpace(path)) throw new ArgumentNullException(nameof(path));
        if (!File.Exists(path)) throw new FileNotFoundException($"Failed to find the specified file '{path}'", path);
        this.Options = options ?? throw new ArgumentNullException(nameof(options));
        this.Path = path;
    }

    /// <summary>
    /// Gets the path to the <see cref="Assembly"/> file used to source <see cref="IPlugin"/>s
    /// </summary>
    protected virtual string Path { get; }

    /// <summary>
    /// Gets the source's options
    /// </summary>
    protected PluginSourceOptions Options { get; }

    /// <inheritdoc/>
    public virtual bool IsLoaded { get; protected set; }

    /// <inheritdoc/>
    public virtual IReadOnlyList<IPlugin> Plugins => this.IsLoaded ? this._plugins.AsReadOnly() : throw new NotSupportedException("The plugin source has not yet been loaded");

    /// <inheritdoc/>
    public virtual async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        if (this.IsLoaded) throw new NotSupportedException("The plugin source is already loaded");

        var assemblyFile = new FileInfo(this.Path);
        if (!assemblyFile.Exists) throw new FileNotFoundException($"Failed to find the specified assembly file '{assemblyFile.FullName}'", assemblyFile.FullName);
        var assemblyLoadContext = new PluginAssemblyLoadContext(assemblyFile.FullName);
        var assembly = assemblyLoadContext.Load();

        foreach (var type in assembly.GetTypes().Where(t => this.Options.TypeFilter.Filters(t)))
        {
            var pluginAttribute = type.GetCustomAttribute<PluginAttribute>();
            var name = pluginAttribute?.Name ?? type.FullName!;
            var version = pluginAttribute?.Version ?? assembly.GetName().Version ?? new(1, 0, 0);
            this._plugins.Add(new Plugin(name, version, type, assembly, assemblyLoadContext));
        }

        this.IsLoaded = true;

        await Task.CompletedTask;
    }

}