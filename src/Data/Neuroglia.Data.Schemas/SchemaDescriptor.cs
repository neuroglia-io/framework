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

namespace Neuroglia.Data
{
    /// <summary>
    /// Represents the default implementation of the <see cref="ISchemaDescriptor"/> interface
    /// </summary>
    public class SchemaDescriptor
        : ISchemaDescriptor
    {

        /// <summary>
        /// Initializes a new <see cref="SchemaDescriptor"/>
        /// </summary>
        /// <param name="type">The type of the described schema</param>
        /// <param name="id">The id of the described schema</param>
        /// <param name="version">The version of the described schema</param>
        /// <param name="documentUri">The <see cref="Uri"/> of the described schema's document</param>
        /// <param name="document">The described schema's document</param>
        public SchemaDescriptor(SchemaType type, string id, string? version, Uri documentUri, object document)
        {
            this.Type = type;
            this.Id = id;
            this.Version = version;
            this.DocumentUri = documentUri;
            this.Document = document;
        }

        /// <inheritdoc/>
        public virtual SchemaType Type { get; }

        /// <inheritdoc/>
        public virtual string Id { get; }

        /// <inheritdoc/>
        public virtual string? Version { get; }

        /// <inheritdoc/>
        public virtual Uri DocumentUri { get; }

        /// <inheritdoc/>
        public virtual object Document { get; }

    }

    /// <summary>
    /// Represents the default implementation of the <see cref="ISchemaDescriptor"/> interface
    /// </summary>
    /// <typeparam name="TSchema">The type of the schema to describe</typeparam>
    public class SchemaDescriptor<TSchema>
        : SchemaDescriptor, ISchemaDescriptor<TSchema>
        where TSchema : class
    {

        /// <inheritdoc/>
        public SchemaDescriptor(SchemaType type, string id, string? version, Uri documentUri, TSchema document) 
            : base(type, id, version, documentUri, document)
        {

        }

        /// <inheritdoc/>
        TSchema ISchemaDescriptor<TSchema>.Document => (TSchema)base.Document;

    }

}
