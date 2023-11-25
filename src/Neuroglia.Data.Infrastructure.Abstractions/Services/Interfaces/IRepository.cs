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

namespace Neuroglia.Data.Infrastructure.Services;

/// <summary>
/// Defines the fundamentals of a service used to manage entities
/// </summary>
public interface IRepository
{

    /// <summary>
    /// Gets the entity with the specified key, if any
    /// </summary>
    /// <param name="key">The key of the entity to find</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The entity with the specified key</returns>
    Task<IIdentifiable?> GetAsync(object key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds the specified entity to the <see cref="IRepository"/>
    /// </summary>
    /// <param name="entity">The entity to add</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The newly added entity</returns>
    Task<IIdentifiable> AddAsync(IIdentifiable entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the specified entity
    /// </summary>
    /// <param name="entity">The entity to update</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The updated entity</returns>
    Task<IIdentifiable> UpdateAsync(IIdentifiable entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes the specified entity from the <see cref="IRepository"/>
    /// </summary>
    /// <param name="entity">The entity to remove</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A boolean indicating whether or not the entity could be removed</returns>
    Task<bool> RemoveAsync(IIdentifiable entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Determines whether or not the <see cref="IRepository"/> contains an entity with the specified key
    /// </summary>
    /// <param name="key">The key to check</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A boolean indicating whether or not the <see cref="IRepository{TEntity}"/> contains an entity with the specified key</returns>
    Task<bool> ContainsAsync(object key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves all pending changes
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);

}

/// <summary>
/// Defines the fundamentals of a service used to manage entities of the specified type
/// </summary>
/// <typeparam name="TEntity">The type of the managed entities</typeparam>
public interface IRepository<TEntity>
    : IRepository
    where TEntity : class, IIdentifiable
{

    /// <summary>
    /// Finds the entity with the specified key
    /// </summary>
    /// <param name="key">The key of the entity to find</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The entity with the specified key</returns>
    new Task<TEntity?> GetAsync(object key, CancellationToken cancellationToken = default);

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
    /// <returns>A boolean indicating whether or not the entity could be removed</returns>
    Task<bool> RemoveAsync(TEntity entity, CancellationToken cancellationToken = default);

}

/// <summary>
/// Defines the fundamentals of a service used to manage entities of the specified type
/// </summary>
/// <typeparam name="TEntity">The type of the managed entities</typeparam>
/// <typeparam name="TKey">The type of key used to identify and distinct managed entities</typeparam>
public interface IRepository<TEntity, TKey>
    : IRepository<TEntity>
    where TEntity : class, IIdentifiable<TKey>
    where TKey : IEquatable<TKey>
{

    /// <summary>
    /// Gets the entity with the specified key, if any
    /// </summary>
    /// <param name="key">The key of the entity to find</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The entity with the specified key</returns>
    Task<TEntity?> GetAsync(TKey key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes the specified entity from the <see cref="IRepository"/>
    /// </summary>
    /// <param name="key">The key of the entity to remove</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A boolean indicating whether or not the entity could be removed</returns>
    Task<bool> RemoveAsync(TKey key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Determines whether or not the <see cref="IRepository"/> contains an entity with the specified key
    /// </summary>
    /// <param name="key">The key to check</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A boolean indicating whether or not the <see cref="IRepository{TEntity}"/> contains an entity with the specified key</returns>
    Task<bool> ContainsAsync(TKey key, CancellationToken cancellationToken = default);

}