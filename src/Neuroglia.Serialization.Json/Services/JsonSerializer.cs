using Microsoft.Extensions.Options;
using System.Net.Mime;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Neuroglia.Serialization.Json;

/// <summary>
/// Represents the System.Text.Json implementation of the <see cref="IJsonSerializer"/> interface
/// </summary>
public class JsonSerializer
    : IJsonSerializer
{

    /// <summary>
    /// Initializes a new <see cref="JsonSerializer"/>
    /// </summary>
    /// <param name="options">The service used to monitor the current <see cref="JsonSerializerOptions"/></param>
    public JsonSerializer(IOptionsMonitor<JsonSerializerOptions> options)
    {
        this.Options = options;
    }

    /// <summary>
    /// Gets the service used to monitor the current <see cref="JsonSerializerOptions"/>
    /// </summary>
    protected IOptionsMonitor<JsonSerializerOptions> Options { get; }

    /// <inheritdoc/>
    public virtual bool Supports(string mediaTypeName) => mediaTypeName == MediaTypeNames.Application.Json || mediaTypeName.EndsWith("+json");

    /// <inheritdoc/>
    public virtual void Serialize(object value, Stream stream, Type? type = null) => System.Text.Json.JsonSerializer.Serialize(stream, value, type ?? value.GetType(), this.Options.CurrentValue);

    /// <inheritdoc/>
    public virtual string SerializeToText(object value, Type? type = null) => System.Text.Json.JsonSerializer.Serialize(value, type ?? value.GetType(), this.Options.CurrentValue);

    /// <summary>
    /// Serializes the specified object into a new <see cref="JsonNode"/>
    /// </summary>
    /// <typeparam name="T">The type of object to serialize</typeparam>
    /// <param name="graph">The object to serialize</param>
    /// <returns>A new <see cref="JsonNode"/></returns>
    /// <inheritdoc/>
    public virtual JsonNode? SerializeToNode<T>(T graph) => System.Text.Json.JsonSerializer.SerializeToNode(graph, this.Options.CurrentValue);

    /// <summary>
    /// Serializes the specified object into a new <see cref="JsonElement"/>
    /// </summary>
    /// <typeparam name="T">The type of object to serialize</typeparam>
    /// <param name="graph">The object to serialize</param>
    /// <returns>A new <see cref="JsonElement"/></returns>
    /// <inheritdoc/>
    public virtual JsonElement? SerializeToElement<T>(T graph) => System.Text.Json.JsonSerializer.SerializeToElement(graph, this.Options.CurrentValue);

    /// <summary>
    /// Serializes the specified object into a new <see cref="JsonDocument"/>
    /// </summary>
    /// <typeparam name="T">The type of object to serialize</typeparam>
    /// <param name="graph">The object to serialize</param>
    /// <returns>A new <see cref="JsonDocument"/></returns>
    /// <inheritdoc/>
    public virtual JsonDocument? SerializeToDocument<T>(T graph) => System.Text.Json.JsonSerializer.SerializeToDocument(graph, this.Options.CurrentValue);

    /// <summary>
    /// Serializes an object to the specified <see cref="Stream"/>
    /// </summary>
    /// <typeparam name="T">The type of the object to serialize</typeparam>
    /// <param name="stream">The <see cref="Stream"/> to serialize the object to</param>
    /// <param name="graph">The object to serialize</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    /// <inheritdoc/>
    public virtual Task SerializeAsync<T>(Stream stream, T graph, CancellationToken cancellationToken = default) => System.Text.Json.JsonSerializer.SerializeAsync(stream, graph, this.Options.CurrentValue, cancellationToken);

    /// <summary>
    /// Serializes an object to the specified <see cref="Stream"/>
    /// </summary>
    /// <param name="stream">The <see cref="Stream"/> to serialize the object to</param>
    /// <param name="graph">The object to serialize</param>
    /// <param name="inputType">The type of the object to serialize</param>
    /// <param name="indented">A boolean indicating whether or not to indent the output</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    /// <inheritdoc/>
    public virtual Task SerializeAsync(Stream stream, object graph, Type inputType, CancellationToken cancellationToken = default) => System.Text.Json.JsonSerializer.SerializeAsync(stream, graph, inputType, this.Options.CurrentValue, cancellationToken);

    /// <inheritdoc/>
    public virtual object? Deserialize(string input, Type type) => System.Text.Json.JsonSerializer.Deserialize(input, type, this.Options.CurrentValue);

    /// <inheritdoc/>
    public virtual object? Deserialize(Stream stream, Type type) => System.Text.Json.JsonSerializer.Deserialize(stream, type, this.Options.CurrentValue);

    /// <summary>
    /// Deserializes the specified <see cref="JsonElement"/>
    /// </summary>
    /// <typeparam name="T">The type to deserialize the specified <see cref="JsonElement"/> into</typeparam>
    /// <param name="element">The <see cref="JsonElement"/> to deserialize</param>
    /// <returns>The deserialized value</returns>
    public virtual T? Deserialize<T>(JsonElement element) => System.Text.Json.JsonSerializer.Deserialize<T>(element, this.Options.CurrentValue);

    /// <summary>
    /// Deserializes the specified JSON input
    /// </summary>
    /// <typeparam name="T">The type to deserialize the specified JSON into</typeparam>
    /// <param name="json">The JSON input to deserialize</param>
    /// <returns>The deserialized value</returns>
    public virtual T? Deserialize<T>(string json) => System.Text.Json.JsonSerializer.Deserialize<T>(json, this.Options.CurrentValue);

    /// <summary>
    /// Deserializes the specified <see cref="JsonNode"/>
    /// </summary>
    /// <typeparam name="T">The type to deserialize the specified <see cref="JsonNode"/> into</typeparam>
    /// <param name="node">The <see cref="JsonNode"/> to deserialize</param>
    /// <returns>The deserialized value</returns>
    public virtual T? Deserialize<T>(JsonNode node) => System.Text.Json.JsonSerializer.Deserialize<T>(node, this.Options.CurrentValue);

    /// <summary>
    /// Deserializes the specified JSON input
    /// </summary>
    /// <typeparam name="T">The type to deserialize the specified JSON into</typeparam>
    /// <param name="buffer">The JSON input to deserialize</param>
    /// <returns>The deserialized value</returns>
    public virtual T? Deserialize<T>(ReadOnlySpan<byte> buffer) => System.Text.Json.JsonSerializer.Deserialize<T>(buffer, this.Options.CurrentValue);

    /// <summary>
    /// Deserializes the specified <see cref="Stream"/> as a new <see cref="IAsyncEnumerable{T}"/>
    /// </summary>
    /// <typeparam name="T">The expected type of elements to enumerate</typeparam>
    /// <param name="stream">The <see cref="Stream"/> to deserialize</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="IAsyncEnumerable{T}"/></returns>
    public virtual IAsyncEnumerable<T?> DeserializeAsyncEnumerable<T>(Stream stream, CancellationToken cancellationToken = default) => System.Text.Json.JsonSerializer.DeserializeAsyncEnumerable<T>(stream, this.Options.CurrentValue, cancellationToken);

}
