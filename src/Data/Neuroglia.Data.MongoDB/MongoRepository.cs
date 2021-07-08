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
    /// <typeparam name="TContext">The type of <see cref="IMongoDbContext"/> the repository belongs to</typeparam>
    public class MongoRepository<TEntity, TKey, TContext>
        : RepositoryBase<TEntity, TKey>
        where TEntity : class, IIdentifiable<TKey>
        where TKey : IEquatable<TKey>
        where TContext : class, IMongoDbContext
    {

        /// <summary>
        /// Initializes a new <see cref="MongoRepository{TDocument, TKey, TDbContext}"/>
        /// </summary>
        /// <param name="loggerFactory">The service used to create <see cref="ILogger"/>s</param>
        /// <param name="dbContext">The <see cref="IMongoDbContext"/> the <see cref="MongoRepository{TDocument, TKey, TDbContext}"/> belongs to</param>
        public MongoRepository(ILoggerFactory loggerFactory, TContext dbContext)
        {
            this.Logger = loggerFactory.CreateLogger(this.GetType());
            this.DbContext = dbContext;
            this.Collection = this.DbContext.Collection<TEntity>();
        }

        /// <summary>
        /// Gets the service used to perform logging
        /// </summary>
        protected ILogger Logger { get; }

        /// <summary>
        /// Gets the <see cref="IMongoDbContext"/> the <see cref="MongoRepository{TDocument, TKey, TDbContext}"/> belongs to
        /// </summary>
        protected TContext DbContext { get; }

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
