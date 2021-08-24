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

namespace Microsoft.AspNetCore.JsonPatch
{
    /// <summary>
    /// Defines the fundamentals of an object used to describe the Json Patch capabilities of a given type
    /// </summary>
    public interface IJsonPatchTypeMetadata
    {

        /// <summary>
        /// Gets the described type
        /// </summary>
        Type Type { get; }

        /// <summary>
        /// Attempts to get the metadata of the specified Json Patch operation
        /// </summary>
        /// <param name="type">The Json Patch operation type</param>
        /// <param name="path">The Json Patch operation path</param>
        /// <param name="operationMetadata">The matching <see cref="IJsonPatchOperationMetadata"/>, of any</param>
        /// <returns>A boolean indicating whether or not the metadata for the specified Json Patch operation could be found</returns>
        bool TryGetOperationMetadata(string type, string path, out IJsonPatchOperationMetadata operationMetadata);

    }

}
