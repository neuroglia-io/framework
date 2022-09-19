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

using Google.Protobuf.Reflection;
using Microsoft.Data.OData;
using Microsoft.OpenApi.Models;
using Neuroglia.Data.Services;

namespace Neuroglia.Data
{

    /// <summary>
    /// Defines extensions for <see cref="ISchemaRegistry"/> instances
    /// </summary>
    public static class ISchemaRegistryExtensions
    {

        /// <summary>
        /// Gets the <see cref="OpenApiDocument"/> at the specified uri
        /// </summary>
        /// <param name="schemaRegistry">The extended <see cref="ISchemaRegistry"/></param>
        /// <param name="documentUri">The <see cref="Uri"/> referencing the schema document to get</param>
        /// <param name="pullPolicy">The schema document pull policy</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>An <see cref="ISchemaDescriptor"/> that describes the <see cref="OpenApiDocument"/> at the specified uri</returns>
        public static async Task<ISchemaDescriptor<OpenApiDocument>> GetOpenApiSchemaAsync(this ISchemaRegistry schemaRegistry, Uri documentUri, SchemaDocumentPullPolicy pullPolicy = SchemaDocumentPullPolicy.IfNotPresent,  CancellationToken cancellationToken = default)
        {
            var schema = await schemaRegistry.GetSchemaAsync(SchemaType.OpenApi, documentUri, pullPolicy, cancellationToken);
            return (ISchemaDescriptor<OpenApiDocument>)schema;
        }

        /// <summary>
        /// Gets the <see cref="FileDescriptorProto"/> at the specified uri
        /// </summary>
        /// <param name="schemaRegistry">The extended <see cref="ISchemaRegistry"/></param>
        /// <param name="documentUri">The <see cref="Uri"/> referencing the schema document to get</param>
        /// <param name="pullPolicy">The schema document pull policy</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>An <see cref="ISchemaDescriptor"/> that describes the <see cref="FileDescriptorProto"/> at the specified uri</returns>
        public static async Task<ISchemaDescriptor<FileDescriptorProto>> GetProtoSchemaAsync(this ISchemaRegistry schemaRegistry, Uri documentUri, SchemaDocumentPullPolicy pullPolicy = SchemaDocumentPullPolicy.IfNotPresent, CancellationToken cancellationToken = default)
        {
            var schema = await schemaRegistry.GetSchemaAsync(SchemaType.Proto, documentUri, pullPolicy, cancellationToken);
            return (ISchemaDescriptor<FileDescriptorProto>)schema;
        }

        /// <summary>
        /// Gets the <see cref="ODataWorkspace"/> at the specified uri
        /// </summary>
        /// <param name="schemaRegistry">The extended <see cref="ISchemaRegistry"/></param>
        /// <param name="documentUri">The <see cref="Uri"/> referencing the schema document to get</param>
        /// <param name="pullPolicy">The schema document pull policy</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>An <see cref="ISchemaDescriptor"/> that describes the <see cref="ODataWorkspace"/> at the specified uri</returns>
        public static async Task<ISchemaDescriptor<ODataWorkspace>> GetODataSchemaAsync(this ISchemaRegistry schemaRegistry, Uri documentUri, SchemaDocumentPullPolicy pullPolicy = SchemaDocumentPullPolicy.IfNotPresent, CancellationToken cancellationToken = default)
        {
            var schema = await schemaRegistry.GetSchemaAsync(SchemaType.OData, documentUri, pullPolicy, cancellationToken);
            return (ISchemaDescriptor<ODataWorkspace>)schema;
        }

    }

}
