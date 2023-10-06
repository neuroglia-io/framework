using System.Reflection;

namespace Neuroglia;

/// <summary>
/// Acts as a helper class for locating <see cref="Assembly"/> instances
/// </summary>
public static class AssemblyLocator
{

    static readonly object Lock = new();

    static readonly List<Assembly> LoadedAssemblies = new();

    static AssemblyLocator()
    {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            LoadedAssemblies.Add(assembly);
        }
        AppDomain.CurrentDomain.AssemblyLoad += OnAssemblyLoad;
    }

    /// <summary>
    /// Get all loaded assemlies
    /// </summary>
    /// <returns>An <see cref="IEnumerable{T}"/> of all loaded assemblies</returns>
    public static IEnumerable<Assembly> GetAssemblies() => LoadedAssemblies.AsEnumerable();

    static void OnAssemblyLoad(object? sender, AssemblyLoadEventArgs e)
    {
        lock (Lock)
        {
            LoadedAssemblies.Add(e.LoadedAssembly);
        }
    }

}