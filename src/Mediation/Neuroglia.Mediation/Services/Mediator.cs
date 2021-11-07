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
using System.Diagnostics;
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

        /// <summary>
        /// Gets the <see cref="Mediator"/>'s <see cref="System.Diagnostics.ActivitySource"/> name
        /// </summary>
        public const string ActivitySourceName = "Neuroglia.Mediation.Diagnostics.ActivitySource";

        private static readonly ActivitySource _ActivitySource = new(ActivitySourceName);
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
            using Activity activity = _ActivitySource.StartActivity(requestType.Name);
            activity?.AddTag("request.type", requestType.FullName);
            activity?.AddTag("request.cqrs_type", request is ICommand ? "command" : "query");
            RequestPipeline<TResult> pipeline = (RequestPipeline<TResult>)_RequestHandlers.GetOrAdd(requestType, t => Activator.CreateInstance(typeof(RequestPipeline<,>).MakeGenericType(requestType, typeof(TResult))));
            TResult result = await pipeline.HandleAsync(request, this.ServiceProvider, cancellationToken).ConfigureAwait(false);
            activity?.AddTag("request.result.code", result.Code);
            return result;
        }

        /// <inheritdoc/>
        public virtual async Task PublishAsync<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        {
            if (notification == null)
                throw new ArgumentNullException(nameof(notification));
            using Activity activity = _ActivitySource.StartActivity(typeof(TNotification).Name);
            activity?.AddTag("notification.type", typeof(TNotification).FullName);
            foreach (INotificationHandler<TNotification> handler in this.ServiceProvider.GetServices<INotificationHandler<TNotification>>())
            {
                await handler.HandleAsync(notification, cancellationToken).ConfigureAwait(false);
            }
        }

    }

}
