namespace Neuroglia.Serialization;

/// <summary>
/// Defines the fundamentals of a service used to serialize and deserialize text data
/// </summary>
public interface ITextSerializer
    : ISerializer
{

    /// <summary>
    /// Serializes the specified value to text
    /// </summary>
    /// <param name="value">The value to serialize</param>
    /// <param name="type">The type to serialize the specified value as. If not specified, type is inferred</param>
    /// <returns>The text representation of the serialized value</returns>
    string SerializeToText(object value, Type? type = null);

    /// <summary>
    /// Deserializes a value from its text representation
    /// </summary>
    /// <param name="input">The text representation of the value to deserialize</param>
    /// <param name="type">The value's expected type</param>
    /// <returns>The deserialized value, if any</returns>
    object? Deserialize(string input, Type type);

}
