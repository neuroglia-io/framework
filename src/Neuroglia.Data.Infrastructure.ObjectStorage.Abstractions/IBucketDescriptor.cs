namespace Neuroglia.Data.Infrastructure.ObjectStorage;

/// <summary>
/// Defines the fundamentals of an object used to describe a bucket
/// </summary>
public interface IBucketDescriptor
{

    /// <summary>
    /// Gets the bucket's name
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the bucket's tags, if any
    /// </summary>
    IDictionary<string, string>? Tags { get; }

}
