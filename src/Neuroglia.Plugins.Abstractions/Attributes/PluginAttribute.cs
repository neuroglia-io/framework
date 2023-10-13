using Neuroglia.Plugins.Services;

namespace Neuroglia.Plugins;

/// <summary>
/// Represents the attribute used to configure plugin-specific behaviors
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class PluginAttribute
    : Attribute
{

    Type? _factoryType;

    /// <summary>
    /// Gets/sets the plugin's name
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets/sets the plugin's version
    /// </summary>
    public Version? Version { get; set; }

    /// <summary>
    /// Gets/sets the <see cref="IPluginFactory"/> type to use, if any, in order to create new instances of the plugin
    /// </summary>
    public Type? FactoryType { get; set; }

}