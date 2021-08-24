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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Neuroglia.Data
{

    /// <summary>
    /// Represents an abstract implementation of the <see cref="IRepository{TEntity, TKey}"/> interface
    /// </summary>
    /// <typeparam name="TEntity">The type of data managed by the <see cref="IRepository{TEntity, TKey}"/></typeparam>
    /// <typeparam name="TKey">The type of key used to uniquely identify entities managed by the <see cref="IRepository{TEntity, TKey}"/></typeparam>
    public abstract class RepositoryBase<TEntity, TKey>
        : IRepository<TEntity, TKey>
        where TEntity : class, IIdentifiable<TKey>
        where TKey : IEquatable<TKey>
    {

        /// <inheritdoc/>
        public abstract Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);

        async Task<object> IRepository.AddAsync(object entity, CancellationToken cancellationToken)
        {
            return await this.AddAsync((TEntity)entity, cancellationToken);
        }

        /// <inheritdoc/>
        public abstract Task<TEntity> FindAsync(TKey key, CancellationToken cancellationToken = default);

        Task<TEntity> IRepository<TEntity>.FindAsync(object key, CancellationToken cancellationToken)
        {
            return this.FindAsync((TKey)key, cancellationToken);
        }

        async Task<object> IRepository.FindAsync(object key, CancellationToken cancellationToken)
        {
            return await this.FindAsync((TKey)key, cancellationToken);
        }

        /// <inheritdoc/>
        public abstract Task<TEntity> FindAsync(object[] keyValues, CancellationToken cancellationToken = default);

        async Task<object> IRepository.FindAsync(object[] keyValues, CancellationToken cancellationToken)
        {
            return await this.FindAsync(keyValues, cancellationToken);
        }

        /// <inheritdoc/>
        public abstract Task<bool> ContainsAsync(TKey key, CancellationToken cancellationToken = default);

        /// <inheritdoc/>
        Task<bool> IRepository.ContainsAsync(object key, CancellationToken cancellationToken) => this.ContainsAsync((TKey)key, cancellationToken);

        /// <inheritdoc/>
        public abstract Task<TEntity> RemoveAsync(TEntity entity, CancellationToken cancellationToken = default);

        async Task<object> IRepository.RemoveAsync(object entity, CancellationToken cancellationToken)
        {
            return await this.RemoveAsync((TEntity)entity, cancellationToken);
        }

        /// <inheritdoc/>
        public virtual async Task RemoveAsync(TKey key, CancellationToken cancellationToken = default)
        {
            TEntity entity = await this.FindAsync(key, cancellationToken);
            if (entity == null)
                throw new NullReferenceException($"Failed to find an entity of type '{typeof(TEntity).Name}' with the specified key '{key}'");
            await this.RemoveAsync(entity, cancellationToken);
        }

        /// <inheritdoc/>
        public abstract Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

        async Task<object> IRepository.UpdateAsync(object entity, CancellationToken cancellationToken)
        {
            return await this.UpdateAsync((TEntity)entity, cancellationToken);
        }

        /// <inheritdoc/>
        public abstract IQueryable<TEntity> AsQueryable();

        IQueryable IRepository.AsQueryable()
        {
            return this.AsQueryable();
        }

        /// <inheritdoc/>
        public abstract Task<List<TEntity>> ToListAsync(CancellationToken cancellationToken = default);

        async Task<List<object>> IRepository.ToListAsync(CancellationToken cancellationToken)
        {
            return (await this.ToListAsync(cancellationToken))
                .OfType<object>()
                .ToList();
        }

        /// <inheritdoc/>
        public abstract Task SaveChangesAsync(CancellationToken cancellationToken = default);

    }

}
