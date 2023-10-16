using System.Reflection;
using System.Runtime.Loader;

namespace Neuroglia.Plugins.Services;

/// <summary>
/// Defines the fundamentals of an object that describes a plugin
/// </summary>
public interface IPluginDescriptor
{

    /// <summary>
    /// Gets the name of the plugin
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the version of the plugin
    /// </summary>
    Version Version { get; }

    /// <summary>
    /// Gets the plugin's type
    /// </summary>
    Type Type { get; }

    /// <summary>
    /// Gets the plugin's assembly
    /// </summary>
    Assembly Assembly { get; }

    /// <summary>
    /// Gets the <see cref="IPluginDescriptor"/>'s <see cref="System.Runtime.Loader.AssemblyLoadContext"/>
    /// </summary>
    AssemblyLoadContext AssemblyLoadContext { get; }

    /// <summary>
    /// Gets the <see cref="IPluginSource"/> the <see cref="IPluginDescriptor"/> is sourced by
    /// </summary>
    IPluginSource Source { get; }

    /// <summary>
    /// Gets a list containing the plugin's tags, if any
    /// </summary>
    IEnumerable<string>? Tags { get; }

}