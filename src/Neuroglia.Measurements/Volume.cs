using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Neuroglia.Measurements;

/// <summary>
/// Represents the measurement of a volume
/// </summary>
[DataContract]
public class Volume
    : Measurement, IComparable<Volume>
{

    /// <summary>
    /// Initializes a new <see cref="Volume"/>
    /// </summary>
    [JsonConstructor]
    protected Volume() { }

    /// <summary>
    /// Initializes a new <see cref="Volume"/>
    /// </summary>
    /// <param name="value">The measured value</param>
    /// <param name="unit">The unit the measure's value is expressed in</param>
    public Volume(decimal value, UnitOfMeasurement unit) : base(value, unit == null ? throw new ArgumentNullException(nameof(unit)) : unit.Type == UnitOfMeasurementType.Volume ? unit : throw new ArgumentException("The specified unit must be of type 'volume'", nameof(unit))) { }

    /// <summary>
    /// Gets the total amount of cubic millimeters
    /// </summary>
    public virtual decimal CubicMillimeters => this.Value * (Units.CubicMillimeter.Ratio / this.Unit.Ratio);

    /// <summary>
    /// Gets the total amount of cubic centimeters
    /// </summary>
    public virtual decimal CubicCentimeters => this.Value * (Units.CubicCentimeter.Ratio / this.Unit.Ratio);

    /// <summary>
    /// Gets the total amount of cubic decimeters
    /// </summary>
    public virtual decimal CubicDecimeters => this.Value * (Units.CubicDecimeter.Ratio / this.Unit.Ratio);

    /// <summary>
    /// Gets the total amount of cubic meters
    /// </summary>
    public virtual decimal CubicMeters => this.Value * (Units.CubicMeter.Ratio / this.Unit.Ratio);

    /// <summary>
    /// Gets the total amount of cubic decameters
    /// </summary>
    public virtual decimal CubicDecameters => this.Value * (Units.CubicDecameter.Ratio / this.Unit.Ratio);

    /// <summary>
    /// Gets the total amount of cubic hectometers
    /// </summary>
    public virtual decimal CubicHectometers => this.Value * (Units.CubicHectometer.Ratio / this.Unit.Ratio);

    /// <summary>
    /// Gets the total amount of cubic kilometers
    /// </summary>
    public virtual decimal CubicKilometers => this.Value * (Units.CubicKilometer.Ratio / this.Unit.Ratio);

    /// <summary>
    /// Gets the total amount of cubic inches
    /// </summary>
    public virtual decimal CubicInches => this.Value * (Units.CubicInch.Ratio / this.Unit.Ratio);

    /// <summary>
    /// Gets the total amount of cubic feet
    /// </summary>
    public virtual decimal CubicFeet => this.Value * (Units.CubicFoot.Ratio / this.Unit.Ratio);

    /// <summary>
    /// Gets the total amount of cubic yards
    /// </summary>
    public virtual decimal CubicYards => this.Value * (Units.CubicYard.Ratio / this.Unit.Ratio);

    /// <summary>
    /// Gets the total amount of cubic miles
    /// </summary>
    public virtual decimal CubicMiles => this.Value * (Units.CubicMile.Ratio / this.Unit.Ratio);

    /// <summary>
    /// Gets the total amount of fluid ounces
    /// </summary>
    public virtual decimal FluidOunces => this.Value * (Units.FluidOunce.Ratio / this.Unit.Ratio);

    /// <summary>
    /// Gets the total amount of imperial gallons
    /// </summary>
    public virtual decimal ImperialGallons => this.Value * (Units.ImperialGallon.Ratio / this.Unit.Ratio);

    /// <summary>
    /// Gets the total amount of U.S. gallons
    /// </summary>
    public virtual decimal USGallons => this.Value * (Units.USGallon.Ratio / this.Unit.Ratio);

    /// <summary>
    /// Adds the specified <see cref="Volume"/>
    /// </summary>
    /// <param name="volume">The <see cref="Volume"/> to add</param>
    /// <returns>A new <see cref="Volume"/> resulting from the addition</returns>
    public virtual Volume Add(Volume volume)
    {
        if (volume == null) throw new ArgumentNullException(nameof(volume));
        if (volume.Unit.Type != this.Unit.Type) throw new ArgumentNullException("Cannot compare volumes with different types of unit", nameof(volume));
        return new Volume(this.Value + volume.Value * (this.Unit.Ratio / volume.Unit.Ratio), this.Unit);
    }

    /// <summary>
    /// Subtracts the specified <see cref="Volume"/>
    /// </summary>
    /// <param name="volume">The <see cref="Volume"/> to subtract</param>
    /// <returns>A new <see cref="Volume"/> resulting from the subtraction</returns>
    public virtual Volume Subtract(Volume volume)
    {
        if (volume == null) throw new ArgumentNullException(nameof(volume));
        if (volume.Unit.Type != this.Unit.Type) throw new ArgumentNullException("Cannot compare volumes with different types of unit", nameof(volume));
        return new Volume(this.Value - volume.Value * (this.Unit.Ratio / volume.Unit.Ratio), this.Unit);
    }

    /// <inheritdoc/>
    public virtual int CompareTo(Volume? other) => this.CubicMeters.CompareTo(other?.CubicMeters);

    /// <summary>
    /// Converts the <see cref="Volume"/> into a new <see cref="Volume"/> measured using the specified <see cref="UnitOfMeasurement"/>
    /// </summary>
    /// <param name="unit">The <see cref="UnitOfMeasurement"/> to convert the <see cref="Volume"/> to</param>
    /// <returns>A new <see cref="Volume"/> measured using the specified <see cref="UnitOfMeasurement"/></returns>
    public virtual Volume ConvertTo(UnitOfMeasurement unit)
    {
        if (unit == null) throw new ArgumentNullException(nameof(unit));
        if (unit.Type != UnitOfMeasurementType.Volume) throw new ArgumentException("The specified unit of measurement must be of type 'volume'", nameof(unit));
        return new(this.Value * (unit.Ratio / this.Unit.Ratio), unit);
    }

    /// <summary>
    /// Creates a new <see cref="Volume"/> from the specified value, expressed in cubic millimeters
    /// </summary>
    /// <param name="value">The amount of millimeters to create a new <see cref="Volume"/> for</param>
    /// <returns>A new <see cref="Volume"/></returns>
    public static Volume FromCubicMillimeters(decimal value) => new(value, Units.CubicMillimeter);

    /// <summary>
    /// Creates a new <see cref="Volume"/> from the specified value, expressed in cubic centimeters
    /// </summary>
    /// <param name="value">The amount of centimeters to create a new <see cref="Volume"/> for</param>
    /// <returns>A new <see cref="Volume"/></returns>
    public static Volume FromCubicCentimeters(decimal value) => new(value, Units.CubicCentimeter);

    /// <summary>
    /// Creates a new <see cref="Volume"/> from the specified value, expressed in cubic decimeters
    /// </summary>
    /// <param name="value">The amount of decimeters to create a new <see cref="Volume"/> for</param>
    /// <returns>A new <see cref="Volume"/></returns>
    public static Volume FromCubicDecimeters(decimal value) => new(value, Units.CubicDecimeter);

    /// <summary>
    /// Creates a new <see cref="Volume"/> from the specified value, expressed in cubic meters
    /// </summary>
    /// <param name="value">The amount of meters to create a new <see cref="Volume"/> for</param>
    /// <returns>A new <see cref="Volume"/></returns>
    public static Volume FromCubicMeters(decimal value) => new(value, Units.CubicMeter);

    /// <summary>
    /// Creates a new <see cref="Volume"/> from the specified value, expressed in cubic decameters
    /// </summary>
    /// <param name="value">The amount of decameters to create a new <see cref="Volume"/> for</param>
    /// <returns>A new <see cref="Volume"/></returns>
    public static Volume FromCubicDecameters(decimal value) => new(value, Units.CubicDecameter);

    /// <summary>
    /// Creates a new <see cref="Volume"/> from the specified value, expressed in cubic hectometers
    /// </summary>
    /// <param name="value">The amount of hectometers to create a new <see cref="Volume"/> for</param>
    /// <returns>A new <see cref="Volume"/></returns>
    public static Volume FromCubicHectometers(decimal value) => new(value, Units.CubicHectometer);

    /// <summary>
    /// Creates a new <see cref="Volume"/> from the specified value, expressed in cubic kilometers
    /// </summary>
    /// <param name="value">The amount of kilometers to create a new <see cref="Volume"/> for</param>
    /// <returns>A new <see cref="Volume"/></returns>
    public static Volume FromCubicKilometers(decimal value) => new(value, Units.CubicKilometer);

    /// <summary>
    /// Creates a new <see cref="Volume"/> from the specified value, expressed in cubic inches
    /// </summary>
    /// <param name="value">The amount of inches to create a new <see cref="Volume"/> for</param>
    /// <returns>A new <see cref="Volume"/></returns>
    public static Volume FromCubicInches(decimal value) => new(value, Units.CubicInch);

    /// <summary>
    /// Creates a new <see cref="Volume"/> from the specified value, expressed in cubic feet
    /// </summary>
    /// <param name="value">The amount of feet to create a new <see cref="Volume"/> for</param>
    /// <returns>A new <see cref="Volume"/></returns>
    public static Volume FromCubicFeet(decimal value) => new(value, Units.CubicFoot);

    /// <summary>
    /// Creates a new <see cref="Volume"/> from the specified value, expressed in cubic yards
    /// </summary>
    /// <param name="value">The amount of yards to create a new <see cref="Volume"/> for</param>
    /// <returns>A new <see cref="Volume"/></returns>
    public static Volume FromCubicYards(decimal value) => new(value, Units.CubicYard);

    /// <summary>
    /// Creates a new <see cref="Volume"/> from the specified value, expressed in cubic miles
    /// </summary>
    /// <param name="value">The amount of miles to create a new <see cref="Volume"/> for</param>
    /// <returns>A new <see cref="Volume"/></returns>
    public static Volume FromCubicMiles(decimal value) => new(value, Units.CubicMile);

    /// <summary>
    /// Creates a new <see cref="Volume"/> from the specified value, expressed in fluid ounces
    /// </summary>
    /// <param name="value">The amount of miles to create a new <see cref="Volume"/> for</param>
    /// <returns>A new <see cref="Volume"/></returns>
    public static Volume FromFluidOunces(decimal value) => new(value, Units.FluidOunce);

    /// <summary>
    /// Creates a new <see cref="Volume"/> from the specified value, expressed in imperial gallons
    /// </summary>
    /// <param name="value">The amount of miles to create a new <see cref="Volume"/> for</param>
    /// <returns>A new <see cref="Volume"/></returns>
    public static Volume FromImperialGallons(decimal value) => new(value, Units.ImperialGallon);

    /// <summary>
    /// Creates a new <see cref="Volume"/> from the specified value, expressed in U.S. gallons
    /// </summary>
    /// <param name="value">The amount of miles to create a new <see cref="Volume"/> for</param>
    /// <returns>A new <see cref="Volume"/></returns>
    public static Volume FromUSGallons(decimal value) => new(value, Units.USGallon);

    /// <summary>
    /// Adds the specified <see cref="Volume"/>s
    /// </summary>
    /// <param name="volume1">The <see cref="Volume"/> to add the specified value to</param>
    /// <param name="volume2">The value to add</param>
    /// <returns>The addition's result</returns>
    public static Volume operator +(Volume volume1, Volume volume2) => volume1.Add(volume2);

    /// <summary>
    /// Subtracts the specified <see cref="Volume"/>s
    /// </summary>
    /// <param name="volume1">The <see cref="Volume"/> to subtract the specified value from</param>
    /// <param name="volume2">The value to subtract</param>
    /// <returns>The subtraction's result</returns>
    public static Volume operator -(Volume volume1, Volume volume2) => volume1.Subtract(volume2);

    /// <summary>
    /// Multiplies the specified <see cref="Volume"/>
    /// </summary>
    /// <param name="volume">The <see cref="Volume"/> to multiply</param>
    /// <param name="multiplier">The multiplier</param>
    /// <returns>The multiplication's result</returns>
    public static Volume operator *(Volume volume, decimal multiplier) => new(volume.Value * multiplier, volume.Unit);

    /// <summary>
    /// Multiplies the specified <see cref="Volume"/>
    /// </summary>
    /// <param name="volume">The <see cref="Volume"/> to multiply</param>
    /// <param name="multiplier">The multiplier</param>
    /// <returns>The multiplication's result</returns>
    public static Volume operator *(Volume volume, int multiplier) => new(volume.Value * multiplier, volume.Unit);

    /// <summary>
    /// Divides the specified <see cref="Volume"/>
    /// </summary>
    /// <param name="volume">The <see cref="Volume"/> to divide</param>
    /// <param name="divider">The divider</param>
    /// <returns>The division's result</returns>
    public static Volume operator /(Volume volume, decimal divider) => new(volume.Value / divider, volume.Unit);

    /// <summary>
    /// Divides the specified <see cref="Volume"/>
    /// </summary>
    /// <param name="volume">The <see cref="Volume"/> to divide</param>
    /// <param name="divider">The divider</param>
    /// <returns>The division's result</returns>
    public static Volume operator /(Volume volume, int divider) => new(volume.Value / divider, volume.Unit);

    /// <summary>
    /// Checks whether or not a <see cref="Volume"/> is lower than another one
    /// </summary>
    /// <param name="volume1">The <see cref="Volume"/> to check</param>
    /// <param name="volume2">The <see cref="Volume"/> to compare to</param>
    /// <returns>A boolean indicating whether or not a <see cref="Volume"/> is lower than another one</returns>
    public static bool operator <(Volume volume1, Volume volume2) => volume1.CompareTo(volume2) < 0;

    /// <summary>
    /// Checks whether or not a <see cref="Volume"/> is higher than another one
    /// </summary>
    /// <param name="volume1">The <see cref="Volume"/> to check</param>
    /// <param name="volume2">The <see cref="Volume"/> to compare to</param>
    /// <returns>A boolean indicating whether or not a <see cref="Volume"/> is higher than another one</returns>
    public static bool operator >(Volume volume1, Volume volume2) => volume1.CompareTo(volume2) > 0;

    /// <summary>
    /// Exposes default volume units
    /// </summary>
    public static class Units
    {

        /// <summary>
        /// Gets the <see cref="UnitOfMeasurement"/> to express cubic millimeters
        /// </summary>
        public static readonly UnitOfMeasurement CubicMillimeter = new(UnitOfMeasurementType.Volume, "Cubic millimeter", "mm³", 1000m);
        /// <summary>
        /// Gets the <see cref="UnitOfMeasurement"/> to express cubic centimeters
        /// </summary>
        public static readonly UnitOfMeasurement CubicCentimeter = new(UnitOfMeasurementType.Volume, "Cubic centimeter", "cm³", 100m);
        /// <summary>
        /// Gets the <see cref="UnitOfMeasurement"/> to express cubic decimeters
        /// </summary>
        public static readonly UnitOfMeasurement CubicDecimeter = new(UnitOfMeasurementType.Volume, "Cubic decimeter", "dm³", 10m);
        /// <summary>
        /// Gets the <see cref="UnitOfMeasurement"/> to express cubic millimeters
        /// </summary>
        public static readonly UnitOfMeasurement CubicMeter = new(UnitOfMeasurementType.Volume, "Cubic meter", "m³", 1);
        /// <summary>
        /// Gets the <see cref="UnitOfMeasurement"/> to express cubic decameters
        /// </summary>
        public static readonly UnitOfMeasurement CubicDecameter = new(UnitOfMeasurementType.Volume, "Cubic decameter", "dam³", 0.1m);
        /// <summary>
        /// Gets the <see cref="UnitOfMeasurement"/> to express cubic hectometers
        /// </summary>
        public static readonly UnitOfMeasurement CubicHectometer = new(UnitOfMeasurementType.Volume, "Cubic hectometer", "hm³", 0.01m);
        /// <summary>
        /// Gets the <see cref="UnitOfMeasurement"/> to express cubic kilometers
        /// </summary>
        public static readonly UnitOfMeasurement CubicKilometer = new(UnitOfMeasurementType.Volume, "Cubic kilometer", "km³", 0.001m);
        /// <summary>
        /// Gets the <see cref="UnitOfMeasurement"/> to express cubic inches
        /// </summary>
        public static readonly UnitOfMeasurement CubicInch = new(UnitOfMeasurementType.Volume, "Cubic inch", "in³", 61023.7441m);
        /// <summary>
        /// Gets the <see cref="UnitOfMeasurement"/> to express cubic feet
        /// </summary>
        public static readonly UnitOfMeasurement CubicFoot = new(UnitOfMeasurementType.Volume, "Cubic foot", "ft³", 35.314667m);
        /// <summary>
        /// Gets the <see cref="UnitOfMeasurement"/> to express cubic yards
        /// </summary>
        public static readonly UnitOfMeasurement CubicYard = new(UnitOfMeasurementType.Volume, "Cubic yard", "yd³", 1.307951m);
        /// <summary>
        /// Gets the <see cref="UnitOfMeasurement"/> to express cubic miles
        /// </summary>
        public static readonly UnitOfMeasurement CubicMile = new(UnitOfMeasurementType.Volume, "Cubic mile", "mi³", 2.3991274853161m);
        /// <summary>
        /// Gets the <see cref="UnitOfMeasurement"/> to express fluid ounces
        /// </summary>
        public static readonly UnitOfMeasurement FluidOunce = new(UnitOfMeasurementType.Volume, "Fluid ounce", "fl oz", 33814.022702m);
        /// <summary>
        /// Gets the <see cref="UnitOfMeasurement"/> to express imperial gallons
        /// </summary>
        public static readonly UnitOfMeasurement ImperialGallon = new(UnitOfMeasurementType.Volume, "Imperial gallon", "imp gal", 219.96924829909m);
        /// <summary>
        /// Gets the <see cref="UnitOfMeasurement"/> to express U.S. gallons
        /// </summary>
        public static readonly UnitOfMeasurement USGallon = new(UnitOfMeasurementType.Volume, "U.S. gallon", "gal", 264.172052m);

        /// <summary>
        /// Gets a new <see cref="IEnumerable{T}"/> containing all default volume units
        /// </summary>
        /// <returns>A new <see cref="IEnumerable{T}"/></returns>
        public static IEnumerable<UnitOfMeasurement> AsEnumerable()
        {
            yield return CubicMillimeter;
            yield return CubicCentimeter;
            yield return CubicCentimeter;
            yield return CubicMeter;
            yield return CubicDecameter;
            yield return CubicHectometer;
            yield return CubicKilometer;
            yield return CubicInch;
            yield return CubicFoot;
            yield return CubicYard;
            yield return CubicMile;
            yield return FluidOunce;
            yield return ImperialGallon;
            yield return USGallon;
        }

    }

}
