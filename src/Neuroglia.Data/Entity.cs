namespace Neuroglia.Data;

/// <summary>
/// Represents the default abstract implementation of the <see cref="IEntity{TKey}"/> interface
/// </summary>
/// <typeparam name="TKey">The type of key used to uniquely identify the <see cref="IEntity{TKey}"/></typeparam>
public abstract class Entity<TKey>
    : Identifiable<TKey>, IEntity<TKey>
    where TKey : IEquatable<TKey>
{

    /// <summary>
    /// Initializes a new <see cref="Entity{TKey}"/>
    /// </summary>
    protected Entity() { }

    /// <summary>
    /// Initializes a new <see cref="Entity{TKey}"/>
    /// </summary>
    /// <param name="id">The <see cref="IEntity"/>'s unique key</param>
    protected Entity(TKey id)
        : base(id)
    {
        this.CreatedAt = DateTimeOffset.UtcNow;
    }

    /// <inheritdoc/>
    public virtual DateTimeOffset CreatedAt { get; protected set; }

    /// <inheritdoc/>
    public virtual DateTimeOffset? LastModified { get; protected set; }

    /// <inheritdoc/>
    public virtual ulong StateVersion { get; set; }

}
