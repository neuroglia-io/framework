using Microsoft.Extensions.DependencyInjection;
using Neuroglia.Data.Infrastructure.EventSourcing.Services;

namespace Neuroglia.Data.Infrastructure.EventSourcing.Redis;

/// <summary>
/// Defines extensions for <see cref="IServiceCollection"/>s
/// </summary>
public static class IServiceCollectionExtensions
{

    /// <summary>
    /// Adds and configures a <see cref="RedisEventStore"/>
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
    /// <param name="setup">An <see cref="Action{T}"/> used to configure the <see cref="RedisEventStore"/></param>
    /// <returns>The configured <see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddRedisEventStore(this IServiceCollection services, Action<IEventStoreOptionsBuilder>? setup = null)
    {
        services.AddEventStore<RedisEventStore>(setup);
        return services;
    }

}
