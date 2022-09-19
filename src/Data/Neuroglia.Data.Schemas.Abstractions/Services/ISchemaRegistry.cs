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
    /// Defines the fundamentals of a service used to manage schemas
    /// </summary>
    public interface ISchemaRegistry
    {

        /// <summary>
        /// Gets the schema at the specified <see cref="Uri"/>
        /// </summary>
        /// <param name="schemaType">The type of schema to get</param>
        /// <param name="documentUri">The <see cref="Uri"/> referencing the document of the schema to get</param>
        /// <param name="pullPolicy">The schema document pull policy</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A <see cref="ISchemaDescriptor"/> that descrubes the schema at the specified uri</returns>
        Task<ISchemaDescriptor> GetSchemaAsync(SchemaType schemaType, Uri documentUri, SchemaDocumentPullPolicy pullPolicy = SchemaDocumentPullPolicy.IfNotPresent, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the schema with the specified id
        /// </summary>
        /// <param name="id">The id of the schema to get</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>An <see cref="ISchemaDescriptor"/> that describes the schema with the specified id</returns>
        Task<ISchemaDescriptor?> GetSchemaByIdAsync(string id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the schema with the specified id
        /// </summary>
        /// <param name="id">The id of the schema to get</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>An <see cref="ISchemaDescriptor"/> that describes the schema with the specified resource <see cref="Uri"/></returns>
        Task<ISchemaDescriptor?> GetSchemaByDocumentUriAsync(Uri documentUri, CancellationToken cancellationToken = default);

    }

}
