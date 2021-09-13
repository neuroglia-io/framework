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
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Neuroglia.Serialization
{

    /// <summary>
    /// Represents the base class for all <see cref="IBinarySerializer"/> implementations
    /// </summary>
    public abstract class BinarySerializerBase
        : IBinarySerializer
    {

        /// <inheritdoc/>
        public abstract IEnumerable<string> SupportedMimeTypes { get; }

        /// <inheritdoc/>
        public abstract string DefaultMimeType { get; }

        /// <inheritdoc/>
        public virtual Encoding DefaultEncoding => Encoding.UTF8;

        /// <inheritdoc/>
        public abstract object Deserialize(Stream input, Type returnType);

        /// <inheritdoc/>
        public abstract Task<object> DeserializeAsync(Stream input, Type returnType, CancellationToken cancellationToken = default);

        /// <inheritdoc/>
        public virtual T Deserialize<T>(Stream input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));
            return (T)this.Deserialize(input, typeof(T));
        }

        /// <inheritdoc/>
        public virtual async Task<T> DeserializeAsync<T>(Stream input, CancellationToken cancellationToken = default)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));
            return (T)await this.DeserializeAsync(input, typeof(T), cancellationToken);
        }

        /// <inheritdoc/>
        public virtual object Deserialize(byte[] input, Type returnType)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));
            if (returnType == null)
                throw new ArgumentNullException(nameof(returnType));
            using MemoryStream stream = new(input);
            return this.Deserialize(stream, returnType);
        }

        /// <inheritdoc/>
        public virtual async Task<object> DeserializeAsync(byte[] input, Type returnType, CancellationToken cancellationToken = default)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));
            if (returnType == null)
                throw new ArgumentNullException(nameof(returnType));
            using MemoryStream stream = new(input);
            return await this.DeserializeAsync(stream, returnType, cancellationToken);
        }

        /// <inheritdoc/>
        public virtual T Deserialize<T>(byte[] input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));
            return (T)this.Deserialize(input, typeof(T));
        }

        /// <inheritdoc/>
        public virtual async Task<T> DeserializeAsync<T>(byte[] input, CancellationToken cancellationToken = default)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));
            return (T)await this.DeserializeAsync(input, typeof(T), cancellationToken);
        }

        /// <inheritdoc/>
        public abstract void Serialize(object value, Stream output, Type type);

        /// <inheritdoc/>
        public abstract Task SerializeAsync(object value, Stream output, Type type, CancellationToken cancellationToken = default);

        /// <inheritdoc/>
        public virtual void Serialize(object value, Stream output)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            if (output == null)
                throw new ArgumentNullException(nameof(value));
            this.Serialize(value, output, value.GetType());
        }

        /// <inheritdoc/>
        public virtual async Task SerializeAsync(object value, Stream output, CancellationToken cancellationToken = default)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            if (output == null)
                throw new ArgumentNullException(nameof(value));
            await this.SerializeAsync(value, output, value.GetType(), cancellationToken);
        }

        /// <inheritdoc/>
        public virtual byte[] Serialize(object value, Type type)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            using MemoryStream stream = new();
            this.Serialize(value, stream, type);
            stream.Flush();
            return stream.ToArray();
        }

        /// <inheritdoc/>
        public virtual async Task<byte[]> SerializeAsync(object value, Type type, CancellationToken cancellationToken = default)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            using MemoryStream stream = new();
            await this.SerializeAsync(value, stream, type, cancellationToken);
            await stream.FlushAsync(cancellationToken);
            return stream.ToArray();
        }

        /// <inheritdoc/>
        public virtual byte[] Serialize(object value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            return this.Serialize(value, value.GetType());
        }

        /// <inheritdoc/>
        public virtual async Task<byte[]> SerializeAsync(object value, CancellationToken cancellationToken = default)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            return await this.SerializeAsync(value, value.GetType(), cancellationToken);
        }

    }

}
