using Microsoft.Extensions.DependencyInjection;
using Neuroglia.Data.Infrastructure.EventSourcing.DistributedCache.Services;
using Neuroglia.Data.Infrastructure.EventSourcing;
using Neuroglia.Data.Infrastructure.EventSourcing.Services;

namespace Neuroglia.Data.Infrastructure.EventSourcing.MemoryCache;

/// <summary>
/// Defines extensions for <see cref="IServiceCollection"/>s
/// </summary>
public static class IServiceCollectionExtensions
{

    /// <summary>
    /// Adds and configures a <see cref="MemoryCacheEventStore"/>
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
    /// <param name="setup">An <see cref="Action{T}"/> used to configure the <see cref="MemoryCacheEventStore"/></param>
    /// <returns>The configured <see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddMemoryCacheEventStore(this IServiceCollection services, Action<IEventStoreOptionsBuilder>? setup = null)
    {
        services.AddMemoryCache();
        services.AddEventStore<MemoryCacheEventStore>(setup);
        return services;
    }

}
