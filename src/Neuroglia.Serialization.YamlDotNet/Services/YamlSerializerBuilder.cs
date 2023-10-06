using YamlDotNet.Serialization;

namespace Neuroglia.Serialization.Yaml;

/// <summary>
/// Represents the default implementation of the <see cref="IYamlSerializerBuilder"/>
/// </summary>
public class YamlSerializerBuilder
    : IYamlSerializerBuilder
{

    /// <inheritdoc/>
    public SerializerBuilder Serializer { get; } = new();

    /// <inheritdoc/>
    public DeserializerBuilder Deserializer { get; } = new();

}
