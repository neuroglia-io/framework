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
/// Defines extensions for <see cref="IQueryHandler"/>s
/// </summary>
#pragma warning disable IDE0060 // Remove unused parameter
public static class IQueryHandlerExtensions
{

    /// <summary>
    /// Creates a new <see cref="OperationResult{T}"/> indicating that the <see cref="Command{T}"/> executed successfully
    /// </summary>
    /// <param name="handler">The extended <see cref="IQueryHandler"/></param>
    /// <param name="data">The data, if any, returned by the operation, in case of success</param>
    /// <returns>A new <see cref="OperationResult{T}"/> indicating that the <see cref="Command{T}"/> executed successfully</returns>
    public static IOperationResult<T> Ok<TQuery, T>(this IQueryHandler<TQuery, T> handler, T? data = default)
        where TQuery : class, IQuery<IOperationResult<T>, T>
    {
        return new OperationResult<T>((int)HttpStatusCode.OK, data);
    }

    /// <summary>
    /// Creates a new <see cref="OperationResult{T}"/> indicating that the <see cref="Command{T}"/> is invalid
    /// </summary>
    /// <param name="handler">The extended <see cref="IQueryHandler"/></param>
    /// <param name="errors">An array containing the <see cref="Error"/>s that have occurred</param>
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
    /// <param name="errors">An array containing the <see cref="Error"/>s that have occurred</param>
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
    /// <param name="errors">An array containing the <see cref="Error"/>s that have occurred</param>
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
    /// <param name="errors">An array containing the <see cref="Error"/>s that have occurred</param>
    /// <returns>A new <see cref="OperationResult{T}"/> indicating that the operation is forbidden to the current user</returns>
    public static IOperationResult<T> Forbidden<TQuery, T>(this IQueryHandler<TQuery, T> handler, params Error[] errors)
        where TQuery : class, IQuery<IOperationResult<T>, T>
    {
        return new OperationResult<T>((int)HttpStatusCode.Forbidden, errors);
    }

    /// <summary>
    /// Creates a new <see cref="OperationResult{T}"/> indicating that an internal error occurred while handling the <see cref="Command{T}"/>
    /// </summary>
    /// <param name="handler">The extended <see cref="IQueryHandler"/></param>
    /// <param name="errors">An array containing the <see cref="Error"/>s that have occurred</param>
    /// <returns>A new <see cref="OperationResult{T}"/> indicating that an internal error occurred while handling the <see cref="Command{T}"/></returns>
    public static IOperationResult<T> InternalError<TQuery, T>(this IQueryHandler<TQuery, T> handler, params Error[] errors)
        where TQuery : class, IQuery<IOperationResult<T>, T>
    {
        return new OperationResult<T>((int)HttpStatusCode.InternalServerError, errors);
    }

}
#pragma warning restore IDE0060 // Remove unused parameter