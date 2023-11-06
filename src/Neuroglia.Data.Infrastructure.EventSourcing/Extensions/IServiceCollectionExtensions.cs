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

namespace Neuroglia.Data.Infrastructure.EventSourcing;

/// <summary>
/// Defines extensions for <see cref="IServiceCollection"/>s
/// </summary>
public static class IServiceCollectionExtensions
{

    /// <summary>
    /// Adds and configures a new <see cref="EventSourcingRepository{TAggregate, TKey}"/>
    /// </summary>
    /// <typeparam name="TAggregate">The type of <see cref="IAggregateRoot{TKey}"/> to add a new <see cref="EventSourcingRepository{TAggregate, TKey}"/> for</typeparam>
    /// <typeparam name="TKey">The type of key used to uniquely identify the <see cref="IAggregateRoot"/>s managed by the <see cref="EventSourcingRepository{TAggregate, TKey}"/> to add</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
    /// <param name="setup">An <see cref="Action{T}"/> used to configure the <see cref="EventSourcingRepository{TAggregate, TKey}"/> to use</param>
    /// <param name="lifetime">The lifetime of the <see cref="EventSourcingRepository{TAggregate, TKey}"/> to add. Defaults to <see cref="ServiceLifetime.Scoped"/></param>
    /// <returns>The configured <see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddEventSourcingRepository<TAggregate, TKey>(this IServiceCollection services, Action<IEventSourcingRepositoryOptionsBuilder<TAggregate, TKey>>? setup = null, ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where TAggregate : class, IAggregateRoot<TKey>
        where TKey : IEquatable<TKey>
    {
        var builder = new EventSourcingRepositoryOptionsBuilder<TAggregate, TKey>();
        setup?.Invoke(builder);
        var options = builder.Build();
        services.AddSingleton(Options.Create(options));
        services.TryAdd(new ServiceDescriptor(typeof(IEventAggregatorFactory), options.AggregatorFactoryType, lifetime));
        services.TryAdd(new ServiceDescriptor(typeof(IEventMigrationManager), options.MigrationManagerType, lifetime));
        services.TryAdd(new ServiceDescriptor(typeof(IAggregateStateManager<TAggregate, TKey>), options.StateManagerType, lifetime));
        services.AddEventSourcingRepository<TAggregate, TKey, EventSourcingRepository<TAggregate, TKey>>(lifetime: lifetime);
        return services;
    }

}
