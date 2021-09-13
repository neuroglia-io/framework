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
using Microsoft.Hadoop.Avro;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Neuroglia.Serialization
{

    /// <summary>
    /// Represents an Avro implementation of the <see cref="IBinarySerializer"/> interface
    /// </summary>
    public class AvroSerializer
        : BinarySerializerBase, IBinarySerializer
    {

        private static readonly MethodInfo CreateSerializerMethod = typeof(Microsoft.Hadoop.Avro.AvroSerializer)
            .GetMethods()
            .Single(m => m.Name == nameof(Microsoft.Hadoop.Avro.AvroSerializer.Create) && m.GetParameters().Length == 1);

        /// <inheritdoc/>
        public override IEnumerable<string> SupportedMimeTypes => new string[] { "avro/binary", "avro/json", "application/avro", "application/avro+binary", "application/avro+json" };

        /// <summary>
        /// Gets the <see cref="AvroSerializerSettings"/> used by underlying <see cref="IAvroSerializer{T}"/>
        /// </summary>
        protected AvroSerializerSettings SerializerSettings => new() 
        {  
            Resolver = new AvroPublicMemberContractResolver()
        };

        /// <inheritdoc/>
        public override string DefaultMimeType => "application/avro";

        /// <inheritdoc/>
        public override void Serialize(object value, Stream output, Type type)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            if (output == null)
                throw new ArgumentNullException(nameof(output));
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            object serializer = this.CreateSerializerFor(type);
            MethodInfo serializeMethod = typeof(IAvroSerializer<>).MakeGenericType(type)
                .GetMethods()
                .Single(m => m.Name == nameof(IAvroSerializer<object>.Serialize) && m.GetParameters().First().ParameterType == typeof(Stream));
            serializeMethod.Invoke(serializer, new object[] { output, value });
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
            object serializer = this.CreateSerializerFor(returnType);
            MethodInfo deserializeMethod = typeof(IAvroSerializer<>).MakeGenericType(returnType)
               .GetMethods()
               .Single(m => m.Name == nameof(IAvroSerializer<object>.Deserialize) && m.GetParameters().First().ParameterType == typeof(Stream));
            return deserializeMethod.Invoke(serializer, new object[] { input });
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

        /// <summary>
        /// Creates a new <see cref="IAvroSerializer{T}"/> for the specified type
        /// </summary>
        /// <param name="type">The type to create a new <see cref="IAvroSerializer{T}"/> for</param>
        /// <returns>A new <see cref="IAvroSerializer{T}"/></returns>
        protected virtual object CreateSerializerFor(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            return CreateSerializerMethod.MakeGenericMethod(type).Invoke(null, new object[] { this.SerializerSettings });
        }

    }

}
