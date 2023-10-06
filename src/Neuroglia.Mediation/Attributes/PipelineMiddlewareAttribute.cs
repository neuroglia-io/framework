namespace Neuroglia.Mediation;

/// <summary>
/// Represents the attribute used to add <see cref="IMiddleware{TRequest, TResult}"/> to an <see cref="IRequestHandler{TRequest, TResult}"/>
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class PipelineMiddlewareAttribute
    : Attribute
{

    /// <summary>
    /// Initializes a new <see cref="PipelineMiddlewareAttribute"/>
    /// </summary>
    /// <param name="pipelineBehaviorType">The type of <see cref="IMiddleware{TRequest, TResult}"/> to apply</param>
    /// <param name="priority">The priority of the the referenced <see cref="IMiddleware{TRequest, TResult}"/></param>
    public PipelineMiddlewareAttribute(Type pipelineBehaviorType, int priority = 99)
    {
        if (pipelineBehaviorType == null) throw new ArgumentNullException(nameof(pipelineBehaviorType));
        if (pipelineBehaviorType.GetGenericType(typeof(IMiddleware<,>)) == null) throw new ArgumentException($"The specified type must be implement the '{typeof(IMiddleware<,>).Name}' interface", nameof(pipelineBehaviorType));
        this.PipelineBehaviorType = pipelineBehaviorType;
        this.Priority = priority;
    }

    /// <summary>
    /// Gets the type of <see cref="IMiddleware{TRequest, TResult}"/> to apply
    /// </summary>
    public Type PipelineBehaviorType { get; }

    /// <summary>
    /// Gets the priority of the referenced <see cref="IMiddleware{TRequest, TResult}"/>
    /// </summary>
    public int Priority { get; }

}
