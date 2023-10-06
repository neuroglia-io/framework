namespace Neuroglia.Data.Infrastructure.EventSourcing.Services;

/// <summary>
/// Defines the fundamentals of a service used to create <see cref="IEventAggregator"/>s
/// </summary>
public interface IEventAggregatorFactory
{

    /// <summary>
    /// Creates a new <see cref="IEventAggregator"/>
    /// </summary>
    /// <typeparam name="TState">The type of the state aggregated events apply to</typeparam>
    /// <returns>A new <see cref="IEventAggregator"/></returns>
    IEventAggregator<TState> CreateAggregator<TState>()
        where TState : class;

    /// <summary>
    /// Creates a new <see cref="IEventAggregator"/>
    /// </summary>
    /// <typeparam name="TState">The type of the state aggregated events apply to</typeparam>
    /// <typeparam name="TEvent">The type of events to aggregate</typeparam>
    /// <returns>A new <see cref="IEventAggregator"/></returns>
    IEventAggregator<TState, TEvent> CreateAggregator<TState, TEvent>()
        where TState : class;

}