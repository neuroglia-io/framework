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
using EventStore.ClientAPI;
using EventStore.ClientAPI.Projections;
using Microsoft.Extensions.Logging;
using Neuroglia.Data.EventSourcing;
using Neuroglia.Data.EventSourcing.EventStore;
using Neuroglia.Serialization;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Neuroglia.Data
{

    /// <summary>
    /// Represents the <see href="https://www.eventstore.com/">Event Store</see> implementation of the <see cref="IEventSourcingRepository{TAggregate, TKey}"/>
    /// </summary>
    /// <typeparam name="TAggregate">The type of <see cref="IAggregateRoot"/> managed by the <see cref="EventStoreRepository{TAggregate}"/></typeparam>
    public class EventStoreRepository<TAggregate>
        : RepositoryBase<TAggregate, string>, IEventSourcingRepository<TAggregate, string>
        where TAggregate : class, IAggregateRoot<string>
    {

        /// <summary>
        /// Initializes a new <see cref="EventStoreRepository{TAggregate}"/>
        /// </summary>
        /// <param name="loggerFactory">The service used to create <see cref=" Microsoft.Extensions.Logging.ILogger"/>s</param>
        /// <param name="serializer">The service used to serialize/deserialize events to/from managed streams</param>
        /// <param name="aggregatorFactory">The service used to create <see cref="IAggregator"/>s</param>
        /// <param name="connection">The underlying connection to the EventStore remote server</param>
        /// <param name="projectionsManager">The service used to manage EventStore projections</param>
        public EventStoreRepository(ILoggerFactory loggerFactory, ISerializer serializer, IAggregatorFactory aggregatorFactory, IEventStoreConnection connection, ProjectionsManager projectionsManager)
        {
            this.Logger = loggerFactory.CreateLogger(this.GetType());
            this.Serializer = serializer;
            this.AggregatorFactory = aggregatorFactory;
            this.Connection = connection;
            this.ProjectionsManager = projectionsManager;
        }

        /// <summary>
        /// Gets the service used to perform logging
        /// </summary>
        protected Microsoft.Extensions.Logging.ILogger Logger { get; }

        /// <summary>
        /// Gets the service used to serialize/deserialize events to/from managed streams
        /// </summary>
        protected ISerializer Serializer { get; }

        /// <summary>
        /// Gets the service used to create <see cref="IAggregator"/>s
        /// </summary>
        protected IAggregatorFactory AggregatorFactory { get; }

        /// <summary>
        /// Gets the underlying connection to the EventStore remote server
        /// </summary>
        protected IEventStoreConnection Connection { get; }

        /// <summary>
        /// Gets the service used to manage EventStore projections
        /// </summary>
        protected internal ProjectionsManager ProjectionsManager { get; }

        /// <summary>
        /// Gets a <see cref="Dictionary{TKey, TValue}"/> containing all pending <see cref="EventStoreTransaction"/>s
        /// </summary>
        protected ConcurrentDictionary<string, EventStoreTransaction> PendingTransactions { get; } = new();

        /// <summary>
        /// Gets the prefix for all streams managed by the <see cref="EventStoreRepository{TAggregate}"/>
        /// </summary>
        protected virtual string StreamPrefix
        {
            get
            {
                return typeof(TAggregate).Name.ToLower();
            }
        }

        /// <inheritdoc/>
        public virtual async Task<IEventStream<string>> GetStreamAsync(string streamId, CancellationToken cancellationToken = default)
        {
            if (!await this.ContainsAsync(streamId, cancellationToken))
                return default;
            RecordedEvent firstEvent = (await this.Connection.ReadEventAsync(this.GetStreamNameFor(streamId), StreamPosition.Start, false)).Event.Value.Event;
            RecordedEvent lastEvent = (await this.Connection.ReadEventAsync(this.GetStreamNameFor(streamId), StreamPosition.End, false)).Event.Value.Event;
            return new EventStream<string>(this.Connection, this.GetStreamNameFor(streamId), lastEvent.EventNumber, firstEvent.Created, lastEvent.Created);
        }

        async Task<IEventStream> IEventSourcingRepository<TAggregate>.GetStreamAsync(object streamId, CancellationToken cancellationToken)
        {
            if (streamId == null)
                throw new ArgumentNullException(nameof(streamId));
            return await this.GetStreamAsync((string)streamId, cancellationToken);
        }

        /// <inheritdoc/>
        public override async Task<bool> ContainsAsync(string key, CancellationToken cancellationToken = default)
        {
            return await this.ContainsAsync(this.GetStreamNameFor(key), cancellationToken);
        }

        /// <inheritdoc/>
        public virtual async Task<TAggregate> FindAsync(string key, long version, CancellationToken cancellationToken = default)
        {
            return this.AggregatorFactory.CreateAggregator<TAggregate>()
                .Aggregate(await this.Connection.ReadAndAbstractStreamEventsForwardAsync(this.GetStreamNameFor(key), version));
        }

        /// <inheritdoc/>
        public override Task<TAggregate> FindAsync(string key, CancellationToken cancellationToken = default)
        {
            return this.FindAsync(key, StreamPosition.Start, cancellationToken);
        }

        Task<TAggregate> IEventSourcingRepository<TAggregate>.FindAsync(object streamId, long version, CancellationToken cancellationToken)
        {
            return this.FindAsync(streamId.ToString(), version, cancellationToken);
        }

        /// <inheritdoc/>
        public override Task<TAggregate> FindAsync(object[] keyValues, CancellationToken cancellationToken = default)
        {
            if (keyValues == null
                || !keyValues.Any())
                throw new ArgumentNullException(nameof(keyValues));
            return this.FindAsync(keyValues[0].ToString(), cancellationToken);
        }

        /// <inheritdoc/>
        public override async Task<TAggregate> AddAsync(TAggregate entity, CancellationToken cancellationToken = default)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            if (await this.ContainsAsync(entity.Id, cancellationToken))
                throw new InvalidOperationException($"An aggregate with the specified id '{entity.Id}' already exists");
            return await this.UpdateAsync(entity, cancellationToken);
        }

        /// <inheritdoc/>
        public override async Task<TAggregate> UpdateAsync(TAggregate entity, CancellationToken cancellationToken = default)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            if (!entity.PendingEvents.Any())
                return entity;
            IEnumerable<IDomainEvent> events = entity.PendingEvents;
            IEnumerable<EventData> eventsData = await this.ProcessEventsDataAsync(events, cancellationToken);
            if (this.PendingTransactions.TryGetValue(this.GetStreamNameFor(entity.Id), out EventStoreTransaction transaction))
                throw new NotImplementedException(); //TODO: await transaction.UnderlyingTransaction.WriteAsync(eventsData);
            else
                await this.Connection.AppendToStreamAsync(entity.Id.ToString(), entity.Version, eventsData);
            entity.ClearPendingEvents();
            return entity;
        }

        /// <inheritdoc/>
        public override async Task<TAggregate> RemoveAsync(TAggregate entity, CancellationToken cancellationToken = default)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            await this.Connection.DeleteStreamAsync(this.GetStreamNameFor(entity.Id), ExpectedVersion.Any);
            return entity;
        }

        /// <inheritdoc/>
        public override Task<List<TAggregate>> ToListAsync(CancellationToken cancellationToken = default)
        {
            //TODO
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override IQueryable<TAggregate> AsQueryable()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            //TODO
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the stream name for the specified <see cref="IAggregateRoot"/>'s key
        /// </summary>
        /// <param name="aggregateId">The <see cref="IAggregateRoot"/>'s key to get the stream name for</param>
        /// <returns>The stream name for the specified <see cref="IAggregateRoot"/>'s key</returns>
        protected virtual string GetStreamNameFor(string aggregateId)
        {
            return $"{this.StreamPrefix}-{aggregateId.ToString().Replace("-", "")}";
        }

        /// <summary>
        /// Processes the specified <see cref="IDomainEvent"/>s
        /// </summary>
        /// <param name="events">An <see cref="IEnumerable{T}"/> containing the <see cref="IDomainEvent"/>s to process</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A new <see cref="IEnumerable{T}"/> containing the processed <see cref="EventData"/></returns>
        protected virtual async Task<IEnumerable<EventData>> ProcessEventsDataAsync(IEnumerable<IDomainEvent> events, CancellationToken cancellationToken = default)
        {
            List<EventData> eventDataList = new(events.Count());
            foreach (IDomainEvent e in events)
            {
                string type = e.GetType().Name.Slugify("-");
                byte[] data = await this.Serializer.SerializeAsync(e, cancellationToken);
                byte[] metadata = await this.Serializer.SerializeAsync(e, cancellationToken);
                eventDataList.Add(new EventData(Guid.NewGuid(), type, this.Serializer is IJsonSerializer, data, metadata));
            }
            return eventDataList;
        }

    }

}
