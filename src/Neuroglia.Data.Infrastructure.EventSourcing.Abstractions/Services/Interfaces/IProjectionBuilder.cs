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

using System.Linq.Expressions;

namespace Neuroglia.Data.Infrastructure.EventSourcing.Services;

/// <summary>
/// Defines the fundamentals of a service used to build event-driven projections
/// </summary>
/// <typeparam name="TState">The type of the state of the event-driven projection to create</typeparam>
public interface IProjectionBuilder<TState>
{

    /// <summary>
    /// Configures the <see cref="Func{TResult}"/> used to create the projection's initial state
    /// </summary>
    /// <param name="factory">The <see cref="Func{TResult}"/> used to create the projection's initial state</param>
    /// <returns>The configured <see cref="IProjectionBuilder{TState}"/></returns>
    IProjectionBuilder<TState> Given(Expression<Func<TState>> factory);

    /// <summary>
    /// Configures the predicate used to to filter incoming event records based on the current projection state
    /// </summary>
    /// <param name="predicate">A <see cref="Func{T1, T2, TResult}"/> used to to filter incoming event records based on the current projection state</param>
    /// <returns>The configured <see cref="IProjectionBuilder{TState}"/></returns>
    IProjectionBuilder<TState> When(Expression<Func<TState, IEventRecord, bool>> predicate);

    /// <summary>
    /// Configures an <see cref="Action{T1, T2}"/> to be performed on the projection state when a matching event record is processed
    /// </summary>
    /// <param name="action">An <see cref="Action{T1, T2}"/> to be performed on the projection state when a matching event record is processed</param>
    /// <returns>The configured <see cref="IProjectionBuilder{TState}"/></returns>
    IProjectionBuilder<TState> Then(Expression<Action<TState, IEventRecord>> action);

}