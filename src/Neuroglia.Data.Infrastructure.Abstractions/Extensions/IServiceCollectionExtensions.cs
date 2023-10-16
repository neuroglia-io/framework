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

using Microsoft.Extensions.DependencyInjection;
using Neuroglia.Data.Infrastructure.Services;

namespace Neuroglia.Data.Infrastructure;

/// <summary>
/// Defines extensions for <see cref="IServiceCollection"/>s
/// </summary>
public static class IServiceCollectionExtensions
{

    /// <summary>
    /// Registers and configures a new <see cref="IRepository{TEntity, TKey}"/> implementation
    /// </summary>
    /// <typeparam name="TEntity">The type of the managed entities</typeparam>
    /// <typeparam name="TKey">The type of key used to identify and distinct managed entities</typeparam>
    /// <typeparam name="TRepository">The type of the <see cref="IRepository{TEntity, TKey}"/> implementation to use</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
    /// <param name="lifetime">The <see cref="IRepository{TEntity, TKey}"/> service lifetime</param>
    /// <returns>The configured <see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddRepository<TEntity, TKey, TRepository>(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Singleton)
        where TEntity : class, IIdentifiable<TKey>
        where TKey : IEquatable<TKey>
        where TRepository : class, IRepository<TEntity, TKey>
    {
        services.Add(new ServiceDescriptor(typeof(TRepository), typeof(TRepository), lifetime));
        services.Add(new ServiceDescriptor(typeof(IRepository), provider => provider.GetRequiredService<TRepository>(), lifetime));
        services.Add(new ServiceDescriptor(typeof(IRepository<TEntity>), provider => provider.GetRequiredService<TRepository>(), lifetime));
        services.Add(new ServiceDescriptor(typeof(IRepository<TEntity, TKey>), provider => provider.GetRequiredService<TRepository>(), lifetime));

        if (typeof(IQueryableRepository).IsAssignableFrom(typeof(TRepository)))
        {
            services.Add(new ServiceDescriptor(typeof(IQueryableRepository), provider => provider.GetRequiredService<TRepository>(), lifetime));
            services.Add(new ServiceDescriptor(typeof(IQueryableRepository<TEntity>), provider => provider.GetRequiredService<TRepository>(), lifetime));
            services.Add(new ServiceDescriptor(typeof(IQueryableRepository<TEntity, TKey>), provider => provider.GetRequiredService<TRepository>(), lifetime));
        }

        return services;
    }

}
