namespace Neuroglia.Data.Infrastructure.Services;

/// <summary>
/// Represents the default base class implementation of the <see cref="IRepository{TEntity, TKey}"/> interface
/// </summary>
/// <typeparam name="TEntity">The type of the managed entities</typeparam>
/// <typeparam name="TKey">The type of key used to identify and distinct managed entities</typeparam>
public abstract class QueryableRepositoryBase<TEntity, TKey>
    : RepositoryBase<TEntity, TKey>, IQueryableRepository<TEntity, TKey>
    where TEntity : class, IIdentifiable<TKey>
    where TKey : IEquatable<TKey>
{

    /// <inheritdoc/>
    public abstract IQueryable<TEntity> AsQueryable();

    IQueryable IQueryableRepository.AsQueryable() => this.AsQueryable();

}

