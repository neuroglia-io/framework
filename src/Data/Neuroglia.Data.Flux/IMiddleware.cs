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
    /// Defines the fundamentals of a middleware
    /// </summary>
    public interface IMiddleware
    {

        /// <summary>
        /// Method that executes right before dispatching a Flux action
        /// </summary>
        /// <param name="action">The action that is about to be dispatched</param>
        void OnDispatching(object action);

        /// <summary>
        /// Method that executes right after dispatching a Flux action
        /// </summary>
        /// <param name="action">The dispatched action</param>
        void OnDispatched(object action);

    }

}
