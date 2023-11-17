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

using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Neuroglia.Measurements;

/// <summary>
/// Represents the measurement of an amount of unfractable units
/// </summary>
[DataContract]
public class Unit
    : Measurement, IComparable<Unit>
{

    /// <summary>
    /// Gets the <see cref="Unit"/>'s rounding precision
    /// </summary>
    public const ushort RoundingPrecision = 2;

    /// <summary>
    /// Initializes a new <see cref="Unit"/>
    /// </summary>
    [JsonConstructor]
    protected Unit() { }

    /// <summary>
    /// Initializes a new <see cref="Unit"/>
    /// </summary>
    /// <param name="value">The measured value</param>
    /// <param name="unit">The unit the measure's value is expressed in</param>
    public Unit(decimal value, UnitOfMeasurement unit) : base(value, unit == null ? throw new ArgumentNullException(nameof(unit)) : unit.Type == UnitOfMeasurementType.Unit ? unit : throw new ArgumentException("The specified unit must be of type 'unit'", nameof(unit))) { }

    /// <summary>
    /// Gets the total amount of units
    /// </summary>
    [IgnoreDataMember, JsonIgnore]
    public virtual decimal TotalUnits => this.Value * (Units.Unit.Ratio / this.Unit.Ratio);

    /// <summary>
    /// Gets the total amount of pairs
    /// </summary>
    [IgnoreDataMember, JsonIgnore]
    public virtual decimal Pairs => this.Value * (Units.Pair.Ratio / this.Unit.Ratio);

    /// <summary>
    /// Gets the total amount of half dozens
    /// </summary>
    [IgnoreDataMember, JsonIgnore]
    public virtual decimal HalfDozens => this.Value * (Units.HalfDozen.Ratio / this.Unit.Ratio);

    /// <summary>
    /// Gets the total amount of dozens
    /// </summary>
    [IgnoreDataMember, JsonIgnore]
    public virtual decimal Dozens => this.Value * (Units.Dozen.Ratio / this.Unit.Ratio);

    /// <summary>
    /// Adds the specified <see cref="Unit"/>
    /// </summary>
    /// <param name="unit">The <see cref="Unit"/> to add</param>
    /// <returns>A new <see cref="Unit"/> resulting from the addition</returns>
    public virtual Unit Add(Unit unit)
    {
        if (unit == null) throw new ArgumentNullException(nameof(unit));
        if (unit.Unit.Type != this.Unit.Type) throw new ArgumentNullException(nameof(unit), "Cannot compare units with different types of unit");
        return new Unit(Math.Round(this.Value + unit.Value * (this.Unit.Ratio / unit.Unit.Ratio), RoundingPrecision), this.Unit);
    }

    /// <summary>
    /// Subtracts the specified <see cref="Unit"/>
    /// </summary>
    /// <param name="unit">The <see cref="Unit"/> to subtract</param>
    /// <returns>A new <see cref="Unit"/> resulting from the subtraction</returns>
    public virtual Unit Subtract(Unit unit)
    {
        if (unit == null) throw new ArgumentNullException(nameof(unit));
        if (unit.Unit.Type != this.Unit.Type) throw new ArgumentNullException(nameof(unit), "Cannot compare units with different types of unit");
        return new Unit(Math.Round(this.Value - unit.Value * (this.Unit.Ratio / unit.Unit.Ratio), RoundingPrecision), this.Unit);
    }

    /// <inheritdoc/>
    public virtual int CompareTo(Unit? other) => this.TotalUnits.CompareTo(other?.TotalUnits);

    /// <summary>
    /// Converts the <see cref="Unit"/> into a new <see cref="Unit"/> measured using the specified <see cref="UnitOfMeasurement"/>
    /// </summary>
    /// <param name="unit">The <see cref="UnitOfMeasurement"/> to convert the <see cref="Unit"/> to</param>
    /// <returns>A new <see cref="Unit"/> measured using the specified <see cref="UnitOfMeasurement"/></returns>
    public new virtual Unit ConvertTo(UnitOfMeasurement unit)
    {
        if (unit == null) throw new ArgumentNullException(nameof(unit));
        if (unit.Type != UnitOfMeasurementType.Unit) throw new ArgumentException("The specified unit of measurement must be of type 'unit'", nameof(unit));
        return new(this.Value * (unit.Ratio / this.Unit.Ratio), unit);
    }

    /// <summary>
    /// Creates a new <see cref="Unit"/> from the specified value, expressed in units
    /// </summary>
    /// <param name="value">The amount of units to create a new <see cref="Unit"/> for</param>
    /// <returns>A new <see cref="Unit"/></returns>
    public static Unit FromUnits(decimal value) => new(value, Units.Unit);

    /// <summary>
    /// Creates a new <see cref="Unit"/> from the specified value, expressed in pairs
    /// </summary>
    /// <param name="value">The amount of pairs to create a new <see cref="Unit"/> for</param>
    /// <returns>A new <see cref="Unit"/></returns>
    public static Unit FromPairs(decimal value) => new(value, Units.Pair);

    /// <summary>
    /// Creates a new <see cref="Unit"/> from the specified value, expressed in half dozens
    /// </summary>
    /// <param name="value">The amount of half dozens to create a new <see cref="Unit"/> for</param>
    /// <returns>A new <see cref="Unit"/></returns>
    public static Unit FromHalfDozens(decimal value) => new(value, Units.HalfDozen);

    /// <summary>
    /// Creates a new <see cref="Unit"/> from the specified value, expressed in dozens
    /// </summary>
    /// <param name="value">The amount of dozens to create a new <see cref="Unit"/> for</param>
    /// <returns>A new <see cref="Unit"/></returns>
    public static Unit FromDozens(decimal value) => new(value, Units.Dozen);

    /// <summary>
    /// Adds the specified <see cref="Unit"/>s
    /// </summary>
    /// <param name="unit1">The <see cref="Unit"/> to add the specified value to</param>
    /// <param name="unit2">The value to add</param>
    /// <returns>The addition's result</returns>
    public static Unit operator +(Unit unit1, Unit unit2) => unit1.Add(unit2);

    /// <summary>
    /// Subtracts the specified <see cref="Unit"/>s
    /// </summary>
    /// <param name="unit1">The <see cref="Unit"/> to subtract the specified value from</param>
    /// <param name="unit2">The value to subtract</param>
    /// <returns>The subtraction's result</returns>
    public static Unit operator -(Unit unit1, Unit unit2) => unit1.Subtract(unit2);

    /// <summary>
    /// Multiplies the specified <see cref="Unit"/>
    /// </summary>
    /// <param name="unit">The <see cref="Unit"/> to multiply</param>
    /// <param name="multiplier">The multiplier</param>
    /// <returns>The multiplication's result</returns>
    public static Unit operator *(Unit unit, decimal multiplier) => new(unit.Value * multiplier, unit.Unit);

    /// <summary>
    /// Multiplies the specified <see cref="Unit"/>
    /// </summary>
    /// <param name="unit">The <see cref="Unit"/> to multiply</param>
    /// <param name="multiplier">The multiplier</param>
    /// <returns>The multiplication's result</returns>
    public static Unit operator *(Unit unit, int multiplier) => new(unit.Value * multiplier, unit.Unit);

    /// <summary>
    /// Divides the specified <see cref="Unit"/>
    /// </summary>
    /// <param name="unit">The <see cref="Unit"/> to divide</param>
    /// <param name="divider">The divider</param>
    /// <returns>The division's result</returns>
    public static Unit operator /(Unit unit, decimal divider) => new(unit.Value / divider, unit.Unit);

    /// <summary>
    /// Divides the specified <see cref="Unit"/>
    /// </summary>
    /// <param name="unit">The <see cref="Unit"/> to divide</param>
    /// <param name="divider">The divider</param>
    /// <returns>The division's result</returns>
    public static Unit operator /(Unit unit, int divider) => new(unit.Value / divider, unit.Unit);

    /// <summary>
    /// Checks whether or not a <see cref="Unit"/> is lower than another one
    /// </summary>
    /// <param name="unit1">The <see cref="Unit"/> to check</param>
    /// <param name="unit2">The <see cref="Unit"/> to compare to</param>
    /// <returns>A boolean indicating whether or not a <see cref="Unit"/> is lower than another one</returns>
    public static bool operator <(Unit unit1, Unit unit2) => unit1.CompareTo(unit2) < 0;

    /// <summary>
    /// Checks whether or not a <see cref="Unit"/> is higher than another one
    /// </summary>
    /// <param name="unit1">The <see cref="Unit"/> to check</param>
    /// <param name="unit2">The <see cref="Unit"/> to compare to</param>
    /// <returns>A boolean indicating whether or not a <see cref="Unit"/> is higher than another one</returns>
    public static bool operator >(Unit unit1, Unit unit2) => unit1.CompareTo(unit2) > 0;

    /// <summary>
    /// Exposes default unit units
    /// </summary>
    public static class Units
    {

        /// <summary>
        /// Gets the <see cref="UnitOfMeasurement"/> to express unfractable units
        /// </summary>
        public static readonly UnitOfMeasurement Unit = new(UnitOfMeasurementType.Unit, "Unit", "u", 1m);
        /// <summary>
        /// Gets the <see cref="UnitOfMeasurement"/> to express pairs
        /// </summary>
        public static readonly UnitOfMeasurement Pair = new(UnitOfMeasurementType.Unit, "Pair", "pr", 0.5m);
        /// <summary>
        /// Gets the <see cref="UnitOfMeasurement"/> to express half dozens
        /// </summary>
        public static readonly UnitOfMeasurement HalfDozen = new(UnitOfMeasurementType.Unit, "Half dozen", "doz", 1m / 6m);
        /// <summary>
        /// Gets the <see cref="UnitOfMeasurement"/> to express dozens
        /// </summary>
        public static readonly UnitOfMeasurement Dozen = new(UnitOfMeasurementType.Unit, "Half dozen", "hdoz", 1m / 12m);

        /// <summary>
        /// Gets a new <see cref="IEnumerable{T}"/> containing all default unit units
        /// </summary>
        /// <returns>A new <see cref="IEnumerable{T}"/></returns>
        public static IEnumerable<UnitOfMeasurement> AsEnumerable()
        {
            yield return Unit;
            yield return Pair;
            yield return HalfDozen;
            yield return Dozen;
        }

    }

}