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

using Microsoft.Extensions.Options;
using Neuroglia.Serialization.Json.Converters;
using System.Net.Mime;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Neuroglia.Serialization.Json;

/// <summary>
/// Represents the System.Text.Json implementation of the <see cref="IJsonSerializer"/> interface
/// </summary>
/// <remarks>
/// Initializes a new <see cref="JsonSerializer"/>
/// </remarks>
/// <param name="options">The current <see cref="JsonSerializerOptions"/></param>
public class JsonSerializer(IOptions<JsonSerializerOptions> options)
    : IJsonSerializer, IAsyncSerializer
{

    static readonly object _lock = new();

    /// <summary>
    /// Gets/sets an <see cref="Action{T}"/> used to configure the <see cref="JsonSerializerOptions"/> used by default
    /// </summary>
    public static Action<JsonSerializerOptions> DefaultOptionsConfiguration { get; set; } = (options) =>
    {
        options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
        options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.TypeInfoResolver = new InheritancePriorityJsonTypeInfoResolver();
        options.Converters.Add(new DictionaryConverter());
        options.Converters.Add(new ExpandoObjectConverter());
    };

    static JsonSerializerOptions? _defaultOptions;
    /// <summary>
    /// Gets/sets the default <see cref="JsonSerializerOptions"/>
    /// </summary>
    public static JsonSerializerOptions DefaultOptions
    {
        get
        {
            lock (_lock)
            {
                if (_defaultOptions != null) return _defaultOptions;
                _defaultOptions = new JsonSerializerOptions();
                DefaultOptionsConfiguration?.Invoke(_defaultOptions);
                return _defaultOptions;
            }
        }
    }

    static JsonSerializer? _default;
    /// <summary>
    /// Gets the default, globally accessible <see cref="JsonSerializer"/>
    /// </summary>
    public static JsonSerializer Default
    {
        get
        {
            _default ??= new(Microsoft.Extensions.Options.Options.Create(DefaultOptions));
            return _default;
        }
    }

    /// <summary>
    /// Gets the current <see cref="JsonSerializerOptions"/>
    /// </summary>
    protected JsonSerializerOptions Options { get; } = options.Value;
    /// <inheritdoc/>
    public virtual bool Supports(string mediaTypeName) => mediaTypeName == MediaTypeNames.Application.Json || mediaTypeName.EndsWith("+json");

    /// <inheritdoc/>
    public virtual void Serialize(object? value, Stream stream, Type? type = null) => System.Text.Json.JsonSerializer.Serialize(stream, value, type ?? value?.GetType()!, this.Options);

    /// <inheritdoc/>
    public virtual string SerializeToText(object? value, Type? type = null) => System.Text.Json.JsonSerializer.Serialize(value, type ?? value?.GetType()!, this.Options);

    /// <inheritdoc/>
    public virtual JsonNode? SerializeToNode<T>(T graph) => System.Text.Json.JsonSerializer.SerializeToNode(graph, this.Options);

    /// <inheritdoc/>
    public virtual JsonElement? SerializeToElement<T>(T graph) => System.Text.Json.JsonSerializer.SerializeToElement(graph, this.Options);

    /// <inheritdoc/>
    public virtual JsonDocument? SerializeToDocument<T>(T graph) => System.Text.Json.JsonSerializer.SerializeToDocument(graph, this.Options);

    /// <inheritdoc/>
    public virtual Task SerializeAsync<T>(Stream stream, T graph, CancellationToken cancellationToken = default) => System.Text.Json.JsonSerializer.SerializeAsync(stream, graph, this.Options, cancellationToken);

    /// <inheritdoc/>
    public virtual Task SerializeAsync(Stream stream, object graph, Type type, CancellationToken cancellationToken = default) => System.Text.Json.JsonSerializer.SerializeAsync(stream, graph, type, this.Options, cancellationToken);

    /// <inheritdoc/>
    public virtual object? Deserialize(string json, Type type) => System.Text.Json.JsonSerializer.Deserialize(json, type, this.Options);

    /// <inheritdoc/>
    public virtual object? Deserialize(Stream stream, Type type) => System.Text.Json.JsonSerializer.Deserialize(stream, type, this.Options);

    /// <inheritdoc/>
    public virtual object? Deserialize(JsonElement element, Type type) => System.Text.Json.JsonSerializer.Deserialize(element, type, this.Options);

    /// <inheritdoc/>
    public virtual T? Deserialize<T>(JsonElement element) => System.Text.Json.JsonSerializer.Deserialize<T>(element, this.Options);

    /// <inheritdoc/>
    public virtual T? Deserialize<T>(string json) => System.Text.Json.JsonSerializer.Deserialize<T>(json, this.Options);

    /// <inheritdoc/>
    public virtual object? Deserialize(JsonNode node, Type type) => System.Text.Json.JsonSerializer.Deserialize(node, type, this.Options);

    /// <inheritdoc/>
    public virtual T? Deserialize<T>(JsonNode node) => System.Text.Json.JsonSerializer.Deserialize<T>(node, this.Options);

    /// <summary>
    /// Deserializes the specified JSON input
    /// </summary>
    /// <param name="buffer">The JSON input to deserialize</param>
    /// <param name="type">The type to deserialize the specified JSON into</param>
    /// <returns>The deserialized value</returns>
    public virtual object? Deserialize(ReadOnlySpan<byte> buffer, Type type) => System.Text.Json.JsonSerializer.Deserialize(buffer, type, this.Options);

    /// <summary>
    /// Deserializes the specified JSON input
    /// </summary>
    /// <typeparam name="T">The type to deserialize the specified JSON into</typeparam>
    /// <param name="buffer">The JSON input to deserialize</param>
    /// <returns>The deserialized value</returns>
    public virtual T? Deserialize<T>(ReadOnlySpan<byte> buffer) => System.Text.Json.JsonSerializer.Deserialize<T>(buffer, this.Options);

    /// <inheritdoc/>
    public virtual async Task<object?> DeserializeAsync(Stream stream, Type type, CancellationToken cancellationToken = default) => await System.Text.Json.JsonSerializer.DeserializeAsync(stream, type, this.Options, cancellationToken).ConfigureAwait(false);

    /// <inheritdoc/>
    public virtual IAsyncEnumerable<T?> DeserializeAsyncEnumerable<T>(Stream stream, CancellationToken cancellationToken = default) => System.Text.Json.JsonSerializer.DeserializeAsyncEnumerable<T>(stream, this.Options, cancellationToken);

}
