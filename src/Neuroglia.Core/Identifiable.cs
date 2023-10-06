namespace Neuroglia;

/// <summary>
/// Represents the default abstract implementation of the <see cref="IIdentifiable{TKey}"/> interface
/// </summary>
/// <typeparam name="TKey"></typeparam>
public abstract class Identifiable<TKey>
    : IIdentifiable<TKey>
    where TKey : IEquatable<TKey>
{

    /// <summary>
    /// Initializes a new <see cref="Identifiable{TKey}"/>
    /// </summary>
    protected Identifiable() { }

    /// <summary>
    /// Initializes a new <see cref="Identifiable{TKey}"/>
    /// </summary>
    /// <param name="id">The key used to identify the object</param>
    protected Identifiable(TKey id)
    {
        this.Id = id;
    }

    /// <inheritdoc/>
    public virtual TKey Id { get; protected set; } = default!;

    object IIdentifiable.Id => this.Id;

    /// <inheritdoc/>
    public virtual bool Equals(IIdentifiable<TKey>? other) => other != null && this.Id.Equals(other.Id);

}
