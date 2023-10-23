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

namespace Neuroglia.Data.Infrastructure.EventSourcing.Services;

/// <summary>
/// Represents the default implementation of the <see cref="IEventSourcingRepositoryBuilder{TAggregate, TKey}"/> interface
/// </summary>
/// <typeparam name="TAggregate">The type of <see cref="IAggregateRoot{TKey}"/> to add a new <see cref="EventSourcingRepository{TAggregate, TKey}"/> for</typeparam>
/// <typeparam name="TKey">The type of key used to uniquely identify the <see cref="IAggregateRoot"/>s managed by the <see cref="EventSourcingRepository{TAggregate, TKey}"/> to add</typeparam>
public class EventSourcingRepositoryBuilder<TAggregate, TKey>
    : IEventSourcingRepositoryBuilder<TAggregate, TKey>
    where TAggregate : class, IAggregateRoot<TKey>
    where TKey : IEquatable<TKey>
{

    /// <summary>
    /// Initializes a new <see cref="EventSourcingRepositoryBuilder{TAggregate, TKey}"/>
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
    /// <param name="serviceLifetime">The lifetime of the <see cref="IEventSourcingRepository{TAggregate, TKey}"/> to build</param>
    public EventSourcingRepositoryBuilder(IServiceCollection services, ServiceLifetime serviceLifetime)
    {
        Services = services;
        ServiceLifetime = serviceLifetime;
    }

    /// <summary>
    /// Gets the <see cref="IServiceCollection"/> to configure
    /// </summary>
    protected IServiceCollection Services { get; }

    /// <summary>
    /// Gets the lifetime of the <see cref="IEventSourcingRepository{TAggregate, TKey}"/> to build
    /// </summary>
    protected ServiceLifetime ServiceLifetime { get; }

    /// <summary>
    /// Gets the type of <see cref="IEventAggregatorFactory"/> to use
    /// </summary>
    protected Type AggregatorFactoryType { get; set; } = typeof(EventAggregatorFactory);

    /// <summary>
    /// Gets the type of <see cref="IEventMigrationManager"/> to use
    /// </summary>
    protected Type MigrationManagerType { get; set; } = typeof(EventMigrationManager);

    /// <summary>
    /// Gets the type of <see cref="IAggregateStateManager{TAggregate, TKey}"/> to use
    /// </summary>
    protected Type StateManagerType { get; set; } = typeof(EventSourcingStateManager<TAggregate, TKey>);

    /// <inheritdoc/>
    public IEventSourcingRepositoryBuilder<TAggregate, TKey> WithAggregatorFactory<TFactory>()
         where TFactory : class, IEventAggregatorFactory
    {
        AggregatorFactoryType = typeof(TFactory);
        return this;
    }

    /// <inheritdoc/>
    public IEventSourcingRepositoryBuilder<TAggregate, TKey> WithMigrationManager<TManager>()
         where TManager : class, IEventMigrationManager
    {
        MigrationManagerType = typeof(TManager);
        return this;
    }

    /// <inheritdoc/>
    public IEventSourcingRepositoryBuilder<TAggregate, TKey> WithStateManager<TManager>()
        where TManager : class, IAggregateStateManager<TAggregate, TKey>
    {
        StateManagerType = typeof(TManager);
        return this;
    }

    /// <inheritdoc/>
    public virtual void Build()
    {
        Services.TryAdd(new ServiceDescriptor(typeof(IEventAggregatorFactory), AggregatorFactoryType, ServiceLifetime));
        Services.TryAdd(new ServiceDescriptor(typeof(IEventMigrationManager), MigrationManagerType, ServiceLifetime));
        Services.TryAdd(new ServiceDescriptor(typeof(IAggregateStateManager<TAggregate, TKey>), StateManagerType, ServiceLifetime));
        Services.AddEventSourcingRepository<TAggregate, TKey, EventSourcingRepository<TAggregate, TKey>>(lifetime: ServiceLifetime);
    }

}