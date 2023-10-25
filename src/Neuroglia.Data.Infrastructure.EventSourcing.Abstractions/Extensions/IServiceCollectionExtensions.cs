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
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Neuroglia.Data.Infrastructure.EventSourcing.Services;
using Neuroglia.Data.Infrastructure.Services;
using System.Reflection;

namespace Neuroglia.Data.Infrastructure.EventSourcing;

/// <summary>
/// Defines extensions for <see cref="IServiceCollection"/>s
/// </summary>
public static class IServiceCollectionExtensions
{

    static readonly MethodInfo CreateOptionsMethod = typeof(Options).GetMethod(nameof(Options.Create))!;
    static readonly MethodInfo ConfigureMethod = typeof(OptionsServiceCollectionExtensions).GetMethods().First(m => m.Name == nameof(OptionsServiceCollectionExtensions.Configure) && m.GetParameters().Length == 2);

    /// <summary>
    /// Adds and configures a new <see cref="IEventStore"/> of the specified type
    /// </summary>
    /// <typeparam name="TEventStore">The type of <see cref="IEventStore"/> to use</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
    /// <param name="setupAction">An <see cref="Action{T}"/> used to configure the <see cref="IEventStore"/></param>
    /// <returns>The configured <see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddEventStore<TEventStore>(this IServiceCollection services, Action<IEventStoreOptionsBuilder>? setupAction = null)
        where TEventStore : class, IEventStore
    {
        var optionsBuilder = new EventStoreOptionsBuilder();
        setupAction?.Invoke(optionsBuilder);
        var options = optionsBuilder.Build();
        services.TryAddSingleton(Options.Create(options));
        services.TryAddSingleton<TEventStore>();
        services.AddSingleton(typeof(IEventStore), provider => provider.GetRequiredService<TEventStore>());
        return services;
    }

    /// <summary>
    /// Adds and configures a new <see cref="IEventSourcingRepository{TAggregate, TKey}"/>
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
    /// <param name="aggregateType">The type of <see cref="IAggregateRoot{TKey}"/> to add a new <see cref="IEventSourcingRepository{TAggregate, TKey}"/> for</param>
    /// <param name="keyType">The type of key used to uniquely identify the <see cref="IAggregateRoot"/>s managed by the <see cref="IEventSourcingRepository{TAggregate, TKey}"/> to add</param>
    /// <param name="implementationType">The type of the <see cref="IEventSourcingRepository{TAggregate, TKey}"/> implementation to register</param>
    /// <param name="lifetime">The lifetime of the <see cref="IEventSourcingRepository{TAggregate, TKey}"/> to add. Defaults to <see cref="ServiceLifetime.Scoped"/></param>
    /// <returns>The configured <see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddEventSourcingRepository(this IServiceCollection services, Type aggregateType, Type keyType, Type implementationType, ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        if (aggregateType == null) throw new ArgumentNullException(nameof(aggregateType));
        if (!typeof(IAggregateRoot<>).MakeGenericType(keyType).IsAssignableFrom(aggregateType)) throw new ArgumentException("The specified type must be an IAggregateRoot<TKey> implementation", nameof(aggregateType));
        if (keyType == null) throw new ArgumentNullException(nameof(keyType));

        services.TryAdd(new ServiceDescriptor(implementationType, implementationType, lifetime));

        services.TryAdd(new ServiceDescriptor(typeof(IEventSourcingRepository<,>).MakeGenericType(aggregateType, keyType), provider => provider.GetRequiredService(implementationType), lifetime));
        services.TryAdd(new ServiceDescriptor(typeof(IEventSourcingRepository<>).MakeGenericType(aggregateType), provider => provider.GetRequiredService(implementationType), lifetime));
        services.TryAdd(new ServiceDescriptor(typeof(IEventSourcingRepository), provider => provider.GetRequiredService(implementationType), lifetime));

        services.TryAdd(new ServiceDescriptor(typeof(IRepository<,>).MakeGenericType(aggregateType, keyType), provider => provider.GetRequiredService(implementationType), lifetime));
        services.TryAdd(new ServiceDescriptor(typeof(IRepository<>).MakeGenericType(aggregateType), provider => provider.GetRequiredService(implementationType), lifetime));
        services.TryAdd(new ServiceDescriptor(typeof(IRepository), provider => provider.GetRequiredService(implementationType), lifetime));

        return services;
    }

    /// <summary>
    /// Adds and configures a new <see cref="IEventSourcingRepository{TAggregate, TKey}"/>
    /// </summary>
    /// <typeparam name="TAggregate">The type of <see cref="IAggregateRoot{TKey}"/> to add a new <see cref="IEventSourcingRepository{TAggregate, TKey}"/> for</typeparam>
    /// <typeparam name="TKey">The type of key used to uniquely identify the <see cref="IAggregateRoot"/>s managed by the <see cref="IEventSourcingRepository{TAggregate, TKey}"/> to add</typeparam>
    /// <typeparam name="TRepository">The type of the <see cref="IEventSourcingRepository{TAggregate, TKey}"/> implementation to register</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
    /// <param name="lifetime">The lifetime of the <see cref="IEventSourcingRepository{TAggregate, TKey}"/> to add. Defaults to <see cref="ServiceLifetime.Scoped"/></param>
    /// <returns>The configured <see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddEventSourcingRepository<TAggregate, TKey, TRepository>(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where TAggregate : class, IAggregateRoot<TKey>
        where TKey : IEquatable<TKey>
        where TRepository : IEventSourcingRepository<TAggregate, TKey>
    {
        return services.AddEventSourcingRepository(typeof(TAggregate), typeof(TKey), typeof(TRepository), lifetime);
    }

}
