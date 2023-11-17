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
/// Represents the measurement of a mass
/// </summary>
[DataContract]
public class Mass
    : Measurement, IComparable<Mass>
{

    /// <summary>
    /// Initializes a new <see cref="Mass"/>
    /// </summary>
    [JsonConstructor]
    protected Mass() { }

    /// <summary>
    /// Initializes a new <see cref="Mass"/>
    /// </summary>
    /// <param name="value">The measured value</param>
    /// <param name="unit">The unit the measure's value is expressed in</param>
    public Mass(decimal value, UnitOfMeasurement unit) : base(value, unit == null ? throw new ArgumentNullException(nameof(unit)) : unit.Type == UnitOfMeasurementType.Mass ? unit : throw new ArgumentException("The specified unit must be of type 'mass'", nameof(unit))) { }

    /// <summary>
    /// Gets the total amount of milligrams
    /// </summary>
    [IgnoreDataMember, JsonIgnore]
    public virtual decimal Milligrams => this.Value * (Units.Milligram.Ratio / this.Unit.Ratio);

    /// <summary>
    /// Gets the total amount of centigrams
    /// </summary>
    [IgnoreDataMember, JsonIgnore]
    public virtual decimal Centigrams => this.Value * (Units.Centigram.Ratio / this.Unit.Ratio);

    /// <summary>
    /// Gets the total amount of decigrams
    /// </summary>
    [IgnoreDataMember, JsonIgnore]
    public virtual decimal Decigrams => this.Value * (Units.Decigram.Ratio / this.Unit.Ratio);

    /// <summary>
    /// Gets the total amount of grams
    /// </summary>
    [IgnoreDataMember, JsonIgnore]
    public virtual decimal Grams => this.Value * (Units.Gram.Ratio / this.Unit.Ratio);

    /// <summary>
    /// Gets the total amount of decagrams
    /// </summary>
    [IgnoreDataMember, JsonIgnore]
    public virtual decimal Decagrams => this.Value * (Units.Decagram.Ratio / this.Unit.Ratio);

    /// <summary>
    /// Gets the total amount of hectograms
    /// </summary>
    [IgnoreDataMember, JsonIgnore]
    public virtual decimal Hectograms => this.Value * (Units.Hectogram.Ratio / this.Unit.Ratio);

    /// <summary>
    /// Gets the total amount of kilograms
    /// </summary>
    [IgnoreDataMember, JsonIgnore]
    public virtual decimal Kilograms => this.Value * (Units.Kilogram.Ratio / this.Unit.Ratio);

    /// <summary>
    /// Gets the total amount of tons
    /// </summary>
    [IgnoreDataMember, JsonIgnore]
    public virtual decimal Tons => this.Value * (Units.Ton.Ratio / this.Unit.Ratio);

    /// <summary>
    /// Gets the total amount of kilotons
    /// </summary>
    [IgnoreDataMember, JsonIgnore]
    public virtual decimal Kilotons => this.Value * (Units.Kiloton.Ratio / this.Unit.Ratio);

    /// <summary>
    /// Gets the total amount of megatons
    /// </summary>
    [IgnoreDataMember, JsonIgnore]
    public virtual decimal Megatons => this.Value * (Units.Megaton.Ratio / this.Unit.Ratio);

    /// <summary>
    /// Gets the total amount of grains
    /// </summary>
    [IgnoreDataMember, JsonIgnore]
    public virtual decimal Grains => this.Value * (Units.Grain.Ratio / this.Unit.Ratio);

    /// <summary>
    /// Gets the total amount of drams
    /// </summary>
    [IgnoreDataMember, JsonIgnore]
    public virtual decimal Drams => this.Value * (Units.Dram.Ratio / this.Unit.Ratio);

    /// <summary>
    /// Gets the total amount of ounces
    /// </summary>
    [IgnoreDataMember, JsonIgnore]
    public virtual decimal Ounces => this.Value * (Units.Ounce.Ratio / this.Unit.Ratio);

    /// <summary>
    /// Gets the total amount of pounds
    /// </summary>
    [IgnoreDataMember, JsonIgnore]
    public virtual decimal Pounds => this.Value * (Units.Pound.Ratio / this.Unit.Ratio);

    /// <summary>
    /// Gets the total amount of stones
    /// </summary>
    [IgnoreDataMember, JsonIgnore]
    public virtual decimal Stones => this.Value * (Units.Stone.Ratio / this.Unit.Ratio);

    /// <summary>
    /// Adds the specified <see cref="Mass"/>
    /// </summary>
    /// <param name="mass">The <see cref="Mass"/> to add</param>
    /// <returns>A new <see cref="Mass"/> resulting from the addition</returns>
    public virtual Mass Add(Mass mass)
    {
        if (mass == null) throw new ArgumentNullException(nameof(mass));
        if (mass.Unit.Type != this.Unit.Type) throw new ArgumentNullException(nameof(mass), "Cannot compare masses with different types of unit");
        return new Mass(this.Value + mass.Value * (this.Unit.Ratio / mass.Unit.Ratio), this.Unit);
    }

    /// <summary>
    /// Subtracts the specified <see cref="Mass"/>
    /// </summary>
    /// <param name="mass">The <see cref="Mass"/> to subtract</param>
    /// <returns>A new <see cref="Mass"/> resulting from the subtraction</returns>
    public virtual Mass Subtract(Mass mass)
    {
        if (mass == null) throw new ArgumentNullException(nameof(mass));
        if (mass.Unit.Type != this.Unit.Type) throw new ArgumentNullException(nameof(mass), "Cannot compare masses with different types of unit");
        return new Mass(this.Value - mass.Value * (this.Unit.Ratio / mass.Unit.Ratio), this.Unit);
    }

    /// <inheritdoc/>
    public virtual int CompareTo(Mass? other) => this.Grams.CompareTo(other?.Grams);

    /// <summary>
    /// Converts the <see cref="Mass"/> into a new <see cref="Mass"/> measured using the specified <see cref="UnitOfMeasurement"/>
    /// </summary>
    /// <param name="unit">The <see cref="UnitOfMeasurement"/> to convert the <see cref="Mass"/> to</param>
    /// <returns>A new <see cref="Mass"/> measured using the specified <see cref="UnitOfMeasurement"/></returns>
    public new virtual Mass ConvertTo(UnitOfMeasurement unit)
    {
        if (unit == null) throw new ArgumentNullException(nameof(unit));
        if (unit.Type != UnitOfMeasurementType.Mass) throw new ArgumentException("The specified unit of measurement must be of type 'mass'", nameof(unit));
        return new(this.Value * (unit.Ratio / this.Unit.Ratio), unit);
    }

    /// <summary>
    /// Creates a new <see cref="Mass"/> from the specified value, expressed in milligrams
    /// </summary>
    /// <param name="value">The amount of milligrams to create a new <see cref="Mass"/> for</param>
    /// <returns>A new <see cref="Mass"/></returns>
    public static Mass FromMilligrams(decimal value) => new(value, Units.Milligram);

    /// <summary>
    /// Creates a new <see cref="Mass"/> from the specified value, expressed in centigrams
    /// </summary>
    /// <param name="value">The amount of centigrams to create a new <see cref="Mass"/> for</param>
    /// <returns>A new <see cref="Mass"/></returns>
    public static Mass FromCentigrams(decimal value) => new(value, Units.Centigram);

    /// <summary>
    /// Creates a new <see cref="Mass"/> from the specified value, expressed in decigrams
    /// </summary>
    /// <param name="value">The amount of decigrams to create a new <see cref="Mass"/> for</param>
    /// <returns>A new <see cref="Mass"/></returns>
    public static Mass FromDecigrams(decimal value) => new(value, Units.Decigram);

    /// <summary>
    /// Creates a new <see cref="Mass"/> from the specified value, expressed in grams
    /// </summary>
    /// <param name="value">The amount of grams to create a new <see cref="Mass"/> for</param>
    /// <returns>A new <see cref="Mass"/></returns>
    public static Mass FromGrams(decimal value) => new(value, Units.Gram);

    /// <summary>
    /// Creates a new <see cref="Mass"/> from the specified value, expressed in decagrams
    /// </summary>
    /// <param name="value">The amount of decagrams to create a new <see cref="Mass"/> for</param>
    /// <returns>A new <see cref="Mass"/></returns>
    public static Mass FromDecagrams(decimal value) => new(value, Units.Decagram);

    /// <summary>
    /// Creates a new <see cref="Mass"/> from the specified value, expressed in hectograms
    /// </summary>
    /// <param name="value">The amount of hectograms to create a new <see cref="Mass"/> for</param>
    /// <returns>A new <see cref="Mass"/></returns>
    public static Mass FromHectograms(decimal value) => new(value, Units.Hectogram);

    /// <summary>
    /// Creates a new <see cref="Mass"/> from the specified value, expressed in kilograms
    /// </summary>
    /// <param name="value">The amount of kilograms to create a new <see cref="Mass"/> for</param>
    /// <returns>A new <see cref="Mass"/></returns>
    public static Mass FromKilograms(decimal value) => new(value, Units.Kilogram);

    /// <summary>
    /// Creates a new <see cref="Mass"/> from the specified value, expressed in tons
    /// </summary>
    /// <param name="value">The amount of tons to create a new <see cref="Mass"/> for</param>
    /// <returns>A new <see cref="Mass"/></returns>
    public static Mass FromTons(decimal value) => new(value, Units.Ton);

    /// <summary>
    /// Creates a new <see cref="Mass"/> from the specified value, expressed in kilotons
    /// </summary>
    /// <param name="value">The amount of kilotons to create a new <see cref="Mass"/> for</param>
    /// <returns>A new <see cref="Mass"/></returns>
    public static Mass FromKilotons(decimal value) => new(value, Units.Kiloton);

    /// <summary>
    /// Creates a new <see cref="Mass"/> from the specified value, expressed in megatons
    /// </summary>
    /// <param name="value">The amount of megatons to create a new <see cref="Mass"/> for</param>
    /// <returns>A new <see cref="Mass"/></returns>
    public static Mass FromMegatons(decimal value) => new(value, Units.Megaton);

    /// <summary>
    /// Creates a new <see cref="Mass"/> from the specified value, expressed in grains
    /// </summary>
    /// <param name="value">The amount of grains to create a new <see cref="Mass"/> for</param>
    /// <returns>A new <see cref="Mass"/></returns>
    public static Mass FromGrains(decimal value) => new(value, Units.Grain);

    /// <summary>
    /// Creates a new <see cref="Mass"/> from the specified value, expressed in drams
    /// </summary>
    /// <param name="value">The amount of drams to create a new <see cref="Mass"/> for</param>
    /// <returns>A new <see cref="Mass"/></returns>
    public static Mass FromDrams(decimal value) => new(value, Units.Dram);

    /// <summary>
    /// Creates a new <see cref="Mass"/> from the specified value, expressed in ounces
    /// </summary>
    /// <param name="value">The amount of ounces to create a new <see cref="Mass"/> for</param>
    /// <returns>A new <see cref="Mass"/></returns>
    public static Mass FromOunces(decimal value) => new(value, Units.Ounce);

    /// <summary>
    /// Creates a new <see cref="Mass"/> from the specified value, expressed in pounds
    /// </summary>
    /// <param name="value">The amount of pounds to create a new <see cref="Mass"/> for</param>
    /// <returns>A new <see cref="Mass"/></returns>
    public static Mass FromPounds(decimal value) => new(value, Units.Pound);

    /// <summary>
    /// Creates a new <see cref="Mass"/> from the specified value, expressed in stones
    /// </summary>
    /// <param name="value">The amount of stones to create a new <see cref="Mass"/> for</param>
    /// <returns>A new <see cref="Mass"/></returns>
    public static Mass FromStones(decimal value) => new(value, Units.Stone);

    /// <summary>
    /// Adds the specified <see cref="Mass"/>s
    /// </summary>
    /// <param name="mass1">The <see cref="Mass"/> to add the specified value to</param>
    /// <param name="mass2">The value to add</param>
    /// <returns>The addition's result</returns>
    public static Mass operator +(Mass mass1, Mass mass2) => mass1.Add(mass2);

    /// <summary>
    /// Subtracts the specified <see cref="Mass"/>s
    /// </summary>
    /// <param name="mass1">The <see cref="Mass"/> to subtract the specified value from</param>
    /// <param name="mass2">The value to subtract</param>
    /// <returns>The subtraction's result</returns>
    public static Mass operator -(Mass mass1, Mass mass2) => mass1.Subtract(mass2);

    /// <summary>
    /// Multiplies the specified <see cref="Mass"/>
    /// </summary>
    /// <param name="mass">The <see cref="Mass"/> to multiply</param>
    /// <param name="multiplier">The multiplier</param>
    /// <returns>The multiplication's result</returns>
    public static Mass operator *(Mass mass, decimal multiplier) => new(mass.Value * multiplier, mass.Unit);

    /// <summary>
    /// Multiplies the specified <see cref="Mass"/>
    /// </summary>
    /// <param name="mass">The <see cref="Mass"/> to multiply</param>
    /// <param name="multiplier">The multiplier</param>
    /// <returns>The multiplication's result</returns>
    public static Mass operator *(Mass mass, int multiplier) => new(mass.Value * multiplier, mass.Unit);

    /// <summary>
    /// Divides the specified <see cref="Mass"/>
    /// </summary>
    /// <param name="mass">The <see cref="Mass"/> to divide</param>
    /// <param name="divider">The divider</param>
    /// <returns>The division's result</returns>
    public static Mass operator /(Mass mass, decimal divider) => new(mass.Value / divider, mass.Unit);

    /// <summary>
    /// Divides the specified <see cref="Mass"/>
    /// </summary>
    /// <param name="mass">The <see cref="Mass"/> to divide</param>
    /// <param name="divider">The divider</param>
    /// <returns>The division's result</returns>
    public static Mass operator /(Mass mass, int divider) => new(mass.Value / divider, mass.Unit);

    /// <summary>
    /// Checks whether or not a <see cref="Mass"/> is lower than another one
    /// </summary>
    /// <param name="mass1">The <see cref="Mass"/> to check</param>
    /// <param name="mass2">The <see cref="Mass"/> to compare to</param>
    /// <returns>A boolean indicating whether or not a <see cref="Mass"/> is lower than another one</returns>
    public static bool operator <(Mass mass1, Mass mass2) => mass1.CompareTo(mass2) < 0;

    /// <summary>
    /// Checks whether or not a <see cref="Mass"/> is higher than another one
    /// </summary>
    /// <param name="mass1">The <see cref="Mass"/> to check</param>
    /// <param name="mass2">The <see cref="Mass"/> to compare to</param>
    /// <returns>A boolean indicating whether or not a <see cref="Mass"/> is higher than another one</returns>
    public static bool operator >(Mass mass1, Mass mass2) => mass1.CompareTo(mass2) > 0;

    /// <summary>
    /// Exposes default mass units
    /// </summary>
    public static class Units
    {

        /// <summary>
        /// Gets the <see cref="UnitOfMeasurement"/> to express milligrams
        /// </summary>
        public static readonly UnitOfMeasurement Milligram = new(UnitOfMeasurementType.Mass, "Milligram", "mg", 1000);
        /// <summary>
        /// Gets the <see cref="UnitOfMeasurement"/> to express centigrams
        /// </summary>
        public static readonly UnitOfMeasurement Centigram = new(UnitOfMeasurementType.Mass, "Centigram", "cg", 100);
        /// <summary>
        /// Gets the <see cref="UnitOfMeasurement"/> to express decigrams
        /// </summary>
        public static readonly UnitOfMeasurement Decigram = new(UnitOfMeasurementType.Mass, "Decigram", "dg", 10);
        /// <summary>
        /// Gets the <see cref="UnitOfMeasurement"/> to express grams
        /// </summary>
        public static readonly UnitOfMeasurement Gram = new(UnitOfMeasurementType.Mass, "Gram", "g", 1);
        /// <summary>
        /// Gets the <see cref="UnitOfMeasurement"/> to express decagrams
        /// </summary>
        public static readonly UnitOfMeasurement Decagram = new(UnitOfMeasurementType.Mass, "Decagram", "dag", 0.1m);
        /// <summary>
        /// Gets the <see cref="UnitOfMeasurement"/> to express hectograms
        /// </summary>
        public static readonly UnitOfMeasurement Hectogram = new(UnitOfMeasurementType.Mass, "Hectogram", "hg", 0.01m);
        /// <summary>
        /// Gets the <see cref="UnitOfMeasurement"/> to express kilograms
        /// </summary>
        public static readonly UnitOfMeasurement Kilogram = new(UnitOfMeasurementType.Mass, "Kilogram", "kg", 0.001m);
        /// <summary>
        /// Gets the <see cref="UnitOfMeasurement"/> to express tons
        /// </summary>
        public static readonly UnitOfMeasurement Ton = new(UnitOfMeasurementType.Mass, "Ton", "t", 0.000001m);
        /// <summary>
        /// Gets the <see cref="UnitOfMeasurement"/> to express kilotons
        /// </summary>
        public static readonly UnitOfMeasurement Kiloton = new(UnitOfMeasurementType.Mass, "Kiloton", "kt", 0.000000001m);
        /// <summary>
        /// Gets the <see cref="UnitOfMeasurement"/> to express megatons
        /// </summary>
        public static readonly UnitOfMeasurement Megaton = new(UnitOfMeasurementType.Mass, "Megaton", "Mt", 0.000000000001m);
        /// <summary>
        /// Gets the <see cref="UnitOfMeasurement"/> to express grains
        /// </summary>
        public static readonly UnitOfMeasurement Grain = new(UnitOfMeasurementType.Mass, "Grain", "gr", 15.4323583529m);
        /// <summary>
        /// Gets the <see cref="UnitOfMeasurement"/> to express drams
        /// </summary>
        public static readonly UnitOfMeasurement Dram = new(UnitOfMeasurementType.Mass, "Dram", "dr", 0.31746031746031m);
        /// <summary>
        /// Gets the <see cref="UnitOfMeasurement"/> to express ounces 
        /// </summary>
        public static readonly UnitOfMeasurement Ounce = new(UnitOfMeasurementType.Mass, "Ounce", "oz", 0.03527396195m);
        /// <summary>
        /// Gets the <see cref="UnitOfMeasurement"/> to express stones
        /// </summary>
        public static readonly UnitOfMeasurement Pound = new(UnitOfMeasurementType.Mass, "Pound", "lb", 0.0022046226m);
        /// <summary>
        /// Gets the <see cref="UnitOfMeasurement"/> to express stones
        /// </summary>
        public static readonly UnitOfMeasurement Stone = new(UnitOfMeasurementType.Mass, "Stone", "st", 0.000157473m);

        /// <summary>
        /// Gets a new <see cref="IEnumerable{T}"/> containing all default volume units
        /// </summary>
        /// <returns>A new <see cref="IEnumerable{T}"/></returns>
        public static IEnumerable<UnitOfMeasurement> AsEnumerable()
        {
            yield return Milligram;
            yield return Centigram;
            yield return Decigram;
            yield return Gram;
            yield return Decagram;
            yield return Hectogram;
            yield return Kilogram;
            yield return Ton;
            yield return Kiloton;
            yield return Megaton;
            yield return Grain;
            yield return Dram;
            yield return Ounce;
            yield return Pound;
            yield return Stone;
        }

    }

}
