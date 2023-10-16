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

namespace Neuroglia.Data;

/// <summary>
/// Exposes methods to help handling <see cref="JsonPatch"/>es
/// </summary>
public static class JsonPatchUtility
{

    /// <summary>
    /// Creates a new <see cref="JsonPatch"/> based on the differences between the specified values
    /// </summary>
    /// <param name="source">The source object</param>
    /// <param name="target">The target object</param>
    /// <returns>A new <see cref="JsonPatch"/> based on the differences between the specified values</returns>
    public static JsonPatch CreateJsonPatchFromDiff<T>(object? source, object? target)
    {
        source ??= new();
        target ??= new();
        var sourceToken = JsonSerializer.Default.SerializeToElement(source)!.Value;
        var targetToken = JsonSerializer.Default.SerializeToElement(target)!.Value;
        var patchDocument = JsonCons.Utilities.JsonPatch.FromDiff(sourceToken, targetToken);
        return JsonSerializer.Default.Deserialize<JsonPatch>(patchDocument.RootElement)!;
    }

}