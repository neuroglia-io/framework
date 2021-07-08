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
    /// Defines extensions for <see cref="ICommandHandler"/>s
    /// </summary>
    public static class ICommandHandlerExtensions
    {

        /// <summary>
        /// Creates a new <see cref="IOperationResult"/> indicating that the <see cref="ICommand"/> executed successfully
        /// </summary>
        /// <param name="handler">The extended <see cref="ICommandHandler"/></param>
        /// <returns>A new <see cref="IOperationResult"/> indicating that the <see cref="ICommand"/> executed successfully</returns>
        public static IOperationResult Ok<TCommand>(this ICommandHandler<TCommand> handler)
            where TCommand : class, ICommand<IOperationResult>
        {
            return new OperationResult(OperationResultCode.Ok);
        }

        /// <summary>
        /// Creates a new <see cref="IOperationResult"/> indicating that the <see cref="ICommand"/> is invalid
        /// </summary>
        /// <param name="handler">The extended <see cref="ICommandHandler"/></param>
        /// <returns>A new <see cref="IOperationResult"/> indicating that the <see cref="ICommand"/> is invalid</returns>
        public static IOperationResult Invalid<TCommand>(this ICommandHandler<TCommand> handler)
            where TCommand : class, ICommand<IOperationResult>
        {
            return new OperationResult(OperationResultCode.Invalid);
        }

        /// <summary>
        /// Creates a new <see cref="IOperationResult"/> indicating that the <see cref="ICommand"/> is invalid
        /// </summary>
        /// <param name="handler">The extended <see cref="ICommandHandler"/></param>
        /// <param name="errorCode">The error code</param>
        /// <param name="errorMessage">The error message</param>
        /// <returns>A new <see cref="IOperationResult"/> indicating that the <see cref="ICommand"/> is invalid</returns>
        public static IOperationResult Invalid<TCommand>(this ICommandHandler<TCommand> handler, string errorCode, string errorMessage)
            where TCommand : class, ICommand<IOperationResult>
        {
            return handler.Invalid()
                .AddError(errorCode, errorMessage);
        }

        /// <summary>
        /// Creates a new <see cref="IOperationResult"/> indicating that the <see cref="ICommand"/> is invalid
        /// </summary>
        /// <param name="handler">The extended <see cref="ICommandHandler"/></param>
        /// <param name="errors">An array containing the <see cref="Error"/>s that have occured</param>
        /// <returns>A new <see cref="IOperationResult"/> indicating that the <see cref="ICommand"/> is invalid</returns>
        public static IOperationResult Invalid<TCommand>(this ICommandHandler<TCommand> handler, params Error[] errors)
            where TCommand : class, ICommand<IOperationResult>
        {
            IOperationResult response = handler.Invalid();
            foreach (Error error in errors)
            {
                response.AddError($"{error.Code}", error.Message);
            }
            return response;
        }

        /// <summary>
        /// Creates a new <see cref="IOperationResult"/> indicating that the <see cref="ICommand"/> is invalid
        /// </summary>
        /// <param name="handler">The extended <see cref="ICommandHandler"/></param>
        /// <param name="errorCode">The error code prefix. Will be concatenated with the code of the specified <see cref="Error"/>s</param>
        /// <param name="errors">An array containing the <see cref="Error"/>s that have occured</param>
        /// <returns>A new <see cref="IOperationResult"/> indicating that the <see cref="ICommand"/> is invalid</returns>
        public static IOperationResult Invalid<TCommand>(this ICommandHandler<TCommand> handler, string errorCode, params Error[] errors)
            where TCommand : class, ICommand<IOperationResult>
        {
            IOperationResult response = handler.Invalid();
            foreach (Error error in errors)
            {
                response.AddError($"{errorCode}{error.Code}", error.Message);
            }
            return response;
        }

        /// <summary>
        /// Creates a new <see cref="IOperationResult"/> indicating that an object related to the <see cref="ICommand"/> could not be found
        /// </summary>
        /// <param name="handler">The extended <see cref="ICommandHandler"/></param>
        /// <returns>A new <see cref="IOperationResult"/> indicating that an object related to the <see cref="ICommand"/> could not be found</returns>
        public static IOperationResult NotFound<TCommand>(this ICommandHandler<TCommand> handler)
            where TCommand : class, ICommand<IOperationResult>
        {
            return new OperationResult(OperationResultCode.NotFound);
        }

        /// <summary>
        /// Creates a new <see cref="IOperationResult"/> indicating that an object related to the <see cref="ICommand"/> could not be found
        /// </summary>
        /// <param name="handler">The extended <see cref="ICommandHandler"/></param>
        /// <param name="errorCode">The error code</param>
        /// <param name="errorMessage">The error message</param>
        /// <returns>A new <see cref="IOperationResult"/> indicating that an object related to the <see cref="ICommand"/> could not be found</returns>
        public static IOperationResult NotFound<TCommand>(this ICommandHandler<TCommand> handler, string errorCode, string errorMessage)
            where TCommand : class, ICommand<IOperationResult>
        {
            return handler.NotFound()
                .AddError(errorCode, errorMessage);
        }

        /// <summary>
        /// Creates a new <see cref="IOperationResult"/> indicating that an object related to the <see cref="ICommand"/> was not modified
        /// </summary>
        /// <param name="handler">The extended <see cref="ICommandHandler"/></param>
        /// <returns>A new <see cref="IOperationResult"/> indicating that an object related to the <see cref="ICommand"/> was not modified</returns>
        public static IOperationResult NotModified<TCommand>(this ICommandHandler<TCommand> handler)
            where TCommand : class, ICommand<IOperationResult>
        {
            return new OperationResult(OperationResultCode.NotModified);
        }

        /// <summary>
        /// Creates a new <see cref="IOperationResult"/> indicating that the operation is forbidden to the current user
        /// </summary>
        /// <param name="handler">The extended <see cref="ICommandHandler"/></param>
        /// <returns>A new <see cref="IOperationResult"/> indicating that the operation is forbidden to the current user</returns>
        public static IOperationResult Forbid<TCommand>(this ICommandHandler<TCommand> handler)
            where TCommand : class, ICommand<IOperationResult>
        {
            return new OperationResult(OperationResultCode.Forbidden);
        }

        /// <summary>
        /// Creates a new <see cref="IOperationResult"/> indicating that an internal error occured while handling the <see cref="ICommand"/>
        /// </summary>
        /// <param name="handler">The extended <see cref="ICommandHandler"/></param>
        /// <returns>A new <see cref="IOperationResult"/> indicating that an internal error occured while handling the <see cref="ICommand"/></returns>
        public static IOperationResult InternalError<TCommand>(this ICommandHandler<TCommand> handler)
            where TCommand : class, ICommand<IOperationResult>
        {
            return new OperationResult(OperationResultCode.InternalError);
        }

        /// <summary>
        /// Creates a new <see cref="OperationResult{T}"/> indicating that the <see cref="Command{T}"/> executed successfully
        /// </summary>
        /// <param name="handler">The extended <see cref="ICommandHandler{TCommand, TResult}"/></param>
        /// <returns>A new <see cref="OperationResult{T}"/> indicating that the <see cref="Command{T}"/> executed successfully</returns>
        public static IOperationResult<T> Ok<TCommand, T>(this ICommandHandler<TCommand, T> handler)
            where TCommand : class, ICommand<IOperationResult<T>, T>
        {
            return new OperationResult<T>(OperationResultCode.Ok);
        }

        /// <summary>
        /// Creates a new <see cref="OperationResult{T}"/> indicating that the <see cref="Command{T}"/> executed successfully
        /// </summary>
        /// <param name="handler">The extended <see cref="ICommandHandler{TCommand, TResult}"/></param>
        /// <param name="result">The data wrapped by the resulting <see cref="OperationResult{T}"/></param>
        /// <returns>A new <see cref="OperationResult{T}"/> indicating that the <see cref="Command{T}"/> executed successfully</returns>
        public static IOperationResult<T> Ok<TCommand, T>(this ICommandHandler<TCommand, T> handler, T result)
            where TCommand : class, ICommand<IOperationResult<T>, T>
        {
            return new OperationResult<T>(result);
        }

        /// <summary>
        /// Creates a new <see cref="OperationResult{T}"/> indicating that the <see cref="Command{T}"/> is invalid
        /// </summary>
        /// <param name="handler">The extended <see cref="ICommandHandler{TCommand, TResult}"/></param>
        /// <returns>A new <see cref="OperationResult{T}"/> indicating that the <see cref="Command{T}"/> is invalid</returns>
        public static IOperationResult<T> Invalid<TCommand, T>(this ICommandHandler<TCommand, T> handler)
            where TCommand : class, ICommand<IOperationResult<T>, T>
        {
            return new OperationResult<T>(OperationResultCode.Invalid);
        }

        /// <summary>
        /// Creates a new <see cref="OperationResult{T}"/> indicating that the <see cref="Command{T}"/> is invalid
        /// </summary>
        /// <param name="handler">The extended <see cref="ICommandHandler{TCommand, TResult}"/></param>
        /// <param name="errorCode">The error code</param>
        /// <param name="errorMessage">The error message</param>
        /// <returns>A new <see cref="OperationResult{T}"/> indicating that the <see cref="Command{T}"/> is invalid</returns>
        public static IOperationResult<T> Invalid<TCommand, T>(this ICommandHandler<TCommand, T> handler, string errorCode, string errorMessage)
            where TCommand : class, ICommand<IOperationResult<T>, T>
        {
            return handler.Invalid()
                .AddError(errorCode, errorMessage);
        }

        /// <summary>
        /// Creates a new <see cref="OperationResult{T}"/> indicating that the <see cref="Command{T}"/> is invalid
        /// </summary>
        /// <param name="handler">The extended <see cref="ICommandHandler{TCommand, TResult}"/></param>
        /// <param name="errors">An array containing the <see cref="Error"/>s that have occured</param>
        /// <returns>A new <see cref="OperationResult{T}"/> indicating that the <see cref="Command{T}"/> is invalid</returns>
        public static IOperationResult<T> Invalid<TCommand, T>(this ICommandHandler<TCommand, T> handler, params Error[] errors)
            where TCommand : class, ICommand<IOperationResult<T>, T>
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
        /// <param name="handler">The extended <see cref="ICommandHandler{TCommand, TResult}"/></param>
        /// <param name="errorPrefix">The error code prefix. Will be concatenated with the code of the specified <see cref="Error"/>s</param>
        /// <param name="errors">An array containing the <see cref="Error"/>s that have occured</param>
        /// <returns>A new <see cref="OperationResult{T}"/> indicating that the <see cref="Command{T}"/> is invalid</returns>
        public static IOperationResult<T> Invalid<TCommand, T>(this ICommandHandler<TCommand, T> handler, string errorPrefix, params Error[] errors)
            where TCommand : class, ICommand<IOperationResult<T>, T>
        {
            IOperationResult<T> response = handler.Invalid();
            foreach (Error error in errors)
            {
                response.AddError($"{errorPrefix}{error.Code}", error.Message);
            }
            return response;
        }

        /// <summary>
        /// Creates a new <see cref="OperationResult{T}"/> indicating that an object related to the <see cref="Command{T}"/> could not be found
        /// </summary>
        /// <param name="handler">The extended <see cref="ICommandHandler{TCommand, TResult}"/></param>
        /// <returns>A new <see cref="OperationResult{T}"/> indicating that an object related to the <see cref="Command{T}"/> could not be found</returns>
        public static IOperationResult<T> NotFound<TCommand, T>(this ICommandHandler<TCommand, T> handler)
            where TCommand : class, ICommand<IOperationResult<T>, T>
        {
            return new OperationResult<T>(OperationResultCode.NotFound);
        }

        /// <summary>
        /// Creates a new <see cref="OperationResult{T}"/> indicating that an object related to the <see cref="Command{T}"/> could not be found
        /// </summary>
        /// <param name="handler">The extended <see cref="ICommandHandler{TCommand, TResult}"/></param>
        /// <param name="errorPrefix">The error code</param>
        /// <param name="errorMessage">The error message</param>
        /// <returns>A new <see cref="OperationResult{T}"/> indicating that an object related to the <see cref="Command{T}"/> could not be found</returns>
        public static IOperationResult<T> NotFound<TCommand, T>(this ICommandHandler<TCommand, T> handler, string errorPrefix, string errorMessage)
            where TCommand : class, ICommand<IOperationResult<T>, T>
        {
            return handler.NotFound()
                .AddError(errorPrefix, errorMessage);
        }

        /// <summary>
        /// Creates a new <see cref="OperationResult{T}"/> indicating that an object related to the <see cref="Command{T}"/> was not modified
        /// </summary>
        /// <param name="handler">The extended <see cref="ICommandHandler{TCommand, TResult}"/></param>
        /// <returns>A new <see cref="OperationResult{T}"/> indicating that an object related to the <see cref="Command{T}"/> was not modified</returns>
        public static IOperationResult<T> NotModified<TCommand, T>(this ICommandHandler<TCommand, T> handler)
            where TCommand : class, ICommand<IOperationResult<T>, T>
        {
            return new OperationResult<T>(OperationResultCode.NotModified);
        }

        /// <summary>
        /// Creates a new <see cref="OperationResult{T}"/> indicating that the operation is forbidden to the current user
        /// </summary>
        /// <param name="handler">The extended <see cref="ICommandHandler{TCommand, TResult}"/></param>
        /// <returns>A new <see cref="OperationResult{T}"/> indicating that the operation is forbidden to the current user</returns>
        public static IOperationResult<T> Forbid<TCommand, T>(this ICommandHandler<TCommand, T> handler)
            where TCommand : class, ICommand<IOperationResult<T>, T>
        {
            return new OperationResult<T>(OperationResultCode.Forbidden);
        }

        /// <summary>
        /// Creates a new <see cref="OperationResult{T}"/> indicating that an internal error occured while handling the <see cref="Command{T}"/>
        /// </summary>
        /// <param name="handler">The extended <see cref="ICommandHandler{TCommand, TResult}"/></param>
        /// <returns>A new <see cref="OperationResult{T}"/> indicating that an internal error occured while handling the <see cref="Command{T}"/></returns>
        public static IOperationResult<T> InternalError<TCommand, T>(this ICommandHandler<TCommand, T> handler)
            where TCommand : class, ICommand<IOperationResult<T>, T>
        {
            return new OperationResult<T>(OperationResultCode.InternalError);
        }

    }

}
