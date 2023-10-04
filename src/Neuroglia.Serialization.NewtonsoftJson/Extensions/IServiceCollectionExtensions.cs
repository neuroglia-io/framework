using Microsoft.Extensions.DependencyInjection;
using Neuroglia.Serialization.DataContract;

namespace Neuroglia.Serialization;

/// <summary>
/// Defines extensions for <see cref="IServiceCollection"/>s
/// </summary>
public static class IServiceCollectionExtensions
{

    /// <summary>
    /// Adds and configure a <see cref="NewtonsoftJsonSerializer"/>
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
    /// <param name="lifetime">The <see cref="NewtonsoftJsonSerializer"/>'s lifetime</param>
    /// <returns>The configured <see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddNewtonsoftJsonSerializer(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Singleton)
    {
        services.AddSerializer<NewtonsoftJsonSerializer>(lifetime);
        return services;
    }

}
