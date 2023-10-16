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
