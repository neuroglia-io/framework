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

namespace Neuroglia.Data.Infrastructure.EventSourcing.Configuration;

/// <summary>
/// Represents the options used to configure state management for the specified <see cref="IAggregateRoot"/> type
/// </summary>
public class StateManagementOptions
{

    /// <summary>
    /// Gets the default snapshot frequency
    /// </summary>
    public const ulong DefaultSnapshotFrequency = 10;

    /// <summary>
    /// Gets/sets the frequency at which to snapshot <see cref="IAggregateRoot"/>s
    /// </summary>
    public virtual ulong? SnapshotFrequency { get; set; } = DefaultSnapshotFrequency;

    /// <summary>
    /// Gets/sets the <see cref="Func{T, TResult}"/> to use to create new instances of the <see cref="IAggregateRoot"/>s to manage the state of
    /// </summary>
    public virtual Func<IServiceProvider, IAggregateRoot>? AggregateFactory { get; set; }

}

/// <summary>
/// Represents the options used to configure state management for the specified <see cref="IAggregateRoot"/> type
/// </summary>
/// <typeparam name="TAggregate">The type of <see cref="IAggregateRoot{TKey}"/> to configure state management for</typeparam>
/// <typeparam name="TKey">The type of key used to identify the <see cref="IAggregateRoot{TKey}"/> to configure state management for</typeparam>
public class StateManagementOptions<TAggregate, TKey>
    : StateManagementOptions
    where TAggregate : class, IAggregateRoot<TKey>
    where TKey : IEquatable<TKey>
{

    /// <summary>
    /// Gets/sets the <see cref="Func{T, TResult}"/> to use to create new instances of the <see cref="IAggregateRoot"/>s to manage the state of
    /// </summary>
    public virtual new Func<IServiceProvider, TAggregate>? AggregateFactory { get; set; } 

}