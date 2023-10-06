namespace Neuroglia.Data;

/// <summary>
/// Represents the default implementation of the <see cref="IDomainEvent{TAggregate, TKey}"/> interface
/// </summary>
/// <typeparam name="TAggregate">The type of <see cref="IAggregateRoot"/> that has produced the <see cref="IDomainEvent"/></typeparam>
/// <typeparam name="TKey">The type of key used to uniquely identify the <see cref="IAggregateRoot"/> that has produced the <see cref="IDomainEvent"/></typeparam>
public abstract class DomainEvent<TAggregate, TKey>
    : IDomainEvent<TAggregate, TKey>
    where TAggregate : IAggregateRoot<TKey>
    where TKey : IEquatable<TKey>
{

    /// <summary>
    /// Initializes a new <see cref="DomainEvent{TAggregate, TKey}"/>
    /// </summary>
    protected DomainEvent() { }

    /// <summary>
    /// Initializes a new <see cref="DomainEvent{TAggregate, TKey}"/>
    /// </summary>
    /// <param name="aggregateId">The id of the <see cref="IAggregateRoot"/> that has produced the <see cref="IDomainEvent"/></param>
    protected DomainEvent(TKey aggregateId)
    {
        this.AggregateId = aggregateId;
        this.CreatedAt = DateTimeOffset.Now;
    }

    /// <inheritdoc/>
    public virtual TKey AggregateId { get; protected set; } = default!;

    object IDomainEvent.AggregateId => this.AggregateId;

    /// <inheritdoc/>
    public virtual Type AggregateType => typeof(TAggregate);

    /// <inheritdoc/>
    public virtual DateTimeOffset CreatedAt { get; protected set; }

}