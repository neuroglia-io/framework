﻿namespace Neuroglia.Data.Flux;

/// <summary>
/// Represents the default implementation of the <see cref="IState{TState}"/> interface
/// </summary>
/// <typeparam name="TState">The type of the <see cref="IState"/>'s value</typeparam>
public class State<TState>
    : IState<TState>
{

    /// <summary>
    /// Initializes a new <see cref="State{TState}"/>
    /// </summary>
    /// <param name="value">The <see cref="State{TState}"/>'s value</param>
    public State(TState value)
    {
        if(value == null) throw new ArgumentNullException(nameof(value));
        this.Value = value;
    }

    /// <inheritdoc/>
    public TState Value { get; protected set; }

    object IState.Value => this.Value!;

    /// <summary>
    /// Gets the <see cref="Subject"/> used to stream state values
    /// </summary>
    protected Subject<TState> Stream { get; } = new();

    /// <summary>
    /// Gets a <see cref="Dictionary{TKey, TValue}"/> containing the type/<see cref="IReducer"/>s mappings
    /// </summary>
    protected Dictionary<Type, List<IReducer<TState>>> Reducers { get; } = [];

    /// <inheritdoc/>
    public virtual void AddReducer(IReducer<TState> reducer)
    {
        ArgumentNullException.ThrowIfNull(reducer);
        var reducerGenericType = reducer.GetType().GetGenericType(typeof(IReducer<,>)) ?? throw new Exception($"The specified {nameof(IReducer<TState>)} '{reducer.GetType()}' does not implement the '{typeof(IReducer<,>)}' interface");
        var actionType = reducerGenericType.GetGenericArguments()[1];
        if (this.Reducers.TryGetValue(actionType, out var reducers)) reducers.Add(reducer);
        else this.Reducers.Add(actionType, [reducer]);
    }

    void IState.AddReducer(IReducer reducer)
    {
        ArgumentNullException.ThrowIfNull(reducer);
        this.AddReducer((IReducer<TState>)reducer);
    }

    /// <inheritdoc/>
    public virtual IDisposable Subscribe(IObserver<TState> observer) => this.Stream.Subscribe(observer);

    /// <inheritdoc/>
    public virtual bool TryDispatch(object action)
    {
        ArgumentNullException.ThrowIfNull(action);
        if (!this.Reducers.TryGetValue(action.GetType(), out var reducers))return false;
        foreach(var reducer in reducers) this.Value = reducer.Reduce(this.Value, action);
        this.Stream.OnNext(this.Value!);
        return true;
    }

}
