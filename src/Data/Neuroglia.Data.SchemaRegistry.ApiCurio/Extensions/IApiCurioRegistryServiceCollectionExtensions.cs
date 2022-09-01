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

namespace Neuroglia.Data.SchemaRegistry
{

    /// <summary>
    /// Defines extensions for <see cref="IServiceCollection"/>s
    /// </summary>
    public static class IApiCurioRegistryServiceCollectionExtensions
    {

        /// <summary>
        /// Adds and configures a new Api Curio Registry client
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
        /// <param name="configuration">The current <see cref="ApiCurioRegistryClientOptions"/></param>
        /// <param name="setup">An <see cref="Action{T}"/> used to setup <see cref="ApiCurioRegistryClientOptions"/></param>
        /// <param name="lifetime"></param>
        /// <returns></returns>
        public static IServiceCollection AddApiCurioRegistryClient(this IServiceCollection services, IConfiguration? configuration, Action<ApiCurioRegistryClientOptions>? setup = null, ServiceLifetime lifetime = ServiceLifetime.Singleton)
        {
            var options = new ApiCurioRegistryClientOptions();
            if(configuration != null)
                configuration.Bind("ApiCurioRegistry", options);
            if (setup != null)
                setup(options);
            services.AddSingleton(Options.Create(options));
            services.AddHttpClient(typeof(ApiCurioRegistryClient).Name, httpClient =>
            {
                httpClient.BaseAddress = options.ServerUri;
            });
            services.Add(new(typeof(ApiCurioRegistryClient), typeof(ApiCurioRegistryClient), lifetime));
            services.Add(new(typeof(IApiCurioRegistryClient), provider => provider.GetRequiredService<ApiCurioRegistryClient>(), lifetime));
            return services;
        }

        /// <summary>
        /// Adds and configures a new Api Curio Registry client
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
        /// <param name="setup">An <see cref="Action{T}"/> used to setup <see cref="ApiCurioRegistryClientOptions"/></param>
        /// <param name="lifetime"></param>
        /// <returns></returns>
        public static IServiceCollection AddApiCurioRegistryClient(this IServiceCollection services, Action<ApiCurioRegistryClientOptions>? setup = null, ServiceLifetime lifetime = ServiceLifetime.Singleton)
        {
            return services.AddApiCurioRegistryClient(null, setup, lifetime);
        }

    }
}
