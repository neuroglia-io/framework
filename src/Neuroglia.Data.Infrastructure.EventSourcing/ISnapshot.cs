namespace Neuroglia.Data.Infrastructure.EventSourcing;

/// <summary>
/// Defines the fundamentals of an <see cref="IAggregateRoot"/> snapshot
/// </summary>
public interface ISnapshot
{

    /// <summary>
    /// Gets the version of the <see cref="ISnapshot"/>'s <see cref="IAggregateRoot"/>
    /// </summary>
    ulong Version { get; }

    /// <summary>
    /// Gets the <see cref="ISnapshot"/>'s <see cref="IAggregateRoot"/>
    /// </summary>
    IAggregateRoot Data { get; }

    /// <summary>
    /// Gets the <see cref="ISnapshot"/>'s metadata, if any
    /// </summary>
    IDictionary<string, object>? Metadata { get; }

}

/// <summary>
/// Defines the fundamentals of an <see cref="IAggregateRoot"/> snapshot
/// </summary>
/// <typeparam name="TAggregate">The type of the snapshot <see cref="IAggregateRoot"/></typeparam>
public interface ISnapshot<TAggregate>
    : ISnapshot
    where TAggregate : class, IAggregateRoot
{

    /// <summary>
    /// Gets the <see cref="ISnapshot"/>'s <see cref="IAggregateRoot"/>
    /// </summary>
    new TAggregate Data { get; }

}
