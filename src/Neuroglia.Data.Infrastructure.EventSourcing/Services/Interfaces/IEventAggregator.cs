namespace Neuroglia.Data.Infrastructure.EventSourcing.Services;

/// <summary>
/// Defines the fundamentals of a service used to aggregate events
/// </summary>
public interface IEventAggregator
{

    /// <summary>
    /// Aggregates the specified events
    /// </summary>
    /// <param name="events">An <see cref="IEnumerable{T}"/> containing the events to aggregate</param>
    /// <param name="state">The current state, if any</param>
    /// <returns>The resulting state</returns>
    object Aggregate(IEnumerable<object> events, object? state = null);

}

/// <summary>
/// Defines the fundamentals of a service used to aggregate events
/// </summary>
/// <typeparam name="TState">The type of the expected aggregate</typeparam>
public interface IEventAggregator<TState>
    : IEventAggregator
    where TState : class
{

    /// <summary>
    /// Aggregates the specified events
    /// </summary>
    /// <param name="events">An <see cref="IEnumerable{T}"/> containing the events to aggregate</param>
    /// <param name="state">The current state, if any</param>
    /// <returns>The resulting state</returns>
    TState Aggregate(IEnumerable<object> events, TState? state = null);

}

/// <summary>
/// Defines the fundamentals of a service used to aggregate events
/// </summary>
/// <typeparam name="TState">The type of the expected aggregate</typeparam>
/// <typeparam name="TEvent">The expected type of events to aggregate</typeparam>
public interface IEventAggregator<TState, TEvent>
    : IEventAggregator<TState>
    where TState : class
{

    /// <summary>
    /// Aggregates the specified events
    /// </summary>
    /// <param name="events">An <see cref="IEnumerable{T}"/> containing the events to aggregate</param>
    /// <param name="state">The current state, if any</param>
    /// <returns>The resulting state</returns>
    TState Aggregate(IEnumerable<TEvent> events, TState? state = null);

}