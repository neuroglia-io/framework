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

using Neuroglia.Serialization.Json.Converters;
using System.ComponentModel;

namespace Neuroglia.Data.Infrastructure.ResourceOriented;

/// <summary>
/// Enumerates all default types of resource-related operations
/// </summary>
[TypeConverter(typeof(EnumMemberTypeConverter))]
[JsonConverter(typeof(StringEnumConverter))]
public enum Operation
{
    /// <summary>
    /// Indicates the operation to create a new resource
    /// </summary>
    [EnumMember(Value = "create")]
    Create = 1,
    /// <summary>
    /// Indicates the operation to replace an existing resource
    /// </summary>
    [EnumMember(Value = "replace")]
    Replace = 2,
    /// <summary>
    /// Indicates the operation to patch an existing resource
    /// </summary>
    [EnumMember(Value = "patch")]
    Patch = 4,
    /// <summary>
    /// Indicates the operation to delete an existing resource
    /// </summary>
    [EnumMember(Value = "delete")]
    Delete = 8
}