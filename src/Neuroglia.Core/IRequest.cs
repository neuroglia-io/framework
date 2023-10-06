namespace Neuroglia;

/// <summary>
/// Defines the fundamentals of an operational request
/// </summary>
public interface IRequest
{



}

/// <summary>
/// Defines the fundamentals of an operational request
/// </summary>
/// <typeparam name="TResult">The expected <see cref="IOperationResult"/> type</typeparam>
public interface IRequest<TResult>
    : IRequest
    where TResult : IOperationResult
{



}

/// <summary>
/// Defines the fundamentals of an operational request
/// </summary>
/// <typeparam name="TResult">The expected <see cref="IOperationResult"/> type</typeparam>
/// <typeparam name="T">The type of data returned by the operation</typeparam>
public interface IRequest<TResult, T>
    : IRequest<TResult>
    where TResult : IOperationResult<T>
{



}
