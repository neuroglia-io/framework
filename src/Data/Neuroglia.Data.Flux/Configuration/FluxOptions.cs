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

using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Neuroglia.Data.Flux.Configuration
{
    /// <summary>
    /// Represents the options used to configure Flux
    /// </summary>
    public class FluxOptions
    {

        /// <summary>
        /// Gets/sets a <see cref="List{T}"/> containing the assemblies to scan for Flux components
        /// </summary>
        public virtual List<Assembly> AssembliesToScan { get; set; } = new();

        /// <summary>
        /// Gets/sets a boolean indicating whether or not to automatically register scanned <see cref="IFeature"/>s and <see cref="IReducer"/>s
        /// </summary>
        public virtual bool AutoRegisterFeatures { get; set; } = true;

        /// <summary>
        /// Gets/sets a boolean indicating whether or not to automatically register scanned <see cref="IEffect"/>s
        /// </summary>
        public virtual bool AutoRegisterEffects { get; set; } = true;

        /// <summary>
        /// Gets/sets a boolean indicating whether or not to automatically register scanned <see cref="IMiddleware"/>s
        /// </summary>
        public virtual bool AutoRegisterMiddlewares { get; set; } = false;

        /// <summary>
        /// Gets/sets the type of <see cref="IDispatcher"/> to use
        /// </summary>
        public virtual Type DispatcherType { get; set; } = typeof(Dispatcher);

        /// <summary>
        /// Gets/sets the type of <see cref="IStoreFactory"/> to use
        /// </summary>
        public virtual Type StoreFactoryType { get; set; } = typeof(StoreFactory);

        /// <summary>
        /// Gets/sets the type of <see cref="IStore"/> to use
        /// </summary>
        public virtual Type StoreType { get; set; } = typeof(Store);

        /// <summary>
        /// Gets/sets the lifetime of all Flux services
        /// </summary>
        public virtual ServiceLifetime ServiceLifetime { get; set; }

        /// <summary>
        /// Gets/sets an <see cref="Action{T}"/> used to configure the Flux <see cref="IStore"/>
        /// </summary>
        public virtual Action<IStore>? StoreSetup { get; set; }

    }

}
