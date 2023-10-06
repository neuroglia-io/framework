namespace Neuroglia.Data;

/// <summary>
/// Defines the fundamentals of an aggregate root
/// </summary>
public interface IAggregateRoot
    : IEntity
{

    /// <summary>
    /// Gets an <see cref="IReadOnlyList{T}"/> containing the <see cref="IAggregateRoot"/>'s pending <see cref="IDomainEvent"/>s
    /// </summary>
    IReadOnlyList<IDomainEvent> PendingEvents { get; }

    /// <summary>
    /// Clears all pending <see cref="IDomainEvent"/>
    /// </summary>
    void ClearPendingEvents();

}

/// <summary>
/// Defines the fundamentals of an aggregate root
/// </summary>
/// <typeparam name="TKey">The type of key used to uniquely identify the <see cref="IAggregateRoot"/></typeparam>
public interface IAggregateRoot<TKey>
    : IAggregateRoot, IEntity<TKey>
    where TKey : IEquatable<TKey>
{



}