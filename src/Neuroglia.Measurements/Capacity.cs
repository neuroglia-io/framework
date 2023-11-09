using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Neuroglia.Measurements;

/// <summary>
/// Represents the measurement of a capacity
/// </summary>
[DataContract]
public class Capacity
    : Measurement, IComparable<Capacity>
{

    /// <summary>
    /// Initializes a new <see cref="Capacity"/>
    /// </summary>
    [JsonConstructor]
    protected Capacity() { }

    /// <summary>
    /// Initializes a new <see cref="Capacity"/>
    /// </summary>
    /// <param name="value">The measured value</param>
    /// <param name="unit">The unit the measure's value is expressed in</param>
    public Capacity(decimal value, UnitOfMeasurement unit) : base(value, unit == null ? throw new ArgumentNullException(nameof(unit)) : unit.Type == UnitOfMeasurementType.Capacity ? unit : throw new ArgumentException("The specified unit must be of type 'capacity'", nameof(unit))) { }

    /// <summary>
    /// Gets the total amount of milliliters
    /// </summary>
    public virtual decimal Milliliters => this.Value * (Units.Milliliter.Ratio / this.Unit.Ratio);

    /// <summary>
    /// Gets the total amount of centiliters
    /// </summary>
    public virtual decimal Centiliters => this.Value * (Units.Centiliter.Ratio / this.Unit.Ratio);

    /// <summary>
    /// Gets the total amount of deciliters
    /// </summary>
    public virtual decimal Deciliters => this.Value * (Units.Deciliter.Ratio / this.Unit.Ratio);

    /// <summary>
    /// Gets the total amount of liters
    /// </summary>
    public virtual decimal Liters => this.Value * (Units.Liter.Ratio / this.Unit.Ratio);

    /// <summary>
    /// Gets the total amount of decaliters
    /// </summary>
    public virtual decimal Decaliters => this.Value * (Units.Decaliter.Ratio / this.Unit.Ratio);

    /// <summary>
    /// Gets the total amount of hectoliters
    /// </summary>
    public virtual decimal Hectoliters => this.Value * (Units.Hectoliter.Ratio / this.Unit.Ratio);

    /// <summary>
    /// Gets the total amount of kiloliters
    /// </summary>
    public virtual decimal Kiloliters => this.Value * (Units.Kiloliter.Ratio / this.Unit.Ratio);

    /// <summary>
    /// Gets the total amount of U.S. fluid drams
    /// </summary>
    public virtual decimal USFluidDrams => this.Value * (Units.USFluidDram.Ratio / this.Unit.Ratio);
    
    /// <summary>
    /// Gets the total amount of U.S. fluid ounces
    /// </summary>
    public virtual decimal USFluidOunces => this.Value * (Units.USFluidOunce.Ratio / this.Unit.Ratio);
    
    /// <summary>
    /// Gets the total amount of U.S. gills
    /// </summary>
    public virtual decimal USGills => this.Value * (Units.USGill.Ratio / this.Unit.Ratio);
    
    /// <summary>
    /// Gets the total amount of U.S. pints
    /// </summary>
    public virtual decimal USPints => this.Value * (Units.USPint.Ratio / this.Unit.Ratio);
    
    /// <summary>
    /// Gets the total amount of U.S. quarts
    /// </summary>
    public virtual decimal USQuarts => this.Value * (Units.USQuart.Ratio / this.Unit.Ratio);
    
    /// <summary>
    /// Gets the total amount of U.S. gallons
    /// </summary>
    public virtual decimal USGallons => this.Value * (Units.USGallon.Ratio / this.Unit.Ratio);
    
    /// <summary>
    /// Gets the total amount of U.S. pecks
    /// </summary>
    public virtual decimal USPecks => this.Value * (Units.USPeck.Ratio / this.Unit.Ratio);
    
    /// <summary>
    /// Gets the total amount of U.S. bushels
    /// </summary>
    public virtual decimal USBushels => this.Value * (Units.USBushel.Ratio / this.Unit.Ratio);

    /// <summary>
    /// Adds the specified <see cref="Capacity"/>
    /// </summary>
    /// <param name="capacity">The <see cref="Capacity"/> to add</param>
    /// <returns>A new <see cref="Capacity"/> resulting from the addition</returns>
    public virtual Capacity Add(Capacity capacity)
    {
        if (capacity == null) throw new ArgumentNullException(nameof(capacity));
        if (capacity.Unit.Type != this.Unit.Type) throw new ArgumentNullException("Cannot compare capacities with different types of unit", nameof(capacity));
        return new Capacity(this.Value + capacity.Value * (this.Unit.Ratio / capacity.Unit.Ratio), this.Unit);
    }

    /// <summary>
    /// Subtracts the specified <see cref="Capacity"/>
    /// </summary>
    /// <param name="capacity">The <see cref="Capacity"/> to subtract</param>
    /// <returns>A new <see cref="Capacity"/> resulting from the subtraction</returns>
    public virtual Capacity Subtract(Capacity capacity)
    {
        if (capacity == null) throw new ArgumentNullException(nameof(capacity));
        if (capacity.Unit.Type != this.Unit.Type) throw new ArgumentNullException("Cannot compare capacities with different types of unit", nameof(capacity));
        return new Capacity(this.Value - capacity.Value * (this.Unit.Ratio / capacity.Unit.Ratio), this.Unit);
    }

    /// <inheritdoc/>
    public virtual int CompareTo(Capacity? other) => this.Liters.CompareTo(other?.Liters);

    /// <summary>
    /// Converts the <see cref="Capacity"/> into a new <see cref="Capacity"/> measured using the specified <see cref="UnitOfMeasurement"/>
    /// </summary>
    /// <param name="unit">The <see cref="UnitOfMeasurement"/> to convert the <see cref="Capacity"/> to</param>
    /// <returns>A new <see cref="Capacity"/> measured using the specified <see cref="UnitOfMeasurement"/></returns>
    public virtual Capacity ConvertTo(UnitOfMeasurement unit)
    {
        if (unit == null) throw new ArgumentNullException(nameof(unit));
        if (unit.Type != UnitOfMeasurementType.Capacity) throw new ArgumentException("The specified unit of measurement must be of type 'capacity'", nameof(unit));
        return new(this.Value * (unit.Ratio / this.Unit.Ratio), unit);
    }

    /// <summary>
    /// Creates a new <see cref="Capacity"/> from the specified value, expressed in milliliters
    /// </summary>
    /// <param name="value">The amount of milliliters to create a new <see cref="Capacity"/> for</param>
    /// <returns>A new <see cref="Capacity"/></returns>
    public static Capacity FromMilliliters(decimal value) => new(value, Units.Milliliter);

    /// <summary>
    /// Creates a new <see cref="Capacity"/> from the specified value, expressed in centiliters
    /// </summary>
    /// <param name="value">The amount of centiliters to create a new <see cref="Capacity"/> for</param>
    /// <returns>A new <see cref="Capacity"/></returns>
    public static Capacity FromCentiliters(decimal value) => new(value, Units.Centiliter);

    /// <summary>
    /// Creates a new <see cref="Capacity"/> from the specified value, expressed in deciliters
    /// </summary>
    /// <param name="value">The amount of deciliters to create a new <see cref="Capacity"/> for</param>
    /// <returns>A new <see cref="Capacity"/></returns>
    public static Capacity FromDeciliters(decimal value) => new(value, Units.Deciliter);

    /// <summary>
    /// Creates a new <see cref="Capacity"/> from the specified value, expressed in liters
    /// </summary>
    /// <param name="value">The amount of liters to create a new <see cref="Capacity"/> for</param>
    /// <returns>A new <see cref="Capacity"/></returns>
    public static Capacity FromLiters(decimal value) => new(value, Units.Liter);

    /// <summary>
    /// Creates a new <see cref="Capacity"/> from the specified value, expressed in decaliters
    /// </summary>
    /// <param name="value">The amount of decaliters to create a new <see cref="Capacity"/> for</param>
    /// <returns>A new <see cref="Capacity"/></returns>
    public static Capacity FromDecaliters(decimal value) => new(value, Units.Decaliter);

    /// <summary>
    /// Creates a new <see cref="Capacity"/> from the specified value, expressed in hectoliters
    /// </summary>
    /// <param name="value">The amount of hectoliters to create a new <see cref="Capacity"/> for</param>
    /// <returns>A new <see cref="Capacity"/></returns>
    public static Capacity FromHectoliters(decimal value) => new(value, Units.Hectoliter);

    /// <summary>
    /// Creates a new <see cref="Capacity"/> from the specified value, expressed in kiloliters
    /// </summary>
    /// <param name="value">The amount of kiloliters to create a new <see cref="Capacity"/> for</param>
    /// <returns>A new <see cref="Capacity"/></returns>
    public static Capacity FromKiloliters(decimal value) => new(value, Units.Kiloliter);

    /// <summary>
    /// Creates a new <see cref="Capacity"/> from the specified value, expressed in U.S. fluid drams
    /// </summary>
    /// <param name="value">The amount of U.S. fluid drams to create a new <see cref="Capacity"/> for</param>
    /// <returns>A new <see cref="Capacity"/></returns>
    public static Capacity FromUSFluidDrams(decimal value) => new(value, Units.USFluidDram);

    /// <summary>
    /// Creates a new <see cref="Capacity"/> from the specified value, expressed in U.S. fluid ounces
    /// </summary>
    /// <param name="value">The amount of U.S. fluid ounces to create a new <see cref="Capacity"/> for</param>
    /// <returns>A new <see cref="Capacity"/></returns>
    public static Capacity FromUSFluidOunces(decimal value) => new(value, Units.USFluidOunce);

    /// <summary>
    /// Creates a new <see cref="Capacity"/> from the specified value, expressed in U.S. gills
    /// </summary>
    /// <param name="value">The amount of U.S. gills to create a new <see cref="Capacity"/> for</param>
    /// <returns>A new <see cref="Capacity"/></returns>
    public static Capacity FromUSGills(decimal value) => new(value, Units.USGill);

    /// <summary>
    /// Creates a new <see cref="Capacity"/> from the specified value, expressed in U.S. pints
    /// </summary>
    /// <param name="value">The amount of U.S. pints to create a new <see cref="Capacity"/> for</param>
    /// <returns>A new <see cref="Capacity"/></returns>
    public static Capacity FromUSPints(decimal value) => new(value, Units.USPint);

    /// <summary>
    /// Creates a new <see cref="Capacity"/> from the specified value, expressed in U.S. quarts
    /// </summary>
    /// <param name="value">The amount of U.S. quarts to create a new <see cref="Capacity"/> for</param>
    /// <returns>A new <see cref="Capacity"/></returns>
    public static Capacity FromUSQuarts(decimal value) => new(value, Units.USQuart);

    /// <summary>
    /// Creates a new <see cref="Capacity"/> from the specified value, expressed in U.S. gallons
    /// </summary>
    /// <param name="value">The amount of U.S. gallons to create a new <see cref="Capacity"/> for</param>
    /// <returns>A new <see cref="Capacity"/></returns>
    public static Capacity FromUSGallons(decimal value) => new(value, Units.USGallon);

    /// <summary>
    /// Creates a new <see cref="Capacity"/> from the specified value, expressed in U.S. pecks
    /// </summary>
    /// <param name="value">The amount of U.S. pecks to create a new <see cref="Capacity"/> for</param>
    /// <returns>A new <see cref="Capacity"/></returns>
    public static Capacity FromUSPecks(decimal value) => new(value, Units.USPeck);

    /// <summary>
    /// Creates a new <see cref="Capacity"/> from the specified value, expressed in U.S. bushels
    /// </summary>
    /// <param name="value">The amount of U.S. pecks to create a new <see cref="Capacity"/> for</param>
    /// <returns>A new <see cref="Capacity"/></returns>
    public static Capacity FromUSBushels(decimal value) => new(value, Units.USBushel);

    /// <summary>
    /// Adds the specified <see cref="Capacity"/>s
    /// </summary>
    /// <param name="capacity1">The <see cref="Capacity"/> to add the specified value to</param>
    /// <param name="capacity2">The value to add</param>
    /// <returns>The addition's result</returns>
    public static Capacity operator +(Capacity capacity1, Capacity capacity2) => capacity1.Add(capacity2);

    /// <summary>
    /// Subtracts the specified <see cref="Capacity"/>s
    /// </summary>
    /// <param name="capacity1">The <see cref="Capacity"/> to subtract the specified value from</param>
    /// <param name="capacity2">The value to subtract</param>
    /// <returns>The subtraction's result</returns>
    public static Capacity operator -(Capacity capacity1, Capacity capacity2) => capacity1.Subtract(capacity2);

    /// <summary>
    /// Multiplies the specified <see cref="Capacity"/>
    /// </summary>
    /// <param name="capacity">The <see cref="Capacity"/> to multiply</param>
    /// <param name="multiplier">The multiplier</param>
    /// <returns>The multiplication's result</returns>
    public static Capacity operator *(Capacity capacity, decimal multiplier) => new(capacity.Value * multiplier, capacity.Unit);

    /// <summary>
    /// Multiplies the specified <see cref="Capacity"/>
    /// </summary>
    /// <param name="capacity">The <see cref="Capacity"/> to multiply</param>
    /// <param name="multiplier">The multiplier</param>
    /// <returns>The multiplication's result</returns>
    public static Capacity operator *(Capacity capacity, int multiplier) => new(capacity.Value * multiplier, capacity.Unit);

    /// <summary>
    /// Divides the specified <see cref="Capacity"/>
    /// </summary>
    /// <param name="capacity">The <see cref="Capacity"/> to divide</param>
    /// <param name="divider">The divider</param>
    /// <returns>The division's result</returns>
    public static Capacity operator /(Capacity capacity, decimal divider) => new(capacity.Value / divider, capacity.Unit);

    /// <summary>
    /// Divides the specified <see cref="Capacity"/>
    /// </summary>
    /// <param name="capacity">The <see cref="Capacity"/> to divide</param>
    /// <param name="divider">The divider</param>
    /// <returns>The division's result</returns>
    public static Capacity operator /(Capacity capacity, int divider) => new(capacity.Value / divider, capacity.Unit);

    /// <summary>
    /// Exposes default capacity units
    /// </summary>
    public static class Units
    {

        /// <summary>
        /// Gets the <see cref="UnitOfMeasurement"/> to express milliliters
        /// </summary>
        public static readonly UnitOfMeasurement Milliliter = new(UnitOfMeasurementType.Capacity, "Milliliter", "ml", 1000);
        /// <summary>
        /// Gets the <see cref="UnitOfMeasurement"/> to express centiliters
        /// </summary>
        public static readonly UnitOfMeasurement Centiliter = new(UnitOfMeasurementType.Capacity, "Centiliter", "cl", 100);
        /// <summary>
        /// Gets the <see cref="UnitOfMeasurement"/> to express deciliters
        /// </summary>
        public static readonly UnitOfMeasurement Deciliter = new(UnitOfMeasurementType.Capacity, "Deciliter", "dl", 10);
        /// <summary>
        /// Gets the <see cref="UnitOfMeasurement"/> to express liters
        /// </summary>
        public static readonly UnitOfMeasurement Liter = new(UnitOfMeasurementType.Capacity, "Liter", "l", 1);
        /// <summary>
        /// Gets the <see cref="UnitOfMeasurement"/> to express decaliters
        /// </summary>
        public static readonly UnitOfMeasurement Decaliter = new(UnitOfMeasurementType.Capacity, "Decaliter", "dal", 0.1m);
        /// <summary>
        /// Gets the <see cref="UnitOfMeasurement"/> to express hectoliters
        /// </summary>
        public static readonly UnitOfMeasurement Hectoliter = new(UnitOfMeasurementType.Capacity, "Hectoliter", "hl", 0.01m);
        /// <summary>
        /// Gets the <see cref="UnitOfMeasurement"/> to express kiloliters
        /// </summary>
        public static readonly UnitOfMeasurement Kiloliter = new(UnitOfMeasurementType.Capacity, "Kiloliter", "kl", 0.001m);

        /// <summary>
        /// Gets the <see cref="UnitOfMeasurement"/> to express fluid drams
        /// </summary>
        public static readonly UnitOfMeasurement USFluidDram = new(UnitOfMeasurementType.Capacity, "Fluid dram", "fl dr (US)", 270.51218161m);
        /// <summary>
        /// Gets the <see cref="UnitOfMeasurement"/> to express U.S. fluid ounces
        /// </summary>
        public static readonly UnitOfMeasurement USFluidOunce = new(UnitOfMeasurementType.Capacity, "Fluid ounce", "fl oz (US)", 33.8140227018m);
        /// <summary>
        /// Gets the <see cref="UnitOfMeasurement"/> to U.S. express gills
        /// </summary>
        public static readonly UnitOfMeasurement USGill = new(UnitOfMeasurementType.Capacity, "Gill", "gi (US)", 8.4535056755m);
        /// <summary>
        /// Gets the <see cref="UnitOfMeasurement"/> to express U.S. pints
        /// </summary>
        public static readonly UnitOfMeasurement USPint = new(UnitOfMeasurementType.Capacity, "Pint", "pt (US)", 2.1133764189m);
        /// <summary>
        /// Gets the <see cref="UnitOfMeasurement"/> to express U.S. quarts
        /// </summary>
        public static readonly UnitOfMeasurement USQuart = new(UnitOfMeasurementType.Capacity, "Quart", "qt (US)", 1.05668821m);
        /// <summary>
        /// Gets the <see cref="UnitOfMeasurement"/> to express U.S. gallons
        /// </summary>
        public static readonly UnitOfMeasurement USGallon = new(UnitOfMeasurementType.Capacity, "Gallon", "gal (US)", 0.2641720524m);
        /// <summary>
        /// Gets the <see cref="UnitOfMeasurement"/> to express U.S. pecks
        /// </summary>
        public static readonly UnitOfMeasurement USPeck = new(UnitOfMeasurementType.Capacity, "Peck", "pk (US)", 0.11351037228269m);
        /// <summary>
        /// Gets the <see cref="UnitOfMeasurement"/> to express U.S. bushels
        /// </summary>
        public static readonly UnitOfMeasurement USBushel = new(UnitOfMeasurementType.Capacity, "Bushel", "bu (US)", 0.028377593070673m);

        /// <summary>
        /// Gets a new <see cref="IEnumerable{T}"/> containing all default volume units
        /// </summary>
        /// <returns>A new <see cref="IEnumerable{T}"/></returns>
        public static IEnumerable<UnitOfMeasurement> AsEnumerable()
        {
            yield return Milliliter;
            yield return Centiliter;
            yield return Deciliter;
            yield return Liter;
            yield return Decaliter;
            yield return Hectoliter;
            yield return Kiloliter;
        }

    }

}