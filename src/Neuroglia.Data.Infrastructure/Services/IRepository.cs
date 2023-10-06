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
    Task<object?> GetAsync(object key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds the specified entity to the <see cref="IRepository"/>
    /// </summary>
    /// <param name="entity">The entity to add</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The newly added entity</returns>
    Task<object> AddAsync(object entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the specified entity
    /// </summary>
    /// <param name="entity">The entity to update</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The updated entity</returns>
    Task<object> UpdateAsync(object entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes the specified entity from the <see cref="IRepository"/>
    /// </summary>
    /// <param name="entity">The entity to remove</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A boolean indicating whether or not the entity could be removed</returns>
    Task<bool> RemoveAsync(object entity, CancellationToken cancellationToken = default);

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
