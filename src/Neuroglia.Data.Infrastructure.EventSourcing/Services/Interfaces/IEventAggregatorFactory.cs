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
/// Defines the fundamentals of a service used to create <see cref="IEventAggregator"/>s
/// </summary>
public interface IEventAggregatorFactory
{

    /// <summary>
    /// Creates a new <see cref="IEventAggregator"/>
    /// </summary>
    /// <typeparam name="TState">The type of the state aggregated events apply to</typeparam>
    /// <returns>A new <see cref="IEventAggregator"/></returns>
    IEventAggregator<TState> CreateAggregator<TState>()
        where TState : class;

    /// <summary>
    /// Creates a new <see cref="IEventAggregator"/>
    /// </summary>
    /// <typeparam name="TState">The type of the state aggregated events apply to</typeparam>
    /// <typeparam name="TEvent">The type of events to aggregate</typeparam>
    /// <returns>A new <see cref="IEventAggregator"/></returns>
    IEventAggregator<TState, TEvent> CreateAggregator<TState, TEvent>()
        where TState : class;

}
