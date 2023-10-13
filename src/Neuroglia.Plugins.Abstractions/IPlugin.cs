using System.Reflection;

namespace Neuroglia.Plugins;

/// <summary>
/// Defines the fundamentals of a plugin
/// </summary>
public interface IPlugin
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

}