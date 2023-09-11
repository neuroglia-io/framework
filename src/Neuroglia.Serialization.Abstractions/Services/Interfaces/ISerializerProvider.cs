namespace Neuroglia.Serialization;

/// <summary>
/// Defines the fundamentals of a service used to manage serializers
/// </summary>
public interface ISerializerProvider
{

    /// <summary>
    /// Gets all registered <see cref="ISerializer"/>s
    /// </summary>
    /// <returns>A new <see cref="IEnumerable{T}"/> containing all registered <see cref="ISerializer"/>s</returns>
    IEnumerable<ISerializer> GetSerializers();

    /// <summary>
    /// Gets all registered <see cref="ISerializer"/> that support the specified media type name
    /// </summary>
    /// <param name="mediaTypeName">The media type name supported by <see cref="ISerializer"/> to get</param>
    /// <returns>A new <see cref="IEnumerable{T}"/> containing all registered <see cref="ISerializer"/> that support the specified media type name</returns>
    IEnumerable<ISerializer> GetSerializersFor(string mediaTypeName);

}
