namespace Neuroglia.Data.Flux;

/// <summary>
/// Represents the default implementation of the <see cref="IActionContext"/> interface
/// </summary>
/// <remarks>
/// Initializes a new <see cref="ActionContext"/>
/// </remarks>
/// <param name="services">The current <see cref="IServiceProvider"/></param>
/// <param name="store">The current <see cref="IStore"/></param>
/// <param name="action">The action to dispatch</param>
public class ActionContext(IServiceProvider services, IStore store, object action)
        : IActionContext
{

    /// <inheritdoc/>
    public IServiceProvider Services { get; } = services;

    /// <inheritdoc/>
    public IStore Store { get; } = store;

    /// <inheritdoc/>
    public object Action { get; set; } = action;

}
