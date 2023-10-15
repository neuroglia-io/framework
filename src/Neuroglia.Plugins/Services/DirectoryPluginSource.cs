using Neuroglia.Plugins.Configuration;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Loader;

namespace Neuroglia.Plugins.Services;

/// <summary>
/// Represents an <see cref="IPluginSource"/> implementation that retrieves plugins from a specific file system directory
/// </summary>
public class DirectoryPluginSource
    : IPluginSource
{

    const string DeafultSearchPattern = "*.dll";
    readonly List<AssemblyPluginSource> _assemblies = new();

    /// <summary>
    /// Initializes a new <see cref="DirectoryPluginSource"/>
    /// </summary>
    /// <param name="name">The name of the source, if any</param>
    /// <param name="options">The source's options</param>
    /// <param name="path">The path to the directory used to source <see cref="IPlugin"/>s</param>
    /// <param name="searchPattern">The search pattern to use to find plugin assembly files</param>
    /// <param name="searchOption">A value indicating whether to search all directories or only the top level ones</param>
    public DirectoryPluginSource(string? name, PluginSourceOptions options, string? path = null, string? searchPattern = null, SearchOption searchOption = SearchOption.TopDirectoryOnly)
    {
        if (string.IsNullOrWhiteSpace(path)) path = AppContext.BaseDirectory;
        if (!Directory.Exists(path)) throw new DirectoryNotFoundException($"Failed to find the specified directory '{path}'");
        if (string.IsNullOrWhiteSpace(searchPattern)) searchPattern = DeafultSearchPattern;
        this.Name = name;
        this.Options = options ?? throw new ArgumentNullException(nameof(options));
        this.Path = path;
        this.SearchPattern = searchPattern;
        this.SearchOption = searchOption;
    }

    /// <inheritdoc/>
    public virtual string? Name { get; }

    /// <summary>
    /// Gets the source's options
    /// </summary>
    protected PluginSourceOptions Options { get; }

    /// <summary>
    /// Gets the path to the directory used to source <see cref="IPlugin"/>s
    /// </summary>
    protected virtual string Path { get; }

    /// <summary>
    /// Gets the search pattern to use to find plugin assembly files
    /// </summary>
    protected virtual string SearchPattern { get; }

    /// <summary>
    /// Gets a value indicating whether to search all directories or only the top level ones
    /// </summary>
    protected SearchOption SearchOption { get; }

    /// <inheritdoc/>
    public virtual bool IsLoaded { get; protected set; }

    /// <inheritdoc/>
    public virtual IReadOnlyList<IPlugin> Plugins => this.IsLoaded ? this._assemblies.SelectMany(a => a.Plugins).ToList().AsReadOnly() : throw new NotSupportedException("The plugin source has not yet been loaded");

    /// <inheritdoc/>
    public virtual async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        if (this.IsLoaded) throw new NotSupportedException("The plugin source is already loaded");

        var sourceDirectory = new DirectoryInfo(this.Path);
        if (!sourceDirectory.Exists) throw new DirectoryNotFoundException($"Failed to find the specified directory '{sourceDirectory.FullName}'");
        
        foreach (var assemblyFilePath in Directory.GetFiles(this.Path, this.SearchPattern, this.SearchOption).ToList())
        {
            if (!await this.IsPluginAssemblyAsync(assemblyFilePath, cancellationToken).ConfigureAwait(false)) continue;
            var assemblySource = new AssemblyPluginSource(this.Name, this.Options, assemblyFilePath);
            await assemblySource.LoadAsync(cancellationToken).ConfigureAwait(false);
            this._assemblies.Add(assemblySource);
        }

        this.IsLoaded = true;
    }

    /// <summary>
    /// Determines whether or not the specified assembly contains plugins
    /// </summary>
    /// <param name="assemblyFilePath">The file to the assembly to check</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A boolean indicating whether or not the specified assembly is a plugin assembly</returns>
    protected virtual Task<bool> IsPluginAssemblyAsync(string assemblyFilePath, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(assemblyFilePath)) throw new ArgumentNullException(nameof(assemblyFilePath));
        
        var assemblyFile = new FileInfo(assemblyFilePath);
        if (!assemblyFile.Exists) throw new FileNotFoundException($"Failed to find the specified assembly file '{assemblyFile.FullName}'", assemblyFile.FullName);

        var runtimeAssemblies = Directory.GetFiles(RuntimeEnvironment.GetRuntimeDirectory(), "*.dll");
        var defaultAssemblies = AssemblyLoadContext.Default.Assemblies.Select(a => a.Location).Except(runtimeAssemblies);
        var appAssemblies = new FileInfo(typeof(DirectoryPluginSource).Assembly.Location).Directory!.GetFiles("*.dll").Select(f => f.FullName).Except(runtimeAssemblies).Except(defaultAssemblies);
        var assemblyFiles = assemblyFile.Directory!.GetFiles("*.dll", SearchOption.TopDirectoryOnly).ToList();
        var assemblies = new List<string>(runtimeAssemblies) { assemblyFile.FullName };
        assemblies.AddRange(defaultAssemblies);
        assemblies.AddRange(appAssemblies);

        var resolver = new PluginPathAssemblyResolver(assemblies.Where(a => !string.IsNullOrWhiteSpace(a)).Distinct(), assemblyFiles.Select(f => f.FullName));
        using var metadataContext = new MetadataLoadContext(resolver);
        var assembly = metadataContext.LoadFromAssemblyPath(assemblyFile.FullName);

        if (this._assemblies.SelectMany(a => a.Plugins).Any(p => p.Assembly.GetName().Name == assembly.GetName().Name)) return Task.FromResult(false);
        else return Task.FromResult(assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsInterface && !t.IsAbstract)
            .Any(t => this.Options.Filter.Filters(t, metadataContext)));
    }

}
