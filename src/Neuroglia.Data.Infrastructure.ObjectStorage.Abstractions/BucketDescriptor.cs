namespace Neuroglia.Data.Infrastructure.ObjectStorage;

/// <summary>
/// Represents the default implementation of the <see cref="IBucketDescriptor"/> interface
/// </summary>
public class BucketDescriptor
    : IBucketDescriptor
{

    /// <summary>
    /// Initializes a new <see cref="BucketDescriptor"/>
    /// </summary>
    protected BucketDescriptor() { }

    /// <summary>
    /// Initializes a new <see cref="BucketDescriptor"/>
    /// </summary>
    /// <param name="createdAt">The date and time the bucket has been created at</param>
    /// <param name="name">The bucket's name</param>
    /// <param name="tags">The bucket's tags, if any</param>
    public BucketDescriptor(DateTimeOffset? createdAt, string name, IDictionary<string, string>? tags = null)
    {
        if(string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));

        this.CreatedAt = createdAt;
        this.Name = name;
        this.Tags = tags;
    }

    /// <summary>
    /// Gets the date and time the bucket has been created at
    /// </summary>
    public virtual DateTimeOffset? CreatedAt { get; protected set; }

    /// <inheritdoc/>
    public virtual string Name { get; protected set; } = null!;

    /// <inheritdoc/>
    public virtual IDictionary<string, string>? Tags { get; protected set; }

}