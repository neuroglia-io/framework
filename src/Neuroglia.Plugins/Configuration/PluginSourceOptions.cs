using System.ComponentModel.DataAnnotations;

namespace Neuroglia.Plugins.Configuration;

/// <summary>
/// Represents an object used to configure a plugin source
/// </summary>
public class PluginSourceOptions
{

    /// <summary>
    /// Gets/sets the plugin type filter to use
    /// </summary>
    [Required]
    public PluginTypeFilter TypeFilter { get; set; } = null!;

}
