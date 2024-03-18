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

using System.Text.Json.Nodes;
using System.Text.Json;

namespace Neuroglia.Data.Infrastructure.ResourceOriented;

/// <summary>
/// Defines extensions for <see cref="object"/>s
/// </summary>
public static class ObjectExtensions
{

    /// <summary>
    /// Clones the object
    /// </summary>
    /// <param name="obj">The object to clone</param>
    /// <returns>The clone</returns>
    public static object? Clone(this object? obj) => Serialization.Json.JsonSerializer.Default.Deserialize<object>(Serialization.Json.JsonSerializer.Default.SerializeToText(obj));

    /// <summary>
    /// Clones the object
    /// </summary>
    /// <typeparam name="T">The type of the object to clone</typeparam>
    /// <param name="obj">The object to clone</param>
    /// <returns>The clone</returns>
    public static T? Clone<T>(this T? obj) => Serialization.Json.JsonSerializer.Default.Deserialize<T>(Serialization.Json.JsonSerializer.Default.SerializeToText(obj));

    /// <summary>
    /// Converts the object into a new <see cref="Dictionary{TKey, TValue}"/>
    /// </summary>
    /// <param name="obj">The object to convert</param>
    /// <returns>A new <see cref="Dictionary{TKey, TValue}"/></returns>
    public static Dictionary<string, object>? ToDictionary(this object? obj) => Serialization.Json.JsonSerializer.Default.Deserialize<Dictionary<string, object>>(Serialization.Json.JsonSerializer.Default.SerializeToText(obj));

    /// <summary>
    /// Converts the object into a new <see cref="Dictionary{TKey, TValue}"/>
    /// </summary>
    /// <param name="obj">The object to convert</param>
    /// <returns>A new <see cref="Dictionary{TKey, TValue}"/></returns>
    public static Dictionary<string, TValue>? ToDictionary<TValue>(this object? obj) => Serialization.Json.JsonSerializer.Default.Deserialize<Dictionary<string, TValue>>(Serialization.Json.JsonSerializer.Default.SerializeToText(obj));

    /// <summary>
    /// Converts the object to the specified type by using JSON serdes
    /// </summary>
    /// <typeparam name="T">The type to convert the object to</typeparam>
    /// <param name="obj">The object to convert</param>
    /// <returns>The converted object</returns>
    public static T? ConvertTo<T>(this object? obj)
    {
        if (obj == null) return default;
        return obj switch
        {
            T t => t,
            JsonElement jsonElem => Serialization.Json.JsonSerializer.Default.Deserialize<T>(jsonElem),
            JsonNode jsonNode => Serialization.Json.JsonSerializer.Default.Deserialize<T>(jsonNode),
            _ => Serialization.Json.JsonSerializer.Default.Deserialize<T>(Serialization.Json.JsonSerializer.Default.SerializeToText(obj))
        };
    }

}