using Microsoft.Extensions.DependencyInjection;

namespace Neuroglia.Data.Expressions.Services;

/// <summary>
/// Represents the default implementation of the <see cref="IExpressionEvaluator"/> interface
/// </summary>
public class ExpressionEvaluatorProvider
    : IExpressionEvaluatorProvider
{

    /// <summary>
    /// Initializes a new <see cref="ExpressionEvaluatorProvider"/>
    /// </summary>
    /// <param name="serviceProvider">The current <see cref="IServiceProvider"/></param>
    public ExpressionEvaluatorProvider(IServiceProvider serviceProvider) { this.ServiceProvider = serviceProvider; }

    /// <summary>
    /// Gets the current <see cref="IServiceProvider"/>
    /// </summary>
    protected IServiceProvider ServiceProvider { get; }

    /// <inheritdoc/>
    public virtual IExpressionEvaluator? GetEvaluator(string language)
    {
        if (string.IsNullOrEmpty(language)) throw new ArgumentNullException(nameof(language));
        return this.GetEvaluators(language).FirstOrDefault();
    }

    /// <inheritdoc/>
    public virtual IEnumerable<IExpressionEvaluator> GetEvaluators(string language) => string.IsNullOrWhiteSpace(language) ? throw new ArgumentNullException(nameof(language)) : this.ServiceProvider.GetServices<IExpressionEvaluator>().Where(s => s.Supports(language));

}
