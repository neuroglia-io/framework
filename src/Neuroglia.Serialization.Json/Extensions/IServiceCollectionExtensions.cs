using Microsoft.Extensions.DependencyInjection;
using Neuroglia.Serialization.Json;

namespace Neuroglia.Serialization;

/// <summary>
/// Defines extensions for <see cref="IServiceCollection"/>s
/// </summary>
public static class IServiceCollectionExtensions
{

    /// <summary>
    /// Adds and configure a <see cref="JsonSerializer"/>
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
    /// <param name="setup">An <see cref="Action{T}"/> used to configure the <see cref="System.Text.Json.JsonSerializerOptions"/> to use</param>
    /// <param name="lifetime">The <see cref="JsonSerializer"/>'s lifetime</param>
    /// <returns>The configured <see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddJsonSerializer(this IServiceCollection services, Action<System.Text.Json.JsonSerializerOptions>? setup = null, ServiceLifetime lifetime = ServiceLifetime.Singleton)
    {
        if(setup != null) services.Configure(setup);
        services.AddSerializer<JsonSerializer>(lifetime);
        return services;
    }

}
