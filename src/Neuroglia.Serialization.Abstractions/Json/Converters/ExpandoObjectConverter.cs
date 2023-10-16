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