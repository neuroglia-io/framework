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
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using IYamlDotNetDeserializer = YamlDotNet.Serialization.IDeserializer;
using IYamlDotNetSerializer = YamlDotNet.Serialization.ISerializer;

namespace Neuroglia.Serialization
{

    /// <summary>
    /// Represents the default <see href="https://github.com/aaubry/YamlDotNet">YamlDotNet</see> implementation of the <see cref="ISerializer"/> interface
    /// </summary>
    public class YamlDotNetSerializer
        : TextSerializerBase, IYamlSerializer
    {

        /// <summary>
        /// Initializes a new <see cref="YamlDotNetSerializer"/>
        /// </summary>
        /// <param name="serializer">The underlying <see cref="IYamlDotNetSerializer"/></param>
        /// <param name="deserializer">The underlying <see cref="IYamlDotNetDeserializer"/></param>
        public YamlDotNetSerializer(IYamlDotNetSerializer serializer, IYamlDotNetDeserializer deserializer)
        {
            this.Serializer = serializer;
            this.Deserializer = deserializer;
        }

        /// <inheritdoc/>
        public override IEnumerable<string> SupportedContentTypes => new string[] { };

        /// <summary>
        /// Gets the underlying <see cref="IYamlDotNetSerializer"/>
        /// </summary>
        protected IYamlDotNetSerializer Serializer { get; }

        /// <summary>
        /// Gets the underlying <see cref="IYamlDotNetDeserializer"/>
        /// </summary>
        protected IYamlDotNetDeserializer Deserializer { get; }

        /// <inheritdoc/>
        public override void Serialize(object value, Stream output, Type type)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            if (output == null)
                throw new ArgumentNullException(nameof(output));
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            using StreamWriter writer = new(output);
            this.Serializer.Serialize(writer, value, type);
        }

        /// <inheritdoc/>
        public override async Task SerializeAsync(object value, Stream output, Type type, CancellationToken cancellationToken = default)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            if (output == null)
                throw new ArgumentNullException(nameof(output));
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            await Task.Run(() => this.Serialize(value, output, type), cancellationToken);
        }

        /// <inheritdoc/>
        public override object Deserialize(Stream input, Type returnType)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));
            if (returnType == null)
                throw new ArgumentNullException(nameof(returnType));
            using StreamReader reader = new(input);
            return this.Deserializer.Deserialize(reader, returnType);
        }

        /// <inheritdoc/>
        public override async Task<object> DeserializeAsync(Stream input, Type returnType, CancellationToken cancellationToken = default)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));
            if (returnType == null)
                throw new ArgumentNullException(nameof(returnType));
            return await Task.Run(() => this.Deserialize(input, returnType), cancellationToken);
        }

    }

}
