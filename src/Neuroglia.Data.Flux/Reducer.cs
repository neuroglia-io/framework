namespace Neuroglia.Data.Flux;

/// <summary>
/// Represents the default implementation of the <see cref="IReducer{TState, TAction}"/> interface
/// </summary>
/// <typeparam name="TState">The type of state to reduce</typeparam>
/// <typeparam name="TAction">The type of flux action the reducer applies to</typeparam>
/// <remarks>
/// Initializes a new <see cref="Reducer{TState, TAction}"/>
/// </remarks>
/// <param name="reduceFunction">The function used to reduce the state</param>
public class Reducer<TState, TAction>(Func<TState, TAction, TState> reduceFunction)
    : IReducer<TState, TAction>
{

    /// <summary>
    /// Gets the function used to reduce the state
    /// </summary>
    protected Func<TState, TAction, TState> ReduceFunction { get; } = reduceFunction;

    /// <inheritdoc/>
    public TState Reduce(TState state, TAction action) => this.ReduceFunction(state, action);

    TState IReducer<TState>.Reduce(TState state, object action) => this.Reduce(state, (TAction)action);

    object IReducer.Reduce(object state, object action) => this.Reduce((TState)state, (TAction)action)!;

}
