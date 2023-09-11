using Microsoft.Extensions.DependencyInjection;
using Neuroglia.Serialization.Protobuf.Services;

namespace Neuroglia.Serialization;

/// <summary>
/// Defines extensions for <see cref="IServiceCollection"/>s
/// </summary>
public static class IServiceCollectionExtensions
{

    /// <summary>
    /// Adds and configure a <see cref="ProtobufNetSerializer"/>
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
    /// <param name="lifetime">The <see cref="ProtobufNetSerializer"/>'s lifetime</param>
    /// <returns>The configured <see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddProtobufNetSerializer(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Singleton)
    {
        services.AddSerializer<ProtobufNetSerializer>(lifetime);
        return services;
    }

}
