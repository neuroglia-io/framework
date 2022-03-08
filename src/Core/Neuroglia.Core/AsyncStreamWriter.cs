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
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Neuroglia
{

    /// <summary>
    /// Represents the default implementation of the <see cref="IAsyncStreamWriter{T}"/> interface
    /// </summary>
    /// <typeparam name="T">The type of data to write</typeparam>
    public class AsyncStreamWriter<T>
        : IAsyncStreamWriter<T>
    {

        /// <summary>
        /// Initializes a new <see cref="AsyncStreamWriter{T}"/>
        /// </summary>
        /// <param name="writer">The <see cref="ChannelWriter{T}"/> used to write data to the stream</param>
        public AsyncStreamWriter(ChannelWriter<T> writer)
        {
           this.Writer = writer ?? throw new ArgumentNullException(nameof(writer));
        }

        /// <summary>
        /// Gets the <see cref="ChannelWriter{T}"/> used to write data to the stream
        /// </summary>
        protected ChannelWriter<T> Writer { get; }

        /// <inheritdoc/>
        public virtual async ValueTask WriteAsync(T data, CancellationToken cancellationToken = default)
        {
            if(data == null)
                throw new ArgumentNullException(nameof(data));
            await this.Writer.WriteAsync(data, cancellationToken);
        }

    }

}
