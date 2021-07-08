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
using Microsoft.AspNetCore.JsonPatch;

namespace Neuroglia.Data
{
    /// <summary>
    /// Defines the fundamentals of an object that allows its status to be patched
    /// </summary>
    public interface IStatusPatchable
        : IPatchable
    {

        /// <summary>
        /// Attempts to get the current status <see cref="JsonPatchDocument"/>
        /// </summary>
        /// <param name="patch">The current status <see cref="JsonPatchDocument"/>, if any</param>
        /// <returns>A boolean indicating whether or not the <see cref="IPatchable"/> has a pending status patch</returns>
        bool TryGetStatusPatch(out JsonPatchDocument patch);

    }

}
