namespace Neuroglia.Mediation;


/// <summary>
/// Defines the fundamentals of a service used to handle <see cref="IRequest"/>s of the specified type
/// </summary>
/// <typeparam name="TRequest">The type of <see cref="IRequest"/> to handle</typeparam>
/// <typeparam name="TResult">The expected <see cref="IOperationResult"/> type</typeparam>
public interface IRequestHandler<TRequest, TResult>
    where TRequest : IRequest<TResult>
    where TResult : IOperationResult
{

    /// <summary>
    /// Handles the specified <see cref="IRequest"/>
    /// </summary>
    /// <param name="request">The <see cref="IRequest"/> to handle</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The resulting <see cref="IOperationResult"/></returns>
    Task<TResult> HandleAsync(TRequest request, CancellationToken cancellationToken = default);

}

/// <summary>
/// Defines the fundamentals of a service used to handle <see cref="IRequest"/>s of the specified type
/// </summary>
/// <typeparam name="TRequest">The type of <see cref="IRequest"/> to handle</typeparam>
/// <typeparam name="TResult">The expected <see cref="IOperationResult"/> type</typeparam>
/// <typeparam name="T">The type of data returned by the requested operation</typeparam>
public interface IRequestHandler<TRequest, TResult, T>
    : IRequestHandler<TRequest, TResult>
    where TRequest : IRequest<TResult>
    where TResult : IOperationResult<T>
{



}
