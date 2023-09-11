namespace Neuroglia.Serialization;

/// <summary>
/// Defines the fundamentals of a service used to serialize and deserialize binary data
/// </summary>
public interface IBinarySerializer
    : ISerializer
{

    /// <summary>
    /// Serializes the specified value to a byte array
    /// </summary>
    /// <param name="value">The value to serialize</param>
    /// <param name="type">The type to serialize the specified value as. If not specified, type is inferred</param>
    /// <returns>The binary representation of the serialized value</returns>
    ReadOnlySpan<byte> SerializeToByteArray(object value, Type? type = null);

    /// <summary>
    /// Deserializes a value from the specified byte array
    /// </summary>
    /// <param name="input">The binary representation of the value to deserialize</param>
    /// <param name="type">The value's expected type</param>
    /// <returns>The deserialized value, if any</returns>
    object? Deserialize(ReadOnlySpan<byte> input, Type type);

}
