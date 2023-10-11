using System.ComponentModel;
using System.Dynamic;

namespace Neuroglia;

/// <summary>
/// Defines extensions for <see cref="object"/>s
/// </summary>
public static class ObjectExtensions
{

    /// <summary>
    /// Creates a new <see cref="IDictionary{TKey, TValue}"/> representing a name/value mapping of the object's properties
    /// </summary>
    /// <param name="source">The source object</param>
    /// <returns>A new <see cref="IDictionary{TKey, TValue}"/> representing a name/value mapping of the object's properties</returns>
    public static IDictionary<string, object>? ToDictionary(this object? source) => source == null ? null : source is IDictionary<string, object> dictionary ? dictionary : TypeDescriptor.GetProperties(source).OfType<PropertyDescriptor>().ToDictionary(p => p.Name, p => p.GetValue(source)!);

    /// <summary>
    /// Converts the object into a new <see cref="ExpandoObject"/>
    /// </summary>
    /// <param name="source">The object to convert</param>
    /// <returns>A new <see cref="ExpandoObject"/></returns>
    public static ExpandoObject? ToExpandoObject(this object? source)
    {
        if (source == null) return null;
        if (source is ExpandoObject expando) return expando;
        expando = new ExpandoObject();
        var inputProperties = source.ToDictionary()!;
        var outputProperties = expando as IDictionary<string, object>;
        
        foreach(var kvp in inputProperties) outputProperties[kvp.Key] = kvp.Value;

        return expando;
    }

}