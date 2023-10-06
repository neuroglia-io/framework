using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace Neuroglia.Mediation;

/// <summary>
/// Represents the default implementation of the <see cref="IMediator"/> interface
/// </summary>
public class Mediator
    : IMediator
{

    /// <summary>
    /// Gets the <see cref="Mediator"/>'s <see cref="ActivitySource"/> name
    /// </summary>
    public const string ActivitySourceName = "Neuroglia.Mediation.Diagnostics.ActivitySource";

    private static readonly ActivitySource _ActivitySource = new(ActivitySourceName);
    private static readonly ConcurrentDictionary<Type, object> _RequestHandlers = new();

    /// <summary>
    /// Initializes a new <see cref="Mediator"/>
    /// </summary>
    /// <param name="serviceProvider">The current <see cref="IServiceProvider"/></param>
    public Mediator(IServiceProvider serviceProvider)
    {
        this.ServiceProvider = serviceProvider;
    }

    /// <summary>
    /// Gets the current <see cref="IServiceProvider"/>
    /// </summary>
    protected IServiceProvider ServiceProvider { get; }

    /// <inheritdoc/>
    public virtual async Task<TResult> ExecuteAsync<TResult>(IRequest<TResult> request, CancellationToken cancellationToken = default) 
        where TResult : IOperationResult
    {
        if (request == null) throw new ArgumentNullException(nameof(request));
        var requestType = request.GetType();
        using var activity = _ActivitySource.StartActivity(requestType.Name);
        activity?.AddTag("request.type", requestType.FullName);
        activity?.AddTag("request.cqrs_type", request is ICommand ? "command" : "query");
        var pipeline = (RequestPipeline<TResult>)_RequestHandlers.GetOrAdd(requestType, t => Activator.CreateInstance(typeof(RequestPipeline<,>).MakeGenericType(requestType, typeof(TResult)))!);
        var result = await pipeline.HandleAsync(request, this.ServiceProvider, cancellationToken).ConfigureAwait(false);
        return result;
    }

    /// <inheritdoc/>
    public virtual async Task PublishAsync<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
    {
        if (notification == null) throw new ArgumentNullException(nameof(notification));
        using var activity = _ActivitySource.StartActivity(typeof(TNotification).Name);
        activity?.AddTag("notification.type", typeof(TNotification).FullName);
        foreach (var handler in this.ServiceProvider.GetServices<INotificationHandler<TNotification>>())
        {
            await handler.HandleAsync(notification, cancellationToken).ConfigureAwait(false);
        }
    }

}
