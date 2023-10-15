using System.Reflection;

namespace Neuroglia.Plugins.Services;

/// <summary>
/// Defines the fundamentals of a service used to filter plugin types
/// </summary>
public interface IPluginTypeFilter
{

    /// <summary>
    /// Determines whether or not to filter the specified type
    /// </summary>
    /// <param name="type">The type to evaluate</param>
    /// <param name="metadataLoadContext">The <see cref="MetadataLoadContext"/> to use, if any</param>
    /// <returns>A boolean indicating whether or not to filter the specified type</returns>
    bool Filters(Type type, MetadataLoadContext? metadataLoadContext = null);

}
