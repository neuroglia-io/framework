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
    /// Defines the fundamentals of a service used to read an <see cref="IAsyncEnumerable{T}"/>
    /// </summary>
    /// <typeparam name="T">The type of data to read</typeparam>
    public interface IAsyncStreamReader<out T>
    {

        /// <summary>
        /// Gets the current message
        /// </summary>
        T? Current { get; }

        /// <summary>
        /// Attempts to read the next message on the stream
        /// </summary>
        /// <returns>A new boolean indicating whether or not a message could be read from the stream. If false, the reader has reached the end of the stream.</returns>
        ValueTask<bool> MoveNextAsync();

    }

}
