using System.Text.Json.Serialization;

namespace Neuroglia;

/// <summary>
/// Defines the fundamentals of an object that supports extension data
/// </summary>
public interface IExtensible
{

    /// <summary>
    /// Gets an name/value mapping, if any, of the object extension data properties
    /// </summary>
    [JsonExtensionData]
    IDictionary<string, object>? ExtensionData { get; }

}