using Neuroglia.Serialization;

namespace Neuroglia.Data.Expressions.JQ.Configuration;

/// <summary>
/// Represents the options used to configure a <see cref="JQExpressionEvaluator"/>
/// </summary>
public class JQExpressionEvaluatorOptions
{

    /// <summary>
    /// Gets/sets the type of <see cref="IJsonSerializer"/> to use
    /// </summary>
    public virtual Type? SerializerType { get; set; }

}
