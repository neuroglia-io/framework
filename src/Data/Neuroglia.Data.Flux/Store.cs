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

namespace Neuroglia.Data.Flux
{

    /// <summary>
    /// Represents the default implementation of the <see cref="IStore"/> interface
    /// </summary>
    public class Store
        : IStore
    {

        /// <summary>
        /// Initializes a new <see cref="IStore"/>
        /// </summary>
        /// <param name="dispatcher">The service used to dispatch actions</param>
        public Store(IDispatcher dispatcher)
        {
            this.Dispatcher = dispatcher;
            this.Dispatcher.Subscribe(this.Dispatch);
        }

        /// <summary>
        /// Gets the service used to dispatch actions
        /// </summary>
        protected IDispatcher Dispatcher { get; }

        /// <summary>
        /// Gets the <see cref="Subject"/> used to stream actions
        /// </summary>
        protected Subject<object> Stream { get; } = new();

        /// <summary>
        /// Gets a <see cref="List{T}"/> containing the <see cref="Store"/>'s <see cref="IFeature"/>s
        /// </summary>
        protected List<IFeature> Features { get; } = new();

        /// <summary>
        /// Gets a <see cref="List{T}"/> containing the <see cref="Store"/>'s <see cref="IMiddleware"/>s
        /// </summary>
        protected List<IMiddleware> Middlewares { get; } = new();

        /// <summary>
        /// Gets a <see cref="List{T}"/> containing the <see cref="Store"/>'s <see cref="IEffect"/>s
        /// </summary>
        protected List<IEffect> Effects { get; } = new(); 

        IDisposable IObservable<object>.Subscribe(IObserver<object> observer)
        {
            if (observer == null)
                throw new ArgumentNullException(nameof(observer));
            return this.Stream.Subscribe(observer);
        }

        /// <inheritdoc/>
        public virtual void AddFeature<TFeature>(IFeature<TFeature> feature)
        {
            if(feature == null)
                throw new ArgumentNullException(nameof(feature));
            this.Features.Add(feature);
            feature.Subscribe(this.OnNextFeature);
        }

        /// <inheritdoc/>
        public virtual void AddMiddleware(IMiddleware middleware)
        {
            if (middleware == null)
                throw new ArgumentNullException(nameof(middleware));
            this.Middlewares.Add(middleware);
        }

        /// <inheritdoc/>
        public virtual void AddEffect(IEffect effect)
        {
            if (effect == null)
                throw new ArgumentNullException(nameof(effect));
            this.Effects.Add(effect);
        }

        /// <inheritdoc/>
        public virtual IFeature<TFeature> GetFeature<TFeature>()
        {
            return (IFeature<TFeature>)this.Features.First(f => f.Value.GetType() == typeof(TFeature));
        }

        /// <summary>
        /// Dispatches the specified action
        /// </summary>
        /// <param name="action">The action to dispatch</param>
        protected virtual void Dispatch(object action)
        {
            this.OnDispatching(action);
            this.OnDispatch(action);
            this.OnDispatched(action);
            this.OnApplyEffects(action);
        }

        /// <summary>
        /// Executes right before dispatching an action
        /// </summary>
        /// <param name="action">The action to dispatch</param>
        protected virtual void OnDispatching(object action)
        {
            foreach (IMiddleware middleWare in this.Middlewares)
            {
                middleWare.OnDispatching(action);
            }   
        }

        /// <summary>
        /// Handles the dispatching of an action
        /// </summary>
        /// <param name="action">The action to dispatch</param>
        protected virtual void OnDispatch(object action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));
            foreach(var feature in this.Features)
            {
                if (feature.TryDispatch(action))
                    return;
            }
        }

        /// <summary>
        /// Executes right after the specified action has been dispacthed
        /// </summary>
        /// <param name="action">The action that has been dispatched</param>
        protected virtual void OnDispatched(object action)
        {
            foreach (var middleWare in this.Middlewares)
            {
                middleWare.OnDispatched(action);
            }
        }

        /// <summary>
        /// Applies <see cref="IEffect"/>s
        /// </summary>
        /// <param name="action">The action to apply <see cref="IEffect"/>s to</param>
        protected virtual void OnApplyEffects(object action)
        {
            var applyEffectTasks = new List<Task>();
            foreach (var effect in this.Effects)
            {
                applyEffectTasks.Add(effect.ApplyAsync(action));
            }
            Task.Run(async () =>
            {
                await Task.WhenAll(applyEffectTasks);
            });
        }

        /// <summary>
        /// Handles the next feature value
        /// </summary>
        /// <typeparam name="TFeature">The type of <see cref="IFeature"/> to handle the next value for</typeparam>
        /// <param name="feature">The next <see cref="IFeature"/> value</param>
        protected virtual void OnNextFeature<TFeature>(TFeature feature)
        {
            this.Stream.OnNext(feature!);
        }

    }

}
