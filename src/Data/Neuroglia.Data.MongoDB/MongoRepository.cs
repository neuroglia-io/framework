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
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Neuroglia.Data
{

    /// <summary>
    /// Represents the default <see href="https://www.mongodb.com/">MongoDB</see> implementation of the <see cref="IRepository{T, TKey}"/> interface
    /// </summary>
    /// <typeparam name="TEntity">The type of entities managed by the repository</typeparam>
    /// <typeparam name="TKey">The type of key used to uniquely identify entities managed by the repository</typeparam>
    public class MongoRepository<TEntity, TKey>
        : RepositoryBase<TEntity, TKey>
        where TEntity : class, IIdentifiable<TKey>
        where TKey : IEquatable<TKey>
    {

        /// <summary>
        /// Initializes a new <see cref="MongoRepository{TDocument, TKey}"/>
        /// </summary>
        /// <param name="loggerFactory">The service used to create <see cref="ILogger"/>s</param>
        /// <param name="pluralizer">The service used to pluralize words</param>
        /// <param name="database">The <see cref="IMongoDatabase"/> the <see cref="MongoRepository{TDocument, TKey}"/> belongs to</param>
        /// <param name="options">The service used to access the <see cref="MongoRepository{TEntity, TKey}"/>'s options</param>
        public MongoRepository(ILoggerFactory loggerFactory, IOptions<MongoRepositoryOptions<TEntity, TKey>> options, IPluralizer pluralizer, IMongoDatabase database)
        {
            this.Logger = loggerFactory.CreateLogger(this.GetType());
            this.Options = options.Value;
            this.Pluralizer = pluralizer;
            this.Database = database;
            this.Collection = this.Database.GetCollection<TEntity>(this.Pluralizer.Pluralize(typeof(TEntity).Name), this.Options.CollectionSettings);
        }

        /// <summary>
        /// Gets the service used to perform logging
        /// </summary>
        protected ILogger Logger { get; }

        /// <summary>
        /// Gets the options used to configure the <see cref="MongoRepository{TEntity, TKey}"/>
        /// </summary>
        protected MongoRepositoryOptions Options { get; }

        /// <summary>
        /// Gets the service used to pluralize words
        /// </summary>
        public IPluralizer Pluralizer { get; }

        /// <summary>
        /// Gets the service used to interact with the MongoDB database the <see cref="MongoRepository{TEntity, TKey}"/> belongs to
        /// </summary>
        protected IMongoDatabase Database { get; }

        /// <summary>
        /// Gets the underlying <see cref="IMongoCollection{TDocument}"/>
        /// </summary>
        protected IMongoCollection<TEntity> Collection { get; }

        /// <summary>
        /// Gets a <see cref="List{T}"/> containing all <see cref="IAggregateRoot"/>s pending the publication of their <see cref="IDomainEvent"/>s
        /// </summary>
        protected List<IAggregateRoot> PendingAggregates { get; } = new List<IAggregateRoot>();

        /// <inheritdoc/>
        public override async Task<bool> ContainsAsync(TKey key, CancellationToken cancellationToken = default)
        {
            return await this.Collection.AsQueryable().AnyAsync(e => e.Id.Equals(key), cancellationToken);
        }

        /// <inheritdoc/>
        public override async Task<TEntity> FindAsync(object[] keyValues, CancellationToken cancellationToken = default)
        {
            if (keyValues == null
                || keyValues.Length < 1)
                throw new ArgumentNullException(nameof(keyValues));
            return await this.FindAsync((TKey)keyValues.First(), cancellationToken);
        }

        /// <inheritdoc/>
        public override async Task<TEntity> FindAsync(TKey key, CancellationToken cancellationToken = default)
        {
            return (await this.Collection.FindAsync(d => d.Id.Equals(key), new FindOptions<TEntity, TEntity>(), cancellationToken)).FirstOrDefault(cancellationToken);
        }

        /// <inheritdoc/>
        public override async Task<TEntity> AddAsync(TEntity elem, CancellationToken cancellationToken = default)
        {
            if (elem is IAggregateRoot aggregate)
                this.PendingAggregates.Add(aggregate);
            await this.Collection.InsertOneAsync(elem, new InsertOneOptions() { }, cancellationToken);
            return await this.FindAsync(elem.Id, cancellationToken);
        }

        /// <inheritdoc/>
        public override async Task<TEntity> UpdateAsync(TEntity elem, CancellationToken cancellationToken = default)
        {
            if (elem is IAggregateRoot aggregate)
                this.PendingAggregates.Add(aggregate);
            await this.Collection.ReplaceOneAsync(d => d.Id.Equals(elem.Id), elem, new ReplaceOptions(), cancellationToken);
            return await this.FindAsync(elem.Id, cancellationToken);
        }

        /// <inheritdoc/>
        public override async Task<TEntity> RemoveAsync(TEntity elem, CancellationToken cancellationToken = default)
        {
            await this.Collection.DeleteOneAsync(d => d.Id.Equals(elem.Id), cancellationToken);
            return elem;
        }

        /// <inheritdoc/>
        public override async Task RemoveAsync(TKey key, CancellationToken cancellationToken = default)
        {
            await this.Collection.DeleteOneAsync(d => d.Id.Equals(key), cancellationToken);
        }

        /// <inheritdoc/>
        public override async Task<List<TEntity>> ToListAsync(CancellationToken cancellationToken = default)
        {
            return (await this.Collection.FindAsync(d => true, new FindOptions<TEntity, TEntity>(), cancellationToken)).ToList(cancellationToken);
        }

        /// <inheritdoc/>
        public override Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            if (this.PendingAggregates.Any())
            {
                foreach (IAggregateRoot aggregate in this.PendingAggregates.ToList())
                {
                    foreach (IDomainEvent e in aggregate.PendingEvents)
                    {
                        //TODO: publish domain events
                    }
                    aggregate.ClearPendingEvents();
                    this.PendingAggregates.Remove(aggregate);
                }
                this.PendingAggregates.Clear();
            }
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public override IQueryable<TEntity> AsQueryable()
        {
            return this.Collection.AsQueryable();
        }

    }


}
