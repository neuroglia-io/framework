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
using System.Threading;
using System.Threading.Tasks;

namespace Neuroglia.Mediation
{

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

}
