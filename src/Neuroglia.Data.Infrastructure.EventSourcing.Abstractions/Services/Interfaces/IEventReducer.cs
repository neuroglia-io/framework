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
/// Defines the fundamentals of a service used to reduce events
/// </summary>
public interface IEventReducer
{

    /// <summary>
    /// Processes the specified event and updates the current state
    /// </summary>
    /// <param name="e">The event to process</param>
    /// <param name="state">The state to apply the specified event to</param>
    object Reduce(object e, object state);

}

/// <summary>
/// Defines the fundamentals of a service used to process events and update the state of the data they apply to
/// </summary>
/// <typeparam name="TEvent">The type of event to reduce</typeparam>
/// <typeparam name="TState">The type of the state to reduce</typeparam>
public interface IEventReducer<TEvent, TState>
    : IEventReducer
{

    /// <summary>
    /// Processes the specified event and updates the current state
    /// </summary>
    /// <param name="e">The event to process</param>
    /// <param name="state">The state to apply the specified event to</param>
    TState Reduce(TEvent e, TState state);

}