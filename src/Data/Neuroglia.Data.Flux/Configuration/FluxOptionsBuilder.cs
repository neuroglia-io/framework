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
    /// Represents the default implementation of the <see cref="IFluxOptionsBuilder"/> interface
    /// </summary>
    public class FluxOptionsBuilder
        : IFluxOptionsBuilder
    {

        /// <summary>
        /// Gets the <see cref="FluxOptions"/> to configure
        /// </summary>
        protected FluxOptions Options { get; } = new();

        /// <inheritdoc/>
        public virtual IFluxOptionsBuilder ScanAssembly(Assembly assembly)
        {
            if(assembly == null)
                throw new ArgumentNullException(nameof(assembly));
            this.Options.AssembliesToScan.Add(assembly);
            return this;
        }

        /// <inheritdoc/>
        public virtual IFluxOptionsBuilder AutoRegisterFeatures(bool autoRegister = true)
        {
            this.Options.AutoRegisterFeatures = autoRegister;
            return this;
        }

        /// <inheritdoc/>
        public virtual IFluxOptionsBuilder AutoRegisterEffects(bool autoRegister = true)
        {
            this.Options.AutoRegisterEffects = autoRegister;
            return this;
        }

        /// <inheritdoc/>
        public virtual IFluxOptionsBuilder AutoRegisterMiddlewares(bool autoRegister = true)
        {
            this.Options.AutoRegisterMiddlewares = autoRegister;
            return this;
        }

        /// <inheritdoc/>
        public virtual IFluxOptionsBuilder UseDispatcher(Type dispatcherType)
        {
            if (dispatcherType == null)
                throw new ArgumentNullException(nameof(dispatcherType));
            if (!typeof(IDispatcher).IsAssignableFrom(dispatcherType))
                throw new ArgumentException($"The specified type must implement the {nameof(IDispatcher)} interface", nameof(dispatcherType));
            this.Options.DispatcherType = dispatcherType;
            return this;
        }

        /// <inheritdoc/>
        public virtual IFluxOptionsBuilder UseStoreFactory(Type storeFactoryType)
        {
            if (storeFactoryType == null)
                throw new ArgumentNullException(nameof(storeFactoryType));
            if (!typeof(IStoreFactory).IsAssignableFrom(storeFactoryType))
                throw new ArgumentException($"The specified type must implement the {nameof(IStoreFactory)} interface", nameof(storeFactoryType));
            this.Options.StoreFactoryType = storeFactoryType;
            return this;
        }

        /// <inheritdoc/>
        public virtual IFluxOptionsBuilder UseStore(Type storeType)
        {
            if (storeType == null)
                throw new ArgumentNullException(nameof(storeType));
            if (!typeof(IStore).IsAssignableFrom(storeType))
                throw new ArgumentException($"The specified type must implement the {nameof(IStore)} interface", nameof(storeType));
            this.Options.StoreType = storeType;
            return this;
        }

        /// <inheritdoc/>
        public virtual IFluxOptionsBuilder WithServiceLifetime(ServiceLifetime lifetime)
        {
            this.Options.ServiceLifetime = lifetime;
            return this;
        }

        /// <inheritdoc/>
        public virtual IFluxOptionsBuilder SetupStore(Action<IStore> setup)
        {
            if (setup == null)
                throw new ArgumentNullException(nameof(setup));
            this.Options.StoreSetup = setup;
            return this;
        }

        /// <inheritdoc/>
        public virtual FluxOptions Build()
        {
            return this.Options;
        }

    }

}
