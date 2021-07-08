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
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.AspNetCore
{
    /// <summary>
    /// Represents the default implementation of the <see cref="IHeadersAccessor"/>
    /// </summary>
    public class HeadersAccessor
        : IHeadersAccessor
    {

        /// <summary>
        /// Initializes a new <see cref="HeadersAccessor"/>
        /// </summary>
        /// <param name="httpContextAccessor">The service used to access the current <see cref="HttpContext"/></param>
        public HeadersAccessor(IHttpContextAccessor httpContextAccessor)
        {
            this.HttpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Gets the service used to access the current <see cref="HttpContext"/>
        /// </summary>
        protected IHttpContextAccessor HttpContextAccessor { get; }

        /// <inheritdoc/>
        public IDictionary<string, StringValues> Headers => new ReadOnlyDictionary<string, StringValues>(this.HttpContextAccessor.HttpContext?.Request?.Headers);

    }

}
