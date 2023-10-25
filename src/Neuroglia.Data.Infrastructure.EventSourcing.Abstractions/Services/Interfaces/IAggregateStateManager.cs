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

namespace Neuroglia.Data.Infrastructure.EventSourcing.Services;

/// <summary>
/// Defines the fundamentals of a service used to manage <see cref="IAggregateRoot"/>s <see cref="IAggregateState"/>s
/// </summary>
/// <typeparam name="TAggregate">The type of the <see cref="IAggregateRoot"/> to manage the state of</typeparam>
/// <typeparam name="TKey">The type of key used to identify the <see cref="IAggregateRoot"/> to manage the state of</typeparam>
public interface IAggregateStateManager<TAggregate, TKey>
    where TAggregate : class, IAggregateRoot<TKey>
    where TKey : IEquatable<TKey>
{

    /// <summary>
    /// Takes an <see cref="ISnapshot"/> of the specified <see cref="IAggregateRoot"/>
    /// </summary>
    /// <param name="aggregate">The <see cref="IAggregateRoot"/> to snapshot</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    Task TakeSnapshotAsync(TAggregate aggregate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Restores the specified <see cref="IAggregateRoot"/>'s state using <see cref="ISnapshot"/>s
    /// </summary>
    /// <param name="id">The id of the <see cref="IAggregateRoot"/> to restore the state of</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The specified <see cref="IAggregateRoot"/>, hydrated using the latest <see cref="ISnapshot"/>, if any</returns>
    Task<TAggregate> RestoreStateAsync(TKey id, CancellationToken cancellationToken = default);

}