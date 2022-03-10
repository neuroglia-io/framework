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
    /// Defines the fundamentals of a Flux feature
    /// </summary>
    public interface IFeature
    {

        /// <summary>
        /// Gets the <see cref="IFeature"/>'s value
        /// </summary>
        object Value { get; }

        /// <summary>
        /// Adds the specified <see cref="IReducer"/> to the <see cref="IFeature"/>
        /// </summary>
        /// <param name="reducer"></param>
        void AddReducer(IReducer reducer);

        /// <summary>
        /// Attempts to dispatch the specified Flux action
        /// </summary>
        /// <param name="action">The action to dispatch</param>
        /// <returns>A boolean indicating whether or not the action could be dispatched</returns>
        bool TryDispatch(object action);

    }

    /// <summary>
    /// Defines the fundamentals of a Flux feature
    /// </summary>
    /// <typeparam name="TFeature">The type of the <see cref="IFeature"/>'s value</typeparam>
    public interface IFeature<TFeature>
        : IFeature, IObservable<TFeature>
    {

        /// <summary>
        /// Gets the <see cref="IFeature"/>'s value
        /// </summary>
        new TFeature Value { get; }

    }

}
