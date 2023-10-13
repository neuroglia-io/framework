using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Neuroglia.Plugins.Services;

namespace Neuroglia.Data.Infrastructure.Services;

/// <summary>
/// Represents a service used to create <see cref="MemoryCacheRepository{TEntity, TKey}"/> instances
/// </summary>
/// <typeparam name="TEntity">The type of the managed entities</typeparam>
/// <typeparam name="TKey">The type of key used to identify and distinct managed entities</typeparam>
public class MemoryCacheRepositoryFactory<TEntity, TKey>
    : IPluginFactory
    where TEntity : class, IIdentifiable<TKey>
    where TKey : IEquatable<TKey>
{

    /// <inheritdoc/>
    public object Create() => new MemoryCacheRepository<TEntity, TKey>(new MemoryCache(Options.Create(new MemoryCacheOptions())));

}