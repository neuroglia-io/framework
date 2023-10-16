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

using Neuroglia.Serialization.Json;

namespace Neuroglia.Data.PatchModel.Services;

/// <summary>
/// Represents the <see cref="IPatchHandler"/> implementation used to handle Json Merge Patches
/// </summary>
public class JsonMergePatchHandler
    : IPatchHandler
{

    /// <inheritdoc/>
    public virtual bool Supports(string type) => type == PatchType.JsonMergePatch;

    /// <inheritdoc/>
    public virtual Task<T?> ApplyPatchAsync<T>(object patch, T? target, CancellationToken cancellationToken = default)
    {
        var targetElement = JsonSerializer.Default.SerializeToElement((object?)target)!.Value;
        var patchElement = JsonSerializer.Default.SerializeToElement(patch)!.Value;
        var updatedDocument = JsonCons.Utilities.JsonMergePatch.ApplyMergePatch(targetElement, patchElement);
        return Task.FromResult(JsonSerializer.Default.Deserialize<T?>(JsonSerializer.Default.SerializeToNode(updatedDocument)!));
    }

}
