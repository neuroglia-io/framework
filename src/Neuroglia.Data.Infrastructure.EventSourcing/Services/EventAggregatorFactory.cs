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

using System.Collections.Concurrent;

namespace Neuroglia.Data.Infrastructure.EventSourcing.Services;

/// <summary>
/// Represents the default implementation of the <see cref="IEventAggregatorFactory"/> interface
/// </summary>
public class EventAggregatorFactory
    : IEventAggregatorFactory
{

    /// <summary>
    /// Initializes a new <see cref="EventAggregatorFactory"/>
    /// </summary>
    public EventAggregatorFactory()
    {
        this.Aggregators = new ConcurrentDictionary<Type, IEventAggregator>();
    }

    /// <summary>
    /// Gets a <see cref="ConcurrentDictionary{TKey, TValue}"/> containing all available <see cref="IEventAggregator"/>s
    /// </summary>
    protected ConcurrentDictionary<Type, IEventAggregator> Aggregators { get; }

    /// <inheritdoc/>
    public virtual IEventAggregator<TState> CreateAggregator<TState>() 
        where TState : class
    {
        if (!this.Aggregators.TryGetValue(typeof(TState), out var aggregator))
        {
            aggregator = new EventAggregator<TState, object>();
            this.Aggregators.TryAdd(typeof(TState), aggregator);
        }
        return (IEventAggregator<TState>)aggregator;
    }

    /// <inheritdoc/>
    public virtual IEventAggregator<TState, TEvent> CreateAggregator<TState, TEvent>() 
        where TState : class
    {
        if (!this.Aggregators.TryGetValue(typeof(TState), out var aggregator))
        {
            aggregator = new EventAggregator<TState, TEvent>();
            this.Aggregators.TryAdd(typeof(TState), aggregator);
        }
        return (IEventAggregator<TState, TEvent>)aggregator;
    }

}