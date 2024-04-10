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
/// Defines the fundamentals of a service used to build a new event-driven projection source
/// </summary>
/// <typeparam name="TState">The type of the state of the projection to build the source for</typeparam>
public interface IProjectionSourceBuilder<TState>
{

    /// <summary>
    /// Builds a new event-driven projection that processes all events
    /// </summary>
    /// <returns>A new <see cref="IProjectionBuilder{TState}"/> implementation used to configure the projection to build</returns>
    IProjectionBuilder<TState> FromAll();

    /// <summary>
    /// Builds a new event-driven projection that processes events from the specified stream
    /// </summary>
    /// <param name="name">The name of the stream from which events will be processed by the projection</param>
    /// <returns>A new <see cref="IProjectionBuilder{TState}"/> implementation used to configure the projection to build</returns>
    IProjectionBuilder<TState> FromStream(string name);

}
