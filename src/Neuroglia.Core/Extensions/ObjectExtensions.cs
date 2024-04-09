// Copyright © 2021-Present Neuroglia SRL. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License"),
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

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

    /// <summary>
    /// Gets the value returned by the specified property
    /// </summary>
    /// <param name="source">The extended object</param>
    /// <param name="name">The name of the property to get</param>
    /// <returns>The value of the specified property</returns>
    /// <remarks>This method is used to dynamically get the property of an object, specifically when building an expression, which does not allow dynamic operations</remarks>
    public static object GetProperty(this object source, string name)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        var property = source.GetType().GetProperty(name) ?? throw new MissingMemberException($"Failed to find a property with the specified name '{name}'", name);
        return property.GetValue(source)!;
    }

    /// <summary>
    /// Gets the value returned by the specified property
    /// </summary>
    /// <typeparam name="T">The type of the property to get</typeparam>
    /// <param name="source">The extended object</param>
    /// <param name="name">The name of the property to get</param>
    /// <returns>The value of the specified property</returns>
    /// <remarks>This method is used to dynamically get the property of an object, specifically when building an expression, which does not allow dynamic operations</remarks>
    public static T GetProperty<T>(this object source, string name) => (T)source.GetProperty(name);

}