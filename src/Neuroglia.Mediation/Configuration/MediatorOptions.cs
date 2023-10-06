using System.Reflection;

namespace Neuroglia.Mediation.Configuration;

/// <summary>
/// Represents the object used to configure a <see cref="Mediator"/> instance
/// </summary>
public class MediatorOptions
{

    /// <summary>
    /// Gets a <see cref="List{T}"/> containing the assemblies to scan for <see cref="IRequestHandler{TRequest, TResponse}"/> and <see cref="INotificationHandler{TNotification}"/> implementations
    /// </summary>
    public List<Assembly> AssembliesToScan { get; } = new();

    /// <summary>
    /// Gets a <see cref="List{T}"/> containing the <see cref="IMiddleware{TRequest, TResult}"/> types to apply to to all <see cref="IRequestHandler{TRequest, TResponse}"/> implementations
    /// </summary>
    public List<Type> DefaultPipelineBehaviors { get; } = new();

}
