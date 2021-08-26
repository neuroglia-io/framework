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
using Microsoft.Extensions.Logging;
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
        /// <param name="logger">The service used to perform logging</param>
        /// <param name="eventStore">The <see cref="IEventStore"/> used to persist data managed by the <see cref="EventSourcingRepository{TEntity, TKey}"/></param>
        /// <param name="aggregatorFactory">The service used to aggregate <see cref="IDomainEvent"/>s</param>
        /// <param name="mediator">The service used to mediate calls</param>
        public EventSourcingRepository(ILogger<EventSourcingRepository<TAggregate, TKey>> logger, IEventStore eventStore, IAggregatorFactory aggregatorFactory, IMediator mediator)
        {
            this.Logger = logger;
            this.EventStore = eventStore;
            this.Aggregator = aggregatorFactory.CreateAggregator<TAggregate>();
            this.Mediator = mediator;
        }

        /// <summary>
        /// Initializes a new <see cref="EventSourcingRepository{TEntity, TKey}"/>
        /// </summary>
        /// <param name="logger">The service used to perform logging</param>
        /// <param name="eventStore">The <see cref="IEventStore"/> used to persist data managed by the <see cref="EventSourcingRepository{TEntity, TKey}"/></param>
        /// <param name="aggregatorFactory">The service used to aggregate <see cref="IDomainEvent"/>s</param>
        public EventSourcingRepository(ILogger<EventSourcingRepository<TAggregate, TKey>> logger, IEventStore eventStore, IAggregatorFactory aggregatorFactory)
            : this(logger, eventStore, aggregatorFactory, null)
        {

        }

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
            return stream == null;
        }

        /// <inheritdoc/>
        public override async Task<TAggregate> FindAsync(TKey key, CancellationToken cancellationToken = default)
        {
            IEnumerable<ISourcedEvent> sourcedEvents = await this.EventStore.ReadAllEventsForwardAsync(this.GetStreamIdFor(key), cancellationToken);
            IEnumerable<IDomainEvent> domainEvents = sourcedEvents.Select(e => e.AsDomainEvent());
            return await Task.FromResult(this.Aggregator.Aggregate(domainEvents));
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
            IEnumerable<IDomainEvent> events = aggregate.PendingEvents;
            await this.EventStore.AppendToStreamAsync(this.GetStreamIdFor(aggregate.Id), events.Select(e => e.GetMetadata()), cancellationToken);
            aggregate.ClearPendingEvents();
            aggregate.SetVersion(events.Count());
            return aggregate;
        }

        /// <inheritdoc/>
        public override async Task<TAggregate> UpdateAsync(TAggregate aggregate, CancellationToken cancellationToken = default)
        {
            if (aggregate == null)
                throw new ArgumentNullException(nameof(aggregate));
            IEnumerable<IDomainEvent> events = aggregate.PendingEvents;
            await this.EventStore.AppendToStreamAsync(this.GetStreamIdFor(aggregate.Id), events.Select(e => e.GetMetadata()), aggregate.Version, cancellationToken);
            aggregate.ClearPendingEvents();
            aggregate.SetVersion(aggregate.Version + events.Count());
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
            throw new NotImplementedException($"Querying an event sourcing repository is not supported. Event sourcing repositories should only be used as write-model stores (cfr. CQRS)");
        }

        /// <inheritdoc/>
        public override IQueryable<TAggregate> AsQueryable()
        {
            throw new NotImplementedException($"Querying an event sourcing repository is not supported. Event sourcing repositories should only be used as write-model stores (cfr. CQRS)");
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

    }

}
