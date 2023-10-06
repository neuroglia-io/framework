namespace Neuroglia.Mediation;


/// <summary>
/// Defines the fundamentals of a service used to handle <see cref="ICommand"/>s of the specified type
/// </summary>
public interface ICommandHandler
{



}

/// <summary>
/// Defines the fundamentals of a service used to handle <see cref="ICommand"/>s of the specified type
/// </summary>
/// <typeparam name="TCommand">The type of <see cref="ICommand"/>s to handle</typeparam>
public interface ICommandHandler<TCommand>
    : ICommandHandler, IRequestHandler<TCommand, IOperationResult>
    where TCommand : class, ICommand<IOperationResult>
{



}

/// <summary>
/// Defines the fundamentals of a service used to handle <see cref="ICommand"/>s of the specified type
/// </summary>
/// <typeparam name="TCommand">The type of <see cref="ICommand"/>s to handle</typeparam>
/// <typeparam name="T">The type of data wrapped by the resulting <see cref="IOperationResult"/></typeparam>
public interface ICommandHandler<TCommand, T>
    : ICommandHandler, IRequestHandler<TCommand, IOperationResult<T>>
    where TCommand : class, ICommand<IOperationResult<T>>
{



}
