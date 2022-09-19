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
    /// Defines the fundamentals of an object used to describe a schema
    /// </summary>
    public interface ISchemaDescriptor
    {

        /// <summary>
        /// Gets the type of the described schema
        /// </summary>
        SchemaType Type { get; }

        /// <summary>
        /// Gets the id of the described schema
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Gets the version of the described schema
        /// </summary>
        string? Version { get; }

        /// <summary>
        /// Gets the <see cref="Uri"/> of the described schema's document
        /// </summary>
        Uri DocumentUri { get; }

        /// <summary>
        /// Gets the described schema's document
        /// </summary>
        object Document { get; }

    }

    /// <summary>
    /// Defines the fundamentals of an object used to describe a schema
    /// </summary>
    /// <typeparam name="TSchema">The type of the schema to describe</typeparam>
    public interface ISchemaDescriptor<TSchema>
        : ISchemaDescriptor
        where TSchema : class
    {

        /// <summary>
        /// Gets the described schema's document
        /// </summary>
        new TSchema Document { get; }

    }

}
