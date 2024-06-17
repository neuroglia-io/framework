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
/// Represents a reflection based <see cref="IPatchHandler"/> implementation used to handle <see href="https://www.rfc-editor.org/rfc/rfc6902">Json Patches</see>
/// </summary>
public class ReflectionBasedJsonPatchHandler
    : IPatchHandler
{

    /// <summary>
    /// Initializes a new <see cref="ReflectionBasedJsonPatchHandler"/>
    /// </summary>
    /// <param name="serviceProvider">The current <see cref="IServiceProvider"/></param>
    public ReflectionBasedJsonPatchHandler(IServiceProvider serviceProvider) { this.ServiceProvider = serviceProvider;  }

    /// <summary>
    /// Gets the current <see cref="IServiceProvider"/>
    /// </summary>
    protected IServiceProvider ServiceProvider { get; }

    /// <inheritdoc/>
    public virtual bool Supports(string type) => type == PatchType.JsonPatch;

    /// <inheritdoc/>
    public virtual async Task<T?> ApplyPatchAsync<T>(object patch, T? target, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(patch);
        if (target == null) return default;

        var jsonPatch = JsonSerializer.Default.Deserialize<JsonPatch>(JsonSerializer.Default.SerializeToText(patch))!;
        var typeInfo = JsonPatchTypeInfo.GetOrCreate<T>();

        foreach(var operation in jsonPatch.Operations)
        {
            var property = typeInfo.GetProperty(operation.Path) ?? throw new NullReferenceException($"Failed to find a property at path '{operation.Path}'");
            var operationDelegate = operation.Op switch
            {
                OperationType.Add => property.AddAsync,
                OperationType.Copy => property.CopyAsync,
                OperationType.Move => property.MoveAsync,
                OperationType.Remove => property.RemoveAsync,
                OperationType.Replace => property.ReplaceAsync,
                OperationType.Test => property.TestAsync,
                _ => throw new NotSupportedException($"The specified {nameof(OperationType)} '{operation.Op}' is not supported")
            } ?? throw new NotSupportedException($"The specified JSON patch operation '{operation.Op}' is not supported for property at path '{operation.Path}'");
            await operationDelegate(this.ServiceProvider, target, operation, cancellationToken).ConfigureAwait(false);
        }

        return target;
    }

}