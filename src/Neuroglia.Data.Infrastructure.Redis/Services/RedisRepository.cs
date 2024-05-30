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

namespace Neuroglia.Data.Infrastructure.Redis.Services;

/// <summary>
/// Represents the default <see href="https://www.mongodb.com/">MongoDB</see> implementation of the <see cref="IRepository{T, TKey}"/> interface
/// </summary>
/// <typeparam name="TEntity">The type of entities managed by the repository</typeparam>
/// <typeparam name="TKey">The type of key used to uniquely identify entities managed by the repository</typeparam>
/// <param name="logger">The service used to perform logging</param>
/// <param name="redis">The current <see cref="IConnectionMultiplexer"/></param>
/// <param name="serializer">The service used to serialize/deserialize objects to/from JSON</param>
public class RedisRepository<TEntity, TKey>(ILogger<RedisRepository<TEntity, TKey>> logger, IConnectionMultiplexer redis, IJsonSerializer serializer)
    : RepositoryBase<TEntity, TKey>, IRepository<TEntity, TKey>, IConcurrentRepository<TEntity, TKey>, IDisposable
    where TEntity : class, IIdentifiable<TKey>
    where TKey : IEquatable<TKey>
{

    bool _disposed;

    /// <summary>
    /// Gets the service used to perform logging
    /// </summary>
    protected ILogger Logger { get; } = logger;

    /// <summary>
    /// Gets the current <see cref="IConnectionMultiplexer"/>
    /// </summary>
    protected IConnectionMultiplexer Redis { get; } = redis;

    /// <summary>
    /// Gets the current <see cref="IDatabase"/>
    /// </summary>
    protected IDatabase Database { get; } = redis.GetDatabase();

    /// <summary>
    /// Gets the service used to serialize/deserialize objects to/from JSON
    /// </summary>
    protected IJsonSerializer Serializer { get; } = serializer;

    /// <inheritdoc/>
    public override Task<bool> ContainsAsync(TKey id, CancellationToken cancellationToken = default) => this.Database.KeyExistsAsync(this.BuildEntityKey(id));

    /// <inheritdoc/>
    public override Task<TEntity?> GetAsync(TKey id, CancellationToken cancellationToken = default) => this.ReadEntityAsync(id, cancellationToken);

    /// <inheritdoc/>
    public override async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        if (await this.ContainsAsync(entity.Id, cancellationToken).ConfigureAwait(false)) throw new Exception($"An entity with the specified key '{entity.Id}' already exists");
        return await this.WriteEntityAsync(entity, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default) => this.ReplaceOneAsync(entity, null, cancellationToken);

    /// <inheritdoc/>
    public virtual Task<TEntity> UpdateAsync(TEntity entity, ulong expectedVersion, CancellationToken cancellationToken = default) => this.ReplaceOneAsync(entity, expectedVersion, cancellationToken);

    /// <summary>
    /// Updates the specified entity
    /// </summary>
    /// <param name="entity">The updated state of the entity</param>
    /// <param name="expectedVersion">The entity's expected version, if any</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The updated entity</returns>
    protected virtual async Task<TEntity> ReplaceOneAsync(TEntity entity, ulong? expectedVersion, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        if (!await this.ContainsAsync(entity.Id, cancellationToken).ConfigureAwait(false)) throw new NullReferenceException($"Failed to find an entity of type {typeof(TEntity).Name} with the specified key '{entity.Id}'");
        return await this.WriteEntityAsync(entity, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override Task<bool> RemoveAsync(TKey id, CancellationToken cancellationToken = default) => this.Database.KeyDeleteAsync(this.BuildEntityKey(id));

    /// <inheritdoc/>
    public override Task<bool> RemoveAsync(TEntity entity, CancellationToken cancellationToken = default) => this.RemoveAsync(entity.Id, cancellationToken);

    /// <inheritdoc/>
    public override Task SaveChangesAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

    /// <summary>
    /// Builds a Redis key based on the specified parameters
    /// </summary>
    /// <param name="id">The key of the entity to build a new Redis key for</param>
    /// <returns>The specified resource's key</returns>
    protected virtual string BuildEntityKey(TKey id) => $"{typeof(TEntity).Name.ToCamelCase()}-{id}";

    /// <summary>
    /// Writes the specified entity to the underlying Redis database
    /// </summary>
    /// <param name="entity">The entity to write</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The entity that has been written to the underlying Redis database</returns>
    protected virtual async Task<TEntity> WriteEntityAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var key = this.BuildEntityKey(entity.Id);
        var json = this.Serializer.SerializeToText(entity);
        await this.Database.StringSetAsync(key, json).ConfigureAwait(false);
        return (await this.GetAsync(entity.Id, cancellationToken).ConfigureAwait(false))!;
    }

    /// <summary>
    /// Reads the entity with the specified key from the underlying Redis database
    /// </summary>
    /// <param name="id">The id of the entity to read</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The entity with the specified id, if any</returns>
    protected virtual async Task<TEntity?> ReadEntityAsync(TKey id, CancellationToken cancellationToken = default)
    {
        var key = this.BuildEntityKey(id);
        var json = (string)(await this.Database.StringGetAsync(key).ConfigureAwait(false))!;
        return string.IsNullOrWhiteSpace(json) ? null : this.Serializer.Deserialize<TEntity?>(json!);
    }

    async Task<IIdentifiable> IConcurrentRepository.UpdateAsync(IIdentifiable entity, ulong expectedVersion, CancellationToken cancellationToken) => await this.UpdateAsync((TEntity)entity, expectedVersion, cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Disposes of the <see cref="RedisRepository{TEntity, TKey}"/>
    /// </summary>
    /// <param name="disposing">A boolean indicating whether or not the <see cref="RedisRepository{TEntity, TKey}"/> is being disposed of</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!this._disposed)
        {
            if (disposing)
            {
                
            }
            this._disposed = true;
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

}
