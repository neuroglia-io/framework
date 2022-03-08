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

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Neuroglia
{
    /// <summary>
    /// Represents the default implementation of the <see cref="IAsyncStreamReader{T}"/> interface
    /// </summary>
    /// <typeparam name="T">The type of data to read</typeparam>
    public class AsyncStreamReader<T>
        : IAsyncStreamReader<T>
    {

        /// <summary>
        /// Initializes a new <see cref="AsyncStreamReader{T}"/>
        /// </summary>
        /// <param name="stream">The <see cref="IAsyncEnumerable{T}"/> to read</param>
        public AsyncStreamReader(IAsyncEnumerable<T> stream)
        {
            this.Stream = stream ?? throw new ArgumentNullException(nameof(stream));
            this.Reader = this.Stream.GetAsyncEnumerator();
        }

        /// <summary>
        /// Gets the <see cref="IAsyncEnumerable{T}"/> to read
        /// </summary>
        protected IAsyncEnumerable<T> Stream { get; }

        /// <summary>
        /// Gets the <see cref="IAsyncEnumerator{T}"/> used to enumerate the <see cref="IAsyncEnumerable{T}"/> to read
        /// </summary>
        protected IAsyncEnumerator<T> Reader { get; }

        /// <inheritdoc/>
        public T? Current => this.Reader.Current;

        /// <inheritdoc/>
        public virtual async ValueTask<bool> MoveNextAsync()
        {
            return await this.Reader.MoveNextAsync();
        }

    }

}
