﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Neuroglia.Plugins.Services;

/// <summary>
/// Represents the default implementation of the <see cref="IPluginProvider"/> interface
/// </summary>
public class PluginProvider
    : IPluginProvider, IHostedService, IDisposable
{

    private bool _disposed;

    /// <summary>
    /// Initializes a new <see cref="PluginProvider"/>
    /// </summary>
    /// <param name="serviceProvider">The current <see cref="IServiceProvider"/></param>
    /// <param name="loggerFactory">The service used to create <see cref="ILogger"/>s</param>
    /// <param name="sources">An <see cref="IEnumerable{T}"/> containing all registered <see cref="IPluginSource"/>s</param>
    public PluginProvider(IServiceProvider serviceProvider, ILoggerFactory loggerFactory, IEnumerable<IPluginSource> sources)
    {
        this.ServiceProvider = serviceProvider;
        this.Logger = loggerFactory.CreateLogger(this.GetType());
        this.Sources = sources;
    }

    /// <summary>
    /// Gets the current <see cref="IServiceProvider"/>
    /// </summary>
    protected IServiceProvider ServiceProvider { get; }

    /// <summary>
    /// Gets the service used to perfrom logging
    /// </summary>
    protected ILogger Logger { get; }

    /// <summary>
    /// Gets an <see cref="IEnumerable{T}"/> containing all registered <see cref="IPluginSource"/>s
    /// </summary>
    protected IEnumerable<IPluginSource> Sources { get; }

    /// <summary>
    /// Gets the <see cref="PluginProvider"/>'s <see cref="System.Threading.CancellationTokenSource"/>
    /// </summary>
    protected CancellationTokenSource? CancellationTokenSource { get; private set; }

    /// <inheritdoc/>
    public virtual async Task StartAsync(CancellationToken cancellationToken)
    {
        this.CancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        await Task.WhenAll(this.Sources.Select(s => s.LoadAsync(this.CancellationTokenSource.Token))).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public virtual Task StopAsync(CancellationToken cancellationToken) => Task.Run(() => this.CancellationTokenSource?.Cancel(), cancellationToken);

    /// <inheritdoc/>
    public virtual IEnumerable<IPluginDescriptor> GetPlugins() => this.Sources.SelectMany(s => s.Plugins);

    /// <inheritdoc/>
    public virtual TService GetPlugin<TService>(string name, Version version, string? sourceName = null)
        where TService : class
    {
        var plugins = string.IsNullOrWhiteSpace(sourceName) ? this.GetPlugins() : this.Sources.FirstOrDefault(s => s.Name == sourceName)?.Plugins ?? throw new NullReferenceException($"Failed to find a plugin source with the specified name '{sourceName}'");
        var plugin = plugins.FirstOrDefault(p => p.Name == name && p.Version == version) ?? throw new NullReferenceException($"Failed to find a plugin with name '{name}' and version '{version}'");
        return (TService)this.CreatePluginInstance(plugin, typeof(TService));
    }

    /// <inheritdoc/>
    public virtual IEnumerable<object> GetPlugins(Type serviceType, string? sourceName = null, IEnumerable<string>? tags = null)
    {
        if (serviceType == null) throw new ArgumentNullException(nameof(serviceType));
        if (!serviceType.IsInterface) throw new ArgumentException("The plugin contract type must be an interface", nameof(serviceType));
        var genericArguments = serviceType.GetGenericArguments();

        var candidatePlugins = this.Sources.SelectMany(s => s.Plugins).Where(p => p.Type.IsGenericType && serviceType.IsGenericType ? serviceType.IsAssignableFrom(p.Type.MakeGenericType(genericArguments)) : serviceType.IsAssignableFrom(p.Type));
        if(!string.IsNullOrWhiteSpace(sourceName)) candidatePlugins = candidatePlugins.Where(p => p.Source.Name == sourceName);
        if (tags != null) candidatePlugins = candidatePlugins.Where(c => c.Tags != null && tags.All(c.Tags.Contains));

        foreach (var plugin in candidatePlugins)
        {
            object? pluginInstance;
            try
            {
                pluginInstance = this.CreatePluginInstance(plugin, serviceType);
            }
            catch (Exception ex)
            {
                this.Logger.LogWarning("An exception occured while instanciating plugin of type '{pluginType}': {ex}", plugin.Type, ex);
                continue;
            }
            yield return pluginInstance;
        }
    }

    /// <inheritdoc/>
    public virtual IEnumerable<TService> GetPlugins<TService>(string? sourceName = null, IEnumerable<string>? tags = null)
        where TService : class
    {
        foreach (var plugin in this.GetPlugins(typeof(TService), sourceName, tags)) yield return (TService)plugin;
    }

    protected virtual object CreatePluginInstance(IPluginDescriptor plugin, Type serviceType)
    {
        var genericArguments = serviceType.GetGenericArguments();
        var pluginAttribute = plugin.Type.GetCustomAttribute<PluginAttribute>();
        if (pluginAttribute?.FactoryType != null)
        {
            var factoryType = pluginAttribute.FactoryType.IsGenericTypeDefinition ? pluginAttribute.FactoryType.MakeGenericType(genericArguments) : pluginAttribute.FactoryType;
            var factory = (IPluginFactory)ActivatorUtilities.CreateInstance(this.ServiceProvider, factoryType);
            var pluginFactory = (IPluginFactory)((PluginAssemblyLoadContext)plugin.AssemblyLoadContext).CreateInstance(this.ServiceProvider, factoryType);
            return pluginFactory.Create();
        }
        else
        {
            var pluginType = plugin.Type.IsGenericTypeDefinition ? plugin.Type.MakeGenericType(genericArguments) : plugin.Type;
            return ((PluginAssemblyLoadContext)plugin.AssemblyLoadContext).CreateInstance(this.ServiceProvider, pluginType);
        }
    }

    /// <summary>
    /// Disposes of the <see cref="PluginProvider"/>
    /// </summary>
    /// <param name="disposing">A boolean indicating whether or not the <see cref="PluginProvider"/> is being disposed of</param>
    protected virtual void Dispose(bool disposing)
    {
        if (this._disposed) return;
        if (disposing) this.CancellationTokenSource?.Dispose();
        this._disposed = true;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

}