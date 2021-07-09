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
using System;

namespace Neuroglia.Data
{

    /// <summary>
    /// Defines extensions for <see cref="IServiceCollection"/>s
    /// </summary>
    public static class IServiceCollectionExtensions
    {

        /// <summary>
        /// Adds and configures a new <see cref="DistributedCacheRepository{TEntity, TKey}"/>
        /// </summary>
        /// <typeparam name="TEntity">The type of entity to add a <see cref="DistributedCacheRepository{TEntity, TKey}"/> for</typeparam>
        /// <typeparam name="TKey">The type of key used to uniquely identify entities to manage</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
        /// <param name="lifetime">The <see cref="ServiceLifetime"/> of the <see cref="DistributedCacheRepository{TEntity, TKey}"/> to add. Defaults to <see cref="ServiceLifetime.Scoped"/></param>
        /// <returns>The configured <see cref="IServiceCollection"/></returns>
        public static IServiceCollection AddDistributedCacheRepository<TEntity, TKey>(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Scoped)
            where TEntity : class, IIdentifiable
            where TKey : IEquatable<TKey>
        {
            Type implementationType = typeof(DistributedCacheRepository<,>).MakeGenericType(typeof(TEntity), typeof(TKey));
            services.TryAdd(new ServiceDescriptor(implementationType, implementationType, lifetime));
            services.TryAdd(new ServiceDescriptor(typeof(IRepository<,>).MakeGenericType(typeof(TEntity), typeof(TKey)), provider => provider.GetRequiredService(implementationType), lifetime));
            services.TryAdd(new ServiceDescriptor(typeof(IRepository<>).MakeGenericType(typeof(TEntity)), provider => provider.GetRequiredService(implementationType), lifetime));
            return services;
        }

    }

}
