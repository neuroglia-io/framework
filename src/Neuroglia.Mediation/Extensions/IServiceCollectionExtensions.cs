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

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Neuroglia.Mediation.Configuration;
using System.Reflection;

namespace Neuroglia.Mediation;

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
    public static IServiceCollection AddMediator(this IServiceCollection services, Action<IMediatorOptionsBuilder>? configurationAction = null)
    {
        var optionsBuilder = new MediatorOptionsBuilder();
        configurationAction?.Invoke(optionsBuilder);
        var options = optionsBuilder.Build();
        services.TryAddTransient<Mediator>();
        services.TryAddTransient<IMediator>(provider => provider.GetRequiredService<Mediator>());
        Action<Type> serviceRegistration;
        foreach (var assembly in options.AssembliesToScan)
        {
            foreach (var implementationType in assembly.GetTypes()
                .Where(t => !t.IsAbstract && !t.IsInterface && !t.IsGenericType))
            {
                if (services.Any(s => s.ServiceType == implementationType)) serviceRegistration = t => services.AddTransient(t, provider => provider.GetRequiredService(implementationType));
                else serviceRegistration = t => services.AddTransient(t, implementationType);
                if (implementationType.IsGenericImplementationOf(typeof(IRequestHandler<,>)))
                {
                    foreach (Type serviceType in implementationType.GetGenericTypes(typeof(IRequestHandler<,>)).Distinct())
                    {
                        var requestType = serviceType.GetGenericArguments().First();
                        var responseType = serviceType.GetGenericArguments().Last();
                        serviceRegistration(serviceType);
                        foreach (var pipelineType in options.DefaultPipelineBehaviors)
                        {
                            try
                            {
                                var behaviorServiceType = typeof(IMiddleware<,>).MakeGenericType(requestType, responseType);
                                var behaviorImplementationType = pipelineType.MakeGenericType(requestType, responseType);
                                services.AddTransient(behaviorServiceType, behaviorImplementationType);
                            }
                            catch { }
                        }
                        foreach (PipelineMiddlewareAttribute middlewareAttribute in requestType
                            .GetCustomAttributes<PipelineMiddlewareAttribute>(true)
                            .OrderBy(a => a.Priority))
                        {
                            var behaviorServiceType = typeof(IMiddleware<,>).MakeGenericType(requestType, responseType);
                            var behaviorImplementationType = middlewareAttribute.PipelineBehaviorType;
                            if (behaviorImplementationType.IsGenericType)
                                behaviorImplementationType = behaviorImplementationType.MakeGenericType(requestType, responseType);
                            services.AddTransient(behaviorServiceType, behaviorImplementationType);
                        }
                    }
                }
                if (implementationType.IsGenericImplementationOf(typeof(INotificationHandler<>)))
                {
                    foreach (var serviceType in implementationType.GetGenericTypes(typeof(INotificationHandler<>)).Distinct())
                    {
                        serviceRegistration(serviceType);
                    }
                }
            }
        }
        return services;
    }

}
