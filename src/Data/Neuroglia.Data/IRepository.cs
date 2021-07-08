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
    /// Defines the fundamentals of a service used to manage data
    /// </summary>
    /// <typeparam name="TEntity">The type of data managed by the <see cref="IRepository{T}"/></typeparam>
    public interface IRepository<TEntity>
        where TEntity : class, IIdentifiable
    {

        /// <summary>
        /// Finds the entity with the specified key
        /// </summary>
        /// <param name="key">The key of the entity to find</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>The entity with the specified key</returns>
        Task<TEntity> FindAsync(object key, CancellationToken cancellationToken = default);

        /// <summary>
        /// Finds the entity with the specified key values
        /// </summary>
        /// <param name="keyValues">The key values of the entity to find</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>The entity with the specified key values</returns>
        Task<TEntity> FindAsync(object[] keyValues, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds the specified entity to the <see cref="IRepository{TEntity}"/>
        /// </summary>
        /// <param name="entity">The entity to add</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>The newly added entity</returns>
        Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates the specified entity
        /// </summary>
        /// <param name="entity">The entity to update</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>The updated entity</returns>
        Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes the specified entity from the <see cref="IRepository{TEntity}"/>
        /// </summary>
        /// <param name="entity">The entity to remove</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>The removed entity</returns>
        Task<TEntity> RemoveAsync(TEntity entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes the specified entity from the <see cref="IRepository{TEntity}"/>
        /// </summary>
        /// <param name="key">The key of the entity to remove</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        Task RemoveAsync(object key, CancellationToken cancellationToken = default);

        /// <summary>
        /// Determines whether or not the <see cref="IRepository{TEntity}"/> contains an entity with the specified key
        /// </summary>
        /// <param name="key">The key to check</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A boolean indicating whether or not the <see cref="IRepository{TEntity}"/> contains an entity with the specified key</returns>
        Task<bool> ContainsAsync(object key, CancellationToken cancellationToken = default);

        /// <summary>
        /// Lists all entities contained in the <see cref="IRepository{TEntity}"/>
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A new <see cref="List{T}"/> containing all the entities contained in the <see cref="IRepository{TEntity}"/></returns>
        Task<List<TEntity>> ToListAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Queries the <see cref="IRepository{TEntity}"/>
        /// </summary>
        /// <returns>A new <see cref="IQueryable"/></returns>
        IQueryable<TEntity> AsQueryable();

        /// <summary>
        /// Saves all pending changes
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        Task SaveChangesAsync(CancellationToken cancellationToken = default);

    }

    /// <summary>
    /// Defines the fundamentals of a service used to manage data
    /// </summary>
    /// <typeparam name="TEntity">The type of data managed by the <see cref="IRepository{TEntity, TKey}"/></typeparam>
    /// <typeparam name="TKey">The type of key used to uniquely identify entities managed by the <see cref="IRepository{TEntity, TKey}"/></typeparam>
    public interface IRepository<TEntity, TKey>
        : IRepository<TEntity>
        where TEntity : class, IIdentifiable<TKey>
        where TKey : IEquatable<TKey>
    {

        /// <summary>
        /// Finds the entity with the specified key
        /// </summary>
        /// <param name="key">The key of the entity to find</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>The entity with the specified key</returns>
        Task<TEntity> FindAsync(TKey key, CancellationToken cancellationToken = default);

        /// <summary>
        /// Determines whether or not the <see cref="IRepository{TEntity}"/> contains an entity with the specified key
        /// </summary>
        /// <param name="key">The key to check</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A boolean indicating whether or not the <see cref="IRepository{TEntity}"/> contains an entity with the specified key</returns>
        Task<bool> ContainsAsync(TKey key, CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes the specified entity from the <see cref="IRepository{TEntity}"/>
        /// </summary>
        /// <param name="key">The key of the entity to remove</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        Task RemoveAsync(TKey key, CancellationToken cancellationToken = default);

    }

}
