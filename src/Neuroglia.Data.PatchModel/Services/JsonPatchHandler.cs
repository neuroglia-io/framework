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

using Json.Patch;
using Neuroglia.Serialization.Json;

namespace Neuroglia.Data.PatchModel.Services;

/// <summary>
/// Represents the <see cref="IPatchHandler"/> implementation used to handle Json Patches
/// </summary>
public class JsonPatchHandler
    : IPatchHandler
{

    /// <inheritdoc/>
    public virtual bool Supports(string type) => type == PatchType.JsonPatch;

    /// <inheritdoc/>
    public virtual Task<T?> ApplyPatchAsync<T>(object patch, T? target, CancellationToken cancellationToken = default)
    {
        var jsonPatch = JsonSerializer.Default.Deserialize<JsonPatch>(JsonSerializer.Default.SerializeToText(patch))!;
        return Task.FromResult(JsonSerializer.Default.Deserialize<T>(jsonPatch.Apply(JsonSerializer.Default.SerializeToNode((object?)target)).Result!));
    }

}
