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
using Neuroglia.Data;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Neuroglia.Mediation
{
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
                if (!this.TryCreateErrorResponse(OperationResultCode.Invalid, out TResult response, new Error(ex.ArgumentName, ex.Message)))
                    throw;
                return response;
            }
            catch (DomainValidationException ex)
            {
                if (!this.TryCreateErrorResponse(OperationResultCode.Invalid, out TResult response, errors: ex.ValidationErrors.ToArray()))
                    throw;
                return response;
            }
            catch (DomainNullReferenceException ex)
            {
                if (!this.TryCreateErrorResponse(OperationResultCode.NotFound, out TResult response, new Error(string.Empty, ex.Message)))
                    throw;
                return response;

            }
            catch (DomainException ex)
            {
                if (!this.TryCreateErrorResponse(OperationResultCode.Invalid, out TResult response, new Error(string.Empty, ex.Message)))
                    throw;
                return response;
            }
        }

        /// <summary>
        /// Creates a new error <see cref="IOperationResult"/>
        /// </summary>
        /// <param name="resultCode">The <see cref="OperationResultCode"/> of the <see cref="IOperationResult"/> to create</param>
        /// <param name="result">The newly created <see cref="IOperationResult"/></param>
        /// <param name="errors">An array containing the <see cref="Error"/>s that have occured during the processing of the <see cref="ICommand"/></param>
        /// <returns>A new error <see cref="IOperationResult"/></returns>
        protected virtual bool TryCreateErrorResponse(string resultCode, out TResult result, params Error[] errors)
        {
            Type responseType;
            if (typeof(IOperationResult).IsAssignableFrom(typeof(TResult)))
            {
                if (typeof(TResult).IsGenericType)
                    responseType = typeof(OperationResult<>).MakeGenericType(typeof(TResult).GetGenericArguments().First());
                else
                    responseType = typeof(OperationResult);
            }
            else
            {
                result = default;
                return false;
            }
            result = (TResult)Activator.CreateInstance(responseType, resultCode, errors);
            return true;
        }

    }

}
