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
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Neuroglia.Mediation
{

    /// <summary>
    /// Represents the default implementation of the <see cref="IMediator"/> interface
    /// </summary>
    public class Mediator
        : IMediator
    {

        private static readonly ConcurrentDictionary<Type, object> _RequestHandlers = new();

        /// <summary>
        /// Initializes a new <see cref="Mediator"/>
        /// </summary>
        /// <param name="serviceProvider">The current <see cref="IServiceProvider"/></param>
        public Mediator(IServiceProvider serviceProvider)
        {
            this.ServiceProvider = serviceProvider;
        }

        /// <summary>
        /// Gets the current <see cref="IServiceProvider"/>
        /// </summary>
        protected IServiceProvider ServiceProvider { get; }

        /// <inheritdoc/>
        public virtual async Task<TResult> ExecuteAsync<TResult>(IRequest<TResult> request, CancellationToken cancellationToken = default) 
            where TResult : IOperationResult
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));
            Type requestType = request.GetType();
            RequestPipeline<TResult> pipeline = (RequestPipeline<TResult>)_RequestHandlers.GetOrAdd(requestType, t => Activator.CreateInstance(typeof(RequestPipeline<,>).MakeGenericType(requestType, typeof(TResult))));
            return await pipeline.HandleAsync(request, this.ServiceProvider, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public virtual Task PublishAsync<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        {
            if (notification == null)
                throw new ArgumentNullException(nameof(notification));
            foreach (INotificationHandler<TNotification> handler in this.ServiceProvider.GetServices<INotificationHandler<TNotification>>())
            {
                _ = Task.Run(async () => await handler.HandleAsync(notification, cancellationToken).ConfigureAwait(false), cancellationToken);
            }
            return Task.CompletedTask;
        }

    }

}
