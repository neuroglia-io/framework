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
    /// Defines the fundamentals of a service used to mediate calls
    /// </summary>
    public interface IMediator
    {

        /// <summary>
        /// Executes the specified <see cref="IRequest"/>
        /// </summary>
        /// <typeparam name="TResult">The expected <see cref="IOperationResult"/> type</typeparam>
        /// <param name="request">The <see cref="IRequest"/> to execute</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>The resulting <see cref="IOperationResult"/></returns>
        Task<TResult> ExecuteAsync<TResult>(IRequest<TResult> request, CancellationToken cancellationToken = default)
            where TResult : IOperationResult;

        /// <summary>
        /// Publishes the specified notification
        /// </summary>
        /// <typeparam name="TNotification">The type of notification to publish</typeparam>
        /// <param name="notification">The notification to publish</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        Task PublishAsync<TNotification>(TNotification notification, CancellationToken cancellationToken = default);

    }

}
