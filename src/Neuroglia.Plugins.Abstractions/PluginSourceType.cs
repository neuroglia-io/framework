using System.ComponentModel;
using System.Runtime.Serialization;

namespace Neuroglia.Plugins;

/// <summary>
/// Enumerates supported plugin source types
/// </summary>
[TypeConverter(typeof(EnumMemberTypeConverter))]
public enum PluginSourceType
{
    /// <summary>
    /// Indicates a file system directory based plugin source
    /// </summary>
    [EnumMember(Value = "directory")]
    Directory,
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