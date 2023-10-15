namespace Neuroglia.Plugins.Services;

/// <summary>
/// Defines the fundamentals of a service used to build <see cref="IPluginSource"/>
/// </summary>
public interface IPluginSourceFinalStageBuilder
{

    /// <summary>
    /// Builds the configured <see cref="IPluginSource"/>
    /// </summary>
    /// <returns>The configured <see cref="IPluginSource"/></returns>
    IPluginSource Build();

}