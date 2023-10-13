﻿namespace Neuroglia.Plugins.Services;

/// <summary>
/// Defines the fundamentals of a service used to provide <see cref="IPlugin"/>s
/// </summary>
public interface IPluginProvider
{

    /// <summary>
    /// Gets all sourced <see cref="IPlugin"/>s
    /// </summary>
    /// <returns>A new <see cref="IEnumerable{T}"/> containing all sourced <see cref="IPlugin"/>s</returns>
    IEnumerable<IPlugin> GetPlugins();

    /// <summary>
    /// Gets all sourced plugins that implement the specified contract
    /// </summary>
    /// <typeparam name="TContract">The type of the contract implemented by sourced plugins. Must be an interface</typeparam>
    /// <returns>A new <see cref="IEnumerable{T}"/> containing the plugins that implement the specified contract</returns>
    IEnumerable<TContract> GetPlugins<TContract>()
        where TContract : class;

}