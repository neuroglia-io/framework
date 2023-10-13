namespace Neuroglia.Plugins.Services;

/// <summary>
/// Defines the fundamentals of a service used to build <see cref="PluginTypeFilter"/>s
/// </summary>
public interface IPluginTypeFilterBuilder
{

    /// <summary>
    /// Configures the filter to match types that are assignable to the specified type
    /// </summary>
    /// <param name="type">The type filtered types must assignable to</param>
    /// <returns>The configured <see cref="PluginTypeFilterBuilder"/></returns>
    IPluginTypeFilterBuilder AssignableFrom(Type type);

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
    /// <param name="type">The type of interface filtered types must implement</param>
    /// <returns>The configured <see cref="PluginTypeFilterBuilder"/></returns>
    IPluginTypeFilterBuilder Implements(Type type);

    /// <summary>
    /// Configures the filter to match types that implement the specified interface
    /// </summary>
    /// <typeparam name="T">The type of interface filtered types must implement</typeparam>
    /// <returns>The configured <see cref="PluginTypeFilterBuilder"/></returns>
    IPluginTypeFilterBuilder Implements<T>()
        where T : class;

    /// <summary>
    /// Configures the filter to match types that inherits from the specified type
    /// </summary>
    /// <param name="type">The type filtered types must inherit from</param>
    /// <returns>The configured <see cref="PluginTypeFilterBuilder"/></returns>
    IPluginTypeFilterBuilder Inherits(Type type);

    /// <summary>
    /// Configures the filter to match types that inherits from the specified type
    /// </summary>
    /// <typeparam name="T">The type filtered types must inherit from</typeparam>
    /// <returns>The configured <see cref="PluginTypeFilterBuilder"/></returns>
    IPluginTypeFilterBuilder Inherits<T>()
        where T : class;

    /// <summary>
    /// Builds the <see cref="PluginTypeFilter"/>
    /// </summary>
    /// <returns>A new <see cref="PluginTypeFilter"/></returns>
    PluginTypeFilter Build();

}
