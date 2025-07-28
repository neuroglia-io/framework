﻿// Copyright © 2021-Present Neuroglia SRL. All rights reserved.
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
/// Enumerates all default types of resource-related event
/// </summary>
[TypeConverter(typeof(EnumMemberTypeConverter))]
[JsonConverter(typeof(StringEnumConverter))]
public enum ResourceWatchEventType
{
    /// <summary>
    /// Indicates an event that describes the creation of a resource
    /// </summary>
    [EnumMember(Value = "created")]
    Created = 0,
    /// <summary>
    /// Indicates an event that describes the update of a resource
    /// </summary>
    [EnumMember(Value = "updated")]
    Updated = 1,
    /// <summary>
    /// Indicates an event that describes the deletion of a resource
    /// </summary>
    [EnumMember(Value = "deleted")]
    Deleted = 2,
    /// <summary>
    /// Indicates an event that describes a resource-related error
    /// </summary>
    [EnumMember(Value = "error")]
    Error = 4,
    /// <summary>
    /// Indicates an event emitted periodically to update the resource version
    /// </summary>
    [EnumMember(Value = "bookmark")]
    Bookmark = 8
}