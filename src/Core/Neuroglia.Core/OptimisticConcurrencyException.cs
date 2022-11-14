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
    /// Represents the <see cref="Exception"/> thrown when an optimistic concurrency error occurs
    /// </summary>
    public class OptimisticConcurrencyException
        : ConcurrencyException
    {

        /// <summary>
        /// Initializes a new <see cref="OptimisticConcurrencyException"/>
        /// </summary>
        public OptimisticConcurrencyException()
        {

        }

        /// <summary>
        /// Initializes a new <see cref="OptimisticConcurrencyException"/>
        /// </summary>
        /// <param name="actualVersion">The expected version</param>
        /// <param name="expectedVersion">The actual version</param>
        public OptimisticConcurrencyException(long? expectedVersion, long? actualVersion)
        {
            this.ExpectedVersion = expectedVersion;
            this.ActualVersion = actualVersion;
        }

        /// <summary>
        /// Initializes a new <see cref="OptimisticConcurrencyException"/>
        /// </summary>
        /// <param name="message">The exception message</param>
        public OptimisticConcurrencyException(string message)
            : base(message)
        {

        }

        /// <summary>
        /// Initializes a new <see cref="OptimisticConcurrencyException"/>
        /// </summary>
        /// <param name="message">The exception message</param>
        /// <param name="actualVersion">The expected version</param>
        /// <param name="expectedVersion">The actual version</param>
        public OptimisticConcurrencyException(string message, long? expectedVersion, long? actualVersion)
            : base(message)
        {
            this.ExpectedVersion = expectedVersion;
            this.ActualVersion = actualVersion;
        }

        /// <summary>
        /// Initializes a new <see cref="OptimisticConcurrencyException"/>
        /// </summary>
        /// <param name="message">The exception message</param>
        /// <param name="innerException">The inner exception</param>
        public OptimisticConcurrencyException(string message, Exception innerException)
            : base(message, innerException)
        {

        }

        /// <summary>
        /// Initializes a new <see cref="OptimisticConcurrencyException"/>
        /// </summary>
        /// <param name="message">The exception message</param>
        /// <param name="innerException">The inner exception</param>
        /// <param name="actualVersion">The expected version</param>
        /// <param name="expectedVersion">The actual version</param>
        public OptimisticConcurrencyException(string message, Exception innerException, long expectedVersion, long actualVersion)
            : base(message, innerException)
        {
            this.ExpectedVersion = expectedVersion;
            this.ActualVersion = actualVersion;
        }

        /// <summary>
        /// Gets the expected version
        /// </summary>
        public virtual long? ExpectedVersion { get; protected set; }

        /// <summary>
        /// Gets the actual version
        /// </summary>
        public virtual long? ActualVersion { get; protected set; }

    }

}
