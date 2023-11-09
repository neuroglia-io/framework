using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Neuroglia.Measurements;

/// <summary>
/// Represents the measurement of a length
/// </summary>
[DataContract]
public class Length
    : Measurement, IComparable<Length>
{

    /// <summary>
    /// Initializes a new <see cref="Measurement"/>
    /// </summary>
    [JsonConstructor]
    protected Length() { }

    /// <summary>
    /// Initializes a new <see cref="Length"/>
    /// </summary>
    /// <param name="value">The measured value</param>
    /// <param name="unit">The unit the measure's value is expressed in</param>
    public Length(decimal value, UnitOfMeasurement unit) : base(value, unit == null ? throw new ArgumentNullException(nameof(unit)) : unit.Type == UnitOfMeasurementType.Length ? unit : throw new ArgumentException("The specified unit must be of type 'length'", nameof(unit))) { }

    /// <summary>
    /// Gets the total amount of millimeters
    /// </summary>
    public virtual decimal Millimeters => this.Value * (Units.Millimeter.Ratio / this.Unit.Ratio);

    /// <summary>
    /// Gets the total amount of centimeters
    /// </summary>
    public virtual decimal Centimeters => this.Value * (Units.Centimeter.Ratio / this.Unit.Ratio);

    /// <summary>
    /// Gets the total amount of decimeters
    /// </summary>
    public virtual decimal Decimeters => this.Value * (Units.Decimeter.Ratio / this.Unit.Ratio);

    /// <summary>
    /// Gets the total amount of meters
    /// </summary>
    public virtual decimal Meters => this.Value * (Units.Meter.Ratio / this.Unit.Ratio);

    /// <summary>
    /// Gets the total amount of decameters
    /// </summary>
    public virtual decimal Decameters => this.Value * (Units.Decameter.Ratio / this.Unit.Ratio);

    /// <summary>
    /// Gets the total amount of hectometers
    /// </summary>
    public virtual decimal Hectometers => this.Value * (Units.Hectometer.Ratio / this.Unit.Ratio);

    /// <summary>
    /// Gets the total amount of kilometers
    /// </summary>
    public virtual decimal Kilometers => this.Value * (Units.Kilometer.Ratio / this.Unit.Ratio);

    /// <summary>
    /// Gets the total amount of inches
    /// </summary>
    public virtual decimal Inches => this.Value * (Units.Inch.Ratio / this.Unit.Ratio);

    /// <summary>
    /// Gets the total amount of feet
    /// </summary>
    public virtual decimal Feet => this.Value * (Units.Foot.Ratio / this.Unit.Ratio);

    /// <summary>
    /// Gets the total amount of yards
    /// </summary>
    public virtual decimal Yards => this.Value * (Units.Yard.Ratio / this.Unit.Ratio);

    /// <summary>
    /// Gets the total amount of miles
    /// </summary>
    public virtual decimal Miles => this.Value * (Units.Mile.Ratio / this.Unit.Ratio);

    /// <summary>
    /// Adds the specified <see cref="Length"/>
    /// </summary>
    /// <param name="length">The <see cref="Length"/> to add</param>
    /// <returns>A new <see cref="Length"/> resulting from the addition</returns>
    public virtual Length Add(Length length)
    {
        if (length == null) throw new ArgumentNullException(nameof(length));
        if (length.Unit.Type != this.Unit.Type) throw new ArgumentNullException("Cannot compare lengths with different types of unit", nameof(length));
        return new Length(this.Value + length.Value * (this.Unit.Ratio / length.Unit.Ratio), this.Unit);
    }

    /// <summary>
    /// Subtracts the specified <see cref="Length"/>
    /// </summary>
    /// <param name="length">The <see cref="Length"/> to subtract</param>
    /// <returns>A new <see cref="Length"/> resulting from the subtraction</returns>
    public virtual Length Subtract(Length length)
    {
        if (length == null) throw new ArgumentNullException(nameof(length));
        if (length.Unit.Type != this.Unit.Type) throw new ArgumentNullException("Cannot compare lengths with different types of unit", nameof(length));
        return new Length(this.Value - length.Value * (this.Unit.Ratio / length.Unit.Ratio), this.Unit);
    }

    /// <inheritdoc/>
    public virtual int CompareTo(Length? other) => this.Meters.CompareTo(other?.Meters);

    /// <summary>
    /// Converts the <see cref="Length"/> into a new <see cref="Length"/> measured using the specified <see cref="UnitOfMeasurement"/>
    /// </summary>
    /// <param name="unit">The <see cref="UnitOfMeasurement"/> to convert the <see cref="Length"/> to</param>
    /// <returns>A new <see cref="Length"/> measured using the specified <see cref="UnitOfMeasurement"/></returns>
    public virtual Length ConvertTo(UnitOfMeasurement unit)
    {
        if(unit == null) throw new ArgumentNullException(nameof(unit));
        if (unit.Type != UnitOfMeasurementType.Length) throw new ArgumentException("The specified unit of measurement must be of type 'length'", nameof(unit));
        return new(this.Value * (unit.Ratio / this.Unit.Ratio), unit);
    }

    /// <summary>
    /// Creates a new <see cref="Length"/> from the specified value, expressed in millimeters
    /// </summary>
    /// <param name="value">The amount of millimeters to create a new <see cref="Length"/> for</param>
    /// <returns>A new <see cref="Length"/></returns>
    public static Length FromMillimeters(decimal value) => new(value, Units.Millimeter);

    /// <summary>
    /// Creates a new <see cref="Length"/> from the specified value, expressed in centimeters
    /// </summary>
    /// <param name="value">The amount of centimeters to create a new <see cref="Length"/> for</param>
    /// <returns>A new <see cref="Length"/></returns>
    public static Length FromCentimeters(decimal value) => new(value, Units.Centimeter);

    /// <summary>
    /// Creates a new <see cref="Length"/> from the specified value, expressed in decimeters
    /// </summary>
    /// <param name="value">The amount of decimeters to create a new <see cref="Length"/> for</param>
    /// <returns>A new <see cref="Length"/></returns>
    public static Length FromDecimeters(decimal value) => new(value, Units.Decimeter);

    /// <summary>
    /// Creates a new <see cref="Length"/> from the specified value, expressed in meters
    /// </summary>
    /// <param name="value">The amount of meters to create a new <see cref="Length"/> for</param>
    /// <returns>A new <see cref="Length"/></returns>
    public static Length FromMeters(decimal value) => new(value, Units.Meter);

    /// <summary>
    /// Creates a new <see cref="Length"/> from the specified value, expressed in decameters
    /// </summary>
    /// <param name="value">The amount of decameters to create a new <see cref="Length"/> for</param>
    /// <returns>A new <see cref="Length"/></returns>
    public static Length FromDecameters(decimal value) => new(value, Units.Decameter);

    /// <summary>
    /// Creates a new <see cref="Length"/> from the specified value, expressed in hectometers
    /// </summary>
    /// <param name="value">The amount of hectometers to create a new <see cref="Length"/> for</param>
    /// <returns>A new <see cref="Length"/></returns>
    public static Length FromHectometers(decimal value) => new(value, Units.Hectometer);

    /// <summary>
    /// Creates a new <see cref="Length"/> from the specified value, expressed in kilometers
    /// </summary>
    /// <param name="value">The amount of kilometers to create a new <see cref="Length"/> for</param>
    /// <returns>A new <see cref="Length"/></returns>
    public static Length FromKilometers(decimal value) => new(value, Units.Kilometer);

    /// <summary>
    /// Creates a new <see cref="Length"/> from the specified value, expressed in inches
    /// </summary>
    /// <param name="value">The amount of inches to create a new <see cref="Length"/> for</param>
    /// <returns>A new <see cref="Length"/></returns>
    public static Length FromInches(decimal value) => new(value, Units.Inch);

    /// <summary>
    /// Creates a new <see cref="Length"/> from the specified value, expressed in feet
    /// </summary>
    /// <param name="value">The amount of feet to create a new <see cref="Length"/> for</param>
    /// <returns>A new <see cref="Length"/></returns>
    public static Length FromFeet(decimal value) => new(value, Units.Foot);

    /// <summary>
    /// Creates a new <see cref="Length"/> from the specified value, expressed in yards
    /// </summary>
    /// <param name="value">The amount of yards to create a new <see cref="Length"/> for</param>
    /// <returns>A new <see cref="Length"/></returns>
    public static Length FromYards(decimal value) => new(value, Units.Yard);

    /// <summary>
    /// Creates a new <see cref="Length"/> from the specified value, expressed in miles
    /// </summary>
    /// <param name="value">The amount of miles to create a new <see cref="Length"/> for</param>
    /// <returns>A new <see cref="Length"/></returns>
    public static Length FromMiles(decimal value) => new(value, Units.Mile);

    /// <summary>
    /// Adds the specified <see cref="Length"/>s
    /// </summary>
    /// <param name="length1">The <see cref="Length"/> to add the specified value to</param>
    /// <param name="length2">The value to add</param>
    /// <returns>The addition's result</returns>
    public static Length operator +(Length length1, Length length2) => length1.Add(length2);

    /// <summary>
    /// Subtracts the specified <see cref="Length"/>s
    /// </summary>
    /// <param name="length1">The <see cref="Length"/> to subtract the specified value from</param>
    /// <param name="length2">The value to subtract</param>
    /// <returns>The subtraction's result</returns>
    public static Length operator -(Length length1, Length length2) => length1.Subtract(length2);

    /// <summary>
    /// Multiplies the specified <see cref="Length"/>
    /// </summary>
    /// <param name="length">The <see cref="Length"/> to multiply</param>
    /// <param name="multiplier">The multiplier</param>
    /// <returns>The multiplication's result</returns>
    public static Length operator *(Length length, decimal multiplier) => new(length.Value * multiplier, length.Unit);

    /// <summary>
    /// Multiplies the specified <see cref="Length"/>
    /// </summary>
    /// <param name="length">The <see cref="Length"/> to multiply</param>
    /// <param name="multiplier">The multiplier</param>
    /// <returns>The multiplication's result</returns>
    public static Length operator *(Length length, int multiplier) => new(length.Value * multiplier, length.Unit);

    /// <summary>
    /// Divides the specified <see cref="Length"/>
    /// </summary>
    /// <param name="length">The <see cref="Length"/> to divide</param>
    /// <param name="divider">The divider</param>
    /// <returns>The division's result</returns>
    public static Length operator /(Length length, decimal divider) => new(length.Value / divider, length.Unit);

    /// <summary>
    /// Divides the specified <see cref="Length"/>
    /// </summary>
    /// <param name="length">The <see cref="Length"/> to divide</param>
    /// <param name="divider">The divider</param>
    /// <returns>The division's result</returns>
    public static Length operator /(Length length, int divider) => new(length.Value / divider, length.Unit);

    /// <summary>
    /// Exposes default length units
    /// </summary>
    public static class Units
    {

        /// <summary>
        /// Gets the <see cref="UnitOfMeasurement"/> to express millimeters
        /// </summary>
        public static readonly UnitOfMeasurement Millimeter = new(UnitOfMeasurementType.Length, "Millimeter", "mm", 1000m);
        /// <summary>
        /// Gets the <see cref="UnitOfMeasurement"/> to express centimeters
        /// </summary>
        public static readonly UnitOfMeasurement Centimeter = new(UnitOfMeasurementType.Length, "Centimeter", "cm", 100m);
        /// <summary>
        /// Gets the <see cref="UnitOfMeasurement"/> to express decimeters
        /// </summary>
        public static readonly UnitOfMeasurement Decimeter = new(UnitOfMeasurementType.Length, "Decimeter", "dm", 10m);
        /// <summary>
        /// Gets the <see cref="UnitOfMeasurement"/> to express meters
        /// </summary>
        public static readonly UnitOfMeasurement Meter = new(UnitOfMeasurementType.Length, "Meter", "m", 1);
        /// <summary>
        /// Gets the <see cref="UnitOfMeasurement"/> to express decameters
        /// </summary>
        public static readonly UnitOfMeasurement Decameter = new(UnitOfMeasurementType.Length, "Decameter", "dam", 0.1m);
        /// <summary>
        /// Gets the <see cref="UnitOfMeasurement"/> to express hectometers
        /// </summary>
        public static readonly UnitOfMeasurement Hectometer = new(UnitOfMeasurementType.Length, "Hectometer", "hm", 0.01m);
        /// <summary>
        /// Gets the <see cref="UnitOfMeasurement"/> to express kilometers
        /// </summary>
        public static readonly UnitOfMeasurement Kilometer = new(UnitOfMeasurementType.Length, "Kilometer", "km", 0.001m);
        /// <summary>
        /// Gets the <see cref="UnitOfMeasurement"/> to express inches
        /// </summary>
        public static readonly UnitOfMeasurement Inch = new(UnitOfMeasurementType.Length, "Inch", "in", 39.3700787402m);
        /// <summary>
        /// Gets the <see cref="UnitOfMeasurement"/> to express feet
        /// </summary>
        public static readonly UnitOfMeasurement Foot = new(UnitOfMeasurementType.Length, "Foot", "ft", 3.28084m);
        /// <summary>
        /// Gets the <see cref="UnitOfMeasurement"/> to express yards
        /// </summary>
        public static readonly UnitOfMeasurement Yard = new(UnitOfMeasurementType.Length, "Yard", "yd", 1.09361m);
        /// <summary>
        /// Gets the <see cref="UnitOfMeasurement"/> to express miles
        /// </summary>
        public static readonly UnitOfMeasurement Mile = new(UnitOfMeasurementType.Length, "Mile", "mi", 0.000621371m);

        /// <summary>
        /// Gets a new <see cref="IEnumerable{T}"/> containing all default length units
        /// </summary>
        /// <returns>A new <see cref="IEnumerable{T}"/></returns>
        public static IEnumerable<UnitOfMeasurement> AsEnumerable()
        {
            yield return Millimeter;
            yield return Centimeter;
            yield return Decimeter;
            yield return Meter;
            yield return Decameter;
            yield return Hectometer;
            yield return Kilometer;
            yield return Inch;
            yield return Foot;
            yield return Yard;
            yield return Mile;
        }

    }

}
