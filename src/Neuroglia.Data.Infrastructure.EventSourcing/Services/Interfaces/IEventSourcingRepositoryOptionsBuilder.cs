﻿// Copyright © 2021-Present Neuroglia SRL. All rights reserved.
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

using Neuroglia.Data.Infrastructure.EventSourcing.Configuration;

namespace Neuroglia.Data.Infrastructure.EventSourcing.Services;

/// <summary>
/// Defines the fundamentals of a service used to configure an <see cref="EventSourcingRepository{TAggregate, TKey}"/>
/// </summary>
/// <typeparam name="TAggregate">The type of <see cref="IAggregateRoot{TKey}"/> to add a new <see cref="EventSourcingRepository{TAggregate, TKey}"/> for</typeparam>
/// <typeparam name="TKey">The type of key used to uniquely identify the <see cref="IAggregateRoot"/>s managed by the <see cref="EventSourcingRepository{TAggregate, TKey}"/> to add</typeparam>
public interface IEventSourcingRepositoryOptionsBuilder<TAggregate, TKey>
    where TAggregate : class, IAggregateRoot<TKey>
    where TKey : IEquatable<TKey>
{

    /// <summary>
    /// Configures the application to use the specified <see cref="IEventAggregatorFactory"/>
    /// </summary>
    /// <typeparam name="TFactory">The type of <see cref="IEventAggregatorFactory"/> to use</typeparam>
    /// <returns>The configured <see cref="IEventSourcingRepositoryOptionsBuilder{TAggregate, TKey}"/></returns>
    IEventSourcingRepositoryOptionsBuilder<TAggregate, TKey> UseAggregatorFactory<TFactory>()
        where TFactory : class, IEventAggregatorFactory;

    /// <summary>
    /// Configures the application to use the specified <see cref="IEventMigrationManager"/>
    /// </summary>
    /// <typeparam name="TManager">The type of <see cref="IEventMigrationManager"/> to use</typeparam>
    /// <returns>The configured <see cref="IEventSourcingRepositoryOptionsBuilder{TAggregate, TKey}"/></returns>
    IEventSourcingRepositoryOptionsBuilder<TAggregate, TKey> UseMigrationManager<TManager>()
        where TManager : class, IEventMigrationManager;

    /// <summary>
    /// Configures the <see cref="IEventSourcingRepository{TAggregate, TKey}"/> to use the specified <see cref="IAggregateStateManager{TAggregate, TKey}"/>
    /// </summary>
    /// <typeparam name="TManager">The type of <see cref="IAggregateStateManager{TAggregate, TKey}"/> to use</typeparam>
    /// <returns>The configured <see cref="IEventSourcingRepositoryOptionsBuilder{TAggregate, TKey}"/></returns>
    IEventSourcingRepositoryOptionsBuilder<TAggregate, TKey> UseStateManager<TManager>()
       where TManager : class, IAggregateStateManager<TAggregate, TKey>;

    /// <summary>
    /// Configures the <see cref="IEventSourcingRepository{TAggregate, TKey}"/> to publish recorded events
    /// </summary>
    /// <param name="publish">A boolean indicating whether or not the <see cref="IEventSourcingRepository{TAggregate, TKey}"/> publishes recorded events</param>
    /// <returns>The configured <see cref="IEventSourcingRepositoryOptionsBuilder{TAggregate, TKey}"/></returns>
    IEventSourcingRepositoryOptionsBuilder<TAggregate, TKey> PublishEvents(bool publish = true);

    /// <summary>
    /// Builds and configures the <see cref="IEventSourcingRepository{TAggregate, TKey}"/>
    /// </summary>
    /// <returns>The configured <see cref="EventSourcingRepositoryOptions{TAggregate, TKey}"/></returns>
    EventSourcingRepositoryOptions<TAggregate, TKey> Build();

}
