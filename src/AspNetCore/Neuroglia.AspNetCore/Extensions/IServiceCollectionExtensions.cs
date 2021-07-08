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
using Neuroglia.AspNetCore.Configuration;
using System;
using System.Linq;

namespace Microsoft.AspNetCore
{

    /// <summary>
    /// Defines extensions for <see cref="IServiceCollection"/>s
    /// </summary>
    public static class IServiceCollectionExtensions
    {

        /// <summary>
        /// Adds and configures services to enable the propagation of headers
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
        /// <param name="configurationAction">An <see cref="Action{T}"/> used to configure the headers to propagate</param>
        /// <returns>The configured <see cref="IServiceCollection"/></returns>
        public static IServiceCollection AddHeadersPropagation(this IServiceCollection services, Action<IHeaderPropagationOptionsBuilder> configurationAction)
        {
            if (configurationAction == null)
                throw new ArgumentNullException(nameof(configurationAction));
            HeaderPropagationOptionsBuilder optionsBuilder = new();
            configurationAction(optionsBuilder);
            services.TryAddSingleton(Options.Create(optionsBuilder.Build()));
            services.AddHttpContextAccessor();
            services.TryAddTransient<IHeadersAccessor, HeadersAccessor>();
            services.TryAddTransient<HeadersPropagationDelegatingHandler>();
            services.TryAddTransient<HeadersPropagationMessageHandlerBuilderFilter>();
            return services;
        }

        /// <summary>
        /// Adds and configures services to enable the propagation of headers
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
        /// <returns>The configured <see cref="IServiceCollection"/></returns>
        public static IServiceCollection AddHeadersPropagation(this IServiceCollection services)
        {
            return services.AddHeadersPropagation(builder => builder.PropagateAll());
        }

        /// <summary>
        /// Adds and configures services to enable the propagation of B3 Tracing headers
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
        /// <returns>The configured <see cref="IServiceCollection"/></returns>
        public static IServiceCollection AddB3TracingHeadersPropagation(this IServiceCollection services)
        {
            services.AddHeadersPropagation();
            services.Configure<HeaderPropagationOptions>(options =>
            {
                if (!options.Headers.Any(kvp => kvp.Equals(B3TracingHeaders.B3_FLAGS, StringComparison.OrdinalIgnoreCase)))
                    options.Headers.Add(B3TracingHeaders.B3_FLAGS);

                if (!options.Headers.Any(kvp => kvp.Equals(B3TracingHeaders.B3_PARENT_SPAN_ID, StringComparison.OrdinalIgnoreCase)))
                    options.Headers.Add(B3TracingHeaders.B3_PARENT_SPAN_ID);

                if (!options.Headers.Any(kvp => kvp.Equals(B3TracingHeaders.B3_SAMPLED, StringComparison.OrdinalIgnoreCase)))
                    options.Headers.Add(B3TracingHeaders.B3_SAMPLED);

                if (!options.Headers.Any(kvp => kvp.Equals(B3TracingHeaders.B3_SPAN_ID, StringComparison.OrdinalIgnoreCase)))
                    options.Headers.Add(B3TracingHeaders.B3_SPAN_ID);

                if (!options.Headers.Any(kvp => kvp.Equals(B3TracingHeaders.B3_TRACE_ID, StringComparison.OrdinalIgnoreCase)))
                    options.Headers.Add(B3TracingHeaders.B3_TRACE_ID);

                if (!options.Headers.Any(kvp => kvp.Equals(B3TracingHeaders.OT_SPAN_CONTEXT, StringComparison.OrdinalIgnoreCase)))
                    options.Headers.Add(B3TracingHeaders.OT_SPAN_CONTEXT);

                if (!options.Headers.Any(kvp => kvp.Equals(B3TracingHeaders.REQUEST_ID, StringComparison.OrdinalIgnoreCase)))
                    options.Headers.Add(B3TracingHeaders.REQUEST_ID);
            });
            return services;
        }

    }

}
