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

namespace Neuroglia.Serialization
{

    /// <summary>
    /// Defines extensions for <see cref="IServiceCollection"/>s
    /// </summary>
    public static class IServiceCollectionExtensions
    {

        /// <summary>
        /// Adds and configures the specified <see cref="ISerializer"/>
        /// </summary>
        /// <typeparam name="TSerializer">The type of <see cref="ISerializer"/> to register</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
        /// <param name="lifetime">The <see cref="ServiceLifetime"/> of the <see cref="ISerializer"/> to add</param>
        /// <returns>The configured <see cref="IServiceCollection"/></returns>
        public static IServiceCollection AddSerializer<TSerializer>(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where TSerializer : class, ISerializer
        {
            services.TryAdd(new ServiceDescriptor(typeof(ISerializerProvider), typeof(SerializerProvider)));
            services.TryAdd(new ServiceDescriptor(typeof(TSerializer), typeof(TSerializer), lifetime));
            if (typeof(IBinarySerializer).IsAssignableFrom(typeof(TSerializer)))
                services.TryAdd(new ServiceDescriptor(typeof(IBinarySerializer), provider => (IBinarySerializer)provider.GetRequiredService(typeof(TSerializer)), lifetime));
            if (typeof(ITextSerializer).IsAssignableFrom(typeof(TSerializer)))
                services.TryAdd(new ServiceDescriptor(typeof(ITextSerializer), provider => (ITextSerializer)provider.GetRequiredService(typeof(TSerializer)), lifetime));
            if (typeof(IJsonSerializer).IsAssignableFrom(typeof(TSerializer)))
                services.TryAdd(new ServiceDescriptor(typeof(IJsonSerializer), provider => (IJsonSerializer)provider.GetRequiredService(typeof(TSerializer)), lifetime));
            if (typeof(IXmlSerializer).IsAssignableFrom(typeof(TSerializer)))
                services.TryAdd(new ServiceDescriptor(typeof(IXmlSerializer), provider => (IXmlSerializer)provider.GetRequiredService(typeof(TSerializer)), lifetime));
            if (typeof(IYamlSerializer).IsAssignableFrom(typeof(IYamlSerializer)))
                services.TryAdd(new ServiceDescriptor(typeof(IYamlSerializer), provider => (IXmlSerializer)provider.GetRequiredService(typeof(TSerializer)), lifetime));
            return services;
        }

    }

}
