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

        services.TryAddEnumerable(new ServiceDescriptor(typeof(IPluginSource), pluginSource));

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
    /// <typeparam name="TContract">The type of the contract implemented by sourced plugins. Must be an interface</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
    /// <param name="serviceLifetime">The plugin service lifetime</param>
    /// <returns>The configured <see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddPlugin<TContract>(this IServiceCollection services, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
        where TContract : class
    {
        if (!typeof(TContract).IsInterface) throw new ArgumentException("The plugin contract must be an interface", nameof(TContract));
        services.AddPluginProvider();
        services.TryAdd(new ServiceDescriptor(typeof(IEnumerable<TContract>), provider => provider.GetRequiredService<IPluginProvider>().GetPlugins<TContract>(), serviceLifetime));
        services.TryAdd(new ServiceDescriptor(typeof(TContract), provider => provider.GetRequiredService<IPluginProvider>().GetPlugins<TContract>().First(), serviceLifetime));
        return services;
    }

}
