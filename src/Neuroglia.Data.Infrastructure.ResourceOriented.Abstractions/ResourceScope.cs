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
/// Enumerates all supported resource scopes
/// </summary>
[TypeConverter(typeof(EnumMemberTypeConverter))]
[JsonConverter(typeof(StringEnumConverter))]
public enum ResourceScope
{
    /// <summary>
    /// Indicates a namespaced resource
    /// </summary>
    [EnumMember(Value = "Namespaced")]
    Namespaced = 1,
    /// <summary>
    /// Indicates a cluster resource
    /// </summary>
    [EnumMember(Value = "Cluster")]
    Cluster = 2
}
