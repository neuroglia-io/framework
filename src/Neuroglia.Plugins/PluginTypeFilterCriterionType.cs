using Neuroglia.Serialization.Json.Converters;
using Neuroglia.Serialization.Services;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Neuroglia.Plugins;

/// <summary>
/// Enumerates all supported plugin filter criterion types
/// </summary>
[TypeConverter(typeof(EnumMemberTypeConverter))]
[JsonConverter(typeof(StringEnumConverter))]
public enum PluginTypeFilterCriterionType
{
    /// <summary>
    /// Indicates that the filter will match types assignable to the specified one
    /// </summary>
    [EnumMember(Value = "assignable")]
    Assignable,
    /// <summary>
    /// Indicates that the filter will match types that implement the specified interface
    /// </summary>
    [EnumMember(Value = "implements")]
    Implements,
    /// <summary>
    /// Indicates that the filter will match types that inherit from the specified one
    /// </summary>
    [EnumMember(Value = "inherits")]
    Inherits
}