// Copyright © 2021-Present Neuroglia SRL. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License"),
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Neuroglia.Plugins.Configuration;
using Neuroglia.Plugins.Services;

namespace Neuroglia.Plugins;

/// <summary>
/// Defines extensions for <see cref="IServiceCollection"/>s
/// </summary>
public static class IServiceCollectionExtensions
{

    const string PluginConfigurationSectionKey = "plugins";

    /// <summary>
    /// Registers and configures the plugin provider service
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
    /// <param name="configuration">The current <see cref="IConfiguration"/>, if any</param>
    /// <returns>The configured <see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddPluginProvider(this IServiceCollection services, IConfiguration? configuration = null)
    {
        if (configuration != null) services.ConfigurePlugins(configuration);
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
        ArgumentNullException.ThrowIfNull(setup);
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
    /// <param name="sourceName">The name of the plugin source, if any, to get the service implementation from</param>
    /// <param name="serviceLifetime">The plugin service lifetime</param>
    /// <returns>The configured <see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddPlugin(this IServiceCollection services, Type serviceType, object? defaultImplementation = null, string? sourceName = null, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
    {
        if (!serviceType.IsInterface) throw new ArgumentException("The plugin contract must be an interface", nameof(serviceType));
        services.TryAdd(new ServiceDescriptor(typeof(IEnumerable<>).MakeGenericType(serviceType), provider => provider.GetRequiredService<IPluginProvider>().GetPlugins(serviceType, sourceName).OfType(serviceType), serviceLifetime));
        services.TryAdd(new ServiceDescriptor(serviceType, provider => PluginProxy.Create(provider.GetRequiredService<IPluginProvider>(), serviceType, defaultImplementation, sourceName), serviceLifetime));
        return services;
    }

    /// <summary>
    /// Registers an plugin of the specified type
    /// </summary>
    /// <typeparam name="TService">The type of the contract implemented by sourced plugins. Must be an interface</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
    /// <param name="defaultImplementation">The default implementation, if any, of the service contract</param>
    /// <param name="sourceName">The name of the plugin source, if any, to get the service implementation from</param>
    /// <param name="serviceLifetime">The plugin service lifetime</param>
    /// <returns>The configured <see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddPlugin<TService>(this IServiceCollection services, TService? defaultImplementation = null, string? sourceName = null, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
        where TService : class
    {
        return services.AddPlugin(typeof(TService), defaultImplementation, sourceName, serviceLifetime);
    }

    /// <summary>
    /// Configures the application's plugins
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
    /// <param name="configuration">The current <see cref="IConfiguration"/></param>
    /// <returns>The configured <see cref="IServiceCollection"/></returns>
    public static IServiceCollection ConfigurePlugins(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        var optionsAccessor = services.FirstOrDefault(d => d.ServiceType == typeof(IOptions<PluginOptions>));
        if (optionsAccessor != null) return services;

        var options = new PluginOptions();
        configuration.Bind(PluginConfigurationSectionKey, options);
        services.AddSingleton(Options.Create(options));

        if(options.Sources != null)
        {
            foreach (var sourceOptions in options.Sources)
            {
                services.Add(new ServiceDescriptor(typeof(IPluginSource), sourceOptions.BuildSource, ServiceLifetime.Singleton));
            }
        }

        if (options.Services != null)
        {
            foreach (var serviceOptions in options.Services)
            {
                var serviceType = Type.GetType(serviceOptions.Type) ?? throw new NullReferenceException($"Failed to find the specified type '{serviceOptions.Type}'");
                services.AddPlugin(serviceType, null, serviceOptions.Source, serviceOptions.Lifetime);
            }
        }

        return services;
    }

}
