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

using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Neuroglia.Measurements;

/// <summary>
/// Represents a unit of measurement
/// </summary>
[DataContract]
public class UnitOfMeasurement
    : IEquatable<UnitOfMeasurement>
{

    /// <summary>
    /// Initializes a new <see cref="UnitOfMeasurement"/>
    /// </summary>
    [JsonConstructor]
    protected UnitOfMeasurement() { }

    /// <summary>
    /// Initializes a new <see cref="UnitOfMeasurement"/>
    /// </summary>
    /// <param name="type">The unit of measure's type</param>
    /// <param name="name">The unit of measure's name</param>
    /// <param name="symbol">The unit of measure's symbol</param>
    /// <param name="ratio">The unit of measure's ratio, compared to the reference unit</param>
    /// <exception cref="ArgumentNullException"></exception>
    public UnitOfMeasurement(UnitOfMeasurementType type, string name, string symbol, decimal ratio)
    {
        if ((int)type < 1) throw new ArgumentNullException(nameof(type));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
        if (string.IsNullOrWhiteSpace(symbol)) throw new ArgumentNullException(nameof(symbol));
        if (ratio <= 0) throw new ArgumentOutOfRangeException(nameof(ratio));
        this.Type = type;
        this.Name = name;
        this.Symbol = symbol;
        this.Ratio = ratio;
    }

    /// <summary>
    /// Gets/sets the unit of measure's type
    /// </summary>
    [DataMember]
    public virtual UnitOfMeasurementType Type { get; set; }

    /// <summary>
    /// Gets/sets the unit of measure's name
    /// </summary>
    [DataMember]
    public virtual string Name { get; set; } = null!;

    /// <summary>
    /// Gets/sets the unit of measure's symbol
    /// </summary>
    [DataMember]
    public virtual string Symbol { get; set; } = null!;

    /// <summary>
    /// Gets/sets the unit of measure's ratio, compared to the reference unit
    /// </summary>
    [DataMember]
    public virtual decimal Ratio { get; set; }

    /// <inheritdoc/>
    public virtual bool Equals(UnitOfMeasurement? other) => other?.Type == this.Type && other?.Symbol == this.Symbol;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => this.Equals(obj as UnitOfMeasurement);

    /// <inheritdoc/>
    public override int GetHashCode() => this.Symbol.GetHashCode();

    /// <inheritdoc/>
    public override string ToString() => this.Symbol;


}
