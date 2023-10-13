namespace Neuroglia.Plugins.Services;

/// <summary>
/// Defines the fundamentals of a service used to create a new instance of a plugin
/// </summary>
public interface IPluginFactory
{

    /// <summary>
    /// Creates the plugin
    /// </summary>
    /// <returns>A new instance of the plugin</returns>
    object Create();

}
