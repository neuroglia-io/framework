using Neuroglia.Data;
using System.Net;

namespace Neuroglia.Mediation.Services;

/// <summary>
/// Represents an <see cref="IMiddleware{TRequest, TResult}"/> used to handle <see cref="DomainException"/>s during the execution of an <see cref="IRequest"/>
/// </summary>
/// <typeparam name="TRequest">The type of <see cref="IRequest"/> to handle</typeparam>
/// <typeparam name="TResult">The type of expected <see cref="IOperationResult"/></typeparam>
public class DomainExceptionHandlingMiddleware<TRequest, TResult>
    : IMiddleware<TRequest, TResult>
    where TRequest : IRequest<TResult>
    where TResult : IOperationResult
{

    /// <inheritdoc/>
    public virtual async Task<TResult> HandleAsync(TRequest request, RequestHandlerDelegate<TResult> next, CancellationToken cancellationToken = default)
    {
        try
        {
            return await next();
        }
        catch (DomainArgumentException ex)
        {
            if (!this.TryCreateErrorResponse((int)HttpStatusCode.BadRequest, out var response, new Error(ErrorTypes.Invalid, ex.GetType().Name, (int)HttpStatusCode.BadRequest, ex.Message))) throw;
            return response;
        }
        catch (DomainValidationException ex)
        {
            if (!this.TryCreateErrorResponse((int)HttpStatusCode.BadRequest, out var response, errors: ex.ValidationErrors.ToArray())) throw;
            return response;
        }
        catch (DomainNullReferenceException ex)
        {
            if (!this.TryCreateErrorResponse((int)HttpStatusCode.NotFound, out var response, new Error(ErrorTypes.NotFound, ex.GetType().Name, (int)HttpStatusCode.NotFound, ex.Message))) throw;
            return response;

        }
        catch (DomainException ex)
        {
            if (!this.TryCreateErrorResponse((int)HttpStatusCode.BadRequest, out var response, new Error(ErrorTypes.Invalid, ex.GetType().Name, (int)HttpStatusCode.BadRequest, ex.Message))) throw;
            return response;
        }
    }

    /// <summary>
    /// Creates a new error <see cref="IOperationResult"/>
    /// </summary>
    /// <param name="resultCode">The result code of the <see cref="IOperationResult"/> to create</param>
    /// <param name="result">The newly created <see cref="IOperationResult"/></param>
    /// <param name="errors">An array containing the <see cref="Error"/>s that have occured during the processing of the <see cref="ICommand"/></param>
    /// <returns>A new error <see cref="IOperationResult"/></returns>
    protected virtual bool TryCreateErrorResponse(int resultCode, out TResult result, params Error[] errors)
    {
        Type responseType;
        if (typeof(IOperationResult).IsAssignableFrom(typeof(TResult)))
        {
            if (typeof(TResult).IsGenericType) responseType = typeof(OperationResult<>).MakeGenericType(typeof(TResult).GetGenericArguments().First());
            else responseType = typeof(OperationResult);
        }
        else
        {
            result = default!;
            return false;
        }
        result = (TResult)Activator.CreateInstance(responseType, resultCode, errors)!;
        return true;
    }

}
