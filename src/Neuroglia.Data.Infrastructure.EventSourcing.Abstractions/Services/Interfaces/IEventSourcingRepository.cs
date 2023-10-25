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

using Neuroglia.Data.Infrastructure.Services;

namespace Neuroglia.Data.Infrastructure.EventSourcing.Services;

/// <summary>
/// Defines the fundamentals of an event sourcing implementation of the <see cref="IRepository"/> interface
/// </summary>
public interface IEventSourcingRepository
    : IRepository
{


}

/// <summary>
/// Defines the fundamentals of an event sourcing implementation of the <see cref="IRepository"/> interface
/// </summary>
/// <typeparam name="TAggregate">The type of the managed <see cref="IAggregateRoot"/>s</typeparam>
public interface IEventSourcingRepository<TAggregate>
    : IEventSourcingRepository, IRepository<TAggregate>
    where TAggregate : class, IAggregateRoot
{



}

/// <summary>
/// Defines the fundamentals of an event sourcing implementation of the <see cref="IRepository"/> interface
/// </summary>
/// <typeparam name="TAggregate">The type of the managed <see cref="IAggregateRoot"/>s</typeparam>
/// <typeparam name="TKey">The key used to identify managed <see cref="IAggregateRoot"/>s</typeparam>
public interface IEventSourcingRepository<TAggregate, TKey>
    : IEventSourcingRepository<TAggregate>, IRepository<TAggregate, TKey>
    where TAggregate : class, IAggregateRoot<TKey>
    where TKey : IEquatable<TKey>
{



}
