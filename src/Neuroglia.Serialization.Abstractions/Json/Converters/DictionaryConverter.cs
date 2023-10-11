using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Neuroglia.Serialization.Json.Converters;

/// <summary>
/// Represents the <see cref="JsonConverter"/> used to serialize and deserialize <see cref="IDictionary{TKey, TValue}"/> instances, and unwraps their values (as opposed to keeping JsonElement values)
/// </summary>
public class DictionaryConverter
    : JsonConverter<object>
{

    /// <inheritdoc/>
    public override bool CanConvert(Type typeToConvert) => typeToConvert == typeof(object);

    /// <inheritdoc/>
    public override object? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => JsonNode.Parse(ref reader)?.ToObject();

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
    {
        var serializerOptions = new JsonSerializerOptions(options);
        serializerOptions.Converters.OfType<DictionaryConverter>().ToList().ForEach(c => serializerOptions.Converters.Remove(c));
        System.Text.Json.JsonSerializer.Serialize(writer, value, serializerOptions);
    }

}
