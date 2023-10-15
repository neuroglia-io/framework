namespace Neuroglia.Plugins.Services;

/// <summary>
/// Defines the fundamentals of a plugin source
/// </summary>
public interface IPluginSource
{

    /// <summary>
    /// Gets the source's name, if any
    /// </summary>
    string? Name { get; }

    /// <summary>
    /// Gets a boolean indicating whether or not the <see cref="IPluginSource"/> has been loaded
    /// </summary>
    bool IsLoaded { get; }

    /// <summary>
    /// Gets a list containing of sourced plugins
    /// </summary>
    IReadOnlyList<IPlugin> Plugins { get; }

    /// <summary>
    /// Loads the <see cref="IPluginSource"/>
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    Task LoadAsync(CancellationToken cancellationToken = default);

}
