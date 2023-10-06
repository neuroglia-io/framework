﻿namespace Neuroglia.Data;

/// <summary>
/// Defines the fundamentals of a domain event, that is an event bounded to a specific domain context
/// </summary>
public interface IDomainEvent
{

    /// <summary>
    /// Gets the type of the <see cref="IAggregateRoot"/> that has produced the <see cref="IDomainEvent"/>
    /// </summary>
    Type AggregateType { get; }

    /// <summary>
    /// Gets the id of the <see cref="IAggregateRoot"/> that has produced the <see cref="IDomainEvent"/>
    /// </summary>
    object AggregateId { get; }

    /// <summary>
    /// Gets the date and time the <see cref="IDomainEvent"/> has been created at
    /// </summary>
    DateTimeOffset CreatedAt { get; }

}

/// <summary>
/// Defines the fundamentals of a domain event, that is an event bounded to a specific domain context
/// </summary>
/// <typeparam name="TAggregate">The type of <see cref="IAggregateRoot"/> that has produced the <see cref="IDomainEvent"/></typeparam>
public interface IDomainEvent<TAggregate>
    : IDomainEvent
    where TAggregate : IAggregateRoot
{


}

/// <summary>
/// Defines the fundamentals of a domain event, that is an event bounded to a specific domain context
/// </summary>
/// <typeparam name="TAggregate">The type of <see cref="IAggregateRoot"/> that has produced the <see cref="IDomainEvent"/></typeparam>
/// <typeparam name="TKey">The type of key used to uniquely identify the <see cref="IAggregateRoot"/> that has produced the <see cref="IDomainEvent"/></typeparam>
public interface IDomainEvent<TAggregate, TKey>
    : IDomainEvent<TAggregate>
    where TAggregate : IAggregateRoot<TKey>
    where TKey : IEquatable<TKey>
{

    /// <summary>
    /// Gets the key of the <see cref="IAggregateRoot"/> that has produced the <see cref="IDomainEvent"/>
    /// </summary>
    new TKey AggregateId { get; }

}
