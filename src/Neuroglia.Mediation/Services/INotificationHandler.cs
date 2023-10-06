namespace Neuroglia.Mediation;

/// <summary>
/// Defines the fundamentals of a service used to handle notifications of the specified type
/// </summary>
/// <typeparam name="TNotification">The type of notification to handle</typeparam>
public interface INotificationHandler<TNotification>
{

    /// <summary>
    /// Handles the specified notification
    /// </summary>
    /// <param name="notification">The notification to handle</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    Task HandleAsync(TNotification notification, CancellationToken cancellationToken = default);

}
