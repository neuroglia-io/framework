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
    /// Defines the fundamentals of a Flux store
    /// </summary>
    public interface IStore
        : IObservable<object>
    {

        /// <summary>
        /// Adds a new <see cref="IFeature"/> to the store
        /// </summary>
        /// <typeparam name="TFeature">The type of <see cref="IFeature"/> to add</typeparam>
        /// <param name="feature">The <see cref="IFeature"/> to add</param>
        void AddFeature<TFeature>(IFeature<TFeature> feature);

        /// <summary>
        /// Adds a new <see cref="IMiddleware"/> to the store
        /// </summary>
        /// <param name="middleware">The <see cref="IMiddleware"/> to add</param>
        void AddMiddleware(IMiddleware middleware);

        /// <summary>
        /// Adds a new <see cref="IEffect"/> to the store
        /// </summary>
        /// <param name="middleware">The <see cref="IEffect"/> to add</param>
        void AddEffect(IEffect effect);

        /// <summary>
        /// Gets the <see cref="IFeature"/> of the specified type
        /// </summary>
        /// <typeparam name="TFeature">The type of the <see cref="IFeature"/> to get</typeparam>
        /// <returns>The <see cref="IFeature"/> of the specified type</returns>
        IFeature<TFeature> GetFeature<TFeature>();

    }

}
