namespace Neuroglia.Data.Expressions.Services;

/// <summary>
/// Defines the fundamentals of a service used to evaluate expressions
/// </summary>
public interface IExpressionEvaluator
{

    /// <summary>
    /// Determines whether or not the <see cref="IExpressionEvaluator"/> supports the specified expression language
    /// </summary>
    /// <param name="language">The expression language to check</param>
    /// <returns>A boolean indicating whether or not the <see cref="IExpressionEvaluator"/> supports the specified expression language</returns>
    bool Supports(string language);

    /// <summary>
    /// Evaluates the specified runtime expression
    /// </summary>
    /// <param name="expression">The runtime expression to evaluate</param>
    /// <param name="input">The data to evaluate the runtime expression against</param>
    /// <param name="arguments">A key/value mapping of the arguments used during evaluation, if any</param>
    /// <param name="expectedType">The expected type of the evaluation's result</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The evaluation's result</returns>
    Task<object?> EvaluateAsync(string expression, object input, IDictionary<string, object>? arguments = null, Type? expectedType = null, CancellationToken cancellationToken = default);

}
