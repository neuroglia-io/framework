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
    /// Initializes a new <see cref="PluginTypeFilterCriterion"/>
    /// </summary>
    /// <param name="type">The criterion's type</param>
    /// <param name="value">The criterion's value</param>
    public PluginTypeFilterCriterion(PluginTypeFilterCriterionType type, string value)
    {
        this.Type = type;
        this.Value = value;
    }

    /// <summary>
    /// Gets/sets the criterion's type
    /// </summary>
    public PluginTypeFilterCriterionType Type { get; set; }

    /// <summary>
    /// Gets/sets the criterion's value
    /// </summary>
    public string Value { get; set; } = null!;

    /// <summary>
    /// Indicates whether or not the criterion is met by the specified type
    /// </summary>
    /// <param name="candidateType">The type to check</param>
    /// <param name="metadataLoadContext">The <see cref="MetadataLoadContext"/> to use, if any</param>
    /// <returns>A boolean indicating whether or not the criterion is met by the specified type</returns>
    public bool IsMetBy(Type candidateType, MetadataLoadContext? metadataLoadContext = null)
    {
        var expectedTypeNameComponents = this.Value.Split(", ", StringSplitOptions.RemoveEmptyEntries);
        var expectedTypeName = expectedTypeNameComponents[0];
        var expectedTypeAssemblyName = expectedTypeNameComponents[1];
        Type? expectedType;
        if(metadataLoadContext == null)
        {
            expectedType = System.Type.GetType(this.Value, true);
        }
        else
        {
            var expectedTypeAssembly = metadataLoadContext.GetAssemblies().FirstOrDefault(a => a.GetName().Name == expectedTypeAssemblyName) ?? metadataLoadContext.LoadFromAssemblyName(expectedTypeAssemblyName);
            expectedType = expectedTypeAssembly.GetType(expectedTypeName, true);
        }
        if(expectedType == null) throw new Exception($"Failed to find the type '{Value}' expected by the plugin type filter criterion");
        return this.Type switch
        {
            PluginTypeFilterCriterionType.Assignable => expectedType.IsAssignableFrom(candidateType),
            PluginTypeFilterCriterionType.Implements => candidateType.GetInterfaces().Contains(expectedType),
            PluginTypeFilterCriterionType.Inherits => candidateType.InheritsFrom(expectedType),
            _ => throw new NotSupportedException($"The specified {nameof(PluginTypeFilterCriterionType)} '{Type}' is not supported")
        };
    }

}
