using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
    /// <param name="sources">An <see cref="IEnumerable{T}"/> containing all registered <see cref="IPluginSource"/>s</param>
    public PluginProvider(IServiceProvider serviceProvider, IEnumerable<IPluginSource> sources)
    {
        this.ServiceProvider = serviceProvider;
        this.Sources = sources;
    }

    /// <summary>
    /// Gets the current <see cref="IServiceProvider"/>
    /// </summary>
    protected IServiceProvider ServiceProvider { get; }

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
    public virtual IEnumerable<IPlugin> GetPlugins() => this.Sources.SelectMany(s => s.Plugins);

    /// <inheritdoc/>
    public virtual IEnumerable<TContract> GetPlugins<TContract>()
        where TContract : class
    {
        if (!typeof(TContract).IsInterface) throw new ArgumentException("The plugin contract type must be an interface", nameof(TContract));

        foreach(var source in this.Sources)
        {
            foreach(var plugin in source.Plugins.Where(p => typeof(TContract).IsAssignableFrom(p.Type)))
            {
                var pluginAttribute = plugin.Type.GetCustomAttribute<PluginAttribute>();
                if(pluginAttribute?.FactoryType != null)
                {
                    var factory = (IPluginFactory)ActivatorUtilities.CreateInstance(this.ServiceProvider, pluginAttribute.FactoryType);
                    yield return (TContract)factory.CreatePlugin();
                }
                else
                {
                    yield return (TContract)ActivatorUtilities.CreateInstance(this.ServiceProvider, plugin.Type);
                }
            }
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