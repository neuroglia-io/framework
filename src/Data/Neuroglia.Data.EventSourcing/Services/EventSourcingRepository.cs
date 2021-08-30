/*
 * Copyright © 2021 Neuroglia SPRL. All rights reserved.
 * <p>
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * <p>
 * http://www.apache.org/licenses/LICENSE-2.0
 * <p>
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 */
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Neuroglia.Data.EventSourcing.Configuration;
using Neuroglia.Mediation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Neuroglia.Data.EventSourcing.Services
{

    /// <summary>
    /// Represents an <see cref="IRepository{TEntity, TKey}"/> implementation that uses an <see cref="IEventStore"/> to persist data
    /// </summary>
    public class EventSourcingRepository<TAggregate, TKey>
        : RepositoryBase<TAggregate, TKey>
        where TAggregate : class, IIdentifiable<TKey>, IAggregateRoot<TKey>
        where TKey : IEquatable<TKey>
    {

        /// <summary>
        /// Initializes a new <see cref="EventSourcingRepository{TEntity, TKey}"/>
        /// </summary>
        /// <param name="options">The service used to access the current <see cref="EventSourcingRepositoryOptions{TAggregate, TKey}"/></param>
        /// <param name="serviceProvider">The current <see cref="IServiceProvider"/></param>
        /// <param name="logger">The service used to perform logging</param>
        /// <param name="eventStore">The <see cref="IEventStore"/> used to persist data managed by the <see cref="EventSourcingRepository{TEntity, TKey}"/></param>
        /// <param name="aggregatorFactory">The service used to aggregate <see cref="IDomainEvent"/>s</param>
        public EventSourcingRepository(IOptions<EventSourcingRepositoryOptions<TAggregate, TKey>> options, IServiceProvider serviceProvider, 
            ILogger<EventSourcingRepository<TAggregate, TKey>> logger, IEventStore eventStore, IAggregatorFactory aggregatorFactory)
        {
            this.Options = options.Value;
            this.ServiceProvider = serviceProvider;
            this.Logger = logger;
            this.EventStore = eventStore;
            this.Aggregator = aggregatorFactory.CreateAggregator<TAggregate>();
            this.Mediator = this.ServiceProvider.GetService<IMediator>();
        }

        /// <summary>
        /// Gets the options used to configure the <see cref="EventSourcingRepositoryOptions{TAggregate, TKey}"/>
        /// </summary>
        protected virtual EventSourcingRepositoryOptions<TAggregate, TKey> Options { get; }

        /// <summary>
        /// Gets the current <see cref="IServiceProvider"/>
        /// </summary>
        protected virtual IServiceProvider ServiceProvider { get; }

        /// <summary>
        /// Gets the service used to perform logging
        /// </summary>
        protected virtual ILogger Logger { get; }

        /// <summary>
        /// Gets the <see cref="IEventStore"/> used to persist data managed by the <see cref="EventSourcingRepository{TEntity, TKey}"/>
        /// </summary>
        protected virtual IEventStore EventStore { get; }

        /// <summary>
        /// Gets the service used to mediate calls
        /// </summary>
        protected virtual IMediator Mediator { get; }

        /// <summary>
        /// Gets the service used to aggregate <see cref="IDomainEvent"/>s
        /// </summary>
        protected virtual IAggregator<TAggregate> Aggregator { get; }

        /// <summary>
        /// Gets the prefix for all streams managed by the <see cref="EventSourcingRepository{TAggregate, TKey}"/>
        /// </summary>
        protected virtual string StreamPrefix
        {
            get
            {
                return typeof(TAggregate).Name.ToLower();
            }
        }

        /// <inheritdoc/>
        public override async Task<bool> ContainsAsync(TKey key, CancellationToken cancellationToken = default)
        {
            IEventStream stream = await this.EventStore.GetStreamAsync(this.GetStreamIdFor(key), cancellationToken);
            return stream != null;
        }

        /// <inheritdoc/>
        public override async Task<TAggregate> FindAsync(TKey key, CancellationToken cancellationToken = default)
        {
            TAggregate aggregate = (await this.GetSnapshotAsync(key, cancellationToken))?.Data;
            IEnumerable<ISourcedEvent> sourcedEvents;
            if (aggregate == null)
            {
                sourcedEvents = await this.EventStore.ReadAllEventsForwardAsync(this.GetStreamIdFor(key), cancellationToken);
                if (sourcedEvents == null)
                    return null;
                aggregate = this.Aggregator.Aggregate(sourcedEvents.Select(e => e.Data).OfType<IDomainEvent>());
            }   
            else
            {
                sourcedEvents = await this.EventStore.ReadEventsForwardAsync(this.GetStreamIdFor(key), aggregate.Version, cancellationToken);
                if (sourcedEvents == null)
                    return null;
                aggregate = this.Aggregator.Aggregate(aggregate, sourcedEvents.Select(e => e.Data).OfType<IDomainEvent>());
            }
            return aggregate;
        }

        /// <inheritdoc/>
        public override async Task<TAggregate> FindAsync(object[] keyValues, CancellationToken cancellationToken = default)
        {
            if (keyValues == null
                || keyValues.Length != 1)
                throw new ArgumentOutOfRangeException(nameof(keyValues));
            return await this.FindAsync((TKey)keyValues[0], cancellationToken);
        }

        /// <inheritdoc/>
        public override async Task<TAggregate> AddAsync(TAggregate aggregate, CancellationToken cancellationToken = default)
        {
            if (aggregate == null)
                throw new ArgumentNullException(nameof(aggregate));
            IEnumerable<IDomainEvent> events = aggregate.PendingEvents.ToList();
            await this.EventStore.AppendToStreamAsync(this.GetStreamIdFor(aggregate.Id), events.Select(e => e.GetMetadata()), cancellationToken);
            aggregate.SetVersion(events.Count());
            aggregate.ClearPendingEvents();
            await this.TrySnapshotAsync(aggregate, cancellationToken);
            if(this.Mediator != null)
            {
                foreach (IDomainEvent e in events)
                {
                    await this.Mediator.PublishAsync((dynamic)e, cancellationToken);
                }
            }
            return aggregate;
        }

        /// <inheritdoc/>
        public override async Task<TAggregate> UpdateAsync(TAggregate aggregate, CancellationToken cancellationToken = default)
        {
            if (aggregate == null)
                throw new ArgumentNullException(nameof(aggregate));
            if (!aggregate.PendingEvents.Any())
                return aggregate;
            IEnumerable<IDomainEvent> events = aggregate.PendingEvents.ToList();
            await this.EventStore.AppendToStreamAsync(this.GetStreamIdFor(aggregate.Id), events.Select(e => e.GetMetadata()), aggregate.Version, cancellationToken);
            aggregate.SetVersion(aggregate.Version + events.Count());
            aggregate.ClearPendingEvents();
            await this.TrySnapshotAsync(aggregate, cancellationToken);
            if (this.Mediator != null)
            {
                foreach (IDomainEvent e in events)
                {
                    await this.Mediator.PublishAsync((dynamic)e, cancellationToken);
                }
            }
            return aggregate;
        }

        /// <inheritdoc/>
        public override async Task<TAggregate> RemoveAsync(TAggregate aggregate, CancellationToken cancellationToken = default)
        {
            if (aggregate == null)
                throw new ArgumentNullException(nameof(aggregate));
            await this.EventStore.DeleteStreamAsync(this.GetStreamIdFor(aggregate.Id), cancellationToken);
            return aggregate;
        }

        /// <inheritdoc/>
        public override Task<List<TAggregate>> ToListAsync(CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException($"Querying an event sourcing repository is not supported. Event sourcing repositories should only be used as write-model stores (cfr. CQRS)");
        }

        /// <inheritdoc/>
        public override IQueryable<TAggregate> AsQueryable()
        {
            throw new NotSupportedException($"Querying an event sourcing repository is not supported. Event sourcing repositories should only be used as write-model stores (cfr. CQRS)");
        }

        /// <inheritdoc/>
        public override Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Gets the id of the stream that corresponds to the specified aggregate key
        /// </summary>
        /// <param name="key">The key of the aggregate to get the stream id for</param>
        /// <returns>The id of the stream that corresponds to the specified aggregate key</returns>
        protected virtual string GetStreamIdFor(TKey key)
        {
            return $"{this.StreamPrefix}-{key.ToString().Replace("-", "")}";
        }

        /// <summary>
        /// Gets the id of the snapshot stream that corresponds to the specified aggregate key
        /// </summary>
        /// <param name="key">The key of the aggregate to get the snapshot stream id for</param>
        /// <returns>The id of the stream that corresponds to the specified aggregate key</returns>
        protected virtual string GetSnapshotStreamIdFor(TKey key)
        {
            return $"{this.StreamPrefix}-snapshots-{key.ToString().Replace("-", "")}";
        }

        /// <summary>
        /// Gets the snapshot of the specified <see cref="IAggregateRoot"/>
        /// </summary>
        /// <param name="key">The key of the <see cref="IAggregateRoot"/> to find</param>
        /// <param name="cancellation">A <see cref="CancellationToken"/></param>
        /// <returns>The snapshot of the <see cref="IAggregateRoot"/> with the specified key</returns>
        protected virtual async Task<ISnapshot<TAggregate>> GetSnapshotAsync(TKey key, CancellationToken cancellation = default)
        {
            ISourcedEvent sourcedEvent = await this.EventStore.ReadSingleEventBackwardAsync(this.GetSnapshotStreamIdFor(key), EventStreamPosition.End, cancellation);
            if (sourcedEvent == null)
                return null;
            return (Snapshot<TAggregate>)sourcedEvent.Data;
        }

        /// <summary>
        /// Attempts to snapshot the specified <see cref="IAggregateRoot"/>
        /// </summary>
        /// <param name="aggregate">The <see cref="IAggregateRoot"/> to create a new <see cref="Snapshot{TAggregate}"/> for</param>
        /// <param name="cancellation">A <see cref="CancellationToken"/></param>
        /// <returns>A boolean indicating whether or not a new <see cref="Snapshot{TAggregate}"/> has been created</returns>
        protected virtual async Task<bool> TrySnapshotAsync(TAggregate aggregate, CancellationToken cancellation = default)
        {
            if (!this.Options.SnapshotFrequency.HasValue)
                    return false;
            ISnapshot snapshot = await this.GetSnapshotAsync(aggregate.Id, cancellation);
            if (snapshot == null)
            {
                if (aggregate.Version < this.Options.SnapshotFrequency.Value)
                    return false;
            }
            else if (snapshot.Version + this.Options.SnapshotFrequency.Value > aggregate.Version)
            {
                return false;
            }
            snapshot = Snapshot.CreateFor(aggregate);
            await this.EventStore.AppendToStreamAsync(this.GetSnapshotStreamIdFor(aggregate.Id), new EventMetadata[] { new("snapshot", snapshot) }, EventStreamPosition.End, cancellation);
            return true;
        }

    }

}
