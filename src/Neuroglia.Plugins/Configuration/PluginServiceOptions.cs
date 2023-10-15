using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;

namespace Neuroglia.Plugins.Configuration;

/// <summary>
/// Represents an object used to configure a plugin service
/// </summary>
public class PluginServiceOptions
{

    /// <summary>
    /// Gets/sets the assembly qualified name of the type of plugin service to register
    /// </summary>
    [Required, MinLength(3)]
    public virtual string Type { get; set; } = null!;

    /// <summary>
    /// Gets/sets the name of the plugin source, if any, to get the plugin implementation from
    /// </summary>
    public virtual string? Source { get; set; } = null;

    /// <summary>
    /// Gets/sets the lifetime of the plugin service
    /// </summary>
    public virtual ServiceLifetime Lifetime { get; set; } = ServiceLifetime.Singleton;

}