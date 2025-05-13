﻿namespace Neuroglia.Data.Flux;

/// <summary>
/// Defines the fundamentals of a Flux feature
/// </summary>
public interface IFeature
{

    /// <summary>
    /// Gets the <see cref="IFeature"/>'s state
    /// </summary>
    object State { get; }

    /// <summary>
    /// Determines whether or not the <see cref="IFeature"/> defines <see cref="IReducer"/>s for the specified action
    /// </summary>
    /// <param name="action">The action to get check for <see cref="IReducer"/>s</param>
    /// <returns>A boolean indicating whether or not the <see cref="IFeature"/>'s state is reduced by the specified action</returns>
    bool ShouldReduceStateFor(object action);

    /// <summary>
    /// Adds the specified <see cref="IReducer"/> to the <see cref="IFeature"/>
    /// </summary>
    /// <param name="reducer">The <see cref="IReducer"/> to add</param>
    void AddReducer(IReducer reducer);

    /// <summary>
    /// Reduces the <see cref="IFeature"/>'s state
    /// </summary>
    /// <param name="context">The <see cref="IActionContext"/> in which to reduce the feature</param>
    /// <param name="reducerPipelineBuilder">A <see cref="Func{T, TResult}"/> used to build the reducer pîpeline</param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    Task ReduceStateAsync(IActionContext context, Func<DispatchDelegate, DispatchDelegate> reducerPipelineBuilder);

}

/// <summary>
/// Defines the fundamentals of a Flux feature
/// </summary>
/// <typeparam name="TState">The type of the <see cref="IFeature"/>'s state</typeparam>
public interface IFeature<TState>
    : IFeature, IObservable<TState>
{

    /// <summary>
    /// Gets the <see cref="IFeature"/>'s state
    /// </summary>
    new TState State { get; }

    /// <summary>
    /// Adds the specified <see cref="IReducer"/> to the <see cref="IFeature"/>
    /// </summary>
    /// <param name="reducer">The <see cref="IReducer"/> to add</param>
    void AddReducer(IReducer<TState> reducer);

}