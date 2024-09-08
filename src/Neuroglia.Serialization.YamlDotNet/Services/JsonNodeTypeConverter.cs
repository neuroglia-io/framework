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
using YamlDotNet.Core.Events;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Neuroglia.Serialization.Yaml;

/// <summary>
/// Represents the <see cref="IYamlTypeConverter"/> used to convert <see cref="JsonNode"/>s
/// </summary>
/// <param name="preservePropertyNameCase">A boolean indicating whether or not to preserve the case of the name of converted properties</param>
public class JsonNodeTypeConverter(bool preservePropertyNameCase = false)
    : IYamlTypeConverter
{

    /// <summary>
    /// Gets a boolean indicating whether or not to preserve the case of the name of converted properties
    /// </summary>
    protected bool PreservePropertyNameCase { get; } = preservePropertyNameCase;

    /// <inheritdoc/>
    public virtual bool Accepts(Type type) => typeof(JsonElement).IsAssignableFrom(type) || typeof(JsonNode).IsAssignableFrom(type);

    /// <inheritdoc/>
    public virtual object? ReadYaml(IParser parser, Type type, ObjectDeserializer rootDeserializer) => throw new NotSupportedException();

    /// <inheritdoc/>
    public virtual void WriteYaml(IEmitter emitter, object? value, Type type, ObjectSerializer rootSerializer)
    {
        if (value is JsonElement jsonElement) value = Json.JsonSerializer.Default.SerializeToNode(jsonElement);
        this.WriteJsonNode(emitter, value as JsonNode);
    }

    /// <summary>
    /// Write the specified <see cref="JsonNode"/>
    /// </summary>
    /// <param name="emitter">The <see cref="IEmitter"/> to use</param>
    /// <param name="jsonNode">The <see cref="JsonNode"/> to write</param>
    protected virtual void WriteJsonNode(IEmitter emitter, JsonNode? jsonNode)
    {
        if (jsonNode == null) return;
        switch (jsonNode)
        {
            case JsonArray jsonArray:
                this.WriteJsonArray(emitter, jsonArray);
                break;
            case JsonObject jsonObject:
                this.WriteJsonObject(emitter, jsonObject);
                break;
            case JsonValue jsonValue:
                this.WriteJsonValue(emitter, jsonValue);
                break;
            default:
                throw new NotSupportedException();
        }
    }

    /// <summary>
    /// Write the specified <see cref="JsonArray"/>
    /// </summary>
    /// <param name="emitter">The <see cref="IEmitter"/> to use</param>
    /// <param name="jsonArray">The <see cref="JsonArray"/> to write</param>
    protected virtual void WriteJsonArray(IEmitter emitter, JsonArray? jsonArray)
    {
        if (jsonArray == null) return;
        emitter.Emit(new SequenceStart(null, null, false, SequenceStyle.Block));
        foreach (var jsonNode in jsonArray)
        {
            this.WriteJsonNode(emitter, jsonNode);
        }
        emitter.Emit(new SequenceEnd());
    }

    /// <summary>
    /// Write the specified <see cref="JsonObject"/>
    /// </summary>
    /// <param name="emitter">The <see cref="IEmitter"/> to use</param>
    /// <param name="jsonObject">The <see cref="JsonObject"/> to write</param>
    protected virtual void WriteJsonObject(IEmitter emitter, JsonObject? jsonObject)
    {
        if (jsonObject == null) return;
        emitter.Emit(new MappingStart(null, null, false, MappingStyle.Block));
        foreach (var property in jsonObject)
        {
            this.WriteJsonObjectProperty(emitter, property);
        }
        emitter.Emit(new MappingEnd());
    }

    /// <summary>
    /// Write the specified JSON property
    /// </summary>
    /// <param name="emitter">The <see cref="IEmitter"/> to use</param>
    /// <param name="jsonProperty">The JSON property to write</param>
    protected virtual void WriteJsonObjectProperty(IEmitter emitter, KeyValuePair<string, JsonNode?> jsonProperty)
    {
        if (jsonProperty.Value == null) return;
        emitter.Emit(new Scalar(this.PreservePropertyNameCase ? jsonProperty.Key : CamelCaseNamingConvention.Instance.Apply(jsonProperty.Key)));
        this.WriteJsonNode(emitter, jsonProperty.Value);
    }

    /// <summary>
    /// Write the specified <see cref="JsonValue"/>
    /// </summary>
    /// <param name="emitter">The <see cref="IEmitter"/> to use</param>
    /// <param name="jsonValue">The <see cref="JsonValue"/> to write</param>
    protected virtual void WriteJsonValue(IEmitter emitter, JsonValue? jsonValue)
    {
        if (jsonValue == null) return;
        var value = jsonValue.Deserialize<object>()?.ToString();
        if (string.IsNullOrWhiteSpace(value)) return;
        emitter.Emit(new Scalar(value));
    }

}