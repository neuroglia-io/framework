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
/// Defines the fundamentals of a queryable <see cref="IRepository"/>
/// </summary>
public interface IQueryableRepository
    : IRepository
{

    /// <summary>
    /// Creates a new query against the repository
    /// </summary>
    /// <returns>A new query</returns>
    IQueryable AsQueryable();

}

/// <summary>
/// Defines the fundamentals of a queryable <see cref="IRepository"/>
/// </summary>
/// <typeparam name="TEntity">The type of entity managed by the repository</typeparam>
public interface IQueryableRepository<TEntity>
    : IRepository<TEntity>, IQueryableRepository
    where TEntity : class, IIdentifiable
{

    /// <summary>
    /// Creates a new query against the repository
    /// </summary>
    /// <returns>A new query</returns>
    new IQueryable<TEntity> AsQueryable();

}

/// <summary>
/// Defines the fundamentals of a queryable <see cref="IRepository"/>
/// </summary>
/// <typeparam name="TEntity">The type of entity managed by the repository</typeparam>
/// <typeparam name="TKey">The type of key used to identify entities managed by the repository</typeparam>
public interface IQueryableRepository<TEntity, TKey>
    : IRepository<TEntity, TKey>, IQueryableRepository<TEntity>
    where TEntity : class, IIdentifiable<TKey>
    where TKey : IEquatable<TKey>
{

    /// <summary>
    /// Creates a new query against the repository
    /// </summary>
    /// <returns>A new query</returns>
    new IQueryable<TEntity> AsQueryable();

}