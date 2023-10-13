using System.Reflection;
using System.Runtime.Loader;

namespace Neuroglia.Plugins.Services;

/// <summary>
/// Represents an <see cref="AssemblyLoadContext"/> used to load <see cref="IPlugin"/> assemblies
/// </summary>
public class PluginAssemblyLoadContext
    : AssemblyLoadContext
{

    /// <summary>
    /// Initializes a new <see cref="PluginAssemblyLoadContext"/>
    /// </summary>
    /// <param name="assemblyDependencyResolver">The service used to resolve assembly dependencies</param>
    private PluginAssemblyLoadContext(AssemblyDependencyResolver assemblyDependencyResolver): base("PluginAssemblyLoadContext", true) => this.AssemblyDependencyResolver = assemblyDependencyResolver ?? throw new ArgumentNullException(nameof(assemblyDependencyResolver));

    /// <summary>
    /// Initializes a new <see cref="PluginAssemblyLoadContext"/>
    /// </summary>
    /// <param name="assemblyPath">The path of the plugin <see cref="Assembly"/> to load</param>
    public PluginAssemblyLoadContext(string assemblyPath) : this(new AssemblyDependencyResolver(assemblyPath)) { this.AssemblyPath = assemblyPath; }

    /// <summary>
    /// Gets the path to the plugin <see cref="Assembly"/> to load
    /// </summary>
    protected string AssemblyPath { get; } = null!;

    /// <summary>
    /// Gets the service used to resolve assembly dependencies
    /// </summary>
    protected AssemblyDependencyResolver AssemblyDependencyResolver { get; }

    /// <summary>
    /// Loads the plugin <see cref="Assembly"/>
    /// </summary>
    /// <returns>The loaded <see cref="Assembly"/></returns>
    public virtual Assembly Load() => this.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(this.AssemblyPath)));

    /// <inheritdoc/>
    protected override Assembly? Load(AssemblyName assemblyName)
    {
        var assembly = Default.Assemblies.FirstOrDefault(a => a.GetName().Name == assemblyName.Name && a.GetName().Version >= assemblyName.Version);
        if (assembly != null) return assembly;
        var assemblyPath = this.AssemblyDependencyResolver.ResolveAssemblyToPath(assemblyName);
        if (string.IsNullOrWhiteSpace(assemblyPath)) return null;
        return this.LoadFromAssemblyPath(assemblyPath);
    }

    /// <inheritdoc/>
    protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
    {
        var assemblyPath = this.AssemblyDependencyResolver.ResolveUnmanagedDllToPath(unmanagedDllName);
        if (string.IsNullOrWhiteSpace(assemblyPath)) return IntPtr.Zero;
        return this.LoadUnmanagedDllFromPath(assemblyPath);
    }

}
