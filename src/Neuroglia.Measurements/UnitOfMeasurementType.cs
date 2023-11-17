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

using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Neuroglia.Measurements;

/// <summary>
/// Enumerates all types of units of measurement
/// </summary>
[TypeConverter(typeof(EnumMemberTypeConverter))]
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum UnitOfMeasurementType
{
    /// <summary>
    /// Indicates a unit of measurement used to measure capacity
    /// </summary>
    [EnumMember(Value = "capacity")]
    Capacity = 1,
    /// <summary>
    /// Indicates a unit of measurement used to measure energy
    /// </summary>
    [EnumMember(Value = "energy")]
    Energy = 2,
    /// <summary>
    /// Indicates a unit of measurement used to measure a distance between two points
    /// </summary>
    [EnumMember(Value = "length")]
    Length = 4,
    /// <summary>
    /// Indicates a unit of measurement used to measure weight
    /// </summary>
    [EnumMember(Value = "mass")]
    Mass = 8,
    /// <summary>
    /// Indicates a unit of measurement used to measure surfaces
    /// </summary>
    [EnumMember(Value = "surface")]
    Surface = 16,
    /// <summary>
    /// Indicates a unit of measurement used to measure unfractable units
    /// </summary>
    [EnumMember(Value = "unit")]
    Unit = 32,
    /// <summary>
    /// Indicates a unit of measurement used to measure volumes
    /// </summary>
    [EnumMember(Value = "volume")]
    Volume = 64,
    /// <summary>
    /// Indicates a unit of measurement used to measure temperatures
    /// </summary>
    [EnumMember(Value = "temperature")]
    Temperature = 128
}