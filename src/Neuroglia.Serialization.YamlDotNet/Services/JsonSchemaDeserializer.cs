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

using Json.Schema;
using System.Text.Json.Nodes;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace Neuroglia.Serialization.Yaml;

/// <summary>
/// Represents the <see cref="INodeDeserializer"/> used to deserialize <see cref="JsonSchema"/>s
/// </summary>
/// <remarks>
/// Initializes a new <see cref="JsonSchemaDeserializer"/>
/// </remarks>
/// <param name="inner">The inner <see cref="INodeDeserializer"/></param>
public class JsonSchemaDeserializer(INodeDeserializer inner)
        : INodeDeserializer
{

    /// <summary>
    /// Gets the inner <see cref="INodeDeserializer"/>
    /// </summary>
    protected INodeDeserializer Inner { get; } = inner;

    /// <inheritdoc/>
    public virtual bool Deserialize(IParser reader, Type expectedType, Func<IParser, Type, object?> nestedObjectDeserializer, out object? value)
    {
        if (!typeof(JsonSchema).IsAssignableFrom(expectedType)) return this.Inner.Deserialize(reader, expectedType, nestedObjectDeserializer, out value!);
        if (!this.Inner.Deserialize(reader, typeof(JsonObject), nestedObjectDeserializer, out value!)) return false;
        var jsonObject = (JsonObject)value;
        var jsonSchema = Json.JsonSerializer.Default.Deserialize<JsonSchema>(jsonObject)!;
        value = jsonSchema;
        return true;
    }

}