using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Neuroglia.Plugins.Services;

namespace Neuroglia.Plugins;

/// <summary>
/// Defines extensions for <see cref="IServiceCollection"/>s
/// </summary>
public static class IServiceCollectionExtensions
{

    /// <summary>
    /// Registers and configures the plugin provider service
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
    /// <returns>The configured <see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddPluginProvider(this IServiceCollection services)
    {
        services.TryAddSingleton<PluginProvider>();
        services.TryAddSingleton<IPluginProvider, PluginProvider>();
        services.TryAddSingleton<IHostedService>(provider => provider.GetRequiredService<PluginProvider>());
        return services;
    }

    /// <summary>
    /// Registers the specified <see cref="IPluginSource"/>
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
    /// <param name="pluginSource">The <see cref="IPluginSource"/> to add</param>
    /// <returns>The configured <see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddPluginSource(this IServiceCollection services, IPluginSource pluginSource)
    {
        services.AddPluginProvider();

        services.Add(new ServiceDescriptor(typeof(IPluginSource), pluginSource));

        return services;
    }

    /// <summary>
    /// Registers the specified <see cref="IPluginSource"/>
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
    /// <param name="setup">An <see cref="Action{T}"/> used to setup the <see cref="IPluginSource"/> to build and register</param>
    /// <returns>The configured <see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddPluginSource(this IServiceCollection services, Action<IPluginSourceBuilder> setup)
    {
        if (setup == null) throw new ArgumentNullException(nameof(setup));
        var builder = new PluginSourceBuilder();
        setup(builder);
        services.AddPluginSource(builder.Build());
        return services;
    }

    /// <summary>
    /// Registers an plugin of the specified type
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
    /// <param name="serviceType">The type of the contract implemented by sourced plugins. Must be an interface</param>
    /// <param name="defaultImplementation">The default implementation, if any, of the service contract</param>
    /// <param name="serviceLifetime">The plugin service lifetime</param>
    /// <returns>The configured <see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddPlugin(this IServiceCollection services, Type serviceType, object? defaultImplementation = null, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
    {
        if (!serviceType.IsInterface) throw new ArgumentException("The plugin contract must be an interface", nameof(serviceType));
        services.AddPluginProvider();
        services.TryAdd(new ServiceDescriptor(typeof(IEnumerable<>).MakeGenericType(serviceType), provider => provider.GetRequiredService<IPluginProvider>().GetPlugins(serviceType).OfType(serviceType), serviceLifetime));
        services.TryAdd(new ServiceDescriptor(serviceType, provider => provider.GetRequiredService<IPluginProvider>().GetPlugins(serviceType).FirstOrDefault() ?? defaultImplementation ?? throw new NullReferenceException($"No plugin or implementation type registered for service type '{serviceType.Name}'"), serviceLifetime));
        return services;
    }

    /// <summary>
    /// Registers an plugin of the specified type
    /// </summary>
    /// <typeparam name="TService">The type of the contract implemented by sourced plugins. Must be an interface</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
    /// <param name="defaultImplementation">The default implementation, if any, of the service contract</param>
    /// <param name="serviceLifetime">The plugin service lifetime</param>
    /// <returns>The configured <see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddPlugin<TService>(this IServiceCollection services, TService? defaultImplementation = null, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
        where TService : class
    {
        return services.AddPlugin(typeof(TService), defaultImplementation, serviceLifetime);
    }

}
