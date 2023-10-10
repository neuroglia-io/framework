using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Neuroglia.Mapping;

/// <summary>
/// Defines extensions for <see cref="IServiceCollection"/>s
/// </summary>
public static class IServiceCollectionExtensions
{

    /// <summary>
    /// Adds and configures a <see cref="Mapper"/> service
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
    /// <param name="assemblies">An array containing the <see cref="Assembly"/> instances to scan for mapping configuration</param>
    /// <returns>The configured <see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddMapper(this IServiceCollection services, params Assembly[] assemblies)
    {
        services.AddAutoMapper(config =>
        {
            config.ShouldMapMethod = _ => false;
        }, assemblies);
        services.AddSingleton<IMapper, Mapper>();
        return services;
    }

}
