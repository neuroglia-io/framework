using YamlDotNet.Serialization;

namespace Neuroglia.Serialization.Yaml;

/// <summary>
/// Represents the default implementation of the <see cref="IYamlDotNetSerializerBuilder"/>
/// </summary>
public class YamlDotNetSerializerBuilder
    : IYamlDotNetSerializerBuilder
{

    /// <inheritdoc/>
    public SerializerBuilder Serializer { get; } = new();

    /// <inheritdoc/>
    public DeserializerBuilder Deserializer { get; } = new();

}