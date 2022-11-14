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

namespace Neuroglia
{

    /// <summary>
    /// Represents the <see cref="Exception"/> thrown when a concurrency error occurs
    /// </summary>
    public class ConcurrencyException
         : Exception
    {

        /// <summary>
        /// Initializes a new <see cref="ConcurrencyException"/>
        /// </summary>
        public ConcurrencyException()
        {

        }

        /// <summary>
        /// Initializes a new <see cref="ConcurrencyException"/>
        /// </summary>
        /// <param name="message">The exception message</param>
        public ConcurrencyException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new <see cref="ConcurrencyException"/>
        /// </summary>
        /// <param name="message">The exception message</param>
        /// <param name="innerException">The inner exception</param>
        public ConcurrencyException(string message, Exception innerException)
            : base(message, innerException)
        {

        }

    }

}
