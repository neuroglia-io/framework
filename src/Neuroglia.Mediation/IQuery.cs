namespace Neuroglia.Mediation;

/// <summary>
/// Defines the fundamentals of a query
/// </summary>
public interface IQuery
{



}

/// <summary>
/// Defines the fundamentals of a query
/// </summary>
/// <typeparam name="TResult">The expected <see cref="IOperationResult"/> type</typeparam>
public interface IQuery<TResult>
    : IQuery, IRequest<TResult>
    where TResult : IOperationResult
{



}

/// <summary>
/// Defines the fundamentals of a query
/// </summary>
/// <typeparam name="TResult">The type of result returned by the <see cref="IQuery"/></typeparam>
/// <typeparam name="T">The type of data wrapped by the <see cref="IOperationResult{T}"/></typeparam>
public interface IQuery<TResult, T>
    : IQuery<TResult>
    where TResult : IOperationResult<T>
{



}
