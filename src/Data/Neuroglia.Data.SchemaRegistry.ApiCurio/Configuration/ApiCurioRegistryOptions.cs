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

namespace Neuroglia.Data.SchemaRegistry.Configuration
{

    /// <summary>
    /// Represents the options used to configure an <see cref="IApiCurioRegistryClient"/>
    /// </summary>
    public class ApiCurioRegistryClientOptions
    {

        /// <summary>
        /// Gets/sets the Api Curio Registry server uri
        /// </summary>
        public virtual Uri ServerUri { get; set; } = null!;

        /// <summary>
        /// Gets/sets the line ending format
        /// </summary>
        public virtual LineEndingFormatMode LineEndingFormatMode { get; set; } = LineEndingFormatMode.Preserve;

    }

}
