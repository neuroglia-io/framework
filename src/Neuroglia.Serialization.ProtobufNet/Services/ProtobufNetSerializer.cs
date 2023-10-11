using ProtoBuf;

namespace Neuroglia.Serialization.Protobuf.Services;

/// <summary>
/// Represents the <see href="ProtobufNet">https://github.com/protobuf-net/protobuf-net</see> implementation of the <see cref="IBinarySerializer"/>
/// </summary>
public class ProtobufNetSerializer
    : ISerializer
{

    /// <inheritdoc/>
    public virtual bool Supports(string mediaTypeName) => mediaTypeName switch
    {
        "application/protobuf" or "application/x-protobuf" or "application/vnd.google.protobuf" => true,
        _ => false
    };

    /// <inheritdoc/>
    public virtual void Serialize(object? value, Stream stream, Type? type = null) => Serializer.Serialize(stream, value);

    /// <inheritdoc/>
    public virtual object? Deserialize(Stream stream, Type type) => Serializer.Deserialize(stream, type);

}