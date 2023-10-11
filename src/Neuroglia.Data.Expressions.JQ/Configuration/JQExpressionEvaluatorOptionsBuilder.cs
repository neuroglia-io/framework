using Neuroglia.Serialization;

namespace Neuroglia.Data.Expressions.JQ.Configuration;

/// <summary>
/// Represents the default implementation of the <see cref="IJQExpressionEvaluatorOptionsBuilder"/> interface
/// </summary>
public class JQExpressionEvaluatorOptionsBuilder
    : IJQExpressionEvaluatorOptionsBuilder
{

    /// <summary>
    /// Gets the <see cref="JQExpressionEvaluatorOptions"/> to configure
    /// </summary>
    protected JQExpressionEvaluatorOptions Options { get; } = new();

    /// <inheritdoc/>
    public virtual IJQExpressionEvaluatorOptionsBuilder UseSerializer<TSerializer>()
        where TSerializer : class, IJsonSerializer
    {
        this.Options.SerializerType = typeof(TSerializer);
        return this;
    }

    /// <inheritdoc/>
    public virtual JQExpressionEvaluatorOptions Build() => this.Options;

}
