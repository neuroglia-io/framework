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

using Neuroglia.Data.Infrastructure.EventSourcing.Services;

namespace Neuroglia.Data.Infrastructure.EventSourcing.Configuration;

/// <summary>
/// Represents the options used to configure an <see cref="EventSourcingRepository{TAggregate, TKey}"/>
/// </summary>
/// <typeparam name="TAggregate">The type of the managed <see cref="IAggregateRoot"/>s</typeparam>
/// <typeparam name="TKey">The key used to identify managed <see cref="IAggregateRoot"/>s</typeparam>
public class EventSourcingRepositoryOptions<TAggregate, TKey>
    where TAggregate : class, IAggregateRoot<TKey>
    where TKey : IEquatable<TKey>
{

    /// <summary>
    /// Gets the type of <see cref="IEventAggregatorFactory"/> to use
    /// </summary>
    public Type AggregatorFactoryType { get; set; } = typeof(EventAggregatorFactory);

    /// <summary>
    /// Gets the type of <see cref="IEventMigrationManager"/> to use
    /// </summary>
    public Type MigrationManagerType { get; set; } = typeof(EventMigrationManager);

    /// <summary>
    /// Gets the type of <see cref="IAggregateStateManager{TAggregate, TKey}"/> to use
    /// </summary>
    public Type StateManagerType { get; set; } = typeof(EventSourcingStateManager<TAggregate, TKey>);

    /// <summary>
    /// Gets/sets a boolean indicating whether or not the configured <see cref="EventSourcingRepository{TAggregate, TKey}"/> should publish events. Defaults to true.
    /// </summary>
    public virtual bool PublishEvents { get; set; } = true;

}
