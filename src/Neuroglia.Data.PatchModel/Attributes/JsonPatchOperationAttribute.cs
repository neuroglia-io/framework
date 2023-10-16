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

namespace Neuroglia.Data.PatchModel.Attributes;

/// <summary>
/// Represents an attribute used to marked methods as Json Patch reducers
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class JsonPatchOperationAttribute
    : Attribute
{

    /// <summary>
    /// Initializes a new <see cref="JsonPatchOperationAttribute"/>
    /// </summary>
    /// <param name="type">The type of the supported Json Patch operation</param>
    /// <param name="path">The supported Json Patch path</param>
    public JsonPatchOperationAttribute(string type, string path)
    {
        if (string.IsNullOrWhiteSpace(type)) throw new ArgumentNullException(nameof(type));
        if (string.IsNullOrWhiteSpace(path)) throw new ArgumentNullException(nameof(path));
        this.Type = type;
        this.Path = path;
    }

    /// <summary>
    /// Initializes a new <see cref="JsonPatchOperationAttribute"/>
    /// </summary>
    /// <param name="type">The type of the supported Json Patch operation</param>
    /// <param name="path">The supported Json Patch path</param>
    public JsonPatchOperationAttribute(OperationType type, string path) : this(type.ToString().ToLower(), path) { }

    /// <summary>
    /// Gets the type of the supported Json Patch operation
    /// </summary>
    public virtual string Type { get; }

    /// <summary>
    /// Gets the supported Json Patch path
    /// </summary>
    public virtual string Path { get; }

    /// <summary>
    /// Gets the type of object referenced by the supplied Json Patch value, if any
    /// </summary>
    public virtual Type? ReferencedType { get; init; }

}
