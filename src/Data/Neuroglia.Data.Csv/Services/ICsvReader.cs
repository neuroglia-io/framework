using System.Data;
using System.IO;
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
using System.Threading;
using System.Threading.Tasks;

namespace Neuroglia.Data.Services
{

    /// <summary>
    /// Defines the fundamentals of a service used to read Comma Separated Values (CSV)
    /// </summary>
    public interface ICsvReader
    {

        /// <summary>
        /// Reads a new <see cref="DataTable"/> from the specified <see cref="Stream"/>
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to read a new <see cref="DataTable"/> from</param>
        /// <param name="options">The options used to configure the <see cref="ICsvReader"/></param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A new <see cref="DataTable"/></returns>
        Task<DataTable> ReadFromAsync(Stream stream, ICsvDocumentOptions options, CancellationToken cancellationToken = default);

    }

}
