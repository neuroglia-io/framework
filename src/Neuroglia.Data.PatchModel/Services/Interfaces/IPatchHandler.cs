// Copyright © 2021-Present Neuroglia SRL. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License"),
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Neuroglia.Data.PatchModel.Services;

/// <summary>
/// Defines the fundamentals of a service used to apply patches of a specific type
/// </summary>
public interface IPatchHandler
{

    /// <summary>
    /// Indicates whether or not the handler supports the specified patch type
    /// </summary>
    /// <param name="type">The type of patch to handle</param>
    /// <returns>A boolean indicating whether or not the handler supports the specified patch type</returns>
    bool Supports(string type);

    /// <summary>
    /// Applies a <see cref="Patch"/> to the specified target
    /// </summary>
    /// <typeparam name="T">The type of the object to apply the patch to</typeparam>
    /// <param name="patch">The <see cref="Patch"/> to apply</param>
    /// <param name="target">The object to apply the patch to</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The patched target</returns>
    Task<T?> ApplyPatchAsync<T>(object patch, T? target, CancellationToken cancellationToken = default);

}