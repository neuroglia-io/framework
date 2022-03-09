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
using Neuroglia.Caching;
using Neuroglia.Mediation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Neuroglia.Data
{

    /// <summary>
    /// Represents an <see cref="IRepository{TEntity, TKey}"/> implementation that uses an <see cref="IDistributedCache"/> to persist data
    /// </summary>
    public class DistributedCacheRepository<TEntity, TKey>
        : RepositoryBase<TEntity, TKey>
        where TEntity : class, IIdentifiable<TKey>
        where TKey : IEquatable<TKey>
    {

        /// <summary>
        /// Initializes a new <see cref="DistributedCacheRepository{TEntity, TKey}"/>
        /// </summary>
        /// <param name="logger">The service used to perform logging</param>
        /// <param name="cache">The <see cref="IDistributedCache"/> used to persist data managed by the <see cref="DistributedCacheRepository{TEntity, TKey}"/></param>
        /// <param name="mediator">The service used to mediate calls</param>
        public DistributedCacheRepository(ILogger<DistributedCacheRepository<TEntity, TKey>> logger, IDistributedCache cache, IMediator mediator)
        {
            this.Logger = logger;
            this.Cache = cache;
            this.Mediator = mediator;
        }

        /// <summary>
        /// Initializes a new <see cref="DistributedCacheRepository{TEntity, TKey}"/>
        /// </summary>
        /// <param name="logger">The service used to perform logging</param>
        /// <param name="cache">The <see cref="IDistributedCache"/> used to persist data managed by the <see cref="DistributedCacheRepository{TEntity, TKey}"/></param>
        public DistributedCacheRepository(ILogger<DistributedCacheRepository<TEntity, TKey>> logger, IDistributedCache cache)
            : this(logger, cache, null)
        {
            
        }

        /// <summary>
        /// Gets the service used to perform logging
        /// </summary>
        protected ILogger Logger { get; }

        /// <summary>
        /// Gets the <see cref="IDistributedCache"/> used to persist data managed by the <see cref="DistributedCacheRepository{TEntity, TKey}"/>
        /// </summary>
        protected IDistributedCache Cache { get; }

        /// <summary>
        /// Gets the service used to mediate calls
        /// </summary>
        protected IMediator Mediator { get; }

        /// <summary>
        /// Gets a string that represents the key used to store entities in the cache
        /// </summary>
        protected virtual string CacheKey
        {
            get
            {
                return typeof(TEntity).FullName;
            }
        }

        /// <summary>
        /// Gets a <see cref="List{T}"/> containing all <see cref="IAggregateRoot"/>s pending the publication of their <see cref="IDomainEvent"/>s
        /// </summary>
        protected List<IAggregateRoot> PendingAggregates { get; } = new List<IAggregateRoot>();

        /// <inheritdoc/>
        public override async Task<bool> ContainsAsync(TKey key, CancellationToken cancellationToken = default)
        {
            return await this.Cache.ContainsListElementAsync(this.CacheKey, key.ToString(), cancellationToken);
        }

        /// <inheritdoc/>
        public override async Task<TEntity> FindAsync(TKey key, CancellationToken cancellationToken = default)
        {
            return await this.Cache.GetListElementAsync<TEntity>(this.CacheKey, key.ToString(), cancellationToken);
        }

        /// <inheritdoc/>
        public override async Task<TEntity> FindAsync(object[] keyValues, CancellationToken cancellationToken = default)
        {
            if (keyValues == null
                || keyValues.Length != 1)
                throw new ArgumentOutOfRangeException(nameof(keyValues));
            return await this.FindAsync((TKey)keyValues[0], cancellationToken);
        }

        /// <inheritdoc/>
        public override async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            if (entity is IAggregateRoot aggregate)
                this.PendingAggregates.Add(aggregate);
            await this.Cache.AddToListAsync(this.CacheKey, entity.Id.ToString(), entity, cancellationToken);
            return entity;
        }

        /// <inheritdoc/>
        public override async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            return await this.AddAsync(entity, cancellationToken);
        }

        /// <inheritdoc/>
        public override async Task<TEntity> RemoveAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            await this.Cache.RemoveFromListAsync(this.CacheKey, entity.Id.ToString(), cancellationToken);
            return entity;
        }

        /// <inheritdoc/>
        public override async Task<List<TEntity>> ToListAsync(CancellationToken cancellationToken = default)
        {
            return await this.Cache.GetListAsync<TEntity>(this.CacheKey, cancellationToken);
        }

        /// <inheritdoc/>
        public override IQueryable<TEntity> AsQueryable()
        {
            return this.Cache.GetList<TEntity>(this.CacheKey).AsQueryable();
        }

        /// <inheritdoc/>
        public override async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            if (this.Mediator != null)
            {
                foreach (IAggregateRoot aggregate in this.PendingAggregates.ToList())
                {
                    foreach (IDomainEvent e in aggregate.PendingEvents.ToList())
                    {
                        await this.Mediator.PublishAsync((dynamic)e, cancellationToken);
                    }
                }
            }
            this.PendingAggregates.ToList()
                .ForEach(a => a.ClearPendingEvents());
            this.PendingAggregates.Clear();
        }

    }

}
