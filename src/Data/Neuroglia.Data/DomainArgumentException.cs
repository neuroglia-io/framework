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

namespace Neuroglia.Data
{
    /// <summary>
    /// Represents an argument-related <see cref="DomainException"/>
    /// </summary>
    public class DomainArgumentException
       : DomainException
    {

        /// <summary>
        /// Initializes a new <see cref="DomainArgumentException"/>
        /// </summary>
        /// <param name="message">The exception message</param>
        /// <param name="argumentName">The argument name</param>
        public DomainArgumentException(string message, string argumentName)
            : base(message)
        {
            this.ArgumentName = argumentName;
        }

        /// <summary>
        /// Initializes a new <see cref="DomainArgumentException"/>
        /// </summary>
        /// <param name="message">The exception message</param>
        /// <param name="argumentName">The argument name</param>
        /// <param name="innerException">The inner exception, if any</param>
        public DomainArgumentException(string message, string argumentName, Exception innerException)
            : base(message, innerException)
        {
            this.ArgumentName = argumentName;
        }

        /// <summary>
        /// Gets the name of the argument at the origin of the exception
        /// </summary>
        public string ArgumentName { get; private set; }

    }

}
