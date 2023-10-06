using Microsoft.Extensions.Options;
using System.Net.Mime;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Neuroglia.Serialization.Json;

/// <summary>
/// Represents the System.Text.Json implementation of the <see cref="IJsonSerializer"/> interface
/// </summary>
public class JsonSerializer
    : IJsonSerializer, IAsyncSerializer
{

    /// <summary>
    /// Gets/sets an <see cref="Action{T}"/> used to configure the <see cref="JsonSerializerOptions"/> used by default
    /// </summary>
    public static Action<JsonSerializerOptions>? DefaultOptionsConfiguration { get; set; } = (options) =>
    {
        options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
        options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    };

    static JsonSerializerOptions? _defaultOptions;
    /// <summary>
    /// Gets/sets the default <see cref="JsonSerializerOptions"/>
    /// </summary>
    public static JsonSerializerOptions DefaultOptions
    {
        get
        {
            if (_defaultOptions != null) return _defaultOptions;
            _defaultOptions = new JsonSerializerOptions();
            DefaultOptionsConfiguration?.Invoke(_defaultOptions);
            return _defaultOptions;
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
    /// Initializes a new <see cref="JsonSerializer"/>
    /// </summary>
    /// <param name="options">The current <see cref="JsonSerializerOptions"/></param>
    public JsonSerializer(IOptions<JsonSerializerOptions> options)
    {
        this.Options = options.Value;
    }

    /// <summary>
    /// Gets the current <see cref="JsonSerializerOptions"/>
    /// </summary>
    protected JsonSerializerOptions Options { get; }

    /// <inheritdoc/>
    public virtual bool Supports(string mediaTypeName) => mediaTypeName == MediaTypeNames.Application.Json || mediaTypeName.EndsWith("+json");

    /// <inheritdoc/>
    public virtual void Serialize(object value, Stream stream, Type? type = null) => System.Text.Json.JsonSerializer.Serialize(stream, value, type ?? value.GetType(), this.Options);

    /// <inheritdoc/>
    public virtual string SerializeToText(object value, Type? type = null) => System.Text.Json.JsonSerializer.Serialize(value, type ?? value.GetType(), this.Options);

    /// <summary>
    /// Serializes the specified object into a new <see cref="JsonNode"/>
    /// </summary>
    /// <typeparam name="T">The type of object to serialize</typeparam>
    /// <param name="graph">The object to serialize</param>
    /// <returns>A new <see cref="JsonNode"/></returns>
    /// <inheritdoc/>
    public virtual JsonNode? SerializeToNode<T>(T graph) => System.Text.Json.JsonSerializer.SerializeToNode(graph, this.Options);

    /// <summary>
    /// Serializes the specified object into a new <see cref="JsonElement"/>
    /// </summary>
    /// <typeparam name="T">The type of object to serialize</typeparam>
    /// <param name="graph">The object to serialize</param>
    /// <returns>A new <see cref="JsonElement"/></returns>
    /// <inheritdoc/>
    public virtual JsonElement? SerializeToElement<T>(T graph) => System.Text.Json.JsonSerializer.SerializeToElement(graph, this.Options);

    /// <summary>
    /// Serializes the specified object into a new <see cref="JsonDocument"/>
    /// </summary>
    /// <typeparam name="T">The type of object to serialize</typeparam>
    /// <param name="graph">The object to serialize</param>
    /// <returns>A new <see cref="JsonDocument"/></returns>
    /// <inheritdoc/>
    public virtual JsonDocument? SerializeToDocument<T>(T graph) => System.Text.Json.JsonSerializer.SerializeToDocument(graph, this.Options);

    /// <summary>
    /// Serializes an object to the specified <see cref="Stream"/>
    /// </summary>
    /// <typeparam name="T">The type of the object to serialize</typeparam>
    /// <param name="stream">The <see cref="Stream"/> to serialize the object to</param>
    /// <param name="graph">The object to serialize</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    /// <inheritdoc/>
    public virtual Task SerializeAsync<T>(Stream stream, T graph, CancellationToken cancellationToken = default) => System.Text.Json.JsonSerializer.SerializeAsync(stream, graph, this.Options, cancellationToken);

    /// <inheritdoc/>
    public virtual Task SerializeAsync(Stream stream, object graph, Type type, CancellationToken cancellationToken = default) => System.Text.Json.JsonSerializer.SerializeAsync(stream, graph, type, this.Options, cancellationToken);

    /// <inheritdoc/>
    public virtual object? Deserialize(string input, Type type) => System.Text.Json.JsonSerializer.Deserialize(input, type, this.Options);

    /// <inheritdoc/>
    public virtual object? Deserialize(Stream stream, Type type) => System.Text.Json.JsonSerializer.Deserialize(stream, type, this.Options);

    /// <summary>
    /// Deserializes the specified <see cref="JsonElement"/>
    /// </summary>
    /// <typeparam name="T">The type to deserialize the specified <see cref="JsonElement"/> into</typeparam>
    /// <param name="element">The <see cref="JsonElement"/> to deserialize</param>
    /// <returns>The deserialized value</returns>
    public virtual T? Deserialize<T>(JsonElement element) => System.Text.Json.JsonSerializer.Deserialize<T>(element, this.Options);

    /// <summary>
    /// Deserializes the specified JSON input
    /// </summary>
    /// <typeparam name="T">The type to deserialize the specified JSON into</typeparam>
    /// <param name="json">The JSON input to deserialize</param>
    /// <returns>The deserialized value</returns>
    public virtual T? Deserialize<T>(string json) => System.Text.Json.JsonSerializer.Deserialize<T>(json, this.Options);

    /// <summary>
    /// Deserializes the specified <see cref="JsonNode"/>
    /// </summary>
    /// <typeparam name="T">The type to deserialize the specified <see cref="JsonNode"/> into</typeparam>
    /// <param name="node">The <see cref="JsonNode"/> to deserialize</param>
    /// <returns>The deserialized value</returns>
    public virtual T? Deserialize<T>(JsonNode node) => System.Text.Json.JsonSerializer.Deserialize<T>(node, this.Options);

    /// <summary>
    /// Deserializes the specified JSON input
    /// </summary>
    /// <typeparam name="T">The type to deserialize the specified JSON into</typeparam>
    /// <param name="buffer">The JSON input to deserialize</param>
    /// <returns>The deserialized value</returns>
    public virtual T? Deserialize<T>(ReadOnlySpan<byte> buffer) => System.Text.Json.JsonSerializer.Deserialize<T>(buffer, this.Options);

    /// <inheritdoc/>
    public virtual async Task<object?> DeserializeAsync(Stream stream, Type type, CancellationToken cancellationToken = default) => await System.Text.Json.JsonSerializer.DeserializeAsync(stream, type, this.Options, cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Deserializes the specified <see cref="Stream"/> as a new <see cref="IAsyncEnumerable{T}"/>
    /// </summary>
    /// <typeparam name="T">The expected type of elements to enumerate</typeparam>
    /// <param name="stream">The <see cref="Stream"/> to deserialize</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="IAsyncEnumerable{T}"/></returns>
    public virtual IAsyncEnumerable<T?> DeserializeAsyncEnumerable<T>(Stream stream, CancellationToken cancellationToken = default) => System.Text.Json.JsonSerializer.DeserializeAsyncEnumerable<T>(stream, this.Options, cancellationToken);

}
