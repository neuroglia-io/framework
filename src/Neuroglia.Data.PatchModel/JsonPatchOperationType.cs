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

namespace Neuroglia.Data.PatchModel;

/// <summary>
/// Enumerates all JSON Patch operation type flags
/// </summary>
[Flags]
public enum JsonPatchOperationType
{
    /// <summary>
    /// Indicates an 'add' operation
    /// </summary>
    Add = 1,
    /// <summary>
    /// Indicates a 'copy' operation
    /// </summary>
    Copy = 2,
    /// <summary>
    /// Indicates a 'move' operation
    /// </summary>
    Move = 4,
    /// <summary>
    /// Indicates a 'remove' operation
    /// </summary>
    Remove = 8,
    /// <summary>
    /// Indicates a 'replace' operation
    /// </summary>
    Replace = 16,
    /// <summary>
    /// Indicates a 'test' operation
    /// </summary>
    Test = 32
}