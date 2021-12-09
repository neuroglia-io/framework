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
        /// Adds and configures a new <see cref="IMongoClient"/>
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
        /// <param name="clientSettings">The <see cref="MongoClientSettings"/> to use</param>
        /// <param name="lifetime">The lifetime of the <see cref="IMongoClient"/> to add and configure. Defaults to <see cref="ServiceLifetime.Scoped"/></param>
        /// <returns>The configured <see cref="IServiceCollection"/></returns>
        public static IServiceCollection AddMongoClient(this IServiceCollection services, MongoClientSettings clientSettings, ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            services.AddPluralizer();
            services.TryAdd(new ServiceDescriptor(typeof(IMongoClient), provider => new MongoClient(clientSettings), lifetime));
            return services;
        }

        /// <summary>
        /// Adds and configures a new <see cref="IMongoClient"/>
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
        /// <param name="configurationAction">An <see cref="Action{T}"/> used to configure the <see cref="MongoClientSettings"/> to use</param>
        /// <param name="lifetime">The lifetime of the <see cref="IMongoClient"/> to add and configure. Defaults to <see cref="ServiceLifetime.Scoped"/></param>
        /// <returns>The configured <see cref="IServiceCollection"/></returns>
        public static IServiceCollection AddMongoClient(this IServiceCollection services, Action<MongoClientSettings> configurationAction, ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            var clientSettings = new MongoClientSettings();
            configurationAction(clientSettings);
            return services.AddMongoClient(clientSettings, lifetime);
        }

        /// <summary>
        /// Adds and configures a new <see cref="IMongoDatabase"/>
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
        /// <param name="databaseName">The name of the <see cref="IMongoDatabase"/> to add and configure.</param>
        /// <param name="lifetime">The lifetime of the <see cref="IMongoDatabase"/> to add and configure. Defaults to <see cref="ServiceLifetime.Scoped"/></param>
        /// <returns>The configured <see cref="IServiceCollection"/></returns>
        public static IServiceCollection AddMongoDatabase(this IServiceCollection services, string databaseName, ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            if(string.IsNullOrWhiteSpace(databaseName))
                throw new ArgumentNullException(nameof(databaseName));
            services.Add(new ServiceDescriptor(typeof(IMongoDatabase), provider => provider.GetRequiredService<IMongoClient>().GetDatabase(databaseName), lifetime));
            return services;
        }

        /// <summary>
        /// Adds and configures a MongoDb implementation of the <see cref="IRepository{TEntity, TKey}"/> interface
        /// </summary>
        /// <typeparam name="TEntity">The type of entity managed by the repository to add</typeparam>
        /// <typeparam name="TKey">The type of key used to uniquely identify entities managed by the repository to add</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
        /// <param name="configurationAction">An <see cref="Action{T}"/> used to configure the <see cref="MongoRepository{TEntity, TKey}"/></param>
        /// <param name="lifetime">The <see cref="ServiceLifetime"/> of the <see cref="MongoRepository{TEntity, TKey}"/> to add. Defaults to <see cref="ServiceLifetime.Scoped"/></param>
        /// <returns>The configured <see cref="IServiceCollection"/></returns>
        public static IServiceCollection AddMongoRepository<TEntity, TKey>(this IServiceCollection services, Action<MongoRepositoryOptions> configurationAction = null,  ServiceLifetime lifetime = ServiceLifetime.Scoped)
            where TEntity : class, IIdentifiable<TKey>
            where TKey : IEquatable<TKey>
        {
            return services.AddMongoRepository(typeof(TEntity), typeof(TKey), configurationAction, lifetime);
        }

        /// <summary>
        /// Adds and configures a new <see cref="MongoRepository{TEntity, TKey}"/>
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
        /// <param name="lifetime">The <see cref="ServiceLifetime"/> of the <see cref="MongoRepository{TEntity, TKey}"/> to add. Defaults to <see cref="ServiceLifetime.Scoped"/></param>
        /// <param name="entityType">The type of entity to store</param>
        /// <param name="keyType">The type of key used to uniquely identify entities to store</param>
        /// <param name="configurationAction">An <see cref="Action{T}"/> used to configure the <see cref="MongoRepository{TEntity, TKey}"/></param>
        /// <returns>The configured <see cref="IServiceCollection"/></returns>
        private static IServiceCollection AddMongoRepository(this IServiceCollection services, Type entityType, Type keyType, Action<MongoRepositoryOptions> configurationAction = null, ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            if (entityType == null)
                throw new ArgumentNullException(nameof(entityType));
            if (!typeof(IIdentifiable).IsAssignableFrom(entityType))
                throw new ArgumentException($"Type '{entityType.Name}' is not an implementation of the '{nameof(IIdentifiable)}' interface", nameof(entityType));
            var identifiableType = entityType.GetGenericType(typeof(IIdentifiable<>));
            var expectedKeyType = identifiableType.GetGenericArguments()[0];
            if (keyType == null)
                throw new ArgumentNullException(nameof(keyType));
            if (!expectedKeyType.IsAssignableFrom(keyType))
                throw new ArgumentException($"Type '{entityType.Name}' expects a key of type '{expectedKeyType.Name}'", nameof(keyType));
            Type implementationType = typeof(MongoRepository<,>).MakeGenericType(entityType, keyType);
            if (configurationAction != null)
                services.Configure(configurationAction);
            services.TryAdd(new ServiceDescriptor(implementationType, implementationType, lifetime));
            services.TryAdd(new ServiceDescriptor(typeof(IRepository<,>).MakeGenericType(entityType, keyType), provider => provider.GetRequiredService(implementationType), lifetime));
            services.TryAdd(new ServiceDescriptor(typeof(IRepository<>).MakeGenericType(entityType), provider => provider.GetRequiredService(implementationType), lifetime));
            return services;
        }

    }

}
