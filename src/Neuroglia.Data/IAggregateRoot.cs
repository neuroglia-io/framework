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

namespace Neuroglia.Data;

/// <summary>
/// Defines the fundamentals of an aggregate root
/// </summary>
public interface IAggregateRoot
    : IEntity
{

    /// <summary>
    /// Gets an <see cref="IReadOnlyList{T}"/> containing the <see cref="IAggregateRoot"/>'s pending <see cref="IDomainEvent"/>s
    /// </summary>
    IReadOnlyList<IDomainEvent> PendingEvents { get; }

    /// <summary>
    /// Clears all pending <see cref="IDomainEvent"/>
    /// </summary>
    void ClearPendingEvents();

}

/// <summary>
/// Defines the fundamentals of an aggregate root
/// </summary>
/// <typeparam name="TKey">The type of key used to uniquely identify the <see cref="IAggregateRoot"/></typeparam>
public interface IAggregateRoot<TKey>
    : IAggregateRoot, IEntity<TKey>
    where TKey : IEquatable<TKey>
{



}