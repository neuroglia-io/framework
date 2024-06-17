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
/// Represents the measurement of a temperature
/// </summary>
[DataContract]
public class Temperature
    : Measurement, IComparable<Temperature>
{

    /// <summary>
    /// Initializes a new <see cref="Temperature"/>
    /// </summary>
    [JsonConstructor]
    protected Temperature() { }

    /// <summary>
    /// Initializes a new <see cref="Temperature"/>
    /// </summary>
    /// <param name="value">The measured value</param>
    /// <param name="unit">The unit the measure's value is expressed in</param>
    public Temperature(decimal value, UnitOfMeasurement unit) : base(value, unit == null ? throw new ArgumentNullException(nameof(unit)) : unit.Type == UnitOfMeasurementType.Temperature ? unit : throw new ArgumentException("The specified unit must be of type 'temperature'", nameof(unit))) { }

    /// <summary>
    /// Gets the total amount of millikelvins
    /// </summary>
    [IgnoreDataMember, JsonIgnore]
    public virtual decimal Millikelvins => this.Value * (Units.Millikelvin.Ratio / this.Unit.Ratio);

    /// <summary>
    /// Gets the total amount of centikelvins
    /// </summary>
    [IgnoreDataMember, JsonIgnore]
    public virtual decimal Centikelvins => this.Value * (Units.Centikelvin.Ratio / this.Unit.Ratio);

    /// <summary>
    /// Gets the total amount of decikelvins
    /// </summary>
    [IgnoreDataMember, JsonIgnore]
    public virtual decimal Decikelvins => this.Value * (Units.Decikelvin.Ratio / this.Unit.Ratio);

    /// <summary>
    /// Gets the total amount of kelvins
    /// </summary>
    [IgnoreDataMember, JsonIgnore]
    public virtual decimal Kelvins => this.Value * (Units.Kelvin.Ratio / this.Unit.Ratio);

    /// <summary>
    /// Gets the total amount of decakelvins
    /// </summary>
    [IgnoreDataMember, JsonIgnore]
    public virtual decimal Decakelvins => this.Value * (Units.Decakelvin.Ratio / this.Unit.Ratio);

    /// <summary>
    /// Gets the total amount of hectokelvins
    /// </summary>
    [IgnoreDataMember, JsonIgnore]
    public virtual decimal Hectokelvins => this.Value * (Units.Hectokelvin.Ratio / this.Unit.Ratio);

    /// <summary>
    /// Gets the total amount of kilokelvins
    /// </summary>
    [IgnoreDataMember, JsonIgnore]
    public virtual decimal Kilokelvins => this.Value * (Units.Kilokelvin.Ratio / this.Unit.Ratio);

    /// <summary>
    /// Adds the specified <see cref="Temperature"/>
    /// </summary>
    /// <param name="temperature">The <see cref="Temperature"/> to add</param>
    /// <returns>A new <see cref="Temperature"/> resulting from the addition</returns>
    public virtual Temperature Add(Temperature temperature)
    {
        ArgumentNullException.ThrowIfNull(temperature);
        if (temperature.Unit.Type != this.Unit.Type) throw new ArgumentNullException(nameof(temperature), "Cannot compare temperatures with different types of unit");
        return new Temperature(this.Value + temperature.Value * (this.Unit.Ratio / temperature.Unit.Ratio), this.Unit);
    }

    /// <summary>
    /// Subtracts the specified <see cref="Temperature"/>
    /// </summary>
    /// <param name="temperature">The <see cref="Temperature"/> to subtract</param>
    /// <returns>A new <see cref="Temperature"/> resulting from the subtraction</returns>
    public virtual Temperature Subtract(Temperature temperature)
    {
        ArgumentNullException.ThrowIfNull(temperature);
        if (temperature.Unit.Type != this.Unit.Type) throw new ArgumentNullException(nameof(temperature), "Cannot compare temperatures with different types of unit");
        return new Temperature(this.Value - temperature.Value * (this.Unit.Ratio / temperature.Unit.Ratio), this.Unit);
    }

    /// <inheritdoc/>
    public virtual int CompareTo(Temperature? other) => this.Kelvins.CompareTo(other?.Kelvins);

    /// <summary>
    /// Converts the <see cref="Temperature"/> into a new <see cref="Temperature"/> measured using the specified <see cref="UnitOfMeasurement"/>
    /// </summary>
    /// <param name="unit">The <see cref="UnitOfMeasurement"/> to convert the <see cref="Temperature"/> to</param>
    /// <returns>A new <see cref="Temperature"/> measured using the specified <see cref="UnitOfMeasurement"/></returns>
    public new virtual Temperature ConvertTo(UnitOfMeasurement unit)
    {
        ArgumentNullException.ThrowIfNull(unit);
        if (unit.Type != UnitOfMeasurementType.Temperature) throw new ArgumentException("The specified unit of measurement must be of type 'temperature'", nameof(unit));
        return new(this.Value * (unit.Ratio / this.Unit.Ratio), unit);
    }

    /// <summary>
    /// Creates a new <see cref="Temperature"/> from the specified value, expressed in millikelvins
    /// </summary>
    /// <param name="value">The amount of millikelvins to create a new <see cref="Temperature"/> for</param>
    /// <returns>A new <see cref="Temperature"/></returns>
    public static Temperature FromMillikelvins(decimal value) => new(value, Units.Millikelvin);

    /// <summary>
    /// Creates a new <see cref="Temperature"/> from the specified value, expressed in centikelvins
    /// </summary>
    /// <param name="value">The amount of centikelvins to create a new <see cref="Temperature"/> for</param>
    /// <returns>A new <see cref="Temperature"/></returns>
    public static Temperature FromCentikelvins(decimal value) => new(value, Units.Centikelvin);

    /// <summary>
    /// Creates a new <see cref="Temperature"/> from the specified value, expressed in decikelvins
    /// </summary>
    /// <param name="value">The amount of decikelvins to create a new <see cref="Temperature"/> for</param>
    /// <returns>A new <see cref="Temperature"/></returns>
    public static Temperature FromDecikelvins(decimal value) => new(value, Units.Decikelvin);

    /// <summary>
    /// Creates a new <see cref="Temperature"/> from the specified value, expressed in kelvins
    /// </summary>
    /// <param name="value">The amount of kelvins to create a new <see cref="Temperature"/> for</param>
    /// <returns>A new <see cref="Temperature"/></returns>
    public static Temperature FromKelvins(decimal value) => new(value, Units.Kelvin);

    /// <summary>
    /// Creates a new <see cref="Temperature"/> from the specified value, expressed in decakelvins
    /// </summary>
    /// <param name="value">The amount of decakelvins to create a new <see cref="Temperature"/> for</param>
    /// <returns>A new <see cref="Temperature"/></returns>
    public static Temperature FromDecakelvins(decimal value) => new(value, Units.Decakelvin);

    /// <summary>
    /// Creates a new <see cref="Temperature"/> from the specified value, expressed in hectokelvins
    /// </summary>
    /// <param name="value">The amount of hectokelvins to create a new <see cref="Temperature"/> for</param>
    /// <returns>A new <see cref="Temperature"/></returns>
    public static Temperature FromHectokelvins(decimal value) => new(value, Units.Hectokelvin);

    /// <summary>
    /// Creates a new <see cref="Temperature"/> from the specified value, expressed in kilokelvins
    /// </summary>
    /// <param name="value">The amount of kilokelvins to create a new <see cref="Temperature"/> for</param>
    /// <returns>A new <see cref="Temperature"/></returns>
    public static Temperature FromKilokelvins(decimal value) => new(value, Units.Kilokelvin);

    /// <summary>
    /// Adds the specified <see cref="Temperature"/>s
    /// </summary>
    /// <param name="temperature1">The <see cref="Temperature"/> to add the specified value to</param>
    /// <param name="temperature2">The value to add</param>
    /// <returns>The addition's result</returns>
    public static Temperature operator +(Temperature temperature1, Temperature temperature2) => temperature1.Add(temperature2);

    /// <summary>
    /// Subtracts the specified <see cref="Temperature"/>s
    /// </summary>
    /// <param name="temperature1">The <see cref="Temperature"/> to subtract the specified value from</param>
    /// <param name="temperature2">The value to subtract</param>
    /// <returns>The subtraction's result</returns>
    public static Temperature operator -(Temperature temperature1, Temperature temperature2) => temperature1.Subtract(temperature2);

    /// <summary>
    /// Multiplies the specified <see cref="Temperature"/>
    /// </summary>
    /// <param name="temperature">The <see cref="Temperature"/> to multiply</param>
    /// <param name="multiplier">The multiplier</param>
    /// <returns>The multiplication's result</returns>
    public static Temperature operator *(Temperature temperature, decimal multiplier) => new(temperature.Value * multiplier, temperature.Unit);

    /// <summary>
    /// Multiplies the specified <see cref="Temperature"/>
    /// </summary>
    /// <param name="temperature">The <see cref="Temperature"/> to multiply</param>
    /// <param name="multiplier">The multiplier</param>
    /// <returns>The multiplication's result</returns>
    public static Temperature operator *(Temperature temperature, int multiplier) => new(temperature.Value * multiplier, temperature.Unit);

    /// <summary>
    /// Divides the specified <see cref="Temperature"/>
    /// </summary>
    /// <param name="temperature">The <see cref="Temperature"/> to divide</param>
    /// <param name="divider">The divider</param>
    /// <returns>The division's result</returns>
    public static Temperature operator /(Temperature temperature, decimal divider) => new(temperature.Value / divider, temperature.Unit);

    /// <summary>
    /// Divides the specified <see cref="Temperature"/>
    /// </summary>
    /// <param name="temperature">The <see cref="Temperature"/> to divide</param>
    /// <param name="divider">The divider</param>
    /// <returns>The division's result</returns>
    public static Temperature operator /(Temperature temperature, int divider) => new(temperature.Value / divider, temperature.Unit);

    /// <summary>
    /// Checks whether or not a <see cref="Temperature"/> is lower than another one
    /// </summary>
    /// <param name="temperature1">The <see cref="Temperature"/> to check</param>
    /// <param name="temperature2">The <see cref="Temperature"/> to compare to</param>
    /// <returns>A boolean indicating whether or not a <see cref="Temperature"/> is lower than another one</returns>
    public static bool operator <(Temperature temperature1, Temperature temperature2) => temperature1.CompareTo(temperature2) < 0;

    /// <summary>
    /// Checks whether or not a <see cref="Temperature"/> is higher than another one
    /// </summary>
    /// <param name="temperature1">The <see cref="Temperature"/> to check</param>
    /// <param name="temperature2">The <see cref="Temperature"/> to compare to</param>
    /// <returns>A boolean indicating whether or not a <see cref="Temperature"/> is higher than another one</returns>
    public static bool operator >(Temperature temperature1, Temperature temperature2) => temperature1.CompareTo(temperature2) > 0;

    /// <summary>
    /// Exposes default temperature units
    /// </summary>
    public static class Units
    {

        /// <summary>
        /// Gets the <see cref="UnitOfMeasurement"/> to express millikelvins
        /// </summary>
        public static readonly UnitOfMeasurement Millikelvin = new(UnitOfMeasurementType.Temperature, "Millikelvin", "mK", 1000);
        /// <summary>
        /// Gets the <see cref="UnitOfMeasurement"/> to express centikelvins
        /// </summary>
        public static readonly UnitOfMeasurement Centikelvin = new(UnitOfMeasurementType.Temperature, "Centikelvin", "cK", 100);
        /// <summary>
        /// Gets the <see cref="UnitOfMeasurement"/> to express decikelvins
        /// </summary>
        public static readonly UnitOfMeasurement Decikelvin = new(UnitOfMeasurementType.Temperature, "Decikelvin", "dK", 10);
        /// <summary>
        /// Gets the <see cref="UnitOfMeasurement"/> to express kelvins
        /// </summary>
        public static readonly UnitOfMeasurement Kelvin = new(UnitOfMeasurementType.Temperature, "Kelvin", "K", 1);
        /// <summary>
        /// Gets the <see cref="UnitOfMeasurement"/> to express decakelvins
        /// </summary>
        public static readonly UnitOfMeasurement Decakelvin = new(UnitOfMeasurementType.Temperature, "Decakelvin", "daK", 0.1m);
        /// <summary>
        /// Gets the <see cref="UnitOfMeasurement"/> to express hectokelvins
        /// </summary>
        public static readonly UnitOfMeasurement Hectokelvin = new(UnitOfMeasurementType.Temperature, "Hectokelvin", "hK", 0.01m);
        /// <summary>
        /// Gets the <see cref="UnitOfMeasurement"/> to express kilokelvins
        /// </summary>
        public static readonly UnitOfMeasurement Kilokelvin = new(UnitOfMeasurementType.Temperature, "Kilokelvin", "kK", 0.001m);
        /// <summary>
        /// Gets the <see cref="UnitOfMeasurement"/> to express Celsius degrees
        /// </summary>
        public static readonly UnitOfMeasurement DegreeCelsius = new(UnitOfMeasurementType.Temperature, "Degree Celsius", "°C", 1);
        /// <summary>
        /// Gets the <see cref="UnitOfMeasurement"/> to express Fahrenheit degrees
        /// </summary>
        public static readonly UnitOfMeasurement DegreeFahrenheit = new(UnitOfMeasurementType.Temperature, "Degree Fahrenheit", "°F", 1);

        /// <summary>
        /// Gets a new <see cref="IEnumerable{T}"/> containing all default temperature units
        /// </summary>
        /// <returns>A new <see cref="IEnumerable{T}"/></returns>
        public static IEnumerable<UnitOfMeasurement> AsEnumerable()
        {
            yield return Millikelvin;
            yield return Centikelvin;
            yield return Decikelvin;
            yield return Kelvin;
            yield return Decakelvin;
            yield return Hectokelvin;
            yield return Kilokelvin;
        }

    }

}
