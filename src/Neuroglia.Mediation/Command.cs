namespace Neuroglia.Mediation;

/// <summary>
/// Represents the default implementation of the <see cref="ICommand{TResult}"/> interface
/// </summary>
public abstract class Command
    : ICommand<IOperationResult>
{

    IDictionary<string, object> ICommand.ContextData { get; } = new Dictionary<string, object>();

}

/// <summary>
/// Represents the default implementation of the <see cref="ICommand{TResult, T}"/> interface
/// </summary>
/// <typeparam name="T">The type of result wrapped by the resulting <see cref="IOperationResult{TResult}"/></typeparam>
public abstract class Command<T>
    : ICommand<IOperationResult<T>, T>
{

    IDictionary<string, object> ICommand.ContextData { get; } = new Dictionary<string, object>();

}
