using System.Reflection;

namespace Neuroglia.Mediation.Configuration;

/// <summary>
/// Defines the fundamentals of a service used to build and configure a <see cref="Mediator"/> instance
/// </summary>
public interface IMediatorOptionsBuilder
{

    /// <summary>
    /// Scans the specified <see cref="Assembly"/> for <see cref="IRequestHandler{TRequest, TResponse}"/> and <see cref="INotificationHandler{TNotification}"/> implementations
    /// </summary>
    /// <param name="assembly">The <see cref="Assembly"/> to scan</param>
    /// <returns>The configured <see cref="IMediatorOptionsBuilder"/></returns>
    IMediatorOptionsBuilder ScanAssembly(Assembly assembly);

    /// <summary>
    /// Applies the specified <see cref="IMiddleware{TRequest, TResponse}"/> to all <see cref="IRequestHandler{TRequest, TResponse}"/> implementations<para></para>
    /// The order in which this method is called defines the order in which the behaviors will be called in the pipeline
    /// </summary>
    /// <param name="pipelineType">The type of the default <see cref="IMiddleware{TRequest, TResult}"/> to use</param>
    /// <returns>The configured <see cref="IMediatorOptionsBuilder"/></returns>
    IMediatorOptionsBuilder UseDefaultPipelineBehavior(Type pipelineType);

    /// <summary>
    /// Builds the <see cref="MediatorOptions"/>
    /// </summary>
    /// <returns>The resulting <see cref="MediatorOptions"/></returns>
    MediatorOptions Build();

}
