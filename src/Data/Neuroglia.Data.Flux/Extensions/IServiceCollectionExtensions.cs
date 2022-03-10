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
using Microsoft.Extensions.Options;
using Neuroglia.Data.Flux.Configuration;

namespace Neuroglia.Data.Flux
{

    /// <summary>
    /// Defines extensions for <see cref="IServiceCollection"/>s
    /// </summary>
    public static class IServiceCollectionExtensions
    {

        /// <summary>
        /// Adds and configures Flux services
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
        /// <param name="setup">An <see cref="Action{T}"/> used to setup Flux</param>
        /// <returns>The configured <see cref="IServiceCollection"/></returns>
        public static IServiceCollection AddFlux(this IServiceCollection services, Action<IFluxOptionsBuilder> setup)
        {
            var builder = new FluxOptionsBuilder();
            setup(builder);
            var options = builder.Build();
            services.AddSingleton(Options.Create(options));
            services.Add(new(typeof(IDispatcher), options.DispatcherType, options.ServiceLifetime));
            services.Add(new(typeof(IStoreFactory), options.StoreFactoryType, options.ServiceLifetime));
            services.Add(new(typeof(IStore), provider => provider.GetRequiredService<IStoreFactory>().CreateStore(), options.ServiceLifetime));
            return services;
        }


        /// <summary>
        /// Adds and configures Flux services
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
        /// <returns>The configured <see cref="IServiceCollection"/></returns>
        public static IServiceCollection AddFlux(this IServiceCollection services) => services.AddFlux(_ => { });

    }

}
