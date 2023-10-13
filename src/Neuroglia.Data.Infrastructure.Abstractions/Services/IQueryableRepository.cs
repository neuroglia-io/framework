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