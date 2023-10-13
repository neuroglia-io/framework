namespace Neuroglia.Data.Infrastructure.Services;

/// <summary>
/// Represents the default base class implementation of the <see cref="IRepository{TEntity, TKey}"/> interface
/// </summary>
/// <typeparam name="TEntity">The type of the managed entities</typeparam>
/// <typeparam name="TKey">The type of key used to identify and distinct managed entities</typeparam>
public abstract class RepositoryBase<TEntity, TKey>
    : IRepository<TEntity, TKey>
    where TEntity : class, IIdentifiable<TKey>
    where TKey : IEquatable<TKey>
{

    /// <inheritdoc/>
    public abstract Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <inheritdoc/>
    public abstract Task<bool> ContainsAsync(TKey key, CancellationToken cancellationToken = default);

    /// <inheritdoc/>
    public abstract Task<TEntity?> GetAsync(TKey key, CancellationToken cancellationToken = default);

    /// <inheritdoc/>
    public abstract Task<bool> RemoveAsync(TKey key, CancellationToken cancellationToken = default);

    /// <inheritdoc/>
    public abstract Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <inheritdoc/>
    public virtual Task<bool> RemoveAsync(TEntity entity, CancellationToken cancellationToken = default) => this.RemoveAsync(entity.Id, cancellationToken);

    /// <inheritdoc/>
    public abstract Task SaveChangesAsync(CancellationToken cancellationToken = default);

    async Task<IIdentifiable> IRepository.AddAsync(IIdentifiable entity, CancellationToken cancellationToken) => await this.AddAsync((TEntity)entity, cancellationToken).ConfigureAwait(false);

    Task<bool> IRepository.ContainsAsync(object key, CancellationToken cancellationToken) => this.ContainsAsync((TKey)key, cancellationToken);

    async Task<IIdentifiable?> IRepository.GetAsync(object key, CancellationToken cancellationToken) => await this.GetAsync((TKey)key, cancellationToken).ConfigureAwait(false);

    Task<TEntity?> IRepository<TEntity>.GetAsync(object key, CancellationToken cancellationToken) => this.GetAsync((TKey)key, cancellationToken);

    async Task<IIdentifiable> IRepository.UpdateAsync(IIdentifiable entity, CancellationToken cancellationToken) => await this.UpdateAsync((TEntity)entity, cancellationToken).ConfigureAwait(false);

    Task<bool> IRepository.RemoveAsync(IIdentifiable entity, CancellationToken cancellationToken) => this.RemoveAsync((TEntity)entity, cancellationToken);

}

