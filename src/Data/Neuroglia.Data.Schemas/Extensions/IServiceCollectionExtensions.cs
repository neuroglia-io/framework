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
using Neuroglia.Data.Services;

namespace Neuroglia.Data
{

    /// <summary>
    /// Defines extensions for <see cref="IServiceCollection"/>s
    /// </summary>
    public static class IServiceCollectionExtensions
    {

        /// <summary>
        /// Registers and configures a new default implementation of the <see cref="ISchemaRegistry"/> interface
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
        /// <returns>The configures <see cref="IServiceCollection"/></returns>
        public static IServiceCollection AddSchemaRegistry(this IServiceCollection services)
        {
            services.AddSingleton<SchemaRegistry>();
            services.AddSingleton<ISchemaRegistry>(provider => provider.GetRequiredService<SchemaRegistry>());

            services.AddSingleton<OpenApiSchemaReader>();
            services.AddSingleton<ISchemaReader>(provider => provider.GetRequiredService<OpenApiSchemaReader>());

            services.AddSingleton<ProtoSchemaReader>();
            services.AddSingleton<ISchemaReader>(provider => provider.GetRequiredService<ProtoSchemaReader>());

            services.AddSingleton<ODataSchemaReader>();
            services.AddSingleton<ISchemaReader>(provider => provider.GetRequiredService<ODataSchemaReader>());

            return services;
        }

    }

}
