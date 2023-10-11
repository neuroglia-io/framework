using Neuroglia.Serialization;

namespace Neuroglia.Data.Expressions.JavaScript.Configuration;

/// <summary>
/// Represents the options used to configure a <see cref="JavaScriptExpressionEvaluator"/>
/// </summary>
public class JavaScriptExpressionEvaluatorOptions
{

    /// <summary>
    /// Gets/sets the type of <see cref="IJsonSerializer"/> to use
    /// </summary>
    public virtual Type? SerializerType { get; set; }

}
