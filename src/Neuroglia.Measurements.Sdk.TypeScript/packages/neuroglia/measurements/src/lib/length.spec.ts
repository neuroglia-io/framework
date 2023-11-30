import { units } from "./known-unit-of-measurements";
import { Length } from "./length";
import { UnitOfMeasurementType } from "./enums";
import { create, all } from 'mathjs';

const math = create(all, {
  precision: 6,
  predictable: true
});

describe('Length Tests', () => {

  it('create length using parameters should work', () => {
    const value = 42;
    const unit = units.length.meter;

    const measurement = new Length(value, unit);

    expect(measurement).toBeDefined();
    expect(measurement.value).toBe(value);
    expect(measurement.unit).toEqual(unit);
  });

  it('create length from object should work', () => {
    const value = 42;
    const unit = units.length.meter;

    const measurement = new Length({ value, unit });

    expect(measurement).toBeDefined();
    expect(measurement.value).toBe(value);
    expect(measurement.unit).toEqual(unit);
  });

  it('create length energy should work', () => {
    const measurement = new Length();

    expect(measurement).toBeDefined();
    expect(measurement.value).toBeUndefined();
    expect(measurement.unit.type).toEqual('length');
  });

  it('create length using fromMillimeters should work', () => {
    const value = 42;
    const millimeters = Length.fromMillimeters(value);

    expect(millimeters).toBeDefined();
    expect(millimeters.value).toBe(value);
    expect(millimeters.unit).toEqual(units.length.millimeter);
  });

  it('create length using fromCentimeters should work', () => {
    const value = 42;
    const centimeters = Length.fromCentimeters(value);

    expect(centimeters).toBeDefined();
    expect(centimeters.value).toBe(value);
    expect(centimeters.unit).toEqual(units.length.centimeter);
  });

  it('create length using fromDecimeters should work', () => {
    const value = 42;
    const decimeters = Length.fromDecimeters(value);

    expect(decimeters).toBeDefined();
    expect(decimeters.value).toBe(value);
    expect(decimeters.unit).toEqual(units.length.decimeter);
  });

  it('create length using fromMeters should work', () => {
    const value = 42;
    const meters = Length.fromMeters(value);

    expect(meters).toBeDefined();
    expect(meters.value).toBe(value);
    expect(meters.unit).toEqual(units.length.meter);
  });

  it('create length using fromDecameters should work', () => {
    const value = 42;
    const hectometers = Length.fromDecameters(value);

    expect(hectometers).toBeDefined();
    expect(hectometers.value).toBe(value);
    expect(hectometers.unit).toEqual(units.length.decameter);
  });

  it('create length using fromHectometers should work', () => {
    const value = 42;
    const hectometers = Length.fromHectometers(value);

    expect(hectometers).toBeDefined();
    expect(hectometers.value).toBe(value);
    expect(hectometers.unit).toEqual(units.length.hectometer);
  });

  it('create length using fromKilometers should work', () => {
    const value = 42;
    const kilometers = Length.fromKilometers(value);

    expect(kilometers).toBeDefined();
    expect(kilometers.value).toBe(value);
    expect(kilometers.unit).toEqual(units.length.kilometer);
  });

  it('create length using fromInches should work', () => {
    const value = 42;
    const inches = Length.fromInches(value);

    expect(inches).toBeDefined();
    expect(inches.value).toBe(value);
    expect(inches.unit).toEqual(units.length.inch);
  });

  it('create length using fromFeet should work', () => {
    const value = 42;
    const feet = Length.fromFeet(value);

    expect(feet).toBeDefined();
    expect(feet.value).toBe(value);
    expect(feet.unit).toEqual(units.length.foot);
  });

  it('create length using fromYards should work', () => {
    const value = 42;
    const yards = Length.fromYards(value);

    expect(yards).toBeDefined();
    expect(yards.value).toBe(value);
    expect(yards.unit).toEqual(units.length.yard);
  });

  it('create length using fromMiles should work', () => {
    const value = 42;
    const miles = Length.fromMiles(value);

    expect(miles).toBeDefined();
    expect(miles.value).toBe(value);
    expect(miles.unit).toEqual(units.length.mile);
  });

  it('create length with incompatible unit type should throw', () => {
    const value = 42;
    const unit = units.capacity.liter

    expect(() => new Length(value, unit)).toThrow(new Error(`Invalid unit of measurement type '${unit.type}', expected '${UnitOfMeasurementType.Length}'.`));
  });

  it('access length properties should work', () => {
    const value = 42;
    const unit = units.length.meter;

    const measurement = new Length(value, unit);

    expect(measurement.millimeters).toBe(math.chain(units.length.millimeter.ratio).divide(unit.ratio).multiply(value).done());
    expect(measurement.centimeters).toBe(math.chain(units.length.centimeter.ratio).divide(unit.ratio).multiply(value).done());
    expect(measurement.decimeters).toBe(math.chain(units.length.decimeter.ratio).divide(unit.ratio).multiply(value).done());
    expect(measurement.meters).toBe(value);
    expect(measurement.base).toBe(value);
    expect(measurement.decameters).toBe(math.chain(units.length.decameter.ratio).divide(unit.ratio).multiply(value).done());
    expect(measurement.hectometers).toBe(math.chain(units.length.hectometer.ratio).divide(unit.ratio).multiply(value).done());
    expect(measurement.kilometers).toBe(math.chain(units.length.kilometer.ratio).divide(unit.ratio).multiply(value).done());
    expect(measurement.inches).toBe(math.chain(units.length.inch.ratio).divide(unit.ratio).multiply(value).done());
    expect(measurement.feet).toBe(math.chain(units.length.foot.ratio).divide(unit.ratio).multiply(value).done());
    expect(measurement.yards).toBe(math.chain(units.length.yard.ratio).divide(unit.ratio).multiply(value).done());
    expect(measurement.miles).toBe(math.chain(units.length.mile.ratio).divide(unit.ratio).multiply(value).done());
  });

  it('compare lengths should work', () => {
    const measurement1 = Length.fromCentimeters(250);
    const measurement2 = Length.fromMeters(5);
    const measurement3 = Length.fromDecimeters(50);

    expect(measurement1.compareTo(measurement2)).toBe(-1);
    expect(measurement2.compareTo(measurement1)).toBe(1);
    expect(measurement3.compareTo(measurement2)).toBe(0);
    expect(measurement3.compareTo(measurement3)).toBe(0);
    expect(measurement3.equals(measurement1)).toBeFalse();
    expect(measurement3.equals(measurement2)).toBeTrue();
    expect(measurement3.equals(measurement2, true)).toBeFalse();
    expect(measurement3.equals(measurement3, true)).toBeTrue();
  });

  it('convert length should work', () => {
    const measurement = Length.fromCentimeters(42);
    const convertionUnit = units.length.decameter;

    const convertedMeasurement = measurement.convertTo(convertionUnit);

    expect(convertedMeasurement.unit).toEqual(convertionUnit);
    expect(convertedMeasurement.value).toBe(math.chain(convertionUnit.ratio).divide(measurement.unit.ratio).multiply(measurement.value).done());
  });

  it('convert length to incompatible type should throw', () => {
    const measurement = Length.fromCentimeters(42);
    const convertionUnit = units.capacity.liter;

    expect(() => measurement.convertTo(convertionUnit)).toThrow(new Error(`Incompatible measurement types '${measurement.unit.type}' and '${convertionUnit.type}'`));
  });

  it('sum lengths with same units should work', () => {
    const value1 = 42;
    const value2 = 24;
    const measurement1 = Length.fromCentimeters(value1);
    const measurement2 = Length.fromCentimeters(value2);

    const result = measurement1.add(measurement2);

    expect(result.unit).toEqual(units.length.centimeter);
    expect(result.value).toBe(value1 + value2);
  });

  it('sum lengths with different units should work', () => {
    const value1 = 42;
    const value2 = 24;
    const unit1 = units.length.centimeter;
    const unit2 = units.length.meter;
    const measurement1 = new Length(value1, unit1);
    const measurement2 = new Length(value2, unit2);

    const result1 = measurement1.add(measurement2);
    const result2 = measurement2.add(measurement1);

    expect(result1.unit).toEqual(unit1);
    expect(result1.value).toBe(measurement1.centimeters + measurement2.centimeters);
    expect(result2.unit).toEqual(unit2);
    expect(result2.value).toBe(measurement2.meters + measurement1.meters);
  });

  it('subtract lengths with same units should work', () => {
    const value1 = 42;
    const value2 = 24;
    const measurement1 = Length.fromCentimeters(value1);
    const measurement2 = Length.fromCentimeters(value2);

    const result = measurement1.subtract(measurement2);

    expect(result.unit).toEqual(units.length.centimeter);
    expect(result.value).toBe(value1 - value2);
  });

  it('subtract lengths with different units should work', () => {
    const value1 = 42;
    const value2 = 24;
    const unit1 = units.length.centimeter;
    const unit2 = units.length.meter;
    const measurement1 = new Length(value1, unit1);
    const measurement2 = new Length(value2, unit2);

    const result1 = measurement1.subtract(measurement2);
    const result2 = measurement2.subtract(measurement1);

    expect(result1.unit).toEqual(unit1);
    expect(result1.value).toBe(measurement1.centimeters - measurement2.centimeters);
    expect(result2.unit).toEqual(unit2);
    expect(result2.value).toBe(measurement2.meters - measurement1.meters);
  });

  it('multiply lengths with same units should work', () => {
    const value1 = 42;
    const value2 = 24;
    const measurement1 = Length.fromCentimeters(value1);
    const measurement2 = Length.fromCentimeters(value2);

    const result = measurement1.multiply(measurement2);

    expect(result.unit).toEqual(units.length.centimeter);
    expect(result.value).toBe(math.multiply(value1, value2));
  });

  it('multiply lengths with different units should work', () => {
    const value1 = 42;
    const value2 = 24;
    const unit1 = units.length.centimeter;
    const unit2 = units.length.meter;
    const measurement1 = new Length(value1, unit1);
    const measurement2 = new Length(value2, unit2);

    const result1 = measurement1.multiply(measurement2);
    const result2 = measurement2.multiply(measurement1);

    expect(result1.unit).toEqual(unit1);
    expect(result1.value).toBe(math.multiply(measurement1.centimeters, measurement2.centimeters));
    expect(result2.unit).toEqual(unit2);
    expect(result2.value).toBe(math.multiply(measurement2.meters, measurement1.meters));
  });

  it('divide lengths with same units should work', () => {
    const value1 = 42;
    const value2 = 24;
    const measurement1 = Length.fromCentimeters(value1);
    const measurement2 = Length.fromCentimeters(value2);

    const result = measurement1.divide(measurement2);

    expect(result.unit).toEqual(units.length.centimeter);
    expect(result.value).toBe(math.divide(value1, value2));
  });

  it('divide lengths with different units should work', () => {
    const value1 = 42;
    const value2 = 24;
    const unit1 = units.length.centimeter;
    const unit2 = units.length.meter;
    const measurement1 = new Length(value1, unit1);
    const measurement2 = new Length(value2, unit2);

    const result1 = measurement1.divide(measurement2);
    const result2 = measurement2.divide(measurement1);

    expect(result1.unit).toEqual(unit1);
    expect(result1.value).toBe(math.divide(measurement1.centimeters, measurement2.centimeters));
    expect(result2.unit).toEqual(unit2);
    expect(result2.value).toBe(math.divide(measurement2.meters, measurement1.meters));
  });

});
