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
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Neuroglia.Data.EventSourcing.EventStore.Configuration;
using System;

namespace Neuroglia.Data.EventSourcing
{

    /// <summary>
    /// Defines extensions for <see cref="IServiceCollection"/>s
    /// </summary>
    public static class IServiceCollectionExtensions
    {

        /// <summary>
        /// Adds and configures the <see cref="ESEventStore"/>
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
        /// <param name="setupAction">An <see cref="Action{T}"/> used to configure the <see cref="ESEventStore"/> to add</param>
        /// <returns>The configured <see cref="IServiceCollection"/></returns>
        public static IServiceCollection AddEventStore(this IServiceCollection services, Action<IEventStoreOptionsBuilder> setupAction)
        {
            IEventStoreOptionsBuilder optionsBuilder = new EventStoreOptionsBuilder();
            setupAction(optionsBuilder);
            EventStoreOptions options = optionsBuilder.Build();
            IEventStoreConnectionBuilder connectionBuilder = new EventStoreConnectionBuilder();
            options.ConnectionConfigurationAction?.Invoke(connectionBuilder);
            IEventStoreProjectionsManagerBuilder projectionManagerBuilder = new EventStoreProjectionsManagerBuilder();
            options.ProjectionsManagerConfigurationAction?.Invoke(projectionManagerBuilder);
            services.TryAddSingleton(Options.Create(options));
            services.TryAddSingleton(typeof(IAggregatorFactory), options.AggregatorFactoryType);
            services.TryAddSingleton(connectionBuilder.Build());
            services.TryAddSingleton(projectionManagerBuilder.Build());
            services.TryAddSingleton<ESEventStore>();
            services.TryAddSingleton<IEventStore>(provider => provider.GetRequiredService<ESEventStore>());
            return services;
        }

        /// <summary>
        /// Adds and configures the <see cref="ESEventStore"/>
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
        /// <returns>The configured <see cref="IServiceCollection"/></returns>
        public static IServiceCollection AddEventStore(this IServiceCollection services)
        {
            return services.AddEventStore(_ => { });
        }

    }

}
