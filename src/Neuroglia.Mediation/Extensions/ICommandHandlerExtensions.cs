// Copyright © 2021-Present Neuroglia SRL. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License"),
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Net;

namespace Neuroglia.Mediation;

/// <summary>
/// Defines extensions for <see cref="ICommandHandler"/>s
/// </summary>
#pragma warning disable IDE0060 // Remove unused parameter
public static class ICommandHandlerExtensions
{

    /// <summary>
    /// Creates a new <see cref="IOperationResult"/> indicating that the <see cref="ICommand"/> executed successfully
    /// </summary>
    /// <param name="handler">The extended <see cref="ICommandHandler"/></param>
    /// <param name="data">The data, if any, returned by the operation in case of success</param>
    /// <returns>A new <see cref="IOperationResult"/> indicating that the <see cref="ICommand"/> executed successfully</returns>

    public static IOperationResult Ok<TCommand>(this ICommandHandler<TCommand> handler, object? data = null)
        where TCommand : class, ICommand<IOperationResult>
    {
        return new OperationResult((int)HttpStatusCode.OK, data);
    }

    /// <summary>
    /// Creates a new <see cref="OperationResult{T}"/> indicating that the <see cref="Command{T}"/> executed successfully
    /// </summary>
    /// <param name="handler">The extended <see cref="ICommandHandler{TCommand, TResult}"/></param>
    /// <param name="data">The data wrapped by the dataing <see cref="OperationResult{T}"/></param>
    /// <returns>A new <see cref="OperationResult{T}"/> indicating that the <see cref="Command{T}"/> executed successfully</returns>
    public static IOperationResult<T> Ok<TCommand, T>(this ICommandHandler<TCommand, T> handler, T? data = default)
        where TCommand : class, ICommand<IOperationResult<T>, T>
    {
        return new OperationResult<T>((int)HttpStatusCode.OK, data);
    }

    /// <summary>
    /// Creates a new <see cref="IOperationResult"/> indicating that the <see cref="ICommand"/> is invalid
    /// </summary>
    /// <param name="handler">The extended <see cref="ICommandHandler"/></param>
    /// <param name="errors">An array containing the <see cref="Error"/>s that have occurred</param>
    /// <returns>A new <see cref="IOperationResult"/> indicating that the <see cref="ICommand"/> is invalid</returns>
    public static IOperationResult Invalid<TCommand>(this ICommandHandler<TCommand> handler, params Error[] errors)
        where TCommand : class, ICommand<IOperationResult>
    {
        return new OperationResult((int)HttpStatusCode.BadRequest, errors: errors);
    }

    /// <summary>
    /// Creates a new <see cref="OperationResult{T}"/> indicating that the <see cref="Command{T}"/> is invalid
    /// </summary>
    /// <param name="handler">The extended <see cref="ICommandHandler{TCommand, TResult}"/></param>
    /// <param name="errors">An array containing the <see cref="Error"/>s that have occurred</param>
    /// <returns>A new <see cref="OperationResult{T}"/> indicating that the <see cref="Command{T}"/> is invalid</returns>
    public static IOperationResult<T> Invalid<TCommand, T>(this ICommandHandler<TCommand, T> handler, params Error[] errors)
        where TCommand : class, ICommand<IOperationResult<T>, T>
    {
        return new OperationResult<T>((int)HttpStatusCode.BadRequest, errors: errors);
    }

    /// <summary>
    /// Creates a new <see cref="IOperationResult"/> indicating that an object related to the <see cref="ICommand"/> could not be found
    /// </summary>
    /// <param name="handler">The extended <see cref="ICommandHandler"/></param>
    /// <param name="errors">An array containing the <see cref="Error"/>s that have occurred</param>
    /// <returns>A new <see cref="IOperationResult"/> indicating that an object related to the <see cref="ICommand"/> could not be found</returns>
    public static IOperationResult NotFound<TCommand>(this ICommandHandler<TCommand> handler, params Error[] errors)
        where TCommand : class, ICommand<IOperationResult>
    {
        return new OperationResult((int)HttpStatusCode.NotFound, errors: errors);
    }

    /// <summary>
    /// Creates a new <see cref="IOperationResult"/> indicating that an object related to the <see cref="ICommand"/> could not be found
    /// </summary>
    /// <param name="handler">The extended <see cref="ICommandHandler"/></param>
    /// <param name="errors">An array containing the <see cref="Error"/>s that have occurred</param>
    /// <returns>A new <see cref="IOperationResult"/> indicating that an object related to the <see cref="ICommand"/> could not be found</returns>
    public static IOperationResult<T> NotFound<TCommand, T>(this ICommandHandler<TCommand, T> handler, params Error[] errors)
        where TCommand : class, ICommand<IOperationResult<T>>
    {
        return new OperationResult<T>((int)HttpStatusCode.NotFound, errors: errors);
    }

    /// <summary>
    /// Creates a new <see cref="IOperationResult"/> indicating that an object related to the <see cref="ICommand"/> was not modified
    /// </summary>
    /// <param name="handler">The extended <see cref="ICommandHandler"/></param>
    /// <returns>A new <see cref="IOperationResult"/> indicating that an object related to the <see cref="ICommand"/> was not modified</returns>
    public static IOperationResult NotModified<TCommand>(this ICommandHandler<TCommand> handler)
        where TCommand : class, ICommand<IOperationResult>
    {
        return new OperationResult((int)HttpStatusCode.NotModified);
    }

    /// <summary>
    /// Creates a new <see cref="IOperationResult"/> indicating that an object related to the <see cref="ICommand"/> was not modified
    /// </summary>
    /// <param name="handler">The extended <see cref="ICommandHandler"/></param>
    /// <returns>A new <see cref="IOperationResult"/> indicating that an object related to the <see cref="ICommand"/> was not modified</returns>
    public static IOperationResult<T> NotModified<TCommand, T>(this ICommandHandler<TCommand, T> handler)
        where TCommand : class, ICommand<IOperationResult<T>>
    {
        return new OperationResult<T>((int)HttpStatusCode.NotModified);
    }

    /// <summary>
    /// Creates a new <see cref="IOperationResult"/> indicating that the operation the current user is unauthorized
    /// </summary>
    /// <param name="handler">The extended <see cref="ICommandHandler"/></param>
    /// <param name="errors">An array containing the <see cref="Error"/>s that have occurred</param>
    /// <returns>A new <see cref="IOperationResult"/> indicating that the operation the current user is unauthorized</returns>
    public static IOperationResult Unauthorized<TCommand>(this ICommandHandler<TCommand> handler, params Error[] errors)
        where TCommand : class, ICommand<IOperationResult>
    {
        return new OperationResult((int)HttpStatusCode.Unauthorized, errors);
    }

    /// <summary>
    /// Creates a new <see cref="IOperationResult"/> indicating that the operation the current user is unauthorized
    /// </summary>
    /// <param name="handler">The extended <see cref="ICommandHandler"/></param>
    /// <param name="errors">An array containing the <see cref="Error"/>s that have occurred</param>
    /// <returns>A new <see cref="IOperationResult"/> indicating that the operation the current user is unauthorized</returns>
    public static IOperationResult<T> Unauthorized<TCommand, T>(this ICommandHandler<TCommand, T> handler, params Error[] errors)
        where TCommand : class, ICommand<IOperationResult<T>>
    {
        return new OperationResult<T>((int)HttpStatusCode.Unauthorized, errors);
    }

    /// <summary>
    /// Creates a new <see cref="IOperationResult"/> indicating that the operation is forbidden to the current user
    /// </summary>
    /// <param name="handler">The extended <see cref="ICommandHandler"/></param>
    /// <param name="errors">An array containing the <see cref="Error"/>s that have occurred</param>
    /// <returns>A new <see cref="IOperationResult"/> indicating that the operation is forbidden to the current user</returns>
    public static IOperationResult Forbidden<TCommand>(this ICommandHandler<TCommand> handler, params Error[] errors)
        where TCommand : class, ICommand<IOperationResult>
    {
        return new OperationResult((int)HttpStatusCode.Forbidden, errors);
    }

    /// <summary>
    /// Creates a new <see cref="IOperationResult"/> indicating that the operation is forbidden to the current user
    /// </summary>
    /// <param name="handler">The extended <see cref="ICommandHandler"/></param>
    /// <param name="errors">An array containing the <see cref="Error"/>s that have occurred</param>
    /// <returns>A new <see cref="IOperationResult"/> indicating that the operation is forbidden to the current user</returns>
    public static IOperationResult<T> Forbidden<TCommand, T>(this ICommandHandler<TCommand, T> handler, params Error[] errors)
        where TCommand : class, ICommand<IOperationResult<T>>
    {
        return new OperationResult<T>((int)HttpStatusCode.Forbidden, errors);
    }

    /// <summary>
    /// Creates a new <see cref="IOperationResult"/> indicating that an internal error occurred while handling the <see cref="ICommand"/>
    /// </summary>
    /// <param name="handler">The extended <see cref="ICommandHandler"/></param>
    /// <param name="errors">An array containing the <see cref="Error"/>s that have occurred</param>
    /// <returns>A new <see cref="IOperationResult"/> indicating that an internal error occurred while handling the <see cref="ICommand"/></returns>
    public static IOperationResult InternalError<TCommand>(this ICommandHandler<TCommand> handler, params Error[] errors)
        where TCommand : class, ICommand<IOperationResult>
    {
        return new OperationResult((int)HttpStatusCode.InternalServerError, errors: errors);
    }

    /// <summary>
    /// Creates a new <see cref="IOperationResult"/> indicating that an internal error occurred while handling the <see cref="ICommand"/>
    /// </summary>
    /// <param name="handler">The extended <see cref="ICommandHandler"/></param>
    /// <param name="errors">An array containing the <see cref="Error"/>s that have occurred</param>
    /// <returns>A new <see cref="IOperationResult"/> indicating that an internal error occurred while handling the <see cref="ICommand"/></returns>
    public static IOperationResult<T> InternalError<TCommand, T>(this ICommandHandler<TCommand, T> handler, params Error[] errors)
        where TCommand : class, ICommand<IOperationResult<T>>
    {
        return new OperationResult<T>((int)HttpStatusCode.InternalServerError, errors: errors);
    }

}
#pragma warning restore IDE0060 // Remove unused parameter