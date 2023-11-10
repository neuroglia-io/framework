using System.Diagnostics.Metrics;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Neuroglia.Measurements;

/// <summary>
/// Represents a measure
/// </summary>
[DataContract]
public class Measurement
    : IEquatable<Measurement>, IComparable<Measurement>
{

    /// <summary>
    /// Initializes a new <see cref="Measurement"/>
    /// </summary>
    [JsonConstructor]
    protected Measurement() { }

    /// <summary>
    /// Initializes a new <see cref="Measurement"/>
    /// </summary>
    /// <param name="value">The measured value</param>
    /// <param name="unit">The unit the measure's value is expressed in</param>
    public Measurement(decimal value, UnitOfMeasurement unit)
    {
        this.Value = value;
        this.Unit = unit ?? throw new ArgumentNullException(nameof(unit));
    }

    /// <summary>
    /// Gets/sets the measured value
    /// </summary>
    [DataMember]
    public virtual decimal Value { get; set; }

    /// <summary>
    /// Gets/sets the unit the measure's value is expressed in
    /// </summary>
    [DataMember]
    public virtual UnitOfMeasurement Unit { get; set; } = null!;

    /// <summary>
    /// Adds the specified <see cref="Measurement"/>
    /// </summary>
    /// <param name="measurement">The <see cref="Measurement"/> to add</param>
    /// <returns>A new <see cref="Measurement"/> resulting from the addition</returns>
    public virtual Measurement Add(Measurement measurement)
    {
        if (measurement == null) throw new ArgumentNullException(nameof(measurement));
        if (measurement.Unit.Type != this.Unit.Type) throw new ArgumentNullException("Cannot compare measurements with different types of unit", nameof(measurement));
        return new Measurement(this.Value + measurement.Value * (this.Unit.Ratio / measurement.Unit.Ratio), this.Unit);
    }

    /// <summary>
    /// Subtracts the specified <see cref="Measurement"/>
    /// </summary>
    /// <param name="measurement">The <see cref="Measurement"/> to subtract</param>
    /// <returns>A new <see cref="Measurement"/> resulting from the subtraction</returns>
    public virtual Measurement Subtract(Measurement measurement)
    {
        if (measurement == null) throw new ArgumentNullException(nameof(measurement));
        if (measurement.Unit.Type != this.Unit.Type) throw new ArgumentNullException("Cannot compare measurements with different types of unit", nameof(measurement));
        return new Measurement(this.Value - measurement.Value * (this.Unit.Ratio / measurement.Unit.Ratio), this.Unit);
    }

    /// <summary>
    /// Determines whether or not the <see cref="Measurement"/> equals the specified one
    /// </summary>
    /// <param name="other">The <see cref="Measurement"/> to compare</param>
    /// <param name="strict">A boolean indicating whether or not to perform a strict comparison. If false, measures with different units are compared (ex: 100 cm vs 1 m)</param>
    /// <returns></returns>
    public virtual bool Equals(Measurement? other, bool strict)
    {
        if (other == null || other.Unit.Type != this.Unit.Type) return false;
        if(strict) return this.Value == other.Value && this.Unit == other.Unit;
        var value1 = this.Value * (1m / this.Unit.Ratio);
        var value2 = other.Value * (1m / other.Unit.Ratio);
        return value1 == value2;
    }

    /// <inheritdoc/>
    public virtual bool Equals(Measurement? other) => this.Equals(other, false);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => this.Equals(obj as Measurement);

    /// <inheritdoc/>
    public override int GetHashCode() => (this.Value * (1m / this.Unit.Ratio)).GetHashCode() * this.Unit.Type.GetHashCode();

    /// <inheritdoc/>
    public virtual int CompareTo(Measurement? other)
    {
        if (other == null) return this.Value.CompareTo(other?.Value);
        if (other.Unit.Type != this.Unit.Type) throw new ArgumentNullException("Cannot compare measurements with different types of unit", nameof(other));
        var value1 = this.Value * (1m / this.Unit.Ratio);
        var value2 = other.Value * (1m / other.Unit.Ratio);
        return value1.CompareTo(value2);
    }

    /// <inheritdoc/>
    public override string ToString() => $"{this.Value} {this.Unit}";

    /// <summary>
    /// Adds the specified <see cref="Measurement"/>s
    /// </summary>
    /// <param name="measurement1">The <see cref="Measurement"/> to add the specified value to</param>
    /// <param name="measurement2">The value to add</param>
    /// <returns>The addition's result</returns>
    public static Measurement operator +(Measurement measurement1, Measurement measurement2) => measurement1.Add(measurement2);

    /// <summary>
    /// Subtracts the specified <see cref="Measurement"/>s
    /// </summary>
    /// <param name="measurement1">The <see cref="Measurement"/> to subtract the specified value from</param>
    /// <param name="measurement2">The value to subtract</param>
    /// <returns>The subtraction's result</returns>
    public static Measurement operator -(Measurement measurement1, Measurement measurement2) => measurement1.Subtract(measurement2);

    /// <summary>
    /// Multiplies the specified <see cref="Measurement"/>
    /// </summary>
    /// <param name="measurement">The <see cref="Measurement"/> to multiply</param>
    /// <param name="multiplier">The multiplier</param>
    /// <returns>The multiplication's result</returns>
    public static Measurement operator *(Measurement measurement, decimal multiplier) => new(measurement.Value * multiplier, measurement.Unit);

    /// <summary>
    /// Multiplies the specified <see cref="Measurement"/>
    /// </summary>
    /// <param name="measurement">The <see cref="Measurement"/> to multiply</param>
    /// <param name="multiplier">The multiplier</param>
    /// <returns>The multiplication's result</returns>
    public static Measurement operator *(Measurement measurement, int multiplier) => new(measurement.Value * multiplier, measurement.Unit);

    /// <summary>
    /// Divides the specified <see cref="Measurement"/>
    /// </summary>
    /// <param name="measurement">The <see cref="Measurement"/> to divide</param>
    /// <param name="divider">The divider</param>
    /// <returns>The division's result</returns>
    public static Measurement operator /(Measurement measurement, decimal divider) => new(measurement.Value / divider, measurement.Unit);

    /// <summary>
    /// Divides the specified <see cref="Measurement"/>
    /// </summary>
    /// <param name="measurement">The <see cref="Measurement"/> to divide</param>
    /// <param name="divider">The divider</param>
    /// <returns>The division's result</returns>
    public static Measurement operator /(Measurement measurement, int divider) => new(measurement.Value / divider, measurement.Unit);

    /// <summary>
    /// Checks whether or not a <see cref="Measurement"/> is lower than another one
    /// </summary>
    /// <param name="measurement1">The <see cref="Measurement"/> to check</param>
    /// <param name="measurement2">The <see cref="Measurement"/> to compare to</param>
    /// <returns>A boolean indicating whether or not a <see cref="Measurement"/> is lower than another one</returns>
    public static bool operator <(Measurement measurement1, Measurement measurement2) => measurement1.CompareTo(measurement2) < 0;

    /// <summary>
    /// Checks whether or not a <see cref="Measurement"/> is higher than another one
    /// </summary>
    /// <param name="measurement1">The <see cref="Measurement"/> to check</param>
    /// <param name="measurement2">The <see cref="Measurement"/> to compare to</param>
    /// <returns>A boolean indicating whether or not a <see cref="Measurement"/> is higher than another one</returns>
    public static bool operator >(Measurement measurement1, Measurement measurement2) => measurement1.CompareTo(measurement2) > 0;

}
