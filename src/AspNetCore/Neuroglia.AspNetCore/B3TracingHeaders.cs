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
namespace Microsoft.AspNetCore
{

    /// <summary>
    /// Exposes constants about B3 tracing headers
    /// </summary>
    public static class B3TracingHeaders
    {

        /// <summary>
        /// Gets the 'x-request-id' header
        /// </summary>
        public const string REQUEST_ID = "x-request-id";

        /// <summary>
        /// Gets the 'x-b3-traceid' header
        /// </summary>
        public const string B3_TRACE_ID = "x-b3-traceid";

        /// <summary>
        /// Gets the 'x-b3-spanid' header
        /// </summary>
        public const string B3_SPAN_ID = "x-b3-spanid";

        /// <summary>
        /// Gets the 'x-b3-parentspanid' header
        /// </summary>
        public const string B3_PARENT_SPAN_ID = "x-b3-parentspanid";

        /// <summary>
        /// Gets the 'x-b3-sampled' header
        /// </summary>
        public const string B3_SAMPLED = "x-b3-sampled";

        /// <summary>
        /// Gets the 'x-b3-flags' header
        /// </summary>
        public const string B3_FLAGS = "x-b3-flags";

        /// <summary>
        /// Gets the 'x-ot-span-context' header
        /// </summary>
        public const string OT_SPAN_CONTEXT = "x-ot-span-context";

    }

}
