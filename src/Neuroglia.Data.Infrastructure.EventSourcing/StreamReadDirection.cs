using Neuroglia.Serialization.Services;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Neuroglia.Data.Infrastructure.EventSourcing;

/// <summary>
/// Enumerates all supported read directions for streams
/// </summary>
[TypeConverter(typeof(EnumMemberTypeConverter))]
[JsonConverter(typeof(StringEnumConverter))]
public enum StreamReadDirection
{
    /// <summary>
    /// Specifies a forward direction
    /// </summary>
    [EnumMember(Value = "forwards")]
    Forwards,
    /// <summary>
    /// Specifies a backward direction
    /// </summary>
    [EnumMember(Value = "backwards")]
    Backwards
}
