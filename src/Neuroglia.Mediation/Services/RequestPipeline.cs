using Microsoft.Extensions.DependencyInjection;
using System.Runtime.ExceptionServices;

namespace Neuroglia.Mediation;

/// <summary>
/// Represents an <see cref="IRequest"/> pipeline
/// </summary>
public abstract class RequestPipeline
{

    /// <summary>
    /// Gets the service used to handle the <see cref="IRequest"/>
    /// </summary>
    /// <typeparam name="THandler">The type of <see cref="IRequestHandler{TRequest, TResponse}"/> to get</typeparam>
    /// <param name="serviceProvider">The current <see cref="IServiceProvider"/></param>
    /// <returns>The <see cref="IRequestHandler{TRequest, TResponse}"/> of the specified type</returns>
    protected virtual THandler GetHandler<THandler>(IServiceProvider serviceProvider)
    {
        IEnumerable<THandler> handlers;
        try
        {
            handlers = serviceProvider.GetServices<THandler>();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error constructing handler for request of type '{typeof(THandler)}'. Register your handlers with the service provider", ex);
        }
        if (handlers == null
            || !handlers.Any())
            throw new InvalidOperationException($"Handler was not found for request of type '{typeof(THandler)}'. Register your handlers with the service provider");
        if(handlers.Count() > 1)
            throw new InvalidOperationException($"Failed to discriminate the handler for request of type '{typeof(THandler)}'. Requests must have exactly one registered handler");
        return handlers.First();
    }

    /// <summary>
    /// Handles the specified <see cref="IRequest"/>
    /// </summary>
    /// <param name="request">The <see cref="IRequest"/> to handle</param>
    /// <param name="serviceProvider">The current <see cref="IServiceProvider"/></param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The resulting <see cref="IOperationResult"/></returns>
    public abstract Task<object> HandleAsync(object request, IServiceProvider serviceProvider, CancellationToken cancellationToken = default);

}

/// <summary>
/// Represents an <see cref="IRequest"/> pipeline
/// </summary>
/// <typeparam name="TResult">The expected type of <see cref="IOperationResult"/></typeparam>
public abstract class RequestPipeline<TResult>
    : RequestPipeline
    where TResult : IOperationResult
{

    /// <summary>
    /// Handles the specified <see cref="IRequest"/>
    /// </summary>
    /// <param name="request">The <see cref="IRequest"/> to handle</param>
    /// <param name="serviceProvider">The current <see cref="IServiceProvider"/></param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The resulting <see cref="IOperationResult"/></returns>
    public abstract Task<TResult> HandleAsync(IRequest<TResult> request, IServiceProvider serviceProvider, CancellationToken cancellationToken = default);

}

/// <summary>
/// Represents an <see cref="IRequest"/> pipeline
/// </summary>
/// <typeparam name="TRequest">The type of <see cref="IRequest"/> to handle</typeparam>
/// <typeparam name="TResult">The expected type of <see cref="IOperationResult"/></typeparam>
public class RequestPipeline<TRequest, TResult>
    : RequestPipeline<TResult>
    where TRequest : IRequest<TResult>
    where TResult : IOperationResult
{

    /// <inheritdoc/>
    public override async Task<object> HandleAsync(object request, IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        return await this.HandleAsync((IRequest<TResult>)request, serviceProvider, cancellationToken)
            .ContinueWith(this.HandleException, cancellationToken);
    }

    /// <inheritdoc/>
    public override async Task<TResult> HandleAsync(IRequest<TResult> request, IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        Task<TResult> handler() => this.GetHandler<IRequestHandler<TRequest, TResult>>(serviceProvider).HandleAsync((TRequest)request, cancellationToken);
        return await serviceProvider
            .GetServices<IMiddleware<TRequest, TResult>>()
            .Reverse()
            .Aggregate((RequestHandlerDelegate<TResult>)handler, (next, pipeline) => () => pipeline.HandleAsync((TRequest)request, next, cancellationToken))();
    }

    /// <summary>
    /// Handles exceptions that might have occured during the execution of the pipeline
    /// </summary>
    /// <param name="pipelineExecutionTask">The <see cref="Task{T}"/> to handle the exceptions for</param>
    /// <returns>The result of the pipeline's execution</returns>
    protected virtual object HandleException(Task<TResult> pipelineExecutionTask)
    {
        if (pipelineExecutionTask.IsFaulted) ExceptionDispatchInfo.Capture(pipelineExecutionTask.Exception!.InnerException!).Throw();
        return pipelineExecutionTask.Result;
    }

}
