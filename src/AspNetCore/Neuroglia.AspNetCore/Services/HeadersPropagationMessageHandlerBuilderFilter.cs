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
using Microsoft.Extensions.Http;
using System;
using System.Net.Http;

namespace Microsoft.AspNetCore
{

    /// <summary>
    /// Represents the <see cref="IHttpMessageHandlerBuilderFilter"/> used to apply the <see cref="HeadersPropagationDelegatingHandler"/>
    /// </summary>
    public class HeadersPropagationMessageHandlerBuilderFilter
        : IHttpMessageHandlerBuilderFilter
    {

        /// <summary>
        /// Initializes a new <see cref="HeadersPropagationMessageHandlerBuilderFilter"/>
        /// </summary>
        /// <param name="headersPropagationDelegatingHandler">The <see cref="DelegatingHandler"/> used to propagate headers</param>
        public HeadersPropagationMessageHandlerBuilderFilter(HeadersPropagationDelegatingHandler headersPropagationDelegatingHandler)
        {
            this.HeadersPropagationDelegatingHandler = headersPropagationDelegatingHandler;
        }

        /// <summary>
        /// Gets the <see cref="DelegatingHandler"/> used to propagate headers
        /// </summary>
        protected HeadersPropagationDelegatingHandler HeadersPropagationDelegatingHandler { get; }

        /// <inheritdoc/>
        public Action<HttpMessageHandlerBuilder> Configure(Action<HttpMessageHandlerBuilder> next)
        {
            if (next == null)
                throw new ArgumentNullException(nameof(next));
            return (builder) =>
            {
                next(builder);
                builder.AdditionalHandlers.Add(this.HeadersPropagationDelegatingHandler);
            };
        }

    }

}
