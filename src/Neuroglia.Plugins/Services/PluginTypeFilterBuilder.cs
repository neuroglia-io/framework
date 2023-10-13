namespace Neuroglia.Plugins.Services;

/// <summary>
/// Represents the service used to build <see cref="PluginTypeFilter"/>s
/// </summary>
public class PluginTypeFilterBuilder
    : IPluginTypeFilterBuilder
{

    /// <summary>
    /// Gets the <see cref="PluginTypeFilter"/> to build
    /// </summary>
    protected PluginTypeFilter Filter { get; } = new();

    /// <inheritdoc/>
    public virtual IPluginTypeFilterBuilder AssignableFrom<T>()
        where T : class
    {
        if (!typeof(T).IsInterface) throw new ArgumentException($"The specified type must be an interface", nameof(T));
        this.Filter.Criteria.Add(new(PluginTypeFilterCriterionType.Implements, typeof(T).AssemblyQualifiedName!));
        return this;
    }

    /// <inheritdoc/>
    public virtual IPluginTypeFilterBuilder Implements<TInterface>()
        where TInterface : class
    {
        if (!typeof(TInterface).IsInterface) throw new ArgumentException($"The specified type must be an interface", nameof(TInterface));
        this.Filter.Criteria.Add(new(PluginTypeFilterCriterionType.Implements, typeof(TInterface).AssemblyQualifiedName!));
        return this;
    }

    /// <inheritdoc/>
    public virtual IPluginTypeFilterBuilder Inherits<TBaseType>()
        where TBaseType : class
    {
        if (!typeof(TBaseType).IsInterface) throw new ArgumentException($"The specified type must be an interface", nameof(TBaseType));
        this.Filter.Criteria.Add(new(PluginTypeFilterCriterionType.Inherits, typeof(TBaseType).AssemblyQualifiedName!));
        return this;
    }

    /// <inheritdoc/>
    public virtual PluginTypeFilter Build() => this.Filter;

}
