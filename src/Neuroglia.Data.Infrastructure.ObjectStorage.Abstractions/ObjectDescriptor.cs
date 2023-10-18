namespace Neuroglia.Data.Infrastructure.ObjectStorage;

/// <summary>
/// Represents the default implementation of the <see cref="IObjectDescriptor"/> interface
/// </summary>
public class ObjectDescriptor
    : IObjectDescriptor
{

    /// <summary>
    /// Initializes a new <see cref="ObjectDescriptor"/>
    /// </summary>
    protected ObjectDescriptor() { }

    /// <summary>
    /// Initializes a new <see cref="ObjectDescriptor"/>
    /// </summary>
    /// <param name="lastModified">The date and time the object has last been modified</param>
    /// <param name="bucketName">The name of the bucket the described object belongs to</param>
    /// <param name="name">The object's name</param>
    /// <param name="contentType">The object's content type</param>
    /// <param name="size">The object's size</param>
    /// <param name="eTag">The object's ETag</param>
    /// <param name="tags">A name/value mapping of the object's tags, if any</param>
    public ObjectDescriptor(DateTimeOffset? lastModified, string bucketName, string name, string contentType, ulong size, string eTag, IDictionary<string, string>? tags = null)
    {
        if (string.IsNullOrWhiteSpace(bucketName)) throw new ArgumentNullException(nameof(bucketName));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
        if (string.IsNullOrWhiteSpace(contentType)) throw new ArgumentNullException(nameof(contentType));
        if (string.IsNullOrWhiteSpace(eTag)) throw new ArgumentNullException(nameof(eTag));

        this.LastModified = lastModified;
        this.BucketName = bucketName;
        this.Name = name;
        this.ContentType = contentType;
        this.Size = size;
        this.ETag = eTag;
        this.Tags = tags?.AsReadOnly();
    }

    /// <inheritdoc/>
    public virtual DateTimeOffset? LastModified { get; protected set; }

    /// <inheritdoc/>
    public virtual string BucketName { get; protected set; } = null!;

    /// <inheritdoc/>
    public virtual string Name { get; protected set; } = null!;

    /// <inheritdoc/>
    public virtual string ContentType { get; protected set; } = null!;

    /// <inheritdoc/>
    public virtual ulong Size { get; protected set; }

    /// <inheritdoc/>
    public virtual string ETag { get; protected set; } = null!;

    /// <inheritdoc/>
    public virtual IReadOnlyDictionary<string, string>? Tags { get; protected set; } = null!;


}