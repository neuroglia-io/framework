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
    /// Represents the default implementation of the <see cref="IEffect{TAction}"/> interface
    /// </summary>
    /// <typeparam name="TAction">The type of action to apply the effect to</typeparam>
    public class Effect<TAction>
        : IEffect<TAction>
    {

        /// <summary>
        /// Initializes a new <see cref="IEffect{TAction}"/>
        /// </summary>
        /// <param name="effectFunction">The effect <see cref="Func{T, TResult}"/></param>
        public Effect(Func<TAction, Task> effectFunction)
        {
            if(effectFunction == null)
                throw new ArgumentNullException(nameof(effectFunction));
            this.EffectFunction = effectFunction;
        }

        /// <summary>
        /// Gets the effect <see cref="Func{T, TResult}"/>
        /// </summary>
        protected Func<TAction, Task> EffectFunction { get; }

        /// <inheritdoc/>
        public virtual bool AppliesTo(object action)
        {
            if(action == null)  
               throw new ArgumentNullException(nameof(action));
            return action is TAction;
        }

        /// <inheritdoc/>
        public virtual async Task ApplyAsync(TAction action, CancellationToken cancellationToken = default)
        {
            await this.EffectFunction(action);
        }

        async Task IEffect.ApplyAsync(object action, CancellationToken cancellationToken)
        {
            await this.ApplyAsync((TAction)action, cancellationToken).ConfigureAwait(false);
        }

    }

}
