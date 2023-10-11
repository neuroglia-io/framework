using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Neuroglia.Data.Expressions.JQ.Configuration;

namespace Neuroglia.Data.Expressions.JQ;

/// <summary>
/// Defines extensions for <see cref="IServiceCollection"/>s
/// </summary>
public static class IServiceCollectionExtensions
{

    /// <summary>
    /// Adds and configures a new <see cref="JQExpressionEvaluator"/>
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
    /// <param name="setup">An <see cref="Action{T}"/> used to configure the <see cref="JQExpressionEvaluator"/></param>
    /// <param name="lifetime">The service's lifetime. Defaults to <see cref="ServiceLifetime.Transient"/></param>
    /// <returns>The configured <see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddJQExpressionEvaluator(this IServiceCollection services, Action<IJQExpressionEvaluatorOptionsBuilder>? setup = null, ServiceLifetime lifetime = ServiceLifetime.Transient)
    {
        IJQExpressionEvaluatorOptionsBuilder builder = new JQExpressionEvaluatorOptionsBuilder();
        setup?.Invoke(builder);
        services.TryAddSingleton(Options.Create(builder.Build()));
        services.AddExpressionEvaluator<JQExpressionEvaluator>(lifetime);
        return services;
    }

}
