namespace Neuroglia.Plugins.Services;

/// <summary>
/// Defines the fundamentals of a service used to build <see cref="PluginTypeFilter"/>s
/// </summary>
public interface IPluginTypeFilterBuilder
{

    /// <summary>
    /// Configures the filter to match types that are assignable to the specified type
    /// </summary>
    /// <typeparam name="T">The type filtered types must assignable to</typeparam>
    /// <returns>The configured <see cref="PluginTypeFilterBuilder"/></returns>
    IPluginTypeFilterBuilder AssignableFrom<T>()
        where T : class;

    /// <summary>
    /// Configures the filter to match types that implement the specified interface
    /// </summary>
    /// <typeparam name="TInterface">The type of interface filtered types must implement</typeparam>
    /// <returns>The configured <see cref="PluginTypeFilterBuilder"/></returns>
    IPluginTypeFilterBuilder Implements<TInterface>()
        where TInterface : class;

    /// <summary>
    /// Configures the filter to match types that inherits from the specified type
    /// </summary>
    /// <typeparam name="TBaseType">The type filtered types must inherit from</typeparam>
    /// <returns>The configured <see cref="PluginTypeFilterBuilder"/></returns>
    IPluginTypeFilterBuilder Inherits<TBaseType>()
        where TBaseType : class;

    /// <summary>
    /// Builds the <see cref="PluginTypeFilter"/>
    /// </summary>
    /// <returns>A new <see cref="PluginTypeFilter"/></returns>
    PluginTypeFilter Build();

}
