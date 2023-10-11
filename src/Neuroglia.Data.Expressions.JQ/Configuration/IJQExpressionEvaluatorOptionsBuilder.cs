using Neuroglia.Serialization;

namespace Neuroglia.Data.Expressions.JQ.Configuration;

/// <summary>
/// Defines the fundamentals of a service used to build <see cref="JQExpressionEvaluatorOptions"/>
/// </summary>
public interface IJQExpressionEvaluatorOptionsBuilder
{

    /// <summary>
    /// Configures the <see cref="JQExpressionEvaluator"/> to use the specified <see cref="IJsonSerializer"/> type
    /// </summary>
    /// <typeparam name="TSerializer">The type of <see cref="IJsonSerializer"/> to use</typeparam>
    /// <returns>The configured <see cref="IJQExpressionEvaluatorOptionsBuilder"/></returns>
    IJQExpressionEvaluatorOptionsBuilder UseSerializer<TSerializer>()
        where TSerializer : class, IJsonSerializer;

    /// <summary>
    /// Builds the <see cref="JQExpressionEvaluatorOptions"/>
    /// </summary>
    /// <returns>New <see cref="JQExpressionEvaluatorOptions"/></returns>
    JQExpressionEvaluatorOptions Build();

}
