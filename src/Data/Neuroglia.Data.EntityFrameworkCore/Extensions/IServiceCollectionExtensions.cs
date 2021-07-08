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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Linq;

namespace Neuroglia.Data
{

    /// <summary>
    /// Defines extensions for <see cref="IServiceCollection"/>s
    /// </summary>
    public static class IServiceCollectionExtensions
    {

        /// <summary>
        /// Adds and configures an <see cref="EFCoreRepository{TEntity, TKey, TDbContext}"/> for each <see cref="DbSet{TEntity}"/> property in the specified <see cref="DbContext"/> type
        /// </summary>
        /// <typeparam name="TContext">The <see cref="DbContext"/> type to register repositories for</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
        /// <param name="lifetime">The <see cref="ServiceLifetime"/> of repositories to add</param>
        /// <returns>The configured <see cref="IServiceCollection"/></returns>
        public static IServiceCollection AddEFCoreRepositories<TContext>(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Scoped)
            where TContext : DbContext
        {
            Type contextType = typeof(TContext);
            foreach (Type dbSetType in contextType.GetProperties()
                .Select(p => p.PropertyType.GetGenericType(typeof(DbSet<>)))
                .Where(t => t != null)
                .Distinct())
            {
                Type entityType = dbSetType.GetGenericArguments().First();
                if (!typeof(IIdentifiable).IsAssignableFrom(entityType))
                    continue;
                Type keyType = entityType.GetGenericType(typeof(IIdentifiable<>)).GetGenericArguments().First();
                Type implementationType = typeof(EFCoreRepository<,,>).MakeGenericType(entityType, keyType, contextType);
                services.AddEFCoreRepository(entityType, keyType, contextType, lifetime);
            }
            return services;
        }

        /// <summary>
        /// Adds and configures a new <see cref="EFCoreRepository{TEntity, TKey, TDbContext}"/>
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
        /// <param name="lifetime">The <see cref="ServiceLifetime"/> of the <see cref="EFCoreRepository{TEntity, TKey, TDbContext}"/> to add. Defaults to <see cref="ServiceLifetime.Scoped"/></param>
        /// <typeparam name="TEntity">The type of entity to store</typeparam>
        /// <typeparam name="TKey">The type of key used to uniquely identify entities to store</typeparam>
        /// <typeparam name="TDbContext">The type of <see cref="DbContext"/> the <see cref="EFCoreRepository{TEntity, TKey, TDbContext}"/> to add belongs to</typeparam>
        /// <returns>The configured <see cref="IServiceCollection"/></returns>
        public static IServiceCollection AddEFCoreRepository<TEntity, TKey, TDbContext>(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Scoped)
            where TEntity : class, IIdentifiable<TKey>
            where TKey : IEquatable<TKey>
            where TDbContext : DbContext
        {
            return services.AddEFCoreRepository(typeof(TEntity), typeof(TKey), typeof(TDbContext), lifetime);
        }

        /// <summary>
        /// Adds and configures a new <see cref="EFCoreRepository{TEntity, TKey, TDbContext}"/>
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
        /// <param name="lifetime">The <see cref="ServiceLifetime"/> of the <see cref="EFCoreRepository{TEntity, TKey, TDbContext}"/> to add. Defaults to <see cref="ServiceLifetime.Scoped"/></param>
        /// <param name="entityType">The type of entity to store</param>
        /// <param name="keyType">The type of key used to uniquely identify entities to store</param>
        /// <param name="contextType">The type of <see cref="DbContext"/> the <see cref="EFCoreRepository{TEntity, TKey, TDbContext}"/> to add belongs to</param>
        /// <returns>The configured <see cref="IServiceCollection"/></returns>
        private static IServiceCollection AddEFCoreRepository(this IServiceCollection services, Type entityType, Type keyType, Type contextType, ServiceLifetime lifetime)
        {
            if (entityType == null)
                throw new ArgumentNullException(nameof(entityType));
            if (keyType == null)
                throw new ArgumentNullException(nameof(keyType));
            if (contextType == null)
                throw new ArgumentNullException(nameof(contextType));
            Type implementationType = typeof(EFCoreRepository<,,>).MakeGenericType(entityType, keyType, contextType);
            services.TryAdd(new ServiceDescriptor(implementationType, implementationType, lifetime));
            services.TryAdd(new ServiceDescriptor(typeof(IRepository<,>).MakeGenericType(entityType, keyType), provider => provider.GetRequiredService(implementationType), lifetime));
            services.TryAdd(new ServiceDescriptor(typeof(IRepository<>).MakeGenericType(entityType), provider => provider.GetRequiredService(implementationType), lifetime));
            return services;
        }

    }

}
