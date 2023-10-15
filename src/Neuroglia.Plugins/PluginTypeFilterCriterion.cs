using System.Reflection;

namespace Neuroglia.Plugins;

/// <summary>
/// Represents an object used to configure a plugin type filter criterion
/// </summary>
public class PluginTypeFilterCriterion
{

    /// <summary>
    /// Initializes a new <see cref="PluginTypeFilterCriterion"/>
    /// </summary>
    public PluginTypeFilterCriterion() { }

    /// <summary>
    /// Gets/sets the type filtered types should be be assignable to, if any
    /// </summary>
    public string? AssignableTo { get; set; }

    /// <summary>
    /// Gets/sets the type of the interface filtered types must implement, if any
    /// </summary>
    public string? Implements { get; set; }

    /// <summary>
    /// Gets/sets the type of the interface filtered types must implement, if any
    /// </summary>
    public string? InheritsFrom { get; set; }

    /// <summary>
    /// Indicates whether or not the criterion is met by the specified type
    /// </summary>
    /// <param name="candidateType">The type to check</param>
    /// <param name="metadataLoadContext">The <see cref="MetadataLoadContext"/> to use, if any</param>
    /// <returns>A boolean indicating whether or not the criterion is met by the specified type</returns>
    public bool IsMetBy(Type candidateType, MetadataLoadContext? metadataLoadContext = null)
    {
        if (!string.IsNullOrWhiteSpace(this.AssignableTo))
        {
            var expectedType = this.LoadType(this.AssignableTo, metadataLoadContext);
            if (expectedType.IsGenericTypeDefinition) return candidateType.IsGenericImplementationOf(expectedType);
            else return expectedType.IsAssignableFrom(candidateType);
        }
        if (!string.IsNullOrWhiteSpace(this.Implements))
        {
            var expectedType = this.LoadType(this.Implements, metadataLoadContext);
            if (expectedType.IsGenericTypeDefinition) return candidateType.IsGenericImplementationOf(expectedType);
            else return candidateType.GetInterfaces().Contains(expectedType);
        }
        if (!string.IsNullOrWhiteSpace(this.InheritsFrom))
        {
            var expectedType = this.LoadType(this.InheritsFrom, metadataLoadContext);
            if (expectedType.IsGenericTypeDefinition) return candidateType.IsGenericImplementationOf(expectedType);
            else return candidateType.InheritsFrom(expectedType);
        }
        return false;
    }

    /// <summary>
    /// Loads the specified type
    /// </summary>
    /// <param name="assemblyQualifiedName">The assembly-qualified name of the type to load</param>
    /// <param name="metadataLoadContext">The <see cref="MetadataLoadContext"/> in which to load the type, if any</param>
    /// <returns>The loaded type</returns>
    protected virtual Type LoadType(string assemblyQualifiedName, MetadataLoadContext? metadataLoadContext = null)
    {
        if (string.IsNullOrWhiteSpace(assemblyQualifiedName)) throw new ArgumentNullException(nameof(assemblyQualifiedName));
        Type? type;
        if (metadataLoadContext == null) type = Type.GetType(assemblyQualifiedName, true);
        else
        {
            var nameComponents = assemblyQualifiedName.Split(", ", StringSplitOptions.RemoveEmptyEntries);
            var typeName = nameComponents[0];
            var assemblyName = nameComponents[1];
            var contextAssembly = metadataLoadContext.GetAssemblies().FirstOrDefault(a => a.GetName().Name == assemblyName) ?? metadataLoadContext.LoadFromAssemblyName(assemblyName);
            type = contextAssembly.GetType(typeName, true);
        }
        return type ?? throw new NullReferenceException($"Failed to find the specified type '{assemblyQualifiedName}'");
    }

}
