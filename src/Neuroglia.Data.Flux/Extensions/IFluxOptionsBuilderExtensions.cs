﻿namespace Neuroglia.Data.Flux;

/// <summary>
/// Defines extensions for <see cref="IFluxOptionsBuilder"/>s
/// </summary>
public static class IFluxOptionsBuilderExtensions
{

    /// <summary>
    /// Configures the <see cref="IFluxOptionsBuilder"/> to scan the <see cref="Assembly"/> the markup type belongs to
    /// </summary>
    /// <typeparam name="TMarkup">The markup type to scan the parent <see cref="Assembly"/> of</typeparam>
    /// <param name="builder">The <see cref="IFluxOptionsBuilder"/> to configure</param>
    /// <returns>The configured <see cref="IFluxOptionsBuilder"/></returns>
    public static IFluxOptionsBuilder ScanMarkupTypeAssembly<TMarkup>(this IFluxOptionsBuilder builder)
    {
        builder.ScanAssembly(typeof(TMarkup).Assembly);
        return builder;
    }

    /// <summary>
    /// Configures the <see cref="IFluxOptionsBuilder"/> to use the specified <see cref="IDispatcher"/> type
    /// </summary>
    /// <typeparam name="TDispatcher">The type of <see cref="IDispatcher"/> to use</typeparam>
    /// <param name="builder">The <see cref="IFluxOptionsBuilder"/> to configure</param>
    /// <returns>The configured <see cref="IFluxOptionsBuilder"/></returns>
    public static IFluxOptionsBuilder UseDispatcher<TDispatcher>(this IFluxOptionsBuilder builder)
    {
        builder.UseDispatcher(typeof(TDispatcher));
        return builder;
    }

    /// <summary>
    /// Configures the <see cref="IFluxOptionsBuilder"/> to use the specified <see cref="IStoreFactory"/> type
    /// </summary>
    /// <typeparam name="TFactory">The type of <see cref="IStoreFactory"/> to use</typeparam>
    /// <param name="builder">The <see cref="IFluxOptionsBuilder"/> to configure</param>
    /// <returns>The configured <see cref="IFluxOptionsBuilder"/></returns>
    public static IFluxOptionsBuilder UseStoreFactory<TFactory>(this IFluxOptionsBuilder builder)
        where TFactory : IStoreFactory
    {
        builder.UseStoreFactory(typeof(TFactory));
        return builder;
    }

    /// <summary>
    /// Configures the <see cref="IFluxOptionsBuilder"/> to use the specified <see cref="IStore"/> type
    /// </summary>
    /// <typeparam name="TStore">The type of <see cref="IStore"/> to use</typeparam>
    /// <param name="builder">The <see cref="IFluxOptionsBuilder"/> to configure</param>
    /// <returns>The configured <see cref="IFluxOptionsBuilder"/></returns>
    public static IFluxOptionsBuilder UseStore<TStore>(this IFluxOptionsBuilder builder)
        where TStore : class, IStore
    {
        builder.UseStore(typeof(TStore));
        return builder;
    }

}
