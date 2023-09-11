using System.Text;
using YamlDotNet.Serialization;

namespace Neuroglia.Serialization.Yaml;

/// <summary>
/// Represents the <see href="YamlDotNet">https://github.com/aaubry/YamlDotNet</see> implementation of the <see cref="IYamlSerializer"/>
/// </summary>
public class YamlDotNetSerializer
    : IYamlSerializer
{

    /// <summary>
    /// Initializes a new <see cref="YamlDotNetSerializer"/>
    /// </summary>
    /// <param name="serializer">The underlying <see cref="YamlDotNet.Serialization.ISerializer"/></param>
    /// <param name="deserializer">The underlying <see cref="IDeserializer"/></param>
    public YamlDotNetSerializer(YamlDotNet.Serialization.ISerializer serializer, IDeserializer deserializer)
    {
        this.Serializer = serializer;
        this.Deserializer = deserializer;
    }

    /// <summary>
    /// Gets the underlying <see cref="YamlDotNet.Serialization.ISerializer"/>
    /// </summary>
    protected YamlDotNet.Serialization.ISerializer Serializer { get; }

    /// <summary>
    /// Gets the underlying <see cref="IDeserializer"/>
    /// </summary>
    protected IDeserializer Deserializer { get; }

    /// <inheritdoc/>
    public virtual bool Supports(string mediaTypeName) => mediaTypeName switch
    {
        "text/yaml" or "text/x-yaml" or "application/yaml" or "application/x-yaml" => true,
        _ => mediaTypeName.EndsWith("+yaml")
    };

    /// <inheritdoc/>
    public virtual void Serialize(object value, Stream stream, Type? type = null)
    {
        using var textWriter = new StreamWriter(stream, leaveOpen: true);
        this.Serializer.Serialize(textWriter, value, type ?? value.GetType());
    }

    /// <inheritdoc/>
    public virtual string SerializeToText(object value, Type? type = null) => this.Serializer.Serialize(value);

    /// <summary>
    /// Serializes the specified object to YAML
    /// </summary>
    /// <typeparam name="T">The type of object to serialize</typeparam>
    /// <param name="graph">The object to serialized</param>
    /// <returns>The YAML of the serialized object</returns>
    public virtual string Serialize<T>(T graph) => this.Serializer.Serialize(graph!);

    /// <summary>
    /// Serializes the specified object to YAML
    /// </summary>
    /// <typeparam name="T">The type of object to serialize</typeparam>
    /// <param name="writer">The <see cref="TextWriter"/> to the YAML to</param>
    /// <param name="graph">The object to serialized</param>
    /// <returns>The YAML of the serialized object</returns>
    public virtual void Serialize<T>(TextWriter writer, T graph) => this.Serializer.Serialize(writer, graph!);

    /// <inheritdoc/>
    public virtual object? Deserialize(string input, Type type) => this.Deserializer.Deserialize(input, type);

    /// <inheritdoc/>
    public virtual object? Deserialize(Stream stream, Type type)
    {
        using var streamReader = new StreamReader(stream, leaveOpen: true);
        return this.Deserializer.Deserialize(streamReader, type);
    }

    /// <summary>
    /// Deserializes the specified YAML input
    /// </summary>
    /// <typeparam name="T">The type to deserialize the specified YAML into</typeparam>
    /// <param name="yaml">The YAML input to deserialize</param>
    /// <returns>The deserialized value</returns>
    public virtual T? Deserialize<T>(string yaml) => this.Deserializer.Deserialize<T>(yaml);

    /// <summary>
    /// Deserializes the specified YAML input
    /// </summary>
    /// <typeparam name="T">The type to deserialize the specified YAML into</typeparam>
    /// <param name="reader">The <see cref="TextReader"/> to read the YAML to deserialize</param>
    /// <returns>The deserialized value</returns>
    public virtual T? Deserialize<T>(TextReader reader) => this.Deserializer.Deserialize<T>(reader);

    /// <summary>
    /// Deserializes the specified YAML input
    /// </summary>
    /// <param name="reader">The <see cref="TextReader"/> to read the YAML to deserialize</param>
    /// <param name="type">The type to deserialize the specified YAML into</param>
    /// <returns>The deserialized value</returns>
    public virtual object? Deserialize(TextReader reader, Type type) => this.Deserializer.Deserialize(reader, type);

    /// <summary>
    /// Deserializes the specified YAML input
    /// </summary>
    /// <typeparam name="T">The type to deserialize the specified YAML into</typeparam>
    /// <param name="buffer">The UTF8 encoded YAML input</param>
    /// <returns>The deserialized value</returns>
    public virtual T? Deserialize<T>(ReadOnlySpan<byte> buffer) => this.Deserializer.Deserialize<T>(Encoding.UTF8.GetString(buffer));


}
