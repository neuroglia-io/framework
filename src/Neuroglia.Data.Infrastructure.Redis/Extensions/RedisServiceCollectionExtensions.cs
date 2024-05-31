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
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Neuroglia.Data.Infrastructure;

/// <summary>
/// Defines extensions for <see cref="IServiceCollection"/>s
/// </summary>
public static class RedisServiceCollectionExtensions
{

    /// <summary>
    /// Adds and configures a new <see cref="IRedis"/>
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
    /// <param name="connectionString">The Redis connection string</param>
    /// <returns>The configured <see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddRedis(this IServiceCollection services, string connectionString)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);
        services.TryAddSingleton<IConnectionMultiplexer>(provider => ConnectionMultiplexer.Connect(connectionString));
        return services;
    }

    /// <summary>
    /// Adds and configures a new <see cref="IRedis"/>
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
    /// <param name="configuration">The current <see cref="IConfiguration"/></param>
    /// <returns>The configured <see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddRedis(this IServiceCollection services, IConfiguration configuration) => services.AddRedis(configuration.GetConnectionString("redis")!);

    /// <summary>
    /// Registers and configures a new <see cref="RedisRepository{TEntity, TKey}"/>
    /// </summary>
    /// <typeparam name="TEntity">The type of the managed entities</typeparam>
    /// <typeparam name="TKey">The type of key used to identify and distinct managed entities</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
    /// <param name="lifetime">The lifetime of the repository</param>
    /// <returns>The configured <see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddRedisRepository<TEntity, TKey>(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Singleton)
        where TEntity : class, IIdentifiable<TKey>
        where TKey : IEquatable<TKey>
    {
        services.AddRepository<TEntity, TKey, RedisRepository<TEntity, TKey>>(lifetime);
        return services;
    }

}
