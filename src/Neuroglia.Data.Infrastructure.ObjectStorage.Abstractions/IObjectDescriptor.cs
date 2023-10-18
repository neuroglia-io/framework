using Neuroglia.Data.Infrastructure.ObjectStorage.Services;

namespace Neuroglia.Data.Infrastructure.ObjectStorage;

/// <summary>
/// Defines the fundamentals of an object used to describe an object stored on an <see cref="IObjectStorage"/>
/// </summary>
public interface IObjectDescriptor
{

    /// <summary>
    /// Gets the date and time the object has been last been modified
    /// </summary>
    DateTimeOffset? LastModified { get; }

    /// <summary>
    /// Gets the name of the bucket the object belongs to
    /// </summary>
    string BucketName { get; }

    /// <summary>
    /// Gets the object's name
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the object's content type
    /// </summary>
    string ContentType { get; }

    /// <summary>
    /// Gets the object's size
    /// </summary>
    ulong Size { get; }

    /// <summary>
    /// Gets the object's ETag
    /// </summary>
    string ETag { get; }

    /// <summary>
    /// Gets a list containing the object's tags, if any
    /// </summary>
    IReadOnlyDictionary<string, string>? Tags { get; }

}