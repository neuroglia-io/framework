namespace Neuroglia.Plugins.Configuration;

/// <summary>
/// Represents an object used to configure plugins
/// </summary>
public class PluginOptions
{

    /// <summary>
    /// Gets/sets a list containing the plugin sources to register
    /// </summary>
    public List<PluginSourceOptions>? Sources { get; set; }

    /// <summary>
    /// Gets/sets a list containing the plugin services to register
    /// </summary>
    public List<PluginServiceOptions>? Services { get; set; }

}
