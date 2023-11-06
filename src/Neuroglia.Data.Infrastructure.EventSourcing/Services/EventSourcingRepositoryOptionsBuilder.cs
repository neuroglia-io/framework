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

using Neuroglia.Data.Infrastructure.EventSourcing.Configuration;

namespace Neuroglia.Data.Infrastructure.EventSourcing.Services;

/// <summary>
/// Represents the default implementation of the <see cref="IEventSourcingRepositoryOptionsBuilder{TAggregate, TKey}"/> interface
/// </summary>
/// <typeparam name="TAggregate">The type of <see cref="IAggregateRoot{TKey}"/> to add a new <see cref="EventSourcingRepository{TAggregate, TKey}"/> for</typeparam>
/// <typeparam name="TKey">The type of key used to uniquely identify the <see cref="IAggregateRoot"/>s managed by the <see cref="EventSourcingRepository{TAggregate, TKey}"/> to add</typeparam>
public class EventSourcingRepositoryOptionsBuilder<TAggregate, TKey>
    : IEventSourcingRepositoryOptionsBuilder<TAggregate, TKey>
    where TAggregate : class, IAggregateRoot<TKey>
    where TKey : IEquatable<TKey>
{

    /// <summary>
    /// Gets the <see cref="EventSourcingRepositoryOptions{TAggregate, TKey}"/> to configure
    /// </summary>
    protected EventSourcingRepositoryOptions<TAggregate, TKey> Options { get; } = new();

    /// <inheritdoc/>
    public virtual IEventSourcingRepositoryOptionsBuilder<TAggregate, TKey> UseAggregatorFactory<TFactory>()
         where TFactory : class, IEventAggregatorFactory
    {
        this.Options.AggregatorFactoryType = typeof(TFactory);
        return this;
    }

    /// <inheritdoc/>
    public virtual IEventSourcingRepositoryOptionsBuilder<TAggregate, TKey> UseMigrationManager<TManager>()
         where TManager : class, IEventMigrationManager
    {
        this.Options.MigrationManagerType = typeof(TManager);
        return this;
    }

    /// <inheritdoc/>
    public virtual IEventSourcingRepositoryOptionsBuilder<TAggregate, TKey> UseStateManager<TManager>()
        where TManager : class, IAggregateStateManager<TAggregate, TKey>
    {
        this.Options.StateManagerType = typeof(TManager);
        return this;
    }

    /// <inheritdoc/>
    public virtual IEventSourcingRepositoryOptionsBuilder<TAggregate, TKey> PublishEvents(bool publish = true)
    {
        this.Options.PublishEvents = publish;
        return this;
    }

    /// <inheritdoc/>
    public virtual EventSourcingRepositoryOptions<TAggregate, TKey> Build() => this.Options;

}