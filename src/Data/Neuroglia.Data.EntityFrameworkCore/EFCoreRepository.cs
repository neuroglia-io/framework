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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Neuroglia.Data
{

    /// <summary>
    /// Represents the default EntityFrameworkCore implementation of the <see cref="IRepository{TEntity, TKey}"/> interface
    /// </summary>
    /// <typeparam name="TEntity">The type of entity to store</typeparam>
    /// <typeparam name="TKey">The type of key used to uniquely identify stored entities</typeparam>
    /// <typeparam name="TContext">The type of <see cref="Microsoft.EntityFrameworkCore.DbContext"/> the <see cref="EFCoreRepository{TEntity, TKey, TDbContext}"/> belongs to</typeparam>
    public class EFCoreRepository<TEntity, TKey, TContext>
        : RepositoryBase<TEntity, TKey>
        where TEntity : class, IIdentifiable<TKey>
        where TKey : IEquatable<TKey>
        where TContext : DbContext
    {

        /// <summary>
        /// Initializes a new <see cref="EFCoreRepository{TEntity, TKey, TDbContext}"/>
        /// </summary>
        /// <param name="logger">The service used to perform logging</param>
        /// <param name="dbContext">The <see cref="Microsoft.EntityFrameworkCore.DbContext"/> the <see cref="EFCoreRepository{TEntity, TKey, TDbContext}"/> belongs to</param>
        public EFCoreRepository(ILogger<EFCoreRepository<TEntity, TKey, TContext>> logger, TContext dbContext)
        {
            this.Logger = logger;
            this.DbContext = dbContext;
            this.DbSet = this.DbContext.Set<TEntity>();
            if (this.DbSet == null)
                throw new NullReferenceException($"Failed to find a DbSet of type '{typeof(TEntity).Name}' in the DbContext of type '{typeof(TContext).Name}'");
        }

        /// <summary>
        /// Gets the service used to perform logging
        /// </summary>
        protected ILogger Logger { get; }

        /// <summary>
        /// Gets the <see cref="Microsoft.EntityFrameworkCore.DbContext"/> the <see cref="EFCoreRepository{TEntity, TKey, TDbContext}"/> belongs to
        /// </summary>
        protected TContext DbContext { get; }

        /// <summary>
        /// Gets the underlying <see cref="DbSet{TEntity}"/>
        /// </summary>
        protected DbSet<TEntity> DbSet { get; }

        /// <inheritdoc/>
        public override async Task<bool> ContainsAsync(TKey key, CancellationToken cancellationToken = default)
        {
            return await this.DbSet.AnyAsync(e => e.Id.Equals(key), cancellationToken);
        }

        /// <inheritdoc/>
        public override async Task<TEntity> FindAsync(TKey key, CancellationToken cancellationToken = default)
        {
            return await this.DbSet.FindAsync(new object[] { key }, cancellationToken);
        }

        /// <inheritdoc/>
        public override async Task<TEntity> FindAsync(object[] keyValues, CancellationToken cancellationToken = default)
        {
            return await this.DbSet.FindAsync(keyValues, cancellationToken);
        }

        /// <inheritdoc/>
        public override async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            return (await this.DbSet.AddAsync(entity, cancellationToken)).Entity;
        }

        /// <inheritdoc/>
        public override async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            this.DbContext.Entry(entity).State = EntityState.Modified;
            return await Task.FromResult(entity);
        }

        /// <inheritdoc/>
        public override async Task<TEntity> RemoveAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            return await Task.Run(() => this.DbSet.Remove(entity)?.Entity, cancellationToken);
        }

        /// <inheritdoc/>
        public override async Task RemoveAsync(TKey key, CancellationToken cancellationToken = default)
        {
            TEntity entity = await this.FindAsync(key, cancellationToken);
            if (entity != null)
                this.DbSet.Remove(entity);
        }

        /// <inheritdoc/>
        public override async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            List<IAggregateRoot> aggregates = this.DbContext.ChangeTracker.Entries<IAggregateRoot>()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified)
                .Select(e => e.Entity)
                .ToList();
            await this.DbContext.SaveChangesAsync(cancellationToken);
            //TODO: publish events
            aggregates.ForEach(a => a.ClearPendingEvents());
        }

        /// <inheritdoc/>
        public override async Task<List<TEntity>> ToListAsync(CancellationToken cancellationToken = default)
        {
            return await this.DbSet.ToListAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public override IQueryable<TEntity> AsQueryable()
        {
            return this.DbSet;
        }

    }

}
