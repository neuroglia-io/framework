// Copyright © 2021-Present Neuroglia SRL. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License"),
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

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
    : IEventSourcingRepository<TAggregate, TKey>
    where TAggregate : class, IAggregateRoot<TKey>
    where TKey : IEquatable<TKey>
{

    /// <summary>
    /// Initializes a new <see cref="EventSourcingRepository{TAggregate, TKey}"/>
    /// </summary>
    /// <param name="options">The service used to access the current <see cref="EventSourcingRepositoryOptions{TAggregate, TKey}"/></param>
    /// <param name="mediator">The service used to mediate calls</param>
    /// <param name="eventStore">The service used to persist events</param>
    /// <param name="aggregatorFactory">The service used to create <see cref="IEventAggregator"/>s</param>
    /// <param name="migrationManager">The service used to manage event migrations</param>
    /// <param name="stateManager">The service used to manage <see cref="ISnapshot"/>s</param>
    public EventSourcingRepository(IOptions<EventSourcingRepositoryOptions<TAggregate, TKey>> options, IMediator mediator, IEventStore eventStore, IEventAggregatorFactory aggregatorFactory, IEventMigrationManager migrationManager, IAggregateStateManager<TAggregate, TKey> stateManager)
    {
        this.Options = options.Value;
        this.Mediator = mediator;
        this.EventStore = eventStore;
        this.Aggregator = aggregatorFactory.CreateAggregator<TAggregate, IDomainEvent>();
        this.MigrationManager = migrationManager;
        this.StateManager = stateManager;
    }

    /// <summary>
    /// Gets the current <see cref="EventSourcingRepositoryOptions{TAggregate, TKey}"/>
    /// </summary>
    protected EventSourcingRepositoryOptions<TAggregate, TKey> Options { get; }

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
    /// Gets the service used to manage <see cref="ISnapshot"/>s
    /// </summary>
    protected IAggregateStateManager<TAggregate, TKey> StateManager { get; }

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
        aggregate.State.StateVersion = (ulong)events.Count;
        aggregate.ClearPendingEvents();
        await this.StateManager.TakeSnapshotAsync(aggregate, cancellationToken).ConfigureAwait(false);
        if(this.Options.PublishEvents) foreach (var e in events) await this.Mediator.PublishAsync((dynamic)e, cancellationToken).ConfigureAwait(false);
        return aggregate;
    }

    /// <inheritdoc/>
    public virtual async Task<bool> ContainsAsync(TKey id, CancellationToken cancellationToken = default)
    {
        try
        {
            var stream = await this.EventStore.GetAsync(this.GetStreamIdFor(id), cancellationToken).ConfigureAwait(false);
            return stream != null;
        }
        catch (StreamNotFoundException)
        {
            return false;
        }
    }

    /// <inheritdoc/>
    public virtual async Task<TAggregate?> GetAsync(TKey id, CancellationToken cancellationToken = default)
    {
        var aggregate = await this.StateManager.RestoreStateAsync(id, cancellationToken).ConfigureAwait(false);
        var recordedEvents = await this.EventStore.ReadAsync(this.GetStreamIdFor(id), StreamReadDirection.Forwards, (long)aggregate.State.StateVersion, null, cancellationToken).ToListAsync(cancellationToken).ConfigureAwait(false);
        if (recordedEvents == null) return null;
        aggregate = this.Aggregator.Aggregate(recordedEvents.Select(e => this.MigrationManager.MigrateEventToLatest(e.Data!)).OfType<IDomainEvent>(), aggregate);
        return aggregate;
    }

    /// <inheritdoc/>
    public virtual async Task<TAggregate?> GetAsync(TKey id, ulong version, CancellationToken cancellationToken = default)
    {
        var recordedEvents = await this.EventStore.ReadAsync(this.GetStreamIdFor(id), StreamReadDirection.Backwards, (long)version, cancellationToken: cancellationToken).ToListAsync(cancellationToken).ConfigureAwait(false);
        if (recordedEvents == null) return null;
        recordedEvents.Reverse();
        return this.Aggregator.Aggregate(recordedEvents.Select(e => this.MigrationManager.MigrateEventToLatest(e.Data!)).OfType<IDomainEvent>());
    }

    /// <inheritdoc/>
    public virtual Task<TAggregate> UpdateAsync(TAggregate aggregate, CancellationToken cancellationToken = default)
    {
        if (aggregate == null) throw new ArgumentNullException(nameof(aggregate));
        return this.UpdateAsync(aggregate, aggregate.State.StateVersion - 1, cancellationToken);
    }

    /// <inheritdoc/>
    public virtual async Task<TAggregate> UpdateAsync(TAggregate aggregate, ulong expectedVersion, CancellationToken cancellationToken = default)
    {
        if (aggregate == null) throw new ArgumentNullException(nameof(aggregate));
        if (!aggregate.PendingEvents.Any()) return aggregate;
        var events = aggregate.PendingEvents.ToList();
        await this.EventStore.AppendAsync(this.GetStreamIdFor(aggregate.Id), events.Select(e => e.GetDescriptor()), (long)expectedVersion, cancellationToken).ConfigureAwait(false);
        aggregate.State.StateVersion += (ulong)events.Count;
        aggregate.ClearPendingEvents();
        await this.StateManager.TakeSnapshotAsync(aggregate, cancellationToken).ConfigureAwait(false);
        if (this.Options.PublishEvents) foreach (var e in events) await this.Mediator.PublishAsync((dynamic)e, cancellationToken).ConfigureAwait(false);
        return aggregate;
    }

    /// <inheritdoc/>
    public virtual async Task<bool> RemoveAsync(TKey id, CancellationToken cancellationToken = default)
    {
        try
        {
            await this.EventStore.DeleteAsync(this.GetStreamIdFor(id), cancellationToken).ConfigureAwait(false);
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
    /// Gets the id of the stream that corresponds to the specified aggregate key
    /// </summary>
    /// <param name="id">The key of the aggregate to get the stream id for</param>
    /// <returns>The id of the stream that corresponds to the specified aggregate key</returns>
    protected virtual string GetStreamIdFor(TKey id) => $"{this.StreamPrefix}-{id}";

    async Task<IIdentifiable> IRepository.AddAsync(IIdentifiable entity, CancellationToken cancellationToken) => await this.AddAsync((TAggregate)entity, cancellationToken).ConfigureAwait(false);

    Task<bool> IRepository.ContainsAsync(object key, CancellationToken cancellationToken) => this.ContainsAsync((TKey)key, cancellationToken);

    async Task<IIdentifiable?> IRepository.GetAsync(object key, CancellationToken cancellationToken) => await this.GetAsync((TKey)key, cancellationToken).ConfigureAwait(false);

    Task<TAggregate?> IRepository<TAggregate>.GetAsync(object key, CancellationToken cancellationToken) => this.GetAsync((TKey)key, cancellationToken);

    Task<TAggregate?> IEventSourcingRepository<TAggregate>.GetAsync(object key, ulong version, CancellationToken cancellationToken) => this.GetAsync((TKey)key, version, cancellationToken);

    async Task<IAggregateRoot?> IEventSourcingRepository.GetAsync(object key, ulong version, CancellationToken cancellationToken) => await this.GetAsync((TKey)key, version, cancellationToken).ConfigureAwait(false);

    async Task<IIdentifiable> IRepository.UpdateAsync(IIdentifiable entity, CancellationToken cancellationToken) => await this.UpdateAsync((TAggregate)entity, cancellationToken).ConfigureAwait(false);

    async Task<IAggregateRoot> IEventSourcingRepository.UpdateAsync(IAggregateRoot aggregate, ulong version, CancellationToken cancellationToken) => await this.UpdateAsync((TAggregate)aggregate, version, cancellationToken).ConfigureAwait(false);

    async Task<bool> IRepository.RemoveAsync(IIdentifiable entity, CancellationToken cancellationToken) => await this.RemoveAsync((TAggregate)entity, cancellationToken).ConfigureAwait(false);

}
