using Microsoft.Extensions.Options;
using Neuroglia.Data.Infrastructure.EventSourcing.Configuration;
using Neuroglia.Data.Infrastructure.Services;
using Neuroglia.Mediation;

namespace Neuroglia.Data.Infrastructure.EventSourcing.Services;

/// <summary>
/// Represents an event sourcing implementation of the <see cref="IRepository{TEntity, TKey}"/> interface
/// </summary>
/// <typeparam name="TAggregate">The type of the managed <see cref="IAggregateRoot"/>s</typeparam>
/// <typeparam name="TKey">The key used to identify managed <see cref="IAggregateRoot"/>s</typeparam>
public class EventSourcingRepository<TAggregate, TKey>
    : IRepository<TAggregate, TKey>
    where TAggregate : class, IAggregateRoot<TKey>
    where TKey : IEquatable<TKey>
{

    /// <summary>
    /// Initializes a new <see cref="EventSourcingRepository{TAggregate, TKey}"/>
    /// </summary>
    /// <param name="options">The service used to access the current <see cref="EventSourcingRepositoryOptions{TAggregate, TKey}"/></param>
    /// <param name="eventStore">The service used to persist events</param>
    /// <param name="aggregatorFactory">The service used to create <see cref="IEventAggregator"/>s</param>
    /// <param name="mediator">The service used to mediate calls</param>
    public EventSourcingRepository(IOptions<EventSourcingRepositoryOptions<TAggregate, TKey>> options, IEventStore eventStore, IEventAggregatorFactory aggregatorFactory, IEventMigrationManager migrationManager, IMediator mediator)
    {
        this.Options = options.Value;
        this.EventStore = eventStore;
        this.Aggregator = aggregatorFactory.CreateAggregator<TAggregate, IDomainEvent>();
        this.MigrationManager = migrationManager;
        this.Mediator = mediator;
    }

    /// <summary>
    /// Gets the options used to configure the <see cref="EventSourcingRepositoryOptions{TAggregate, TKey}"/>
    /// </summary>
    protected virtual EventSourcingRepositoryOptions<TAggregate, TKey> Options { get; }

    /// <summary>
    /// Gets the service used to persist events
    /// </summary>
    protected IEventStore EventStore { get; }

    /// <summary>
    /// Gets the service used to aggregate events
    /// </summary>
    protected IEventAggregator<TAggregate, IDomainEvent> Aggregator { get; }

    /// <summary>
    /// Gets the service used to manage event migrations
    /// </summary>
    protected IEventMigrationManager MigrationManager { get; }

    /// <summary>
    /// Gets the service, if any, used to mediate calls
    /// </summary>
    protected IMediator Mediator { get; }

    /// <summary>
    /// Gets the prefix for all streams managed by the <see cref="EventSourcingRepository{TAggregate, TKey}"/>
    /// </summary>
    protected virtual string StreamPrefix => typeof(TAggregate).Name.ToLower();

    /// <inheritdoc/>
    public virtual async Task<TAggregate> AddAsync(TAggregate aggregate, CancellationToken cancellationToken = default)
    {
        if (aggregate == null) throw new ArgumentNullException(nameof(aggregate));
        var events = aggregate.PendingEvents.ToList();
        await this.EventStore.AppendAsync(this.GetStreamIdFor(aggregate.Id), events.Select(e => e.GetDescriptor()), cancellationToken: cancellationToken).ConfigureAwait(false);
        aggregate.StateVersion = (ulong)events.Count;
        aggregate.ClearPendingEvents();
        await this.TrySnapshotAsync(aggregate, cancellationToken).ConfigureAwait(false);
        foreach (var e in events)
        {
            await this.Mediator.PublishAsync((dynamic)e, cancellationToken).ConfigureAwait(false);
        }
        return aggregate;
    }

    /// <inheritdoc/>
    public virtual async Task<bool> ContainsAsync(TKey key, CancellationToken cancellationToken = default)
    {
        try
        {
            var stream = await this.EventStore.GetAsync(this.GetStreamIdFor(key), cancellationToken).ConfigureAwait(false);
            return stream != null;
        }
        catch (StreamNotFoundException)
        {
            return false;
        }
    }

    /// <inheritdoc/>
    public virtual async Task<TAggregate?> GetAsync(TKey key, CancellationToken cancellationToken = default)
    {
        var aggregate = (await this.GetSnapshotAsync(key, cancellationToken).ConfigureAwait(false))?.Data;
        IEnumerable<IEventRecord> recordedEvents;
        if (aggregate == null)
        {
            recordedEvents = await this.EventStore.ReadAsync(this.GetStreamIdFor(key), StreamReadDirection.Forwards, StreamPosition.StartOfStream, null, cancellationToken).ToListAsync(cancellationToken).ConfigureAwait(false);
            if (recordedEvents == null) return null;
            aggregate = this.Aggregator.Aggregate(recordedEvents.Select(e => this.MigrationManager.MigrateEventToLatest(e.Data!)).OfType<IDomainEvent>());
        }
        else
        {
            recordedEvents = await this.EventStore.ReadAsync(this.GetStreamIdFor(key), StreamReadDirection.Forwards, (long)aggregate.StateVersion, null, cancellationToken).ToListAsync(cancellationToken).ConfigureAwait(false);
            if (recordedEvents == null) return null;
            aggregate = this.Aggregator.Aggregate(recordedEvents.Select(e => this.MigrationManager.MigrateEventToLatest(e.Data!)).OfType<IDomainEvent>(), aggregate);
        }
        return aggregate;
    }

    /// <inheritdoc/>
    public virtual async Task<TAggregate> UpdateAsync(TAggregate aggregate, CancellationToken cancellationToken = default)
    {
        if (aggregate == null) throw new ArgumentNullException(nameof(aggregate));
        if (!aggregate.PendingEvents.Any()) return aggregate;
        var events = aggregate.PendingEvents.ToList();
        await this.EventStore.AppendAsync(this.GetStreamIdFor(aggregate.Id), events.Select(e => e.GetDescriptor()), (long)aggregate.StateVersion, cancellationToken).ConfigureAwait(false);
        aggregate.StateVersion += (ulong)events.Count;
        aggregate.ClearPendingEvents();
        await this.TrySnapshotAsync(aggregate, cancellationToken);
        foreach (var e in events)
        {
            await this.Mediator.PublishAsync((dynamic)e, cancellationToken).ConfigureAwait(false);
        }
        return aggregate;
    }

    /// <inheritdoc/>
    public virtual async Task<bool> RemoveAsync(TKey key, CancellationToken cancellationToken = default)
    {
        try
        {
            await this.EventStore.DeleteAsync(this.GetStreamIdFor(key), cancellationToken).ConfigureAwait(false);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <inheritdoc/>
    public virtual Task<bool> RemoveAsync(TAggregate aggregate, CancellationToken cancellationToken = default) => this.RemoveAsync(aggregate.Id, cancellationToken);

    /// <inheritdoc/>
    public virtual Task SaveChangesAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

    /// <summary>
    /// Gets the snapshot of the specified <see cref="IAggregateRoot"/>
    /// </summary>
    /// <param name="key">The key of the <see cref="IAggregateRoot"/> to find</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The snapshot of the <see cref="IAggregateRoot"/> with the specified key</returns>
    protected virtual async Task<ISnapshot<TAggregate>?> GetSnapshotAsync(TKey key, CancellationToken cancellationToken = default)
    {
        try
        {
            var e = await this.EventStore.ReadAsync(this.GetSnapshotStreamIdFor(key), StreamReadDirection.Backwards, StreamPosition.EndOfStream, 1, cancellationToken).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
            return e == null ? null : (Snapshot<TAggregate>)e.Data!;
        }
        catch (StreamNotFoundException)
        {
            return null;
        }
    }

    /// <summary>
    /// Attempts to snapshot the specified <see cref="IAggregateRoot"/>
    /// </summary>
    /// <param name="aggregate">The <see cref="IAggregateRoot"/> to create a new <see cref="Snapshot{TAggregate}"/> for</param>
    /// <param name="cancellation">A <see cref="CancellationToken"/></param>
    /// <returns>A boolean indicating whether or not a new <see cref="Snapshot{TAggregate}"/> has been created</returns>
    protected virtual async Task<bool> TrySnapshotAsync(TAggregate aggregate, CancellationToken cancellation = default)
    {
        if (!this.Options.SnapshotFrequency.HasValue) return false;
        var snapshot = await this.GetSnapshotAsync(aggregate.Id, cancellation);
        if (snapshot == null)
        {
            if (aggregate.StateVersion < this.Options.SnapshotFrequency.Value) return false;
        }
        else if (snapshot.Version + this.Options.SnapshotFrequency.Value > aggregate.StateVersion) return false;
        snapshot = Snapshot.CreateFor(aggregate);
        await this.EventStore.AppendAsync(this.GetSnapshotStreamIdFor(aggregate.Id), new EventDescriptor[] { new("snapshot", snapshot) }, null, cancellation).ConfigureAwait(false);
        return true;
    }

    /// <summary>
    /// Gets the id of the stream that corresponds to the specified aggregate key
    /// </summary>
    /// <param name="key">The key of the aggregate to get the stream id for</param>
    /// <returns>The id of the stream that corresponds to the specified aggregate key</returns>
    protected virtual string GetStreamIdFor(TKey key) => $"{this.StreamPrefix}-{key}";

    /// <summary>
    /// Gets the id of the snapshot stream that corresponds to the specified aggregate key
    /// </summary>
    /// <param name="key">The key of the aggregate to get the snapshot stream id for</param>
    /// <returns>The id of the stream that corresponds to the specified aggregate key</returns>
    protected virtual string GetSnapshotStreamIdFor(TKey key) => $"{this.StreamPrefix}-snapshots-{key}";

    async Task<object> IRepository.AddAsync(object entity, CancellationToken cancellationToken) => await this.AddAsync((TAggregate)entity, cancellationToken).ConfigureAwait(false);

    Task<bool> IRepository.ContainsAsync(object key, CancellationToken cancellationToken) => this.ContainsAsync((TKey)key, cancellationToken);

    async Task<object?> IRepository.GetAsync(object key, CancellationToken cancellationToken) => await this.GetAsync((TKey)key, cancellationToken).ConfigureAwait(false);

    Task<TAggregate?> IRepository<TAggregate>.GetAsync(object key, CancellationToken cancellationToken) => this.GetAsync((TKey)key, cancellationToken);

    async Task<object> IRepository.UpdateAsync(object entity, CancellationToken cancellationToken) => await this.UpdateAsync((TAggregate)entity, cancellationToken).ConfigureAwait(false);

    async Task<bool> IRepository.RemoveAsync(object entity, CancellationToken cancellationToken) => await this.RemoveAsync((TAggregate)entity, cancellationToken).ConfigureAwait(false);

}
