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

namespace Neuroglia.Data.Services
{

    /// <summary>
    /// Defines the fundamentals of a service used to read schema documents
    /// </summary>
    public interface ISchemaReader
    {

        /// <summary>
        /// Gets the type of schema document the <see cref="ISchemaReader"/> handles
        /// </summary>
        SchemaType SchemaType { get; }

        /// <summary>
        /// Reads the schema document at the specified <see cref="Uri"/>
        /// </summary>
        /// <param name="documentUri">The <see cref="Uri"/> of the schema document to read</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>The schema document</returns>
        Task<ISchemaDescriptor> ReadFromAsync(Uri documentUri, CancellationToken cancellationToken = default);

    }

}
