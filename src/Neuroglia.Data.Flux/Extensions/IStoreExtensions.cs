﻿namespace Neuroglia.Data.Flux;

/// <summary>
/// Defines extensions for <see cref="IStore"/>s
/// </summary>
public static class IStoreExtensions
{

    /// <summary>
    /// Adds a new <see cref="IFeature"/> to the store
    /// </summary>
    /// <typeparam name="TState">The type of state managed by the <see cref="IFeature"/> to add</typeparam>
    /// <param name="store">The <see cref="IStore"/> to add the <see cref="IFeature"/> to</param>
    /// <param name="state">The value of the <see cref="IFeature"/> to add</param>
    /// <param name="reducers">An array that contains the <see cref="IReducer"/>s to initialize the <see cref="IFeature"/> to add with</param>
    public static void AddFeature<TState>(this IStore store, TState state, params IReducer<TState>[] reducers)
    {
        var feature = new Feature<TState>(state);
        foreach (var reducer in reducers) feature.AddReducer(reducer);
        store.AddFeature(feature);
    }

    /// <summary>
    /// Adds a new <see cref="IFeature"/> to the store
    /// </summary>
    /// <typeparam name="TState">The type of state of the <see cref="IFeature"/> to add</typeparam>
    /// <param name="store">The <see cref="IStore"/> to add the <see cref="IFeature"/> to</param>
    /// <param name="reducers">An array that contains the <see cref="IReducer"/>s to initialize the <see cref="IFeature"/> to add with</param>
    public static void AddFeature<TState>(this IStore store, params IReducer<TState>[] reducers) => store.AddFeature(Activator.CreateInstance<TState>(), reducers);

    /// <summary>
    /// Adds a new <see cref="IMiddleware"/> to the store
    /// </summary>
    /// <typeparam name="TMiddleware">The type of <see cref="IMiddleware"/> to add</typeparam>
    /// <param name="store">The <see cref="IStore"/> to add the <see cref="IMiddleware"/> to</param>
    public static void AddMiddleware<TMiddleware>(this IStore store) where TMiddleware : IMiddleware => store.AddMiddleware(typeof(TMiddleware));

    /// <summary>
    /// Adds a new <see cref="IEffect"/> to the store
    /// </summary>
    /// <typeparam name="TEffect">The type of <see cref="IEffect"/> to add</typeparam>
    /// <param name="store">The <see cref="IStore"/> to add the <see cref="IEffect"/> to</param>
    public static void AddEffect<TEffect>(this IStore store) where TEffect : IEffect => store.AddEffect(Activator.CreateInstance<TEffect>());

}
