namespace Neuroglia.Plugins.Services;

/// <summary>
/// Defines the fundamentals of a service used to build <see cref="IPluginTypeFilter"/>s
/// </summary>
public interface IPluginTypeFilterBuilder
{

    /// <summary>
    /// Configures the filter to match types that are assignable to the specified type
    /// </summary>
    /// <param name="type">The type filtered types must assignable to</param>
    /// <returns>The configured <see cref="IPluginTypeFilterBuilder"/></returns>
    IPluginTypeFilterBuilder AssignableTo(Type type);

    /// <summary>
    /// Configures the filter to match types that are assignable to the specified type
    /// </summary>
    /// <typeparam name="T">The type filtered types must assignable to</typeparam>
    /// <returns>The configured <see cref="IPluginTypeFilterBuilder"/></returns>
    IPluginTypeFilterBuilder AssignableTo<T>()
        where T : class;

    /// <summary>
    /// Configures the filter to match types that implement the specified interface
    /// </summary>
    /// <param name="type">The type of interface filtered types must implement</param>
    /// <returns>The configured <see cref="IPluginTypeFilterBuilder"/></returns>
    IPluginTypeFilterBuilder Implements(Type type);

    /// <summary>
    /// Configures the filter to match types that implement the specified interface
    /// </summary>
    /// <typeparam name="T">The type of interface filtered types must implement</typeparam>
    /// <returns>The configured <see cref="IPluginTypeFilterBuilder"/></returns>
    IPluginTypeFilterBuilder Implements<T>()
        where T : class;

    /// <summary>
    /// Configures the filter to match types that inherits from the specified type
    /// </summary>
    /// <param name="type">The type filtered types must inherit from</param>
    /// <returns>The configured <see cref="IPluginTypeFilterBuilder"/></returns>
    IPluginTypeFilterBuilder InheritsFrom(Type type);

    /// <summary>
    /// Configures the filter to match types that inherits from the specified type
    /// </summary>
    /// <typeparam name="T">The type filtered types must inherit from</typeparam>
    /// <returns>The configured <see cref="IPluginTypeFilterBuilder"/></returns>
    IPluginTypeFilterBuilder Inherits<T>()
        where T : class;

    /// <summary>
    /// Builds the <see cref="IPluginTypeFilter"/>
    /// </summary>
    /// <returns>A new <see cref="IPluginTypeFilter"/></returns>
    IPluginTypeFilter Build();

}
