﻿using System.Dynamic;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Neuroglia.Serialization;

/// <summary>
/// Defines extensions for <see cref="JsonNode"/>s
/// </summary>
public static class JsonNodeExtensions
{

    /// <summary>
    /// Unwraps and deserializes and  the <see cref="JsonNode"/>
    /// </summary>
    /// <param name="jsonNode">The <see cref="JsonNode"/> to unwrap</param>
    /// <returns>The unwrapped <see cref="JsonNode"/></returns>
    public static object? ToObject(this JsonNode jsonNode)
    {
        return jsonNode switch
        {
            JsonArray array => array.ToObject(),
            JsonObject obj => obj.ToObject(),
            JsonValue value => value.ToObject(),
            _ => throw new NotSupportedException($"The specified JsonNode type '{jsonNode.GetType()}' is not supported")
        };
    }

    /// <summary>
    /// Unwraps and deserializes and  the <see cref="JsonArray"/>
    /// </summary>
    /// <param name="jsonArray">The <see cref="JsonArray"/> to unwrap</param>
    /// <returns>The unwrapped <see cref="JsonArray"/></returns>
    public static IEnumerable<object>? ToObject(this JsonArray jsonArray)
    {
        foreach (var jsonNode in jsonArray)
        {
            if (jsonNode == null) yield return null!;
            else yield return jsonNode!.ToObject()!;
        }
    }

    /// <summary>
    /// Unwraps and deserializes and  the <see cref="JsonObject"/>
    /// </summary>
    /// <param name="jsonObject">The <see cref="JsonObject"/> to unwrap</param>
    /// <returns>The unwrapped <see cref="JsonObject"/></returns>
    public static object? ToObject(this JsonObject jsonObject)
    {
        var expandoObject = new ExpandoObject();
        foreach (var property in jsonObject)
        {
            ((IDictionary<string, object>)expandoObject!).Add(property.Key, property.Value?.ToObject()!);
        }
        return expandoObject;
    }

    /// <summary>
    /// Unwraps and deserializes and  the <see cref="JsonValue"/>
    /// </summary>
    /// <param name="jsonValue">The <see cref="JsonValue"/> to unwrap</param>
    /// <returns>The unwrapped <see cref="JsonValue"/></returns>
    public static object? ToObject(this JsonValue jsonValue)
    {
        var jsonElement = jsonValue.Deserialize<JsonElement>();
        var json = jsonElement.ToString();
        return jsonElement.ValueKind switch
        {
            JsonValueKind.Undefined or JsonValueKind.Null => null,
            JsonValueKind.String => jsonElement.Deserialize<string>(),
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.Number => json.Contains('.') || json.Contains(',') ? jsonElement.Deserialize<decimal>() : jsonElement.Deserialize<long>(),
            _ => throw new NotSupportedException($"The specified {nameof(JsonValueKind)} '{jsonElement.ValueKind}' is not supported")
        };
    }

}