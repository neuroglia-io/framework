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

using System.Reflection;

namespace Neuroglia.Data.Infrastructure.EventSourcing.Services;

/// <summary>
/// Represents the default implementation of the <see cref="IEventReducer{TEvent, TState}"/> interface
/// </summary>
/// <typeparam name="TState">The type of aggregate to handle</typeparam>
/// <typeparam name="TEvent">The type of events to aggregate</typeparam>
public class EventReducer<TEvent, TState>
   : IEventReducer<TEvent, TState>
   where TState : class
{

    /// <summary>
    /// Initializes a new <see cref="EventReducer{TEvent, TAggregate}"/>
    /// </summary>
    /// <param name="reducerMethod">The reducer <see cref="MethodInfo"/></param>
    public EventReducer(MethodInfo reducerMethod)
    {
        this.ReducerMethod = reducerMethod ?? throw new ArgumentNullException(nameof(reducerMethod));
    }

    /// <summary>
    /// Gets the reducer <see cref="MethodInfo"/>
    /// </summary>
    protected MethodInfo ReducerMethod { get; }

    /// <inheritdoc/>
    public virtual TState Reduce(TEvent e, TState state)
    {
        if (e == null) throw new ArgumentNullException(nameof(e));
        ArgumentNullException.ThrowIfNull(state);
        this.ReducerMethod.Invoke(state is IAggregateRoot aggregate ? aggregate.State : state, new object[] { e });
        return state;
    }

    /// <inheritdoc/>
    object IEventReducer.Reduce(object e, object state) => this.Reduce((TEvent)e, (TState)state);

}