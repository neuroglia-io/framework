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
using System.Linq;

namespace Microsoft.AspNetCore.JsonPatch
{

    /// <summary>
    /// Represents the default implementation of the <see cref="IJsonPatchTypeMetadata"/> interface
    /// </summary>
    public class JsonPatchTypeMetadata
        : IJsonPatchTypeMetadata
    {

        /// <summary>
        /// Initializes a new <see cref="JsonPatchTypeMetadata"/>
        /// </summary>
        /// <param name="type">The type to describe</param>
        /// <param name="operations">An <see cref="IEnumerable{T}"/> containing the type's Json Patch operations</param>
        public JsonPatchTypeMetadata(Type type, IEnumerable<IJsonPatchOperationMetadata> operations)
        {
            if (operations == null)
                throw new ArgumentNullException(nameof(operations));
            this.Type = type ?? throw new ArgumentNullException(nameof(type));
            this.Operations = operations.ToList().AsReadOnly();
        }

        /// <inheritdoc/>
        public virtual Type Type { get; }

        /// <inheritdoc/>
        public virtual IReadOnlyCollection<IJsonPatchOperationMetadata> Operations { get; }

        /// <inheritdoc/>
        public virtual bool TryGetOperationMetadata(string type, string path, out IJsonPatchOperationMetadata operationMetadata)
        {
            if (string.IsNullOrWhiteSpace(type))
                throw new ArgumentNullException(nameof(type));
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));
            string pathSegment = path.Split('/', StringSplitOptions.RemoveEmptyEntries).First();
            operationMetadata = this.Operations
                .FirstOrDefault(o => 
                    o.OperationType.Equals(type, StringComparison.OrdinalIgnoreCase) 
                    && pathSegment.Equals(o.Path, StringComparison.OrdinalIgnoreCase));
            return operationMetadata != null;
        }

    }

}
