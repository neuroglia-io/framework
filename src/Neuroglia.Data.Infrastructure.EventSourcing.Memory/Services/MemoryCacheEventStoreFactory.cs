using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Neuroglia.Plugins.Services;

namespace Neuroglia.Data.Infrastructure.EventSourcing.DistributedCache.Services;

/// <summary>
/// Represents the service used to create <see cref="MemoryCacheEventStore"/>s
/// </summary>
public class MemoryCacheEventStoreFactory
    : IPluginFactory
{

    /// <inheritdoc/>
    public virtual object Create() => new MemoryCacheEventStore(new MemoryCache(Options.Create(new MemoryCacheOptions())));

}
