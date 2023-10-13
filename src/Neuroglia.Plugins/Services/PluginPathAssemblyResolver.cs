using System.Reflection;

namespace Neuroglia.Plugins.Services;

/// <summary>
/// Represents a <see cref="PathAssemblyResolver"/> implementation that rely on another <see cref="PathAssemblyResolver"/> when faling to resolve given assemblies
/// </summary>
public class PluginPathAssemblyResolver
    : PathAssemblyResolver
{

    /// <inheritdoc/>
    public PluginPathAssemblyResolver(IEnumerable<string> assemblyPaths, IEnumerable<string> pluginAssemblyPaths) : base(assemblyPaths) { this.PluginAssemblyResolver = new(pluginAssemblyPaths); }

    /// <summary>
    /// Gets the <see cref="PathAssemblyResolver"/> to use when failing to resolve given assemblies
    /// </summary>
    protected PathAssemblyResolver PluginAssemblyResolver { get; }

    /// <inheritdoc/>
    public override Assembly? Resolve(MetadataLoadContext context, AssemblyName assemblyName) => base.Resolve(context, assemblyName) ?? this.PluginAssemblyResolver.Resolve(context, assemblyName);

}