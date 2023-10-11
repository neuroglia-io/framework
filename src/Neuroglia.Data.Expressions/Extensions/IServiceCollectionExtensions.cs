using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Neuroglia.Data.Expressions.Services;

namespace Neuroglia.Data.Expressions;

/// <summary>
/// Defines extensions for <see cref="IServiceCollection"/>s
/// </summary>
public static class IServiceCollectionExtensions
{

    /// <summary>
    /// Adds and configures an <see cref="IExpressionEvaluator"/> of the specified type
    /// </summary>
    /// <typeparam name="TEvaluator">The type of <see cref="IExpressionEvaluator"/> to register</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
    /// <param name="lifetime">The <see cref="ServiceLifetime"/>. Defaults to <see cref="ServiceLifetime.Transient"/></param>
    /// <returns>The configured <see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddExpressionEvaluator<TEvaluator>(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Transient)
        where TEvaluator : class, IExpressionEvaluator
    {
        services.TryAddSingleton<IExpressionEvaluatorProvider, ExpressionEvaluatorProvider>();
        services.TryAdd(new ServiceDescriptor(typeof(TEvaluator), typeof(TEvaluator), lifetime));
        services.Add(new ServiceDescriptor(typeof(IExpressionEvaluator), provider => provider.GetRequiredService<TEvaluator>(), lifetime));
        return services;
    }

}
