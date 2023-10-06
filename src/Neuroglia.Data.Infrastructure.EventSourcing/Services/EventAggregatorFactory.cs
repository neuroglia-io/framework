using System.Collections.Concurrent;

namespace Neuroglia.Data.Infrastructure.EventSourcing.Services;

/// <summary>
/// Represents the default implementation of the <see cref="IEventAggregatorFactory"/> interface
/// </summary>
public class EventAggregatorFactory
    : IEventAggregatorFactory
{

    /// <summary>
    /// Initializes a new <see cref="EventAggregatorFactory"/>
    /// </summary>
    public EventAggregatorFactory()
    {
        this.Aggregators = new ConcurrentDictionary<Type, IEventAggregator>();
    }

    /// <summary>
    /// Gets a <see cref="ConcurrentDictionary{TKey, TValue}"/> containing all available <see cref="IEventAggregator"/>s
    /// </summary>
    protected ConcurrentDictionary<Type, IEventAggregator> Aggregators { get; }

    /// <inheritdoc/>
    public virtual IEventAggregator<TState> CreateAggregator<TState>() 
        where TState : class
    {
        if (!this.Aggregators.TryGetValue(typeof(TState), out var aggregator))
        {
            aggregator = new EventAggregator<TState, object>();
            this.Aggregators.TryAdd(typeof(TState), aggregator);
        }
        return (IEventAggregator<TState>)aggregator;
    }

    /// <inheritdoc/>
    public virtual IEventAggregator<TState, TEvent> CreateAggregator<TState, TEvent>() 
        where TState : class
    {
        if (!this.Aggregators.TryGetValue(typeof(TState), out var aggregator))
        {
            aggregator = new EventAggregator<TState, TEvent>();
            this.Aggregators.TryAdd(typeof(TState), aggregator);
        }
        return (IEventAggregator<TState, TEvent>)aggregator;
    }

}