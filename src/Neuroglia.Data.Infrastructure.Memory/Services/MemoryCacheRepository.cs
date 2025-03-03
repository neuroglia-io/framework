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

using Microsoft.Extensions.Caching.Memory;

namespace Neuroglia.Data.Infrastructure.Services;

/// <summary>
/// Represents an <see cref="IMemoryCache"/> implementation of the <see cref="IRepository"/> interface
/// </summary>
/// <typeparam name="TEntity">The type of the managed entities</typeparam>
/// <typeparam name="TKey">The type of key used to identify and distinct managed entities</typeparam>
[Factory(typeof(MemoryCacheRepositoryFactory<,>))]
public class MemoryCacheRepository<TEntity, TKey>
    : RepositoryBase<TEntity, TKey>, IQueryableRepository<TEntity, TKey>
    where TEntity : class, IIdentifiable<TKey>
    where TKey : IEquatable<TKey>
{

    /// <summary>
    /// Initializes a new <see cref="MemoryCacheRepository{TEntity, TKey}"/>
    /// </summary>
    /// <param name="cache">The <see cref="IMemoryCache"/> used to cache entities</param>
    public MemoryCacheRepository(IMemoryCache cache)
    {
        this.Cache = cache;
    }

    /// <summary>
    /// Gets the <see cref="IMemoryCache"/> used to cache entities
    /// </summary>
    protected IMemoryCache Cache { get; }

    /// <inheritdoc/>
    public override Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        var collectionKey = this.BuildCacheKey();
        var entityKey = this.BuildCacheKey(entity.Id);
        if (this.Cache.TryGetValue(collectionKey, out List<string>? keys) && keys != null)
        {
            if (keys.Contains(entityKey)) throw new Exception($"An entity with the specified id '{entity.Id}' already exists in the repository");
        }
        else keys = [];
        keys.Add(entityKey);
        this.Cache.Set(collectionKey, keys);
        this.Cache.Set(entityKey, entity);
        return Task.FromResult(entity);
    }

    /// <inheritdoc/>
    public override Task<bool> ContainsAsync(TKey key, CancellationToken cancellationToken = default) => Task.FromResult(this.Cache.TryGetValue(this.BuildCacheKey(key), out _));

    /// <inheritdoc/>
    public override Task<TEntity?> GetAsync(TKey key, CancellationToken cancellationToken = default) => Task.FromResult(this.Cache.TryGetValue<TEntity?>(this.BuildCacheKey(key), out var entity) && entity != null ? entity : null);

    /// <inheritdoc/>
    public override Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        var entityKey = this.BuildCacheKey(entity.Id);
        if (!this.Cache.TryGetValue(entityKey, out _)) throw new NullReferenceException($"Failed to find an entity with the specified id '{entity.Id}'");
        this.Cache.Set(entityKey, entity);
        return Task.FromResult(entity);
    }

    /// <inheritdoc/>
    public override Task<bool> RemoveAsync(TKey key, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(key);
        var entityKey = this.BuildCacheKey(key);
        var exists = this.Cache.TryGetValue(entityKey, out _);
        if (exists) this.Cache.Remove(entityKey);
        return Task.FromResult(exists);
    }

    /// <inheritdoc/>
    public override Task SaveChangesAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

    /// <inheritdoc/>
    public virtual IQueryable<TEntity> AsQueryable() => this.AsEnumerable().AsQueryable();

    /// <summary>
    /// Gets a new <see cref="IEnumerable{T}"/> containing all managed entities
    /// </summary>
    /// <returns>A new <see cref="IEnumerable{T}"/> containing all managed entities</returns>
    protected virtual IEnumerable<TEntity> AsEnumerable()
    {
        var collectionKey = this.BuildCacheKey();
        if (!this.Cache.TryGetValue(collectionKey, out List<string>? keys) || keys == null) yield break;
        foreach (var key in keys) yield return this.Cache.Get<TEntity>(key) ?? throw new NullReferenceException($"Failed to find an entity with the specified id '{key}'");
    }

    /// <summary>
    /// Builds a new cache key
    /// </summary>
    /// <param name="id">The id, if any, to build the cache key for</param>
    /// <returns>A new cache key</returns>
    protected virtual string BuildCacheKey(TKey? id = default) => id == null || id.Equals(default) ? nameof(TEntity).ToKebabCase() : $"{nameof(TEntity).ToKebabCase()}-{id}";

    IQueryable IQueryableRepository.AsQueryable() => this.AsQueryable();

}
