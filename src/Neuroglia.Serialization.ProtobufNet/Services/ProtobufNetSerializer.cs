using ProtoBuf;

namespace Neuroglia.Serialization.Protobuf.Services;

/// <summary>
/// Represents the <see href="ProtobufNet">https://github.com/protobuf-net/protobuf-net</see> implementation of the <see cref="IBinarySerializer"/>
/// </summary>
public class ProtobufNetSerializer
    : IBinarySerializer
{

    /// <inheritdoc/>
    public virtual bool Supports(string mediaTypeName) => mediaTypeName switch
    {
        "application/protobuf" or "application/x-protobuf" or "application/vnd.google.protobuf" => true,
        _ => false
    };

    /// <inheritdoc/>
    public virtual void Serialize(object value, Stream stream, Type? type = null) => Serializer.Serialize(stream, value);

    /// <inheritdoc/>
    public virtual ReadOnlySpan<byte> SerializeToByteArray(object value, Type? type = null)
    {
        using var stream = new MemoryStream();
        this.Serialize(value, stream, type);
        stream.Flush();
        stream.Position = 0;
        return stream.ToArray();
    }

    /// <inheritdoc/>
    public virtual object? Deserialize(Stream stream, Type type) => Serializer.Deserialize(stream, type);

    /// <inheritdoc/>
    public virtual object? Deserialize(ReadOnlySpan<byte> input, Type type)
    {
        using var stream = new MemoryStream(input.ToArray());
        return this.Deserialize(stream, type);
    }

}