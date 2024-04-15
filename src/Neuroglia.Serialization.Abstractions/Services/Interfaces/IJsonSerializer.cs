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

namespace Neuroglia.Serialization;

/// <summary>
/// Defines the fundamentals of a service used to serialize and deserialize JSON
/// </summary>
public interface IJsonSerializer
    : ITextSerializer
{

    /// <summary>
    /// Serializes the specified object into a new <see cref="JsonNode"/>
    /// </summary>
    /// <typeparam name="T">The type of object to serialize</typeparam>
    /// <param name="graph">The object to serialize</param>
    /// <returns>A new <see cref="JsonNode"/></returns>
    JsonNode? SerializeToNode<T>(T graph);

    /// <summary>
    /// Serializes the specified object into a new <see cref="JsonElement"/>
    /// </summary>
    /// <typeparam name="T">The type of object to serialize</typeparam>
    /// <param name="graph">The object to serialize</param>
    /// <returns>A new <see cref="JsonElement"/></returns>
    JsonElement? SerializeToElement<T>(T graph);

    /// <summary>
    /// Serializes the specified object into a new <see cref="JsonDocument"/>
    /// </summary>
    /// <typeparam name="T">The type of object to serialize</typeparam>
    /// <param name="graph">The object to serialize</param>
    /// <returns>A new <see cref="JsonDocument"/></returns>
    JsonDocument? SerializeToDocument<T>(T graph);

    /// <summary>
    /// Deserializes a value from its JSON representation
    /// </summary>
    /// <typeparam name="T">The value's expected type</typeparam>
    /// <param name="json">The JSON representation of the value to deserialize</param>
    /// <returns>The deserialized value, if any</returns>
    T? Deserialize<T>(string json);

    /// <summary>
    /// Deserializes a <see cref="JsonElement"/> into a new instance of the specified type
    /// </summary>
    /// <param name="element">The <see cref="JsonElement"/> to deserialize</param>
    /// <param name="type">The type to deserialize the <see cref="JsonElement"/> to</param>
    /// <returns>The deserialized <see cref="JsonElement"/></returns>
    object? Deserialize(JsonElement element, Type type);

    /// <summary>
    /// Deserializes a <see cref="JsonElement"/> into a new instance of the specified type
    /// </summary>
    /// <typeparam name="T">The type to deserialize the <see cref="JsonElement"/> to</typeparam>
    /// <param name="element">The <see cref="JsonElement"/> to deserialize</param>
    /// <returns>The deserialized <see cref="JsonElement"/></returns>
    T? Deserialize<T>(JsonElement element);

    /// <summary>
    /// Deserializes a <see cref="JsonNode"/> into a new instance of the specified type
    /// </summary>
    /// <param name="node">The <see cref="JsonNode"/> to deserialize</param>
    /// <param name="type">The type to deserialize the <see cref="JsonNode"/> to</param>
    /// <returns>The deserialized <see cref="JsonNode"/></returns>
    object? Deserialize(JsonNode node, Type type);

    /// <summary>
    /// Deserializes a <see cref="JsonNode"/> into a new instance of the specified type
    /// </summary>
    /// <typeparam name="T">The type to deserialize the <see cref="JsonNode"/> to</typeparam>
    /// <param name="node">The <see cref="JsonNode"/> to deserialize</param>
    /// <returns>The deserialized <see cref="JsonNode"/></returns>
    T? Deserialize<T>(JsonNode node);

    /// <summary>
    /// Deserializes the specified <see cref="Stream"/> as a new <see cref="IAsyncEnumerable{T}"/>
    /// </summary>
    /// <typeparam name="T">The expected type of elements to enumerate</typeparam>
    /// <param name="stream">The <see cref="Stream"/> to deserialize</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="IAsyncEnumerable{T}"/></returns>
    IAsyncEnumerable<T?> DeserializeAsyncEnumerable<T>(Stream stream, CancellationToken cancellationToken = default);

}
