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
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Neuroglia.AspNetCore.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore
{
    /// <summary>
    /// Represents a <see cref="DelegatingHandler"/> used to propagate headers
    /// </summary>
    public class HeadersPropagationDelegatingHandler
        : DelegatingHandler
    {

        /// <summary>
        /// Initializes a new <see cref="HeadersPropagationDelegatingHandler"/>
        /// </summary>
        /// <param name="options">The service used to access the current <see cref="HeaderPropagationOptions"/></param>
        /// <param name="headersAccessor">The service used to access current headers</param>
        public HeadersPropagationDelegatingHandler(IOptions<HeaderPropagationOptions> options, IHeadersAccessor headersAccessor)
        {
            this.Options = options.Value;
            this.HeadersAccessor = headersAccessor;
        }

        /// <summary>
        /// Gets the current <see cref="HeaderPropagationOptions"/>
        /// </summary>
        protected HeaderPropagationOptions Options { get; }

        /// <summary>
        /// Gets the service used to access current headers
        /// </summary>
        protected IHeadersAccessor HeadersAccessor { get; }

        /// <inheritdoc/>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            IDictionary<string, StringValues> headersToPropagate = this.HeadersAccessor.Headers;
            if (!this.Options.PropagateAll)
                headersToPropagate = headersToPropagate
                    .Where(kvp => this.Options.Headers.Any(h => h.Equals(kvp.Key, StringComparison.OrdinalIgnoreCase)))
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            foreach (KeyValuePair<string, StringValues> header in headersToPropagate)
            {
                request.Headers.TryAddWithoutValidation(header.Key, (IEnumerable<string>)header.Value);
            }
            return await base.SendAsync(request, cancellationToken);
        }

    }

}
