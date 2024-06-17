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
/// Represents the measurement of a surface
/// </summary>
[DataContract]
public class Surface
    : Measurement, IComparable<Surface>
{

    /// <summary>
    /// Initializes a new <see cref="Surface"/>
    /// </summary>
    [JsonConstructor]
    protected Surface() { }

    /// <summary>
    /// Initializes a new <see cref="Surface"/>
    /// </summary>
    /// <param name="value">The measured value</param>
    /// <param name="unit">The unit the measure's value is expressed in</param>
    public Surface(decimal value, UnitOfMeasurement unit) : base(value, unit == null ? throw new ArgumentNullException(nameof(unit)) : unit.Type == UnitOfMeasurementType.Surface ? unit : throw new ArgumentException("The specified unit must be of type 'surface'", nameof(unit))) { }

    /// <summary>
    /// Gets the total amount of square millimeters
    /// </summary>
    [IgnoreDataMember, JsonIgnore]
    public virtual decimal SquareMillimeters => this.Value * (Units.SquareMillimeter.Ratio / this.Unit.Ratio);

    /// <summary>
    /// Gets the total amount of square centimeters
    /// </summary>
    [IgnoreDataMember, JsonIgnore]
    public virtual decimal SquareCentimeters => this.Value * (Units.SquareCentimeter.Ratio / this.Unit.Ratio);

    /// <summary>
    /// Gets the total amount of square decimeters
    /// </summary>
    [IgnoreDataMember, JsonIgnore]
    public virtual decimal SquareDecimeters => this.Value * (Units.SquareDecimeter.Ratio / this.Unit.Ratio);

    /// <summary>
    /// Gets the total amount of square meters
    /// </summary>
    [IgnoreDataMember, JsonIgnore]
    public virtual decimal SquareMeters => this.Value * (Units.SquareMeter.Ratio / this.Unit.Ratio);

    /// <summary>
    /// Gets the total amount of square decameters
    /// </summary>
    [IgnoreDataMember, JsonIgnore]
    public virtual decimal SquareDecameters => this.Value * (Units.SquareDecameter.Ratio / this.Unit.Ratio);

    /// <summary>
    /// Gets the total amount of square hectometers
    /// </summary>
    [IgnoreDataMember, JsonIgnore]
    public virtual decimal SquareHectometers => this.Value * (Units.SquareHectometer.Ratio / this.Unit.Ratio);

    /// <summary>
    /// Gets the total amount of square kilometers
    /// </summary>
    [IgnoreDataMember, JsonIgnore]
    public virtual decimal SquareKilometers => this.Value * (Units.SquareKilometer.Ratio / this.Unit.Ratio);

    /// <summary>
    /// Gets the total amount of square inches
    /// </summary>
    [IgnoreDataMember, JsonIgnore]
    public virtual decimal SquareInches => this.Value * (Units.SquareInch.Ratio / this.Unit.Ratio);

    /// <summary>
    /// Gets the total amount of square feet
    /// </summary>
    [IgnoreDataMember, JsonIgnore]
    public virtual decimal SquareFeet => this.Value * (Units.SquareFoot.Ratio / this.Unit.Ratio);

    /// <summary>
    /// Gets the total amount of square yards
    /// </summary>
    [IgnoreDataMember, JsonIgnore]
    public virtual decimal SquareYards => this.Value * (Units.SquareYard.Ratio / this.Unit.Ratio);

    /// <summary>
    /// Gets the total amount of square miles
    /// </summary>
    [IgnoreDataMember, JsonIgnore]
    public virtual decimal SquareMiles => this.Value * (Units.SquareMile.Ratio / this.Unit.Ratio);

    /// <summary>
    /// Adds the specified <see cref="Surface"/>
    /// </summary>
    /// <param name="surface">The <see cref="Surface"/> to add</param>
    /// <returns>A new <see cref="Surface"/> resulting from the addition</returns>
    public virtual Surface Add(Surface surface)
    {
        ArgumentNullException.ThrowIfNull(surface);
        if (surface.Unit.Type != this.Unit.Type) throw new ArgumentNullException(nameof(surface), "Cannot compare surfaces with different types of unit");
        return new Surface(this.Value + surface.Value * (this.Unit.Ratio / surface.Unit.Ratio), this.Unit);
    }

    /// <summary>
    /// Subtracts the specified <see cref="Surface"/>
    /// </summary>
    /// <param name="surface">The <see cref="Surface"/> to subtract</param>
    /// <returns>A new <see cref="Surface"/> resulting from the subtraction</returns>
    public virtual Surface Subtract(Surface surface)
    {
        ArgumentNullException.ThrowIfNull(surface);
        if (surface.Unit.Type != this.Unit.Type) throw new ArgumentNullException(nameof(surface), "Cannot compare surfaces with different types of unit");
        return new Surface(this.Value - surface.Value * (this.Unit.Ratio / surface.Unit.Ratio), this.Unit);
    }

    /// <inheritdoc/>
    public virtual int CompareTo(Surface? other) => this.SquareMeters.CompareTo(other?.SquareMeters);

    /// <summary>
    /// Converts the <see cref="Surface"/> into a new <see cref="Surface"/> measured using the specified <see cref="UnitOfMeasurement"/>
    /// </summary>
    /// <param name="unit">The <see cref="UnitOfMeasurement"/> to convert the <see cref="Surface"/> to</param>
    /// <returns>A new <see cref="Surface"/> measured using the specified <see cref="UnitOfMeasurement"/></returns>
    public new virtual Surface ConvertTo(UnitOfMeasurement unit)
    {
        ArgumentNullException.ThrowIfNull(unit);
        if (unit.Type != UnitOfMeasurementType.Surface) throw new ArgumentException("The specified unit of measurement must be of type 'surface'", nameof(unit));
        return new(this.Value * (unit.Ratio / this.Unit.Ratio), unit);
    }

    /// <summary>
    /// Creates a new <see cref="Surface"/> from the specified value, expressed in square millimeters
    /// </summary>
    /// <param name="value">The amount of millimeters to create a new <see cref="Surface"/> for</param>
    /// <returns>A new <see cref="Surface"/></returns>
    public static Surface FromSquareMillimeters(decimal value) => new(value, Units.SquareMillimeter);

    /// <summary>
    /// Creates a new <see cref="Surface"/> from the specified value, expressed in square centimeters
    /// </summary>
    /// <param name="value">The amount of centimeters to create a new <see cref="Surface"/> for</param>
    /// <returns>A new <see cref="Surface"/></returns>
    public static Surface FromSquareCentimeters(decimal value) => new(value, Units.SquareCentimeter);

    /// <summary>
    /// Creates a new <see cref="Surface"/> from the specified value, expressed in square decimeters
    /// </summary>
    /// <param name="value">The amount of decimeters to create a new <see cref="Surface"/> for</param>
    /// <returns>A new <see cref="Surface"/></returns>
    public static Surface FromSquareDecimeters(decimal value) => new(value, Units.SquareDecimeter);

    /// <summary>
    /// Creates a new <see cref="Surface"/> from the specified value, expressed in square meters
    /// </summary>
    /// <param name="value">The amount of meters to create a new <see cref="Surface"/> for</param>
    /// <returns>A new <see cref="Surface"/></returns>
    public static Surface FromSquareMeters(decimal value) => new(value, Units.SquareMeter);

    /// <summary>
    /// Creates a new <see cref="Surface"/> from the specified value, expressed in square decameters
    /// </summary>
    /// <param name="value">The amount of decameters to create a new <see cref="Surface"/> for</param>
    /// <returns>A new <see cref="Surface"/></returns>
    public static Surface FromSquareDecameters(decimal value) => new(value, Units.SquareDecameter);

    /// <summary>
    /// Creates a new <see cref="Surface"/> from the specified value, expressed in square hectometers
    /// </summary>
    /// <param name="value">The amount of hectometers to create a new <see cref="Surface"/> for</param>
    /// <returns>A new <see cref="Surface"/></returns>
    public static Surface FromSquareHectometers(decimal value) => new(value, Units.SquareHectometer);

    /// <summary>
    /// Creates a new <see cref="Surface"/> from the specified value, expressed in square kilometers
    /// </summary>
    /// <param name="value">The amount of kilometers to create a new <see cref="Surface"/> for</param>
    /// <returns>A new <see cref="Surface"/></returns>
    public static Surface FromSquareKilometers(decimal value) => new(value, Units.SquareKilometer);

    /// <summary>
    /// Creates a new <see cref="Surface"/> from the specified value, expressed in square inches
    /// </summary>
    /// <param name="value">The amount of inches to create a new <see cref="Surface"/> for</param>
    /// <returns>A new <see cref="Surface"/></returns>
    public static Surface FromSquareInches(decimal value) => new(value, Units.SquareInch);

    /// <summary>
    /// Creates a new <see cref="Surface"/> from the specified value, expressed in square feet
    /// </summary>
    /// <param name="value">The amount of feet to create a new <see cref="Surface"/> for</param>
    /// <returns>A new <see cref="Surface"/></returns>
    public static Surface FromSquareFeet(decimal value) => new(value, Units.SquareFoot);

    /// <summary>
    /// Creates a new <see cref="Surface"/> from the specified value, expressed in square yards
    /// </summary>
    /// <param name="value">The amount of yards to create a new <see cref="Surface"/> for</param>
    /// <returns>A new <see cref="Surface"/></returns>
    public static Surface FromSquareYards(decimal value) => new(value, Units.SquareYard);

    /// <summary>
    /// Creates a new <see cref="Surface"/> from the specified value, expressed in square miles
    /// </summary>
    /// <param name="value">The amount of miles to create a new <see cref="Surface"/> for</param>
    /// <returns>A new <see cref="Surface"/></returns>
    public static Surface FromSquareMiles(decimal value) => new(value, Units.SquareMile);

    /// <summary>
    /// Adds the specified <see cref="Surface"/>s
    /// </summary>
    /// <param name="surface1">The <see cref="Surface"/> to add the specified value to</param>
    /// <param name="surface2">The value to add</param>
    /// <returns>The addition's result</returns>
    public static Surface operator +(Surface surface1, Surface surface2) => surface1.Add(surface2);

    /// <summary>
    /// Subtracts the specified <see cref="Surface"/>s
    /// </summary>
    /// <param name="surface1">The <see cref="Surface"/> to subtract the specified value from</param>
    /// <param name="surface2">The value to subtract</param>
    /// <returns>The subtraction's result</returns>
    public static Surface operator -(Surface surface1, Surface surface2) => surface1.Subtract(surface2);

    /// <summary>
    /// Multiplies the specified <see cref="Surface"/>
    /// </summary>
    /// <param name="surface">The <see cref="Surface"/> to multiply</param>
    /// <param name="multiplier">The multiplier</param>
    /// <returns>The multiplication's result</returns>
    public static Surface operator *(Surface surface, decimal multiplier) => new(surface.Value * multiplier, surface.Unit);

    /// <summary>
    /// Multiplies the specified <see cref="Surface"/>
    /// </summary>
    /// <param name="surface">The <see cref="Surface"/> to multiply</param>
    /// <param name="multiplier">The multiplier</param>
    /// <returns>The multiplication's result</returns>
    public static Surface operator *(Surface surface, int multiplier) => new(surface.Value * multiplier, surface.Unit);

    /// <summary>
    /// Divides the specified <see cref="Surface"/>
    /// </summary>
    /// <param name="surface">The <see cref="Surface"/> to divide</param>
    /// <param name="divider">The divider</param>
    /// <returns>The division's result</returns>
    public static Surface operator /(Surface surface, decimal divider) => new(surface.Value / divider, surface.Unit);

    /// <summary>
    /// Divides the specified <see cref="Surface"/>
    /// </summary>
    /// <param name="surface">The <see cref="Surface"/> to divide</param>
    /// <param name="divider">The divider</param>
    /// <returns>The division's result</returns>
    public static Surface operator /(Surface surface, int divider) => new(surface.Value / divider, surface.Unit);

    /// <summary>
    /// Checks whether or not a <see cref="Surface"/> is lower than another one
    /// </summary>
    /// <param name="surface1">The <see cref="Surface"/> to check</param>
    /// <param name="surface2">The <see cref="Surface"/> to compare to</param>
    /// <returns>A boolean indicating whether or not a <see cref="Surface"/> is lower than another one</returns>
    public static bool operator <(Surface surface1, Surface surface2) => surface1.CompareTo(surface2) < 0;

    /// <summary>
    /// Checks whether or not a <see cref="Surface"/> is higher than another one
    /// </summary>
    /// <param name="surface1">The <see cref="Surface"/> to check</param>
    /// <param name="surface2">The <see cref="Surface"/> to compare to</param>
    /// <returns>A boolean indicating whether or not a <see cref="Surface"/> is higher than another one</returns>
    public static bool operator >(Surface surface1, Surface surface2) => surface1.CompareTo(surface2) > 0;

    /// <summary>
    /// Exposes default surface units
    /// </summary>
    public static class Units
    {

        /// <summary>
        /// Gets the <see cref="UnitOfMeasurement"/> to express square millimeters
        /// </summary>
        public static readonly UnitOfMeasurement SquareMillimeter = new(UnitOfMeasurementType.Surface, "Square millimeter", "mm²", 1000m);
        /// <summary>
        /// Gets the <see cref="UnitOfMeasurement"/> to express square centimeters
        /// </summary>
        public static readonly UnitOfMeasurement SquareCentimeter = new(UnitOfMeasurementType.Surface, "Square centimeter", "cm²", 100m);
        /// <summary>
        /// Gets the <see cref="UnitOfMeasurement"/> to express square decimeters
        /// </summary>
        public static readonly UnitOfMeasurement SquareDecimeter = new(UnitOfMeasurementType.Surface, "Square decimeter", "dm²", 10m);
        /// <summary>
        /// Gets the <see cref="UnitOfMeasurement"/> to express square millimeters
        /// </summary>
        public static readonly UnitOfMeasurement SquareMeter = new(UnitOfMeasurementType.Surface, "Square meter", "m²", 1);
        /// <summary>
        /// Gets the <see cref="UnitOfMeasurement"/> to express square decameters
        /// </summary>
        public static readonly UnitOfMeasurement SquareDecameter = new(UnitOfMeasurementType.Surface, "Square decameter", "dam²", 0.1m);
        /// <summary>
        /// Gets the <see cref="UnitOfMeasurement"/> to express square hectometers
        /// </summary>
        public static readonly UnitOfMeasurement SquareHectometer = new(UnitOfMeasurementType.Surface, "Square hectometer", "hm²", 0.01m);
        /// <summary>
        /// Gets the <see cref="UnitOfMeasurement"/> to express square kilometers
        /// </summary>
        public static readonly UnitOfMeasurement SquareKilometer = new(UnitOfMeasurementType.Surface, "Square kilometer", "km²", 0.001m);
        /// <summary>
        /// Gets the <see cref="UnitOfMeasurement"/> to express square inches
        /// </summary>
        public static readonly UnitOfMeasurement SquareInch = new(UnitOfMeasurementType.Surface, "Square inch", "in²", 1550.0031m);
        /// <summary>
        /// Gets the <see cref="UnitOfMeasurement"/> to express square feet
        /// </summary>
        public static readonly UnitOfMeasurement SquareFoot = new(UnitOfMeasurementType.Surface, "Square foot", "ft²", 10.76391041671m);
        /// <summary>
        /// Gets the <see cref="UnitOfMeasurement"/> to express square yards
        /// </summary>
        public static readonly UnitOfMeasurement SquareYard = new(UnitOfMeasurementType.Surface, "Square yard", "yd²", 1.1959900463011m);
        /// <summary>
        /// Gets the <see cref="UnitOfMeasurement"/> to express square miles
        /// </summary>
        public static readonly UnitOfMeasurement SquareMile = new(UnitOfMeasurementType.Surface, "Square mile", "mi²", 0.00000038610215855m);

        /// <summary>
        /// Gets a new <see cref="IEnumerable{T}"/> containing all default surface units
        /// </summary>
        /// <returns>A new <see cref="IEnumerable{T}"/></returns>
        public static IEnumerable<UnitOfMeasurement> AsEnumerable()
        {
            yield return SquareMillimeter;
            yield return SquareCentimeter;
            yield return SquareCentimeter;
            yield return SquareMeter;
            yield return SquareDecameter;
            yield return SquareHectometer;
            yield return SquareKilometer;
            yield return SquareInch;
            yield return SquareFoot;
            yield return SquareYard;
            yield return SquareMile;
        }

    }

}
