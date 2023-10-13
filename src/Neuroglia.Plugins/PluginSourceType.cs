using Neuroglia.Serialization.Json.Converters;
using Neuroglia.Serialization.Services;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Neuroglia.Plugins;

/// <summary>
/// Enumerates supported plugin source types
/// </summary>
[TypeConverter(typeof(EnumMemberTypeConverter))]
[JsonConverter(typeof(StringEnumConverter))]
public enum PluginSourceType
{
    /// <summary>
    /// Indicates a file system based plugin source
    /// </summary>
    [EnumMember(Value = "folder")]
    Folder,
    /// <summary>
    /// Indicates an assembly based plugin source
    /// </summary>
    [EnumMember(Value = "assembly")]
    Assembly,
    /// <summary>
    /// Indicates a Nuget package plugin source
    /// </summary>
    [EnumMember(Value = "nuget")]
    Nuget
}