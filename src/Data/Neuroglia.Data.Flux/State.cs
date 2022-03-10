﻿/*
 * Copyright © 2021 Neuroglia SPRL. All rights reserved.
 * <p>
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * <p>
 * http://www.apache.org/licenses/LICENSE-2.0
 * <p>
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 */

namespace Neuroglia.Data.Flux
{

    /// <summary>
    /// Represents the default implementation of the <see cref="IState{TState}"/> interface
    /// </summary>
    /// <typeparam name="TState">The type of the <see cref="IState"/>'s value</typeparam>
    public class State<TState>
        : IState<TState>
    {

        /// <summary>
        /// Initializes a new <see cref="State{TState}"/>
        /// </summary>
        /// <param name="value">The <see cref="State{TState}"/>'s value</param>
        public State(TState value)
        {
            if(value == null)
                throw new ArgumentNullException(nameof(value));
            this.Value = value;
        }

        /// <inheritdoc/>
        public TState Value { get; protected set; }

        object IState.Value => this.Value!;

        /// <summary>
        /// Gets the <see cref="Subject"/> used to stream state values
        /// </summary>
        protected Subject<TState> Stream { get; } = new();

        /// <summary>
        /// Gets a <see cref="Dictionary{TKey, TValue}"/> containing the type/<see cref="IReducer"/>s mappings
        /// </summary>
        protected Dictionary<Type, List<IReducer<TState>>> Reducers { get; } = new();

        /// <inheritdoc/>
        public virtual void AddReducer(IReducer<TState> reducer)
        {
            if(reducer == null)
                throw new ArgumentNullException(nameof(reducer));
            var reducerGenericType = reducer.GetType().GetGenericType(typeof(IReducer<,>));
            var actionType = reducerGenericType.GetGenericArguments()[1];
            if (this.Reducers.TryGetValue(actionType, out var reducers))
                reducers.Add(reducer);
            else
                this.Reducers.Add(actionType, new List<IReducer<TState>>() { reducer });
        }

        void IState.AddReducer(IReducer reducer)
        {
            if (reducer == null)
                throw new ArgumentNullException(nameof(reducer));
            this.AddReducer((IReducer<TState>)reducer);
        }

        /// <inheritdoc/>
        public virtual IDisposable Subscribe(IObserver<TState> observer)
        {
            return this.Stream.Subscribe(observer);
        }

        /// <inheritdoc/>
        public virtual bool TryDispatch(object action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));
            if (!this.Reducers.TryGetValue(action.GetType(), out var reducers))
                return false;
            foreach(var reducer in reducers)
            {
                this.Value = reducer.Reduce(this.Value, action);
            }
            this.Stream.OnNext(this.Value!);
            return true;
        }

    }

}
