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

namespace Neuroglia.Data.PatchModel;

/// <summary>
/// Represents an object used to describe a JSON patchable property
/// </summary>
public class JsonPatchPropertyInfo
{

    /// <summary>
    /// Initializes a new <see cref="JsonPatchPropertyInfo"/>
    /// </summary>
    /// <param name="path">The path to the described property</param>
    /// <param name="type">The type of the described property</param>
    /// <param name="getValueDelegate"></param>
    public JsonPatchPropertyInfo(string path, Type type, Func<IServiceProvider, object, CancellationToken, Task<object?>> getValueDelegate)
    {
        this.Path = path;
        this.Type = type;
        this.GetValueAsync = getValueDelegate;
    }

    /// <summary>
    /// Gets the path to the described property
    /// </summary>
    public virtual string Path { get; protected set; }

    /// <summary>
    /// Gets the type of the described property
    /// </summary>
    public virtual Type Type { get; protected set; }

    /// <summary>
    /// Gets the delegate <see cref="Func{T1, T2, T3, TResult}"/> used to get the value of the describe property
    /// </summary>
    public virtual Func<IServiceProvider, object, CancellationToken, Task<object?>> GetValueAsync { get; protected set; }

    /// <summary>
    /// Gets the delegate <see cref="Func{T1, T2, T3, TResult}"/> used to handle JSON Patch operations of type <see cref="OperationType.Add"/>, if any
    /// </summary>
    public virtual Func<IServiceProvider, object, PatchOperation, CancellationToken, Task>? AddAsync { get; set; }

    /// <summary>
    /// Gets the delegate <see cref="Func{T1, T2, T3, TResult}"/> used to handle JSON Patch operations of type <see cref="OperationType.Copy"/>, if any
    /// </summary>
    public virtual Func<IServiceProvider, object, PatchOperation, CancellationToken, Task>? CopyAsync { get; set; }

    /// <summary>
    /// Gets the delegate <see cref="Func{T1, T2, T3, TResult}"/> used to handle JSON Patch operations of type <see cref="OperationType.Move"/>, if any
    /// </summary>
    public virtual Func<IServiceProvider, object, PatchOperation, CancellationToken, Task>? MoveAsync { get; set; }

    /// <summary>
    /// Gets the delegate <see cref="Func{T1, T2, T3, TResult}"/> used to handle JSON Patch operations of type <see cref="OperationType.Remove"/>, if any
    /// </summary>
    public virtual Func<IServiceProvider, object, PatchOperation, CancellationToken, Task>? RemoveAsync { get; set; }

    /// <summary>
    /// Gets the delegate <see cref="Func{T1, T2, T3, TResult}"/> used to handle JSON Patch operations of type <see cref="OperationType.Replace"/>, if any
    /// </summary>
    public virtual Func<IServiceProvider, object, PatchOperation, CancellationToken, Task>? ReplaceAsync { get; set; }

    /// <summary>
    /// Gets the delegate <see cref="Func{T1, T2, T3, TResult}"/> used to handle JSON Patch operations of type <see cref="OperationType.Test"/>, if any
    /// </summary>
    public virtual Func<IServiceProvider, object, PatchOperation, CancellationToken, Task>? TestAsync { get; set; }

    /// <inheritdoc/>
    public override string ToString() => this.Path;

}
