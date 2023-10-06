using System.Net;

namespace Neuroglia.Mediation;

/// <summary>
/// Defines extensions for <see cref="IQueryHandler"/>s
/// </summary>
public static class IQueryHandlerExtensions
{

    /// <summary>
    /// Creates a new <see cref="OperationResult{T}"/> indicating that the <see cref="Command{T}"/> executed successfully
    /// </summary>
    /// <param name="handler">The extended <see cref="IQueryHandler"/></param>
    /// <returns>A new <see cref="OperationResult{T}"/> indicating that the <see cref="Command{T}"/> executed successfully</returns>
    public static IOperationResult<T> Ok<TQuery, T>(this IQueryHandler<TQuery, T> handler, T? result = default)
        where TQuery : class, IQuery<IOperationResult<T>, T>
    {
        return new OperationResult<T>((int)HttpStatusCode.OK, result);
    }

    /// <summary>
    /// Creates a new <see cref="OperationResult{T}"/> indicating that the <see cref="Command{T}"/> is invalid
    /// </summary>
    /// <param name="handler">The extended <see cref="IQueryHandler"/></param>
    /// <param name="errors">An array containing the <see cref="Error"/>s that have occured</param>
    /// <returns>A new <see cref="OperationResult{T}"/> indicating that the <see cref="Command{T}"/> is invalid</returns>
    public static IOperationResult<T> Invalid<TQuery, T>(this IQueryHandler<TQuery, T> handler, params Error[] errors)
        where TQuery : class, IQuery<IOperationResult<T>, T>
    {
        return new OperationResult<T>((int)HttpStatusCode.BadRequest, errors);
    }

    /// <summary>
    /// Creates a new <see cref="OperationResult{T}"/> indicating that an object related to the <see cref="Command{T}"/> could not be found
    /// </summary>
    /// <param name="handler">The extended <see cref="IQueryHandler"/></param>
    /// <param name="errors">An array containing the <see cref="Error"/>s that have occured</param>
    /// <returns>A new <see cref="OperationResult{T}"/> indicating that an object related to the <see cref="Command{T}"/> could not be found</returns>
    public static IOperationResult<T> NotFound<TQuery, T>(this IQueryHandler<TQuery, T> handler, params Error[] errors)
        where TQuery : class, IQuery<IOperationResult<T>, T>
    {
        return new OperationResult<T>((int)HttpStatusCode.NotFound, errors);
    }

    /// <summary>
    /// Creates a new <see cref="OperationResult{T}"/> indicating that an object related to the <see cref="Command{T}"/> was not modified
    /// </summary>
    /// <param name="handler">The extended <see cref="IQueryHandler"/></param>
    /// <returns>A new <see cref="OperationResult{T}"/> indicating that an object related to the <see cref="Command{T}"/> was not modified</returns>
    public static IOperationResult<T> NotModified<TQuery, T>(this IQueryHandler<TQuery, T> handler)
        where TQuery : class, IQuery<IOperationResult<T>, T>
    {
        return new OperationResult<T>((int)HttpStatusCode.NotModified);
    }

    /// <summary>
    /// Creates a new <see cref="OperationResult{T}"/> indicating that the current user is unauthorized
    /// </summary>
    /// <param name="handler">The extended <see cref="IQueryHandler"/></param>
    /// <param name="errors">An array containing the <see cref="Error"/>s that have occured</param>
    /// <returns>A new <see cref="OperationResult{T}"/> indicating that the operation is forbidden to the current user</returns>
    public static IOperationResult<T> Unauthorized<TQuery, T>(this IQueryHandler<TQuery, T> handler, params Error[] errors)
        where TQuery : class, IQuery<IOperationResult<T>, T>
    {
        return new OperationResult<T>((int)HttpStatusCode.Unauthorized, errors);
    }

    /// <summary>
    /// Creates a new <see cref="OperationResult{T}"/> indicating that the operation is forbidden to the current user
    /// </summary>
    /// <param name="handler">The extended <see cref="IQueryHandler"/></param>
    /// <param name="errors">An array containing the <see cref="Error"/>s that have occured</param>
    /// <returns>A new <see cref="OperationResult{T}"/> indicating that the operation is forbidden to the current user</returns>
    public static IOperationResult<T> Forbidden<TQuery, T>(this IQueryHandler<TQuery, T> handler, params Error[] errors)
        where TQuery : class, IQuery<IOperationResult<T>, T>
    {
        return new OperationResult<T>((int)HttpStatusCode.Forbidden, errors);
    }

    /// <summary>
    /// Creates a new <see cref="OperationResult{T}"/> indicating that an internal error occured while handling the <see cref="Command{T}"/>
    /// </summary>
    /// <param name="handler">The extended <see cref="IQueryHandler"/></param>
    /// <param name="errors">An array containing the <see cref="Error"/>s that have occured</param>
    /// <returns>A new <see cref="OperationResult{T}"/> indicating that an internal error occured while handling the <see cref="Command{T}"/></returns>
    public static IOperationResult<T> InternalError<TQuery, T>(this IQueryHandler<TQuery, T> handler, params Error[] errors)
        where TQuery : class, IQuery<IOperationResult<T>, T>
    {
        return new OperationResult<T>((int)HttpStatusCode.InternalServerError, errors);
    }

}
