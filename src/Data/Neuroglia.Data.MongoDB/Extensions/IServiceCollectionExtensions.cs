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
using Microsoft.Extensions.Options;
using Neuroglia.Data.MongoDB;
using System;

namespace Neuroglia.Data
{

    /// <summary>
    /// Defines extensions for <see cref="IServiceCollection"/>s
    /// </summary>
    public static class IServiceCollectionExtensions
    {

        /// <summary>
        /// Adds and configures an <see cref="IMongoDbContext"/> of the specified type
        /// </summary>
        /// <typeparam name="TDbContext">The type of <see cref="IMongoDbContext"/> to add</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
        /// <param name="configurationAction">An <see cref="Action{T}"/> used to configure the <see cref="IMongoDbContext"/></param>
        /// <returns>The configured <see cref="IServiceCollection"/></returns>
        public static IServiceCollection AddMongoDbContext<TDbContext>(this IServiceCollection services, Action<IMongoDbContextOptionsBuilder> configurationAction)
            where TDbContext : class, IMongoDbContext
        {
            IMongoDbContextOptionsBuilder<TDbContext> optionsBuilder = new MongoDbContextOptionsBuilder<TDbContext>();
            configurationAction(optionsBuilder);
            MongoDbContextOptions<TDbContext> options = optionsBuilder.Build();
            services.AddPluralizer();
            services.AddSingleton(Options.Create(options));
            services.AddSingleton<TDbContext>();
            services.AddSingleton<IMongoDbContext>(provider => provider.GetRequiredService<TDbContext>());
            return services;
        }

        /// <summary>
        /// Adds and configures a MongoDb implementation of the <see cref="IRepository{TEntity, TKey}"/> interface
        /// </summary>
        /// <typeparam name="TEntity">The type of entity managed by the repository to add</typeparam>
        /// <typeparam name="TKey">The type of key used to uniquely identify entities managed by the repository to add</typeparam>
        /// <typeparam name="TDbContext">The type of <see cref="IMongoDbContext"/> the repository to add belongs to</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
        /// <param name="lifetime">The <see cref="ServiceLifetime"/> of the <see cref="MongoRepository{TEntity, TKey, TContext}"/> to add. Defaults to <see cref="ServiceLifetime.Scoped"/></param>
        /// <returns>The configured <see cref="IServiceCollection"/></returns>
        public static IServiceCollection AddMongoRepository<TEntity, TKey, TDbContext>(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Scoped)
            where TEntity : class, IIdentifiable<TKey>
            where TKey : IEquatable<TKey>
            where TDbContext : class, IMongoDbContext
        {
            return services.AddMongoRepository(typeof(TEntity), typeof(TKey), typeof(TDbContext), lifetime);
        }

        /// <summary>
        /// Adds and configures a new <see cref="MongoRepository{TEntity, TKey, TContext}"/>
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
        /// <param name="lifetime">The <see cref="ServiceLifetime"/> of the <see cref="MongoRepository{TEntity, TKey, TContext}"/> to add. Defaults to <see cref="ServiceLifetime.Scoped"/></param>
        /// <param name="entityType">The type of entity to store</param>
        /// <param name="keyType">The type of key used to uniquely identify entities to store</param>
        /// <param name="contextType">The type of <see cref="IMongoDbContext"/> the <see cref="MongoRepository{TEntity, TKey, TContext}"/> to add belongs to</param>
        /// <returns>The configured <see cref="IServiceCollection"/></returns>
        private static IServiceCollection AddMongoRepository(this IServiceCollection services, Type entityType, Type keyType, Type contextType, ServiceLifetime lifetime)
        {
            if (entityType == null)
                throw new ArgumentNullException(nameof(entityType));
            if (keyType == null)
                throw new ArgumentNullException(nameof(keyType));
            if (contextType == null)
                throw new ArgumentNullException(nameof(contextType));
            Type implementationType = typeof(MongoRepository<,,>).MakeGenericType(entityType, keyType, contextType);
            services.TryAdd(new ServiceDescriptor(implementationType, implementationType, lifetime));
            services.TryAdd(new ServiceDescriptor(typeof(IRepository<,>).MakeGenericType(entityType, keyType), provider => provider.GetRequiredService(implementationType), lifetime));
            services.TryAdd(new ServiceDescriptor(typeof(IRepository<>).MakeGenericType(entityType), provider => provider.GetRequiredService(implementationType), lifetime));
            return services;
        }

    }

}
