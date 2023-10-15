using Neuroglia.Plugins.Services;
using System.Reflection;

namespace Neuroglia.Plugins;

/// <summary>
/// Represents an object used to configure how to filter plugin types
/// </summary>
public class PluginTypeFilter
    : IPluginTypeFilter
{

    /// <summary>
    /// Gets/sets a list containing the filter's filtering criteria
    /// </summary>
    public List<PluginTypeFilterCriterion> Criteria { get; set; } = new();

    /// <summary>
    /// Determines whether or not to filter the specified type
    /// </summary>
    /// <param name="type">The type to check</param>
    /// <param name="metadataLoadContext">The <see cref="MetadataLoadContext"/> to use, if any</param>
    /// <returns>A boolean indicating whether or not to filter the specified type</returns>
    public virtual bool Filters(Type type, MetadataLoadContext? metadataLoadContext = null) => Criteria.All(c => c.IsMetBy(type, metadataLoadContext));

}

