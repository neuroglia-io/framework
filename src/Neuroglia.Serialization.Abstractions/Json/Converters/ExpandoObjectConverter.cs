using System.Text.Json.Serialization;
using System.Text.Json;
using System.Dynamic;
using System.Text.Json.Nodes;

namespace Neuroglia.Serialization.Json.Converters;

/// <summary>
/// Represents the <see cref="JsonConverter"/> used to serialize and deserialize <see cref="ExpandoObject"/>s
/// </summary>
public class ExpandoObjectConverter
    : JsonConverter<ExpandoObject>
{

    /// <inheritdoc/>
    public override bool CanConvert(Type typeToConvert) => typeToConvert == typeof(ExpandoObject);

    /// <inheritdoc/>
    public override ExpandoObject? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var json = JsonNode.Parse(ref reader)?.ToJsonString(options)!;
        var inputProperties = System.Text.Json.JsonSerializer.Deserialize<IDictionary<string, object>?>(json, options) ?? null;
        if (inputProperties == null) return null;

        var expando = new ExpandoObject();
        var outputProperties = expando as IDictionary<string, object>;
        foreach (var kvp in inputProperties) outputProperties[kvp.Key] = kvp.Value;

        return expando;
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, ExpandoObject value, JsonSerializerOptions options) => System.Text.Json.JsonSerializer.Serialize(writer, value);

}