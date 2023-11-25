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
/// Defines the fundamentals of an <see cref="IRepository"/> implementation that supports optimistic concurrency control during entity updates
/// </summary>
public interface IConcurrentRepository
    : IRepository
{

    /// <summary>
    /// Updates the specified entity
    /// </summary>
    /// <param name="entity">The entity to update</param>
    /// <param name="expectedVersion">The version number that the entity is expected to have in the <see cref="IRepository"/> at the time of the update</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The updated entity</returns>
    Task<IIdentifiable> UpdateAsync(IIdentifiable entity, ulong expectedVersion, CancellationToken cancellationToken = default);

}

/// <summary>
/// Defines the fundamentals of an <see cref="IRepository"/> implementation that supports optimistic concurrency control during entity updates
/// </summary>
/// <typeparam name="TEntity">The type of the managed entities</typeparam>
public interface IConcurrentRepository<TEntity>
    : IConcurrentRepository, IRepository<TEntity>
    where TEntity : class, IIdentifiable
{

    /// <summary>
    /// Updates the specified entity
    /// </summary>
    /// <param name="entity">The entity to update</param>
    /// <param name="expectedVersion">The version number that the entity is expected to have in the <see cref="IRepository"/> at the time of the update</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The updated entity</returns>
    Task<TEntity> UpdateAsync(TEntity entity, ulong expectedVersion, CancellationToken cancellationToken = default);

}


/// <summary>
/// Defines the fundamentals of an <see cref="IRepository"/> implementation that supports optimistic concurrency control during entity updates
/// </summary>
/// <typeparam name="TEntity">The type of the managed entities</typeparam>
/// <typeparam name="TKey">The type of key used to identify and distinct managed entities</typeparam>
public interface IConcurrentRepository<TEntity, TKey>
    : IConcurrentRepository<TEntity>, IRepository<TEntity, TKey>
    where TEntity : class, IIdentifiable<TKey>
    where TKey : IEquatable<TKey>
{



}
