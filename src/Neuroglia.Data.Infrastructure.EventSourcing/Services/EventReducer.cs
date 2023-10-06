using System.Reflection;

namespace Neuroglia.Data.Infrastructure.EventSourcing.Services;

/// <summary>
/// Represents the default implementation of the <see cref="IEventReducer{TEvent, TState}"/> interface
/// </summary>
/// <typeparam name="TState">The type of aggregate to handle</typeparam>
/// <typeparam name="TEvent">The type of events to aggregate</typeparam>
public class EventReducer<TEvent, TState>
   : IEventReducer<TEvent, TState>
   where TState : class
{

    /// <summary>
    /// Initializes a new <see cref="EventReducer{TEvent, TAggregate}"/>
    /// </summary>
    /// <param name="reducerMethod">The reducer <see cref="MethodInfo"/></param>
    public EventReducer(MethodInfo reducerMethod)
    {
        this.ReducerMethod = reducerMethod ?? throw new ArgumentNullException(nameof(reducerMethod));
    }

    /// <summary>
    /// Gets the reducer <see cref="MethodInfo"/>
    /// </summary>
    protected MethodInfo ReducerMethod { get; }

    /// <inheritdoc/>
    public virtual TState Reduce(TEvent e, TState state)
    {
        if (e == null) throw new ArgumentNullException(nameof(e));
        if (state == null) throw new ArgumentNullException(nameof(state));
        this.ReducerMethod.Invoke(state, new object[] { e });
        return state;
    }

    /// <inheritdoc/>
    object IEventReducer.Reduce(object e, object state) => this.Reduce((TEvent)e, (TState)state);

}