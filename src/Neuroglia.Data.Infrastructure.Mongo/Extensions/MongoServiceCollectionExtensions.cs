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

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MongoDB.Driver;
using Neuroglia.Data.Infrastructure.Mongo.Configuration;
using Neuroglia.Data.Infrastructure.Mongo.Services;

namespace Neuroglia.Data.Infrastructure;

/// <summary>
/// Defines extensions for <see cref="IServiceCollection"/>s
/// </summary>
public static class MongoServiceCollectionExtensions
{

    /// <summary>
    /// Adds and configures a new <see cref="IMongoClient"/>
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
    /// <param name="clientSettings">The <see cref="MongoClientSettings"/> to use</param>
    /// <param name="lifetime">The lifetime of the <see cref="IMongoClient"/> to add and configure. Defaults to <see cref="ServiceLifetime.Scoped"/></param>
    /// <returns>The configured <see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddMongo(this IServiceCollection services, MongoClientSettings? clientSettings = null, ServiceLifetime lifetime = ServiceLifetime.Singleton)
    {
        services.TryAdd(new ServiceDescriptor(typeof(IMongoClient), provider => 
        {
            clientSettings ??= MongoClientSettings.FromConnectionString(provider.GetRequiredService<IConfiguration>().GetConnectionString("mongo"));
            return new MongoClient(clientSettings);
        }, lifetime));
        return services;
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
        if (string.IsNullOrWhiteSpace(databaseName)) throw new ArgumentNullException(nameof(databaseName));
        services.Add(new ServiceDescriptor(typeof(IMongoDatabase), provider => provider.GetRequiredService<IMongoClient>().GetDatabase(databaseName), lifetime));
        return services;
    }

    /// <summary>
    /// Registers and configures a new <see cref="MongoRepository{TEntity, TKey}"/>
    /// </summary>
    /// <typeparam name="TEntity">The type of the managed entities</typeparam>
    /// <typeparam name="TKey">The type of key used to identify and distinct managed entities</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
    /// <param name="lifetime">The lifetime of the repository</param>
    /// <param name="setupAction">An <see cref="Action{T}"/> used to setup the <see cref="MongoRepositoryOptions"/></param>
    /// <returns>The configured <see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddMongoRepository<TEntity, TKey>(this IServiceCollection services, Action<MongoRepositoryOptions>? setupAction = null, ServiceLifetime lifetime = ServiceLifetime.Singleton)
        where TEntity : class, IIdentifiable<TKey>
        where TKey : IEquatable<TKey>
    {
        setupAction ??= _ => { };
        services.Configure(setupAction);
        services.AddMongo(lifetime: lifetime);
        services.AddRepository<TEntity, TKey, MongoRepository<TEntity, TKey>>(lifetime);
        return services;
    }

}
