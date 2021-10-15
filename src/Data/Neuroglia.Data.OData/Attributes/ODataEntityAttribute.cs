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
using System;

namespace Neuroglia.Data
{

    /// <summary>
    /// Represents an <see cref="Attribute"/> used to mark a class as an OData entity
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ODataEntityAttribute
        : Attribute
    {

        /// <summary>
        /// Initializes a new <see cref="ODataEntityAttribute"/>
        /// </summary>
        public ODataEntityAttribute()
        {

        }

        /// <summary>
        /// Initializes a new <see cref="ODataEntityAttribute"/>
        /// </summary>
        /// <param name="collection">The name of the collection the marked class belongs to</param>
        public ODataEntityAttribute(string collection)
        {
            this.Collection = collection;
        }

        /// <summary>
        /// Gets/sets the name of the collection the marked class belongs to
        /// </summary>
        public string Collection { get; set; }

    }

}
