/*
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

using System.Reflection;

namespace Neuroglia.Data.Flux
{

    /// <summary>
    /// Represents the default implementation of the <see cref="IFeature"/> interface
    /// </summary>
    /// <typeparam name="TFeature">The type of the <see cref="IFeature"/>'s value</typeparam>
    public class Feature<TFeature>
        : IFeature<TFeature>
    {

        /// <summary>
        /// Initializes a new <see cref="Feature{TFeature}"/>
        /// </summary>
        /// <param name="value">The <see cref="Feature{TFeature}"/>'s value</param>
        public Feature(TFeature value)
        {
            if(value == null)
                throw new ArgumentNullException(nameof(value)); 
            this.Value = value;
            this.Stream = new(this.Value);
            this.Initialize();
        }

        /// <inheritdoc/>
        public TFeature Value { get; protected set; }

        object IFeature.Value => this.Value!;

        /// <summary>
        /// Gets a <see cref="List{T}"/> containing the states the <see cref="Feature{TFeature}"/> is made out of
        /// </summary>
        protected List<IState> States { get; } = new();

        /// <summary>
        /// Gets the <see cref="BehaviorSubject{T}"/> used to stream the <see cref="Feature{TFeature}"/> changes
        /// </summary>
        protected BehaviorSubject<TFeature> Stream { get; }

        /// <summary>
        /// Initializes the <see cref="Feature{TFeature}"/>
        /// </summary>
        protected virtual void Initialize()
        {
            var genericSubscribeMethod = typeof(ObservableExtensions).GetMethods().First(m => m.Name == nameof(ObservableExtensions.Subscribe) && m.GetParameters().Length == 2);
            foreach(var property in typeof(TFeature).GetProperties())
            {
                var genericStateType = typeof(State<>).MakeGenericType(property.PropertyType);
                var stateValue = property.GetValue(this.Value);
                var state = (IState)Activator.CreateInstance(genericStateType, stateValue)!;
                this.States.Add(state);
                genericSubscribeMethod.MakeGenericMethod(property.PropertyType).Invoke(null, new object[] { state, (object stateValue) => this.OnNextState(property, stateValue) });
            }
            this.Stream.OnNext(this.Value);
        }

        /// <summary>
        /// Adds a <see cref="IReducer"/>
        /// </summary>
        /// <param name="reducer">The <see cref="IReducer"/> to add</param>
        public void AddReducer(IReducer reducer)
        {
            if(reducer == null)
                throw new ArgumentNullException(nameof(reducer));
            var genericReducerType = reducer.GetType().GetGenericType(typeof(IReducer<>));
            var stateType = genericReducerType.GetGenericArguments().First();
            this.States
                .First(s => s.Value?.GetType() == stateType)
                .AddReducer(reducer);
        }

        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<TFeature> observer)
        {
            return this.Stream.Subscribe(observer);
        }

        /// <inheritdoc/>
        public bool TryDispatch(object action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));
            foreach(var state in this.States)
            {
                if (state.TryDispatch(action))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Handles the next <see cref="Feature{TFeature}"/>'s value
        /// </summary>
        /// <param name="stateProperty">The <see cref="PropertyInfo"/> used to get the state to handle the next value of</param>
        /// <param name="stateValue">The next value of the state to handle</param>
        protected virtual void OnNextState(PropertyInfo stateProperty, object stateValue)
        {
            stateProperty.SetValue(this.Value, stateValue);
            this.Stream.OnNext(this.Value!);
        }

    }

}
