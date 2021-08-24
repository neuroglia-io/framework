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
    /// Defines the fundamentals of a service used to describe a Json Patch operation
    /// </summary>
    public interface IJsonPatchOperationMetadata
    {

        /// <summary>
        /// Gets the Json Patch operation type
        /// </summary>
        string Type { get; }

        /// <summary>
        /// Gets the Json Patch operation path
        /// </summary>
        string Path { get; }

        /// <summary>
        /// Gets the type of the object referenced by the Json Patch value and that is expected by the operation's setter
        /// </summary>
        Type ReferencedType { get; }

        /// <summary>
        /// Executes the Json Patch operation
        /// </summary>
        /// <param name="target">The operation's target</param>
        /// <param name="value">The operation's value</param>
        void ApplyTo(object target, object value);

    }

}
