using YamlDotNet.Serialization;

namespace Neuroglia.Serialization.Yaml;

/// <summary>
/// Defines the fundamentals of a service used to build and configure a <see cref="YamlDotNetSerializer"/>
/// </summary>
public interface IYamlDotNetSerializerBuilder
{

    /// <summary>
    /// Gets the service used to build and configure the underlying <see cref="YamlDotNet.Serialization.ISerializer"/>
    /// </summary>
    SerializerBuilder Serializer { get; }

    /// <summary>
    /// Gets the service used to build and configure the underlying <see cref="IDeserializer"/>
    /// </summary>
    DeserializerBuilder Deserializer { get; }

}
