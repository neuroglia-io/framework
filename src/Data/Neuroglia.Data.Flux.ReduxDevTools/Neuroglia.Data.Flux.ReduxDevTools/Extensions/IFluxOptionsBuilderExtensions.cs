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
using Neuroglia.Data.Flux.Configuration;

namespace Neuroglia.Data.Flux
{

    /// <summary>
    /// Defines extensions for <see cref="IFluxOptionsBuilder"/>s
    /// </summary>
    public static class IFluxOptionsBuilderExtensions
    {

        /// <summary>
        /// Configures the <see cref="IFluxOptionsBuilder"/> to use Redux dev tools
        /// </summary>
        /// <param name="builder">The <see cref="IFluxOptionsBuilder"/> to configure</param>
        /// <returns>The configured <see cref="IFluxOptionsBuilder"/></returns>
        public static IFluxOptionsBuilder UseReduxDevTools(this IFluxOptionsBuilder builder)
        {
            builder.AddMiddleware<ReduxDevToolsMiddleware>();
            builder.Services.AddSingleton<IReduxDevToolsPlugin, ReduxDevToolsPlugin>();
            return builder;
        }

    }

}
