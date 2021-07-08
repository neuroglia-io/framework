/*
 * Copyright © 2021 Neuroglia SPRL. All rights reserved.
 * <p>
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * <p>
 * http://www.apache.org/licenses/LICENSE-2.0
 * <p>
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 */
namespace Neuroglia.Mediation
{
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
        public static IOperationResult<T> Ok<TQuery, T>(this IQueryHandler<TQuery, T> handler)
            where TQuery : class, IQuery<IOperationResult<T>, T>
        {
            return new OperationResult<T>(OperationResultCode.Ok);
        }

        /// <summary>
        /// Creates a new <see cref="OperationResult{T}"/> indicating that the <see cref="Command{T}"/> executed successfully
        /// </summary>
        /// <param name="handler">The extended <see cref="IQueryHandler"/></param>
        /// <param name="result">The data wrapped by the resulting <see cref="OperationResult{T}"/></param>
        /// <returns>A new <see cref="OperationResult{T}"/> indicating that the <see cref="Command{T}"/> executed successfully</returns>
        public static IOperationResult<T> Ok<TQuery, T>(this IQueryHandler<TQuery, T> handler, T result)
            where TQuery : class, IQuery<IOperationResult<T>, T>
        {
            return new OperationResult<T>(result);
        }

        /// <summary>
        /// Creates a new <see cref="OperationResult{T}"/> indicating that the <see cref="Command{T}"/> is invalid
        /// </summary>
        /// <param name="handler">The extended <see cref="IQueryHandler"/></param>
        /// <returns>A new <see cref="OperationResult{T}"/> indicating that the <see cref="Command{T}"/> is invalid</returns>
        public static IOperationResult<T> Invalid<TQuery, T>(this IQueryHandler<TQuery, T> handler)
            where TQuery : class, IQuery<IOperationResult<T>, T>
        {
            return new OperationResult<T>(OperationResultCode.Invalid);
        }

        /// <summary>
        /// Creates a new <see cref="OperationResult{T}"/> indicating that the <see cref="Command{T}"/> is invalid
        /// </summary>
        /// <param name="handler">The extended <see cref="IQueryHandler"/></param>
        /// <param name="errorCode">The error code</param>
        /// <param name="errorMessage">The error message</param>
        /// <returns>A new <see cref="OperationResult{T}"/> indicating that the <see cref="Command{T}"/> is invalid</returns>
        public static IOperationResult<T> Invalid<TQuery, T>(this IQueryHandler<TQuery, T> handler, string errorCode, string errorMessage)
            where TQuery : class, IQuery<IOperationResult<T>, T>
        {
            return handler.Invalid()
                .AddError(errorCode, errorMessage);
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
            IOperationResult<T> response = handler.Invalid();
            foreach (Error error in errors)
            {
                response.AddError($"{error.Code}", error.Message);
            }
            return response;
        }

        /// <summary>
        /// Creates a new <see cref="OperationResult{T}"/> indicating that the <see cref="Command{T}"/> is invalid
        /// </summary>
        /// <param name="handler">The extended <see cref="IQueryHandler"/></param>
        /// <param name="errorCode">The error code prefix. Will be concatenated with the code of the specified <see cref="Error"/>s</param>
        /// <param name="errors">An array containing the <see cref="Error"/>s that have occured</param>
        /// <returns>A new <see cref="OperationResult{T}"/> indicating that the <see cref="Command{T}"/> is invalid</returns>
        public static IOperationResult<T> Invalid<TQuery, T>(this IQueryHandler<TQuery, T> handler, string errorCode, params Error[] errors)
            where TQuery : class, IQuery<IOperationResult<T>, T>
        {
            IOperationResult<T> response = handler.Invalid();
            foreach (Error error in errors)
            {
                response.AddError($"{errorCode}{error.Code}", error.Message);
            }
            return response;
        }

        /// <summary>
        /// Creates a new <see cref="OperationResult{T}"/> indicating that an object related to the <see cref="Command{T}"/> could not be found
        /// </summary>
        /// <param name="handler">The extended <see cref="IQueryHandler"/></param>
        /// <returns>A new <see cref="OperationResult{T}"/> indicating that an object related to the <see cref="Command{T}"/> could not be found</returns>
        public static IOperationResult<T> NotFound<TQuery, T>(this IQueryHandler<TQuery, T> handler)
            where TQuery : class, IQuery<IOperationResult<T>, T>
        {
            return new OperationResult<T>(OperationResultCode.NotFound);
        }

        /// <summary>
        /// Creates a new <see cref="OperationResult{T}"/> indicating that an object related to the <see cref="Command{T}"/> could not be found
        /// </summary>
        /// <param name="handler">The extended <see cref="IQueryHandler"/></param>
        /// <param name="errorCode">The error code</param>
        /// <param name="errorMessage">The error message</param>
        /// <returns>A new <see cref="OperationResult{T}"/> indicating that an object related to the <see cref="Command{T}"/> could not be found</returns>
        public static IOperationResult<T> NotFound<TQuery, T>(this IQueryHandler<TQuery, T> handler, string errorCode, string errorMessage)
            where TQuery : class, IQuery<IOperationResult<T>, T>
        {
            return (IOperationResult<T>)handler.NotFound()
                .AddError(errorCode, errorMessage);
        }

        /// <summary>
        /// Creates a new <see cref="OperationResult{T}"/> indicating that an object related to the <see cref="Command{T}"/> was not modified
        /// </summary>
        /// <param name="handler">The extended <see cref="IQueryHandler"/></param>
        /// <returns>A new <see cref="OperationResult{T}"/> indicating that an object related to the <see cref="Command{T}"/> was not modified</returns>
        public static IOperationResult<T> NotModified<TQuery, T>(this IQueryHandler<TQuery, T> handler)
            where TQuery : class, IQuery<IOperationResult<T>, T>
        {
            return new OperationResult<T>(OperationResultCode.NotModified);
        }

        /// <summary>
        /// Creates a new <see cref="OperationResult{T}"/> indicating that the operation is forbidden to the current user
        /// </summary>
        /// <param name="handler">The extended <see cref="IQueryHandler"/></param>
        /// <returns>A new <see cref="OperationResult{T}"/> indicating that the operation is forbidden to the current user</returns>
        public static IOperationResult<T> Forbid<TQuery, T>(this IQueryHandler<TQuery, T> handler)
            where TQuery : class, IQuery<IOperationResult<T>, T>
        {
            return new OperationResult<T>(OperationResultCode.Forbidden);
        }

        /// <summary>
        /// Creates a new <see cref="OperationResult{T}"/> indicating that an internal error occured while handling the <see cref="Command{T}"/>
        /// </summary>
        /// <param name="handler">The extended <see cref="IQueryHandler"/></param>
        /// <returns>A new <see cref="OperationResult{T}"/> indicating that an internal error occured while handling the <see cref="Command{T}"/></returns>
        public static IOperationResult<T> InternalError<TQuery, T>(this IQueryHandler<TQuery, T> handler)
            where TQuery : class, IQuery<IOperationResult<T>, T>
        {
            return new OperationResult<T>(OperationResultCode.InternalError);
        }

    }

}
