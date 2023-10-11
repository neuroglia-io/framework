using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Neuroglia.Data.Expressions.JavaScript.Configuration;

namespace Neuroglia.Data.Expressions.JavaScript;

/// <summary>
/// Defines extensions for <see cref="IServiceCollection"/>s
/// </summary>
public static class IServiceCollectionExtensions
{

    /// <summary>
    /// Adds and configures a new <see cref="JavaScriptExpressionEvaluator"/>
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
    /// <param name="setup">An <see cref="Action{T}"/> used to configure the <see cref="JavaScriptExpressionEvaluator"/></param>
    /// <param name="lifetime">The service's lifetime. Defaults to <see cref="ServiceLifetime.Transient"/></param>
    /// <returns>The configured <see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddJavaScriptExpressionEvaluator(this IServiceCollection services, Action<IJavaScriptExpressionEvaluatorOptionsBuilder>? setup = null, ServiceLifetime lifetime = ServiceLifetime.Transient)
    {
        var builder = new JavaScriptExpressionEvaluatorOptionsBuilder();
        setup?.Invoke(builder);
        services.TryAddSingleton(Options.Create(builder.Build()));
        services.AddExpressionEvaluator<JavaScriptExpressionEvaluator>(lifetime);
        return services;
    }

}
