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

namespace Neuroglia.Data
{
    /// <summary>
    /// Represents a <see cref="DomainException"/> thrown whenever the validation of an entity has failed
    /// </summary>
    public class DomainValidationException
        : DomainException
    {

        /// <summary>
        /// Initializes a new <see cref="DomainValidationException"/>
        /// </summary>
        /// <param name="errors">An array containing the <see cref="Error"/>s that describe the validation failures</param>
        public DomainValidationException(params Error[] errors)
        {
            this.ValidationErrors = errors;
        }

        /// <summary>
        /// Gets an <see cref="IReadOnlyCollection{T}"/> containing the <see cref="Error"/>s that describe the validation failures
        /// </summary>
        public IReadOnlyCollection<Error> ValidationErrors { get; }

    }

}
