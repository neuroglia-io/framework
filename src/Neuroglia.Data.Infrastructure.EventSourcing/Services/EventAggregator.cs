using System.Reflection;

namespace Neuroglia.Data.Infrastructure.EventSourcing.Services;

/// <summary>
/// Represents the default implementation of the <see cref="IEventAggregator{TAggregate}"/> interface
/// </summary>
public class EventAggregator<TState>
    : IEventAggregator<TState>
    where TState : class
{

    /// <summary>
    /// Gets the default name of event reducer methods
    /// </summary>
    public const string DefaultReducerMethodName = "On";
    /// <summary>
    /// Gets the default function used to create new instance of the aggregation's state
    /// </summary>
    static readonly Func<TState> DefaultStateFactory = () => (TState)Activator.CreateInstance(typeof(TState), true)!;

    /// <summary>
    /// Initializes a new <see cref="EventAggregator{TAggregate}"/>
    /// </summary>
    /// <param name="reducerMethodName">The name of event reducer methods</param>
    /// <param name="stateFactory">The function used to create new instance of the aggregation's state</param>
    public EventAggregator(string reducerMethodName = DefaultReducerMethodName, Func<TState>? stateFactory = null)
    {
        if (string.IsNullOrWhiteSpace(reducerMethodName)) reducerMethodName = DefaultReducerMethodName;
        this.StateFactory = stateFactory ?? DefaultStateFactory;
        this.Reducers = typeof(TState).GetMethods(BindingFlags.Default | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            .Where(m => m.Name == reducerMethodName && m.GetParameters().Length == 1)
            .ToDictionary(m => m.GetParameters().First().ParameterType, this.CreateReducer);
    }

    /// <summary>
    /// Gets the function used to create new instance of the aggregation's state
    /// </summary>
    protected Func<TState> StateFactory { get; }

    /// <summary>
    /// Gets an <see cref="IReadOnlyDictionary{TKey, TValue}"/> containing all available <see cref="IEventReducer"/>s
    /// </summary>
    protected IReadOnlyDictionary<Type, IEventReducer> Reducers { get; }

    /// <inheritdoc/>
    public virtual TState Aggregate(IEnumerable<object> events, TState? state = null)
    {
        state ??= (TState)this.StateFactory.Invoke();
        foreach (var e in events)
        {
            if (e == null || !this.Reducers.TryGetValue(e.GetType(), out var reducer) || reducer == null) continue;
            reducer.Reduce(e, state);
        }
        if (state is IVersionedState versionedState) versionedState.StateVersion += (ulong)events.Count();
        return state;
    }

    /// <summary>
    /// Creates a new <see cref="IEventReducer"/>
    /// </summary>
    /// <param name="reducerMethod">The reducer method</param>
    /// <returns>A new <see cref="IEventReducer"/></returns>
    protected virtual IEventReducer CreateReducer(MethodInfo reducerMethod)
    {
        var eventType = reducerMethod.GetParameters()[0].ParameterType;
        var stateType = typeof(TState);
        var reducerType = typeof(EventReducer<,>).MakeGenericType(eventType, stateType);
        return (IEventReducer)Activator.CreateInstance(reducerType, reducerMethod)!;
    }

    object IEventAggregator.Aggregate(IEnumerable<object> events, object? state) => this.Aggregate(events, (TState?)state);

    TState IEventAggregator<TState>.Aggregate(IEnumerable<object> events, TState? state) => this.Aggregate(events, state);

}

/// <summary>
/// Represents the default implementation of the <see cref="IEventAggregator{TAggregate}"/> interface
/// </summary>
/// <typeparam name="TState">The type of the aggregation's state</typeparam>
public class EventAggregator<TState, TEvent>
    : IEventAggregator<TState, TEvent>
    where TState : class
{

    /// <summary>
    /// Gets the default name of event reducer methods
    /// </summary>
    public const string DefaultReducerMethodName = "On";
    /// <summary>
    /// Gets the default function used to create new instance of the aggregation's state
    /// </summary>
    static readonly Func<TState> DefaultStateFactory = () => (TState)Activator.CreateInstance(typeof(TState), true)!;

    /// <summary>
    /// Initializes a new <see cref="EventAggregator{TAggregate}"/>
    /// </summary>
    /// <param name="reducerMethodName">The name of event reducer methods</param>
    /// <param name="stateFactory">The function used to create new instance of the aggregation's state</param>
    public EventAggregator(string reducerMethodName = DefaultReducerMethodName, Func<TState>? stateFactory = null)
    {
        if (string.IsNullOrWhiteSpace(reducerMethodName)) reducerMethodName = DefaultReducerMethodName;
        this.StateFactory = stateFactory ?? DefaultStateFactory;
        this.Reducers = typeof(TState).GetMethods(BindingFlags.Default | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            .Where(m => m.Name == reducerMethodName && m.GetParameters().Length == 1 && typeof(TEvent).IsAssignableFrom(m.GetParameters()[0].ParameterType))
            .ToDictionary(m => m.GetParameters().First().ParameterType, this.CreateReducer);
    }

    /// <summary>
    /// Gets the function used to create new instance of the aggregation's state
    /// </summary>
    protected Func<TState> StateFactory { get; }

    /// <summary>
    /// Gets an <see cref="IReadOnlyDictionary{TKey, TValue}"/> containing all available <see cref="IEventReducer"/>s
    /// </summary>
    protected IReadOnlyDictionary<Type, IEventReducer> Reducers { get; }

    /// <inheritdoc/>
    public virtual TState Aggregate(IEnumerable<TEvent> events, TState? state = null)
    {
        state ??= (TState)this.StateFactory.Invoke();
        foreach (var e in events)
        {
            if (e == null || !this.Reducers.TryGetValue(e.GetType(), out var reducer) || reducer == null) continue;
            reducer.Reduce(e, state);
        }
        if (state is IVersionedState versionedState) versionedState.StateVersion += (ulong)events.Count();
        return state;
    }

    /// <summary>
    /// Creates a new <see cref="IEventReducer"/>
    /// </summary>
    /// <param name="reducerMethod">The reducer method</param>
    /// <returns>A new <see cref="IEventReducer"/></returns>
    protected virtual IEventReducer CreateReducer(MethodInfo reducerMethod)
    {
        var eventType = reducerMethod.GetParameters()[0].ParameterType;
        var stateType = typeof(TState);
        var reducerType = typeof(EventReducer<,>).MakeGenericType(eventType, stateType);
        return (IEventReducer)Activator.CreateInstance(reducerType, reducerMethod)!;
    }

    object IEventAggregator.Aggregate(IEnumerable<object> events, object? state) => this.Aggregate(events.OfType<TEvent>(), (TState?)state);

    TState IEventAggregator<TState>.Aggregate(IEnumerable<object> events, TState? state) => this.Aggregate(events.OfType<TEvent>(), state);

}
