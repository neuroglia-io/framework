using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Neuroglia.Data.Infrastructure.EventSourcing.DistributedCache.Services;

/// <summary>
/// Represents the service used to create <see cref="MemoryCacheEventStore"/>s
/// </summary>
public class MemoryCacheEventStoreFactory
    : IFactory<MemoryCacheEventStore>
{

    /// <inheritdoc/>
    public virtual MemoryCacheEventStore Create() => new(new MemoryCache(Options.Create(new MemoryCacheOptions())));

    object IFactory.Create() => this.Create();

}
