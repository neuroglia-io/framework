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
using System.Threading;
using System.Threading.Tasks;

namespace Neuroglia
{

    /// <summary>
    /// Defines the fundamentals of a service used to read data from an <see cref="IAsyncEnumerable{T}"/>
    /// </summary>
    /// <typeparam name="T">The type of data to write</typeparam>
    public interface IAsyncStreamWriter<in T>
    {

        /// <summary>
        /// Writes a messages
        /// </summary>
        /// <param name="data">The data to write</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A new awaitable <see cref="ValueTask"/></returns>
        ValueTask WriteAsync(T data, CancellationToken cancellationToken = default);

    }

}
