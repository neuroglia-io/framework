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
    /// Defines the fundamentals of a service used to interact with the Redux dev tools browser plugin
    /// </summary>
    public interface IReduxDevToolsPlugin
        : IDisposable
    {

        /// <summary>
        /// Gets a boolean indicating whether or not the browser Redux dev tools plugin has been enabled
        /// </summary>
        bool IsEnabled { get; }

        /// <summary>
        /// Dispatches the specified action
        /// </summary>
        /// <param name="action"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        Task DispatchAsync(object action, object state);

    }

}