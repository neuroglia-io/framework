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
using Microsoft.AspNetCore.JsonPatch.Operations;
using System;

namespace Microsoft.AspNetCore.JsonPatch
{
    /// <summary>
    /// Defines extensions for <see cref="IJsonPatchTypeMetadata"/>
    /// </summary>
    public static class JsonPatchTypeMetadataExtensions
    {

        /// <summary>
        /// Attempts to get the metadata of the specified Json Patch operation
        /// </summary>
        /// <param name="typeMetadata">The extended <see cref="IJsonPatchTypeMetadata"/></param>
        /// <param name="operation">The Json Patch operation</param>
        /// <param name="operationMetadata">The matching <see cref="IJsonPatchOperationMetadata"/>, of any</param>
        /// <returns>A boolean indicating whether or not the metadata for the specified Json Patch operation could be found</returns>
        public static bool TryGetOperationMetadata(this IJsonPatchTypeMetadata typeMetadata, Operation operation, out IJsonPatchOperationMetadata operationMetadata)
        {
            if(typeMetadata == null)
                throw new ArgumentNullException(nameof(typeMetadata));
            if (operation == null)
                throw new ArgumentNullException(nameof(operation));
            return typeMetadata.TryGetOperationMetadata(operation.op, operation.path, out operationMetadata);
        }

    }

}
