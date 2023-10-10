using Neuroglia.Data.Infrastructure.EventSourcing.Services;

namespace Neuroglia.Data.Infrastructure.EventSourcing.Configuration;

/// <summary>
/// Represents the options used to configure an <see cref="EventSourcingRepository{TAggregate, TKey}"/>
/// </summary>
public class EventSourcingRepositoryOptions
{

    /// <summary>
    /// Gets the default snapshot frequency
    /// </summary>
    public const ulong DefaultSnapshotFrequency = 10;

    /// <summary>
    /// Gets/sets the frequency at which to snapshot <see cref="IAggregateRoot"/>s
    /// </summary>
    public virtual ulong? SnapshotFrequency { get; set; } = DefaultSnapshotFrequency;

    /// <summary>
    /// Gets/sets the type of <see cref="IEventAggregatorFactory"/> to use to create <see cref="IEventAggregator"/>s
    /// </summary>
    public Type AggregatorFactoryType { get; set; } = typeof(EventAggregatorFactory);

    /// <summary>
    /// Gets/sets the type of <see cref="IEventMigrationManager"/> to use to migrate events
    /// </summary>
    public Type MigrationManagerType { get; set; } = typeof(EventMigrationManager);

}

/// <summary>
/// Represents the options used to configure an <see cref="EventSourcingRepository{TAggregate, TKey}"/>
/// </summary>
/// <typeparam name="TAggregate">The type of <see cref="IAggregateRoot"/> managed by the <see cref="EventSourcingRepository{TAggregate, TKey}"/> to configure</typeparam>
/// <typeparam name="TKey">The type of key used to uniquely identify the <see cref="IAggregateRoot"/> managed by the <see cref="EventSourcingRepository{TAggregate, TKey}"/> to configure</typeparam>
public class EventSourcingRepositoryOptions<TAggregate, TKey>
    : EventSourcingRepositoryOptions
    where TAggregate : class, IAggregateRoot<TKey>
    where TKey : IEquatable<TKey>
{



}