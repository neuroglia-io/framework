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
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Neuroglia.Serialization
{

    /// <summary>
    /// Represents the default implementation of the <see cref="IProtobufSerializer"/> interface
    /// </summary>
    public class ProtobufSerializer
        : BinarySerializerBase, IProtobufSerializer
    {

        /// <inheritdoc/>
        public override IEnumerable<string> SupportedContentTypes => new string[] 
        {
            "application/protobuf",
            "application/x-protobuf",
            "application/vnd.google.protobuf"
        };

        /// <inheritdoc/>
        public override object Deserialize(Stream input, Type returnType)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));
            if (returnType == null)
                throw new ArgumentNullException(nameof(returnType));
            return Serializer.Deserialize(returnType, input);
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

        /// <inheritdoc/>
        public override void Serialize(object value, Stream output, Type type)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            if (output == null)
                throw new ArgumentNullException(nameof(output));
            Serializer.Serialize(output, value);
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

    }

}
