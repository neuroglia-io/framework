namespace Neuroglia.Data.Expressions.Services;

/// <summary>
/// Defines the fundamentals of a service used to manage <see cref="IExpressionEvaluator"/>s
/// </summary>
public interface IExpressionEvaluatorProvider
{

    /// <summary>
    /// Gets the first <see cref="IExpressionEvaluator"/> that supports the specified expression language, if any
    /// </summary>
    /// <param name="language">The expression language for which to get an <see cref="IExpressionEvaluator"/></param>
    /// <returns>The <see cref="IExpressionEvaluator"/> that supports the specified expression language, if any</returns>
    IExpressionEvaluator? GetEvaluator(string language);

    /// <summary>
    /// Gets the <see cref="IExpressionEvaluator"/>s that support the specified expression language
    /// </summary>
    /// <param name="language">The expression language to get the <see cref="IExpressionEvaluator"/>s for</param>
    /// <returns>A new <see cref="IEnumerable{T}"/> containing the <see cref="IExpressionEvaluator"/> that support the specified expression language</returns>
    IEnumerable<IExpressionEvaluator> GetEvaluators(string language);

}
