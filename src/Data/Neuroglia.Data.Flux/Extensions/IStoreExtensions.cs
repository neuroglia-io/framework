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
    /// Defines extensions for <see cref="IStore"/>s
    /// </summary>
    public static class IStoreExtensions
    {

        /// <summary>
        /// Adds a new <see cref="IFeature"/> to the store
        /// </summary>
        /// <typeparam name="TFeature">The type of <see cref="IFeature"/> to add</typeparam>
        /// <param name="store">The <see cref="IStore"/> to add the <see cref="IFeature"/> to</param>
        /// <param name="featureValue">The value of the <see cref="IFeature"/> to add</param>
        /// <param name="reducers">An array that contains the <see cref="IReducer"/>s to initialize the <see cref="IFeature"/> to add with</param>
        public static void AddFeature<TFeature>(this IStore store, TFeature featureValue, params IReducer[] reducers)
        {
            var feature = new Feature<TFeature>(featureValue);
            foreach (var reducer in reducers)
            {
                feature.AddReducer(reducer);
            }
            store.AddFeature(feature);
        }

        /// <summary>
        /// Adds a new <see cref="IFeature"/> to the store
        /// </summary>
        /// <typeparam name="TFeature">The type of <see cref="IFeature"/> to add</typeparam>
        /// <param name="store">The <see cref="IStore"/> to add the <see cref="IFeature"/> to</param>
        /// <param name="reducers">An array that contains the <see cref="IReducer"/>s to initialize the <see cref="IFeature"/> to add with</param>
        public static void AddFeature<TFeature>(this IStore store, params IReducer[] reducers)
        {
            store.AddFeature(Activator.CreateInstance<TFeature>(), reducers);
        }

        /// <summary>
        /// Adds a new <see cref="IMiddleware"/> to the store
        /// </summary>
        /// <typeparam name="TMiddleware">The type of <see cref="IMiddleware"/> to add</typeparam>
        /// <param name="store">The <see cref="IStore"/> to add the <see cref="IMiddleware"/> to</param>
        public static void AddMiddleware<TMiddleware>(this IStore store)
            where TMiddleware : IMiddleware
        {
            store.AddMiddleware(Activator.CreateInstance<TMiddleware>());
        }

        /// <summary>
        /// Adds a new <see cref="IEffect"/> to the store
        /// </summary>
        /// <typeparam name="TEffect">The type of <see cref="IEffect"/> to add</typeparam>
        /// <param name="store">The <see cref="IStore"/> to add the <see cref="IEffect"/> to</param>
        public static void AddEffect<TEffect>(this IStore store)
            where TEffect : IEffect
        {
            store.AddEffect(Activator.CreateInstance<TEffect>());
        }

    }

}
