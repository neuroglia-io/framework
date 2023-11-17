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
/// Represents the measurement of a capacity
/// </summary>
[DataContract]
public class Energy
    : Measurement, IComparable<Energy>
{

    /// <summary>
    /// Initializes a new <see cref="Energy"/>
    /// </summary>
    [JsonConstructor]
    protected Energy() { }

    /// <summary>
    /// Initializes a new <see cref="Energy"/>
    /// </summary>
    /// <param name="value">The measured value</param>
    /// <param name="unit">The unit the measure's value is expressed in</param>
    public Energy(decimal value, UnitOfMeasurement unit) : base(value, unit == null ? throw new ArgumentNullException(nameof(unit)) : unit.Type == UnitOfMeasurementType.Energy ? unit : throw new ArgumentException("The specified unit must be of type 'energy'", nameof(unit))) { }

    /// <summary>
    /// Gets the total amount of millicalories
    /// </summary>
    [IgnoreDataMember, JsonIgnore]
    public virtual decimal Millicalories => this.Value * (Units.Millicalorie.Ratio / this.Unit.Ratio);

    /// <summary>
    /// Gets the total amount of centicalories
    /// </summary>
    [IgnoreDataMember, JsonIgnore]
    public virtual decimal Centicalories => this.Value * (Units.Centicalorie.Ratio / this.Unit.Ratio);

    /// <summary>
    /// Gets the total amount of decicalories
    /// </summary>
    [IgnoreDataMember, JsonIgnore]
    public virtual decimal Decicalories => this.Value * (Units.Decicalorie.Ratio / this.Unit.Ratio);

    /// <summary>
    /// Gets the total amount of calories
    /// </summary>
    [IgnoreDataMember, JsonIgnore]
    public virtual decimal Calories => this.Value * (Units.Calorie.Ratio / this.Unit.Ratio);

    /// <summary>
    /// Gets the total amount of decacalories
    /// </summary>
    [IgnoreDataMember, JsonIgnore]
    public virtual decimal Decacalories => this.Value * (Units.Decacalorie.Ratio / this.Unit.Ratio);

    /// <summary>
    /// Gets the total amount of hectocalories
    /// </summary>
    [IgnoreDataMember, JsonIgnore]
    public virtual decimal Hectocalories => this.Value * (Units.Hectocalorie.Ratio / this.Unit.Ratio);

    /// <summary>
    /// Gets the total amount of kilocalories
    /// </summary>
    [IgnoreDataMember, JsonIgnore]
    public virtual decimal Kilocalories => this.Value * (Units.Kilocalorie.Ratio / this.Unit.Ratio);

    /// <summary>
    /// Gets the total amount of millijoules
    /// </summary>
    [IgnoreDataMember, JsonIgnore]
    public virtual decimal Millijoules => this.Value * (Units.Millijoule.Ratio / this.Unit.Ratio);

    /// <summary>
    /// Gets the total amount of centijoules
    /// </summary>
    [IgnoreDataMember, JsonIgnore]
    public virtual decimal Centijoules => this.Value * (Units.Centijoule.Ratio / this.Unit.Ratio);

    /// <summary>
    /// Gets the total amount of decijoules
    /// </summary>
    [IgnoreDataMember, JsonIgnore]
    public virtual decimal Decijoules => this.Value * (Units.Decijoule.Ratio / this.Unit.Ratio);

    /// <summary>
    /// Gets the total amount of joules
    /// </summary>
    [IgnoreDataMember, JsonIgnore]
    public virtual decimal Joules => this.Value * (Units.Joule.Ratio / this.Unit.Ratio);

    /// <summary>
    /// Gets the total amount of decajoules
    /// </summary>
    [IgnoreDataMember, JsonIgnore]
    public virtual decimal Decajoules => this.Value * (Units.Decajoule.Ratio / this.Unit.Ratio);

    /// <summary>
    /// Gets the total amount of hectojoules
    /// </summary>
    [IgnoreDataMember, JsonIgnore]
    public virtual decimal Hectojoules => this.Value * (Units.Hectojoule.Ratio / this.Unit.Ratio);

    /// <summary>
    /// Gets the total amount of kilojoules
    /// </summary>
    [IgnoreDataMember, JsonIgnore]
    public virtual decimal Kilojoules => this.Value * (Units.Kilojoule.Ratio / this.Unit.Ratio);

    /// <summary>
    /// Adds the specified <see cref="Energy"/>
    /// </summary>
    /// <param name="energy">The <see cref="Energy"/> to add</param>
    /// <returns>A new <see cref="Energy"/> resulting from the addition</returns>
    public virtual Energy Add(Energy energy)
    {
        if (energy == null) throw new ArgumentNullException(nameof(energy));
        if (energy.Unit.Type != this.Unit.Type) throw new ArgumentNullException(nameof(energy), "Cannot compare energies with different types of unit");
        return new Energy(this.Value + energy.Value * (this.Unit.Ratio / energy.Unit.Ratio), this.Unit);
    }

    /// <summary>
    /// Subtracts the specified <see cref="Energy"/>
    /// </summary>
    /// <param name="energy">The <see cref="Energy"/> to subtract</param>
    /// <returns>A new <see cref="Energy"/> resulting from the subtraction</returns>
    public virtual Energy Subtract(Energy energy)
    {
        if (energy == null) throw new ArgumentNullException(nameof(energy));
        if (energy.Unit.Type != this.Unit.Type) throw new ArgumentNullException(nameof(energy), "Cannot compare energies with different types of unit");
        return new Energy(this.Value - energy.Value * (this.Unit.Ratio / energy.Unit.Ratio), this.Unit);
    }

    /// <inheritdoc/>
    public virtual int CompareTo(Energy? other) => this.Calories.CompareTo(other?.Calories);

    /// <summary>
    /// Converts the <see cref="Energy"/> into a new <see cref="Energy"/> measured using the specified <see cref="UnitOfMeasurement"/>
    /// </summary>
    /// <param name="unit">The <see cref="UnitOfMeasurement"/> to convert the <see cref="Energy"/> to</param>
    /// <returns>A new <see cref="Energy"/> measured using the specified <see cref="UnitOfMeasurement"/></returns>
    public new virtual Energy ConvertTo(UnitOfMeasurement unit)
    {
        if (unit == null) throw new ArgumentNullException(nameof(unit));
        if (unit.Type != UnitOfMeasurementType.Energy) throw new ArgumentException("The specified unit of measurement must be of type 'energy'", nameof(unit));
        return new(this.Value * (unit.Ratio / this.Unit.Ratio), unit);
    }

    /// <summary>
    /// Creates a new <see cref="Energy"/> from the specified value, expressed in millicalories
    /// </summary>
    /// <param name="value">The amount of millicalories to create a new <see cref="Energy"/> for</param>
    /// <returns>A new <see cref="Energy"/></returns>
    public static Energy FromMillicalories(decimal value) => new(value, Units.Millicalorie);

    /// <summary>
    /// Creates a new <see cref="Energy"/> from the specified value, expressed in centicalories
    /// </summary>
    /// <param name="value">The amount of centicalories to create a new <see cref="Energy"/> for</param>
    /// <returns>A new <see cref="Energy"/></returns>
    public static Energy FromCenticalories(decimal value) => new(value, Units.Centicalorie);

    /// <summary>
    /// Creates a new <see cref="Energy"/> from the specified value, expressed in decicalories
    /// </summary>
    /// <param name="value">The amount of decicalories to create a new <see cref="Energy"/> for</param>
    /// <returns>A new <see cref="Energy"/></returns>
    public static Energy FromDecicalories(decimal value) => new(value, Units.Decicalorie);

    /// <summary>
    /// Creates a new <see cref="Energy"/> from the specified value, expressed in calories
    /// </summary>
    /// <param name="value">The amount of calories to create a new <see cref="Energy"/> for</param>
    /// <returns>A new <see cref="Energy"/></returns>
    public static Energy FromCalories(decimal value) => new(value, Units.Calorie);

    /// <summary>
    /// Creates a new <see cref="Energy"/> from the specified value, expressed in decacalories
    /// </summary>
    /// <param name="value">The amount of decacalories to create a new <see cref="Energy"/> for</param>
    /// <returns>A new <see cref="Energy"/></returns>
    public static Energy FromDecacalories(decimal value) => new(value, Units.Decacalorie);

    /// <summary>
    /// Creates a new <see cref="Energy"/> from the specified value, expressed in hectocalories
    /// </summary>
    /// <param name="value">The amount of hectocalories to create a new <see cref="Energy"/> for</param>
    /// <returns>A new <see cref="Energy"/></returns>
    public static Energy FromHectocalories(decimal value) => new(value, Units.Hectocalorie);

    /// <summary>
    /// Creates a new <see cref="Energy"/> from the specified value, expressed in kilocalories
    /// </summary>
    /// <param name="value">The amount of kilocalories to create a new <see cref="Energy"/> for</param>
    /// <returns>A new <see cref="Energy"/></returns>
    public static Energy FromKilocalories(decimal value) => new(value, Units.Kilocalorie);

    /// <summary>
    /// Creates a new <see cref="Energy"/> from the specified value, expressed in millijoules
    /// </summary>
    /// <param name="value">The amount of millijoules to create a new <see cref="Energy"/> for</param>
    /// <returns>A new <see cref="Energy"/></returns>
    public static Energy FromMillijoules(decimal value) => new(value, Units.Millijoule);

    /// <summary>
    /// Creates a new <see cref="Energy"/> from the specified value, expressed in centijoules
    /// </summary>
    /// <param name="value">The amount of centijoules to create a new <see cref="Energy"/> for</param>
    /// <returns>A new <see cref="Energy"/></returns>
    public static Energy FromCentijoules(decimal value) => new(value, Units.Centijoule);

    /// <summary>
    /// Creates a new <see cref="Energy"/> from the specified value, expressed in decijoules
    /// </summary>
    /// <param name="value">The amount of decijoules to create a new <see cref="Energy"/> for</param>
    /// <returns>A new <see cref="Energy"/></returns>
    public static Energy FromDecijoules(decimal value) => new(value, Units.Decijoule);

    /// <summary>
    /// Creates a new <see cref="Energy"/> from the specified value, expressed in joules
    /// </summary>
    /// <param name="value">The amount of joules to create a new <see cref="Energy"/> for</param>
    /// <returns>A new <see cref="Energy"/></returns>
    public static Energy FromJoules(decimal value) => new(value, Units.Joule);

    /// <summary>
    /// Creates a new <see cref="Energy"/> from the specified value, expressed in decajoules
    /// </summary>
    /// <param name="value">The amount of decajoules to create a new <see cref="Energy"/> for</param>
    /// <returns>A new <see cref="Energy"/></returns>
    public static Energy FromDecajoules(decimal value) => new(value, Units.Decajoule);

    /// <summary>
    /// Creates a new <see cref="Energy"/> from the specified value, expressed in hectojoules
    /// </summary>
    /// <param name="value">The amount of hectojoules to create a new <see cref="Energy"/> for</param>
    /// <returns>A new <see cref="Energy"/></returns>
    public static Energy FromHectojoules(decimal value) => new(value, Units.Hectojoule);

    /// <summary>
    /// Creates a new <see cref="Energy"/> from the specified value, expressed in kilojoules
    /// </summary>
    /// <param name="value">The amount of kilojoules to create a new <see cref="Energy"/> for</param>
    /// <returns>A new <see cref="Energy"/></returns>
    public static Energy FromKilojoules(decimal value) => new(value, Units.Kilojoule);

    /// <summary>
    /// Adds the specified <see cref="Energy"/>s
    /// </summary>
    /// <param name="energy1">The <see cref="Energy"/> to add the specified value to</param>
    /// <param name="energy2">The value to add</param>
    /// <returns>The addition's result</returns>
    public static Energy operator +(Energy energy1, Energy energy2) => energy1.Add(energy2);

    /// <summary>
    /// Subtracts the specified <see cref="Energy"/>s
    /// </summary>
    /// <param name="energy1">The <see cref="Energy"/> to subtract the specified value from</param>
    /// <param name="energy2">The value to subtract</param>
    /// <returns>The subtraction's result</returns>
    public static Energy operator -(Energy energy1, Energy energy2) => energy1.Subtract(energy2);

    /// <summary>
    /// Multiplies the specified <see cref="Energy"/>
    /// </summary>
    /// <param name="energy">The <see cref="Energy"/> to multiply</param>
    /// <param name="multiplier">The multiplier</param>
    /// <returns>The multiplication's result</returns>
    public static Energy operator *(Energy energy, decimal multiplier) => new(energy.Value * multiplier, energy.Unit);

    /// <summary>
    /// Multiplies the specified <see cref="Energy"/>
    /// </summary>
    /// <param name="energy">The <see cref="Energy"/> to multiply</param>
    /// <param name="multiplier">The multiplier</param>
    /// <returns>The multiplication's result</returns>
    public static Energy operator *(Energy energy, int multiplier) => new(energy.Value * multiplier, energy.Unit);

    /// <summary>
    /// Divides the specified <see cref="Energy"/>
    /// </summary>
    /// <param name="energy">The <see cref="Energy"/> to divide</param>
    /// <param name="divider">The divider</param>
    /// <returns>The division's result</returns>
    public static Energy operator /(Energy energy, decimal divider) => new(energy.Value / divider, energy.Unit);

    /// <summary>
    /// Divides the specified <see cref="Energy"/>
    /// </summary>
    /// <param name="energy">The <see cref="Energy"/> to divide</param>
    /// <param name="divider">The divider</param>
    /// <returns>The division's result</returns>
    public static Energy operator /(Energy energy, int divider) => new(energy.Value / divider, energy.Unit);

    /// <summary>
    /// Checks whether or not a <see cref="Energy"/> is lower than another one
    /// </summary>
    /// <param name="energy1">The <see cref="Energy"/> to check</param>
    /// <param name="energy2">The <see cref="Energy"/> to compare to</param>
    /// <returns>A boolean indicating whether or not a <see cref="Energy"/> is lower than another one</returns>
    public static bool operator <(Energy energy1, Energy energy2) => energy1.CompareTo(energy2) < 0;

    /// <summary>
    /// Checks whether or not a <see cref="Energy"/> is higher than another one
    /// </summary>
    /// <param name="energy1">The <see cref="Energy"/> to check</param>
    /// <param name="energy2">The <see cref="Energy"/> to compare to</param>
    /// <returns>A boolean indicating whether or not a <see cref="Energy"/> is higher than another one</returns>
    public static bool operator >(Energy energy1, Energy energy2) => energy1.CompareTo(energy2) > 0;

    /// <summary>
    /// Exposes default energy units
    /// </summary>
    public static class Units
    {

        /// <summary>
        /// Gets the <see cref="UnitOfMeasurement"/> to express millicalories
        /// </summary>
        public static readonly UnitOfMeasurement Millicalorie = new(UnitOfMeasurementType.Energy, "Millicalorie", "ml", 1000);
        /// <summary>
        /// Gets the <see cref="UnitOfMeasurement"/> to express centicalories
        /// </summary>
        public static readonly UnitOfMeasurement Centicalorie = new(UnitOfMeasurementType.Energy, "Centicalorie", "cl", 100);
        /// <summary>
        /// Gets the <see cref="UnitOfMeasurement"/> to express decicalories
        /// </summary>
        public static readonly UnitOfMeasurement Decicalorie = new(UnitOfMeasurementType.Energy, "Decicalorie", "dl", 10);
        /// <summary>
        /// Gets the <see cref="UnitOfMeasurement"/> to express calories
        /// </summary>
        public static readonly UnitOfMeasurement Calorie = new(UnitOfMeasurementType.Energy, "Calorie", "l", 1);
        /// <summary>
        /// Gets the <see cref="UnitOfMeasurement"/> to express decacalories
        /// </summary>
        public static readonly UnitOfMeasurement Decacalorie = new(UnitOfMeasurementType.Energy, "Decacalorie", "dal", 0.1m);
        /// <summary>
        /// Gets the <see cref="UnitOfMeasurement"/> to express hectocalories
        /// </summary>
        public static readonly UnitOfMeasurement Hectocalorie = new(UnitOfMeasurementType.Energy, "Hectocalorie", "hl", 0.01m);
        /// <summary>
        /// Gets the <see cref="UnitOfMeasurement"/> to express kilocalories
        /// </summary>
        public static readonly UnitOfMeasurement Kilocalorie = new(UnitOfMeasurementType.Energy, "Kilocalorie", "kl", 0.001m);
        /// <summary>
        /// Gets the <see cref="UnitOfMeasurement"/> to express millijoules
        /// </summary>
        public static readonly UnitOfMeasurement Millijoule = new(UnitOfMeasurementType.Energy, "Millijoule", "J", 4184m);
        /// <summary>
        /// Gets the <see cref="UnitOfMeasurement"/> to express centijoules
        /// </summary>
        public static readonly UnitOfMeasurement Centijoule = new(UnitOfMeasurementType.Energy, "Centijoule", "J", 418.4m);
        /// <summary>
        /// Gets the <see cref="UnitOfMeasurement"/> to express decijoules
        /// </summary>
        public static readonly UnitOfMeasurement Decijoule = new(UnitOfMeasurementType.Energy, "Decijoule", "J", 41.84m);
        /// <summary>
        /// Gets the <see cref="UnitOfMeasurement"/> to express joules
        /// </summary>
        public static readonly UnitOfMeasurement Joule = new(UnitOfMeasurementType.Energy, "Joule", "J", 4.184m);
        /// <summary>
        /// Gets the <see cref="UnitOfMeasurement"/> to express decajoules
        /// </summary>
        public static readonly UnitOfMeasurement Decajoule = new(UnitOfMeasurementType.Energy, "Decajoule", "daJ", 0.4184m);
        /// <summary>
        /// Gets the <see cref="UnitOfMeasurement"/> to express hectojoules
        /// </summary>
        public static readonly UnitOfMeasurement Hectojoule = new(UnitOfMeasurementType.Energy, "Hectojoule", "hJ", 0.04184m);
        /// <summary>
        /// Gets the <see cref="UnitOfMeasurement"/> to express kilojoules
        /// </summary>
        public static readonly UnitOfMeasurement Kilojoule = new(UnitOfMeasurementType.Energy, "Kilojoule", "kJ", 0.004184m);

    }

}
