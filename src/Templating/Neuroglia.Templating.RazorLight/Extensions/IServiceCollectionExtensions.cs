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
using Neuroglia.Templating.Services;
using RazorLight;
using System;

namespace Neuroglia.Templating
{

    /// <summary>
    /// Defines extensions for <see cref="IServiceCollection"/>
    /// </summary>
    public static class IServiceCollectionExtensions
    {

        /// <summary>
        /// Adds and configures a <see cref="RazorLightTemplateRenderer"/>
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
        /// <param name="setup">An <see cref="Action{T}"/> used to setup the underlying <see cref="RazorLightEngine"/></param>
        /// <param name="lifetime">The <see cref="ServiceLifetime"/> of all configured services</param>
        /// <returns>The configured <see cref="IServiceCollection"/></returns>
        public static IServiceCollection AddRazorLightTemplateRenderer(this IServiceCollection services, Action<RazorLightEngineBuilder> setup, ServiceLifetime lifetime = ServiceLifetime.Singleton)
        {
            if (setup == null)
                throw new ArgumentNullException(nameof(setup));
            services.TryAdd(new ServiceDescriptor(typeof(IRazorLightEngine), provider =>
            {
                RazorLightEngineBuilder builder = new();
                setup(builder);
                return builder.Build();
            }, lifetime));
            services.TryAdd(new ServiceDescriptor(typeof(RazorLightTemplateRenderer), typeof(RazorLightTemplateRenderer), lifetime));
            services.TryAdd(new ServiceDescriptor(typeof(ITemplateRenderer), provider => provider.GetRequiredService<RazorLightTemplateRenderer>(), lifetime));
            return services;
        }

    }

}
