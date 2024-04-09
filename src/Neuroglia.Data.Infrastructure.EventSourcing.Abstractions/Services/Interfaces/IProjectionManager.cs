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
/// Defines the fundamentals of a service used to create and retrieve event-driven projections
/// </summary>
public interface IProjectionManager
{

    /// <summary>
    /// Creates a new event-driven projection
    /// </summary>
    /// <typeparam name="TState">The type of the state of the projection to create</typeparam>
    /// <param name="name">The name of the projection to create</param>
    /// <param name="setup">An <see cref="Action{T}"/> used to build and configure the event-driven projection to create</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    Task CreateAsync<TState>(string name, Action<IProjectionSourceBuilder<TState>> setup, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the current state of the event-driven projection with the specified name
    /// </summary>
    /// <typeparam name="TState">The type of the projection's state</typeparam>
    /// <param name="name">The name of the event-driven projection to get the current state of</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The current state of the specified event-driven projection</returns>
    Task<TState> GetStateAsync<TState>(string name, CancellationToken cancellationToken = default);

}
