namespace Neuroglia.Plugins.Configuration;

/// <summary>
/// Represents an object used to configure plugins
/// </summary>
public class PluginOptions
{

    /// <summary>
    /// Gets/sets a list containing the plugin sources
    /// </summary>
    public List<PluginSourceOptions>? Sources { get; set; }

}
