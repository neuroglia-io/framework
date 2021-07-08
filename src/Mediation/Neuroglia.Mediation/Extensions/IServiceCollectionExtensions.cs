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
using Microsoft.Extensions.DependencyInjection.Extensions;
using Neuroglia.Mediation.Configuration;
using System;
using System.Linq;
using System.Reflection;

namespace Neuroglia.Mediation
{

    /// <summary>
    /// Defines extensions for <see cref="IServiceCollection"/>s
    /// </summary>
    public static class IServiceCollectionExtensions
    {

        /// <summary>
        /// Adds and configures a new <see cref="Mediator"/> service
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
        /// <param name="configurationAction">The <see cref="Action{T}"/> used to configure the <see cref="IMediator"/> options</param>
        /// <returns>The configured <see cref="IServiceCollection"/></returns>
        public static IServiceCollection AddMediator(this IServiceCollection services, Action<IMediatorOptionsBuilder> configurationAction)
        {
            IMediatorOptionsBuilder optionsBuilder = new MediatorOptionsBuilder();
            configurationAction(optionsBuilder);
            MediatorOptions options = optionsBuilder.Build();
            services.TryAddTransient<Mediator>();
            services.TryAddTransient<IMediator>(provider => provider.GetRequiredService<Mediator>());
            Action<Type> serviceRegistration;
            foreach (Assembly assembly in options.AssembliesToScan)
            {
                foreach (Type implementationType in assembly.GetTypes()
                    .Where(t => !t.IsAbstract && !t.IsInterface && !t.IsGenericType))
                {
                    if (services.Any(s => s.ServiceType == implementationType))
                        serviceRegistration = t => services.AddTransient(t, provider => provider.GetRequiredService(implementationType));
                    else
                        serviceRegistration = t => services.AddTransient(t, implementationType);
                    if (implementationType.IsGenericImplementationOf(typeof(IRequestHandler<,>)))
                    {
                        foreach (Type serviceType in implementationType.GetGenericTypes(typeof(IRequestHandler<,>)).Distinct())
                        {
                            Type requestType = serviceType.GetGenericArguments().First();
                            Type responseType = serviceType.GetGenericArguments().Last();
                            serviceRegistration(serviceType);
                            foreach (Type pipelineType in options.DefaultPipelineBehaviors)
                            {
                                try
                                {
                                    Type behaviorServiceType = typeof(IMiddleware<,>).MakeGenericType(requestType, responseType);
                                    Type behaviorImplementationType = pipelineType.MakeGenericType(requestType, responseType);
                                    services.AddTransient(behaviorServiceType, behaviorImplementationType);
                                }
                                catch { }
                            }
                            foreach (PipelineMiddlewareAttribute middlewareAttribute in requestType
                                .GetCustomAttributes<PipelineMiddlewareAttribute>(true)
                                .OrderBy(a => a.Priority))
                            {
                                Type behaviorServiceType = typeof(IMiddleware<,>).MakeGenericType(requestType, responseType);
                                Type behaviorImplementationType = middlewareAttribute.PipelineBehaviorType;
                                if (behaviorImplementationType.IsGenericType)
                                    behaviorImplementationType = behaviorImplementationType.MakeGenericType(requestType, responseType);
                                services.AddTransient(behaviorServiceType, behaviorImplementationType);
                            }
                        }
                    }
                    if (implementationType.IsGenericImplementationOf(typeof(INotificationHandler<>)))
                    {
                        foreach (Type serviceType in implementationType.GetGenericTypes(typeof(INotificationHandler<>)).Distinct())
                        {
                            serviceRegistration(serviceType);
                        }
                    }
                }
            }
            return services;
        }

    }

}
