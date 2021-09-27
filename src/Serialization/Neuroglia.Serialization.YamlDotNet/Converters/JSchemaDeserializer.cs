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
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using YamlDotNet.Core;

namespace YamlDotNet.Serialization
{
    /// <summary>
    /// Represents the <see cref="IYamlTypeConverter"/> used to serialize <see cref="JSchema"/>s
    /// </summary>
    public class JSchemaDeserializer
        : INodeDeserializer
    {

        /// <summary>
        /// Initializes a new <see cref="JSchemaDeserializer"/>
        /// </summary>
        /// <param name="inner">The inner <see cref="INodeDeserializer"/></param>
        public JSchemaDeserializer(INodeDeserializer inner)
        {
            this.Inner = inner;
        }

        /// <summary>
        /// Gets the inner <see cref="INodeDeserializer"/>
        /// </summary>
        protected INodeDeserializer Inner { get; }

        /// <inheritdoc/>
        public virtual bool Deserialize(IParser reader, Type expectedType, Func<IParser, Type, object> nestedObjectDeserializer, out object value)
        {
            if (!typeof(JSchema).IsAssignableFrom(expectedType))
                return this.Inner.Deserialize(reader, expectedType, nestedObjectDeserializer, out value);
            if (!this.Inner.Deserialize(reader, typeof(JToken), nestedObjectDeserializer, out value))
                return false;
            JToken jtoken = value as JToken;
            string json = jtoken.ToString();
            JSchema jschema = JSchema.Parse(json);
            value = jschema;
            return true;
        }

    }

}
