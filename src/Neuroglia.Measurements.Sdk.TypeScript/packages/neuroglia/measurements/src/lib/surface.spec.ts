import { units } from "./known-unit-of-measurements";
import { Surface } from "./surface";
import { UnitOfMeasurementType } from "./enums";
import { create, all } from 'mathjs';

const math = create(all, {
  precision: 6,
  predictable: true
});

describe('Surface Tests', () => {

  it('create surface using parameters should work', () => {
    const value = 42;
    const unit = units.surface.squareMeter;

    const measurement = new Surface(value, unit);

    expect(measurement).toBeDefined();
    expect(measurement.value).toBe(value);
    expect(measurement.unit).toEqual(unit);
  });

  it('create surface from object should work', () => {
    const value = 42;
    const unit = units.surface.squareMeter;

    const measurement = new Surface({ value, unit });

    expect(measurement).toBeDefined();
    expect(measurement.value).toBe(value);
    expect(measurement.unit).toEqual(unit);
  });

  it('create surface energy should work', () => {
    const measurement = new Surface();

    expect(measurement).toBeDefined();
    expect(measurement.value).toBeUndefined();
    expect(measurement.unit.type).toEqual('surface');
  });

  it('create surface using fromSquareMillimeters should work', () => {
    const value = 42;
    const squareMillimeters = Surface.fromSquareMillimeters(value);

    expect(squareMillimeters).toBeDefined();
    expect(squareMillimeters.value).toBe(value);
    expect(squareMillimeters.unit).toEqual(units.surface.squareMillimeter);
  });

  it('create surface using fromSquareCentimeters should work', () => {
    const value = 42;
    const squareCentimeters = Surface.fromSquareCentimeters(value);

    expect(squareCentimeters).toBeDefined();
    expect(squareCentimeters.value).toBe(value);
    expect(squareCentimeters.unit).toEqual(units.surface.squareCentimeter);
  });

  it('create surface using fromSquareDecimeters should work', () => {
    const value = 42;
    const squareDecimeters = Surface.fromSquareDecimeters(value);

    expect(squareDecimeters).toBeDefined();
    expect(squareDecimeters.value).toBe(value);
    expect(squareDecimeters.unit).toEqual(units.surface.squareDecimeter);
  });

  it('create surface using fromSquareMeters should work', () => {
    const value = 42;
    const squarMeters = Surface.fromSquareMeters(value);

    expect(squarMeters).toBeDefined();
    expect(squarMeters.value).toBe(value);
    expect(squarMeters.unit).toEqual(units.surface.squareMeter);
  });

  it('create surface using fromSquareDecameters should work', () => {
    const value = 42;
    const squareHectometers = Surface.fromSquareDecameters(value);

    expect(squareHectometers).toBeDefined();
    expect(squareHectometers.value).toBe(value);
    expect(squareHectometers.unit).toEqual(units.surface.squareDecameter);
  });

  it('create surface using fromSquareHectometers should work', () => {
    const value = 42;
    const squareHectometers = Surface.fromSquareHectometers(value);

    expect(squareHectometers).toBeDefined();
    expect(squareHectometers.value).toBe(value);
    expect(squareHectometers.unit).toEqual(units.surface.squareHectometer);
  });

  it('create surface using fromSquareKilometers should work', () => {
    const value = 42;
    const squareKilometers = Surface.fromSquareKilometers(value);

    expect(squareKilometers).toBeDefined();
    expect(squareKilometers.value).toBe(value);
    expect(squareKilometers.unit).toEqual(units.surface.squareKilometer);
  });

  it('create length using fromSquareInches should work', () => {
    const value = 42;
    const squareInches = Surface.fromSquareInches(value);

    expect(squareInches).toBeDefined();
    expect(squareInches.value).toBe(value);
    expect(squareInches.unit).toEqual(units.surface.squareInch);
  });

  it('create length using fromSquareFeet should work', () => {
    const value = 42;
    const squareFeet = Surface.fromSquareFeet(value);

    expect(squareFeet).toBeDefined();
    expect(squareFeet.value).toBe(value);
    expect(squareFeet.unit).toEqual(units.surface.squareFoot);
  });

  it('create length using fromSquareYards should work', () => {
    const value = 42;
    const squareYards = Surface.fromSquareYards(value);

    expect(squareYards).toBeDefined();
    expect(squareYards.value).toBe(value);
    expect(squareYards.unit).toEqual(units.surface.squareYard);
  });

  it('create length using fromSquareMiles should work', () => {
    const value = 42;
    const squareMiles = Surface.fromSquareMiles(value);

    expect(squareMiles).toBeDefined();
    expect(squareMiles.value).toBe(value);
    expect(squareMiles.unit).toEqual(units.surface.squareMile);
  });

  it('create surface with incompatible unit type should throw', () => {
    const value = 42;
    const unit = units.capacity.liter

    expect(() => new Surface(value, unit)).toThrow(new Error(`Invalid unit of measurement type '${unit.type}', expected '${UnitOfMeasurementType.Surface}'.`));
  });

  it('access surface properties should work', () => {
    const value = 42;
    const unit = units.surface.squareMeter;

    const measurement = new Surface(value, unit);

    expect(measurement.squareMillimeters).toBe(math.chain(units.surface.squareMillimeter.ratio).divide(unit.ratio).multiply(value).done());
    expect(measurement.squareCentimeters).toBe(math.chain(units.surface.squareCentimeter.ratio).divide(unit.ratio).multiply(value).done());
    expect(measurement.squareDecimeters).toBe(math.chain(units.surface.squareDecimeter.ratio).divide(unit.ratio).multiply(value).done());
    expect(measurement.squareMeters).toBe(value);
    expect(measurement.base).toBe(value);
    expect(measurement.squareDecameters).toBe(math.chain(units.surface.squareDecameter.ratio).divide(unit.ratio).multiply(value).done());
    expect(measurement.squareHectometers).toBe(math.chain(units.surface.squareHectometer.ratio).divide(unit.ratio).multiply(value).done());
    expect(measurement.squareKilometers).toBe(math.chain(units.surface.squareKilometer.ratio).divide(unit.ratio).multiply(value).done());
    expect(measurement.squareInches).toBe(math.chain(units.surface.squareInch.ratio).divide(unit.ratio).multiply(value).done());
    expect(measurement.squareFeet).toBe(math.chain(units.surface.squareFoot.ratio).divide(unit.ratio).multiply(value).done());
    expect(measurement.squareYards).toBe(math.chain(units.surface.squareYard.ratio).divide(unit.ratio).multiply(value).done());
    expect(measurement.squareMiles).toBe(math.chain(units.surface.squareMile.ratio).divide(unit.ratio).multiply(value).done());
  });

  it('compare surfaces should work', () => {
    const measurement1 = Surface.fromSquareCentimeters(250);
    const measurement2 = Surface.fromSquareMeters(5);
    const measurement3 = Surface.fromSquareDecimeters(50);

    expect(measurement1.compareTo(measurement2)).toBe(-1);
    expect(measurement2.compareTo(measurement1)).toBe(1);
    expect(measurement3.compareTo(measurement2)).toBe(0);
    expect(measurement3.compareTo(measurement3)).toBe(0);
    expect(measurement3.equals(measurement1)).toBeFalse();
    expect(measurement3.equals(measurement2)).toBeTrue();
    expect(measurement3.equals(measurement2, true)).toBeFalse();
    expect(measurement3.equals(measurement3, true)).toBeTrue();
  });

  it('convert surface should work', () => {
    const measurement = Surface.fromSquareCentimeters(42);
    const convertionUnit = units.surface.squareDecameter;

    const convertedMeasurement = measurement.convertTo(convertionUnit);

    expect(convertedMeasurement.unit).toEqual(convertionUnit);
    expect(convertedMeasurement.value).toBe(math.chain(convertionUnit.ratio).divide(measurement.unit.ratio).multiply(measurement.value).done());
  });

  it('convert surface to incompatible type should throw', () => {
    const measurement = Surface.fromSquareCentimeters(42);
    const convertionUnit = units.capacity.liter;

    expect(() => measurement.convertTo(convertionUnit)).toThrow(new Error(`Incompatible measurement types '${measurement.unit.type}' and '${convertionUnit.type}'`));
  });

  it('sum surfaces with same units should work', () => {
    const value1 = 42;
    const value2 = 24;
    const measurement1 = Surface.fromSquareCentimeters(value1);
    const measurement2 = Surface.fromSquareCentimeters(value2);

    const result = measurement1.add(measurement2);

    expect(result.unit).toEqual(units.surface.squareCentimeter);
    expect(result.value).toBe(value1 + value2);
  });

  it('sum surfaces with different units should work', () => {
    const value1 = 42;
    const value2 = 24;
    const unit1 = units.surface.squareCentimeter;
    const unit2 = units.surface.squareMeter;
    const measurement1 = new Surface(value1, unit1);
    const measurement2 = new Surface(value2, unit2);

    const result1 = measurement1.add(measurement2);
    const result2 = measurement2.add(measurement1);

    expect(result1.unit).toEqual(unit1);
    expect(result1.value).toBe(measurement1.squareCentimeters + measurement2.squareCentimeters);
    expect(result2.unit).toEqual(unit2);
    expect(result2.value).toBe(measurement2.squareMeters + measurement1.squareMeters);
  });

  it('subtract surfaces with same units should work', () => {
    const value1 = 42;
    const value2 = 24;
    const measurement1 = Surface.fromSquareCentimeters(value1);
    const measurement2 = Surface.fromSquareCentimeters(value2);

    const result = measurement1.subtract(measurement2);

    expect(result.unit).toEqual(units.surface.squareCentimeter);
    expect(result.value).toBe(value1 - value2);
  });

  it('subtract surfaces with different units should work', () => {
    const value1 = 42;
    const value2 = 24;
    const unit1 = units.surface.squareCentimeter;
    const unit2 = units.surface.squareMeter;
    const measurement1 = new Surface(value1, unit1);
    const measurement2 = new Surface(value2, unit2);

    const result1 = measurement1.subtract(measurement2);
    const result2 = measurement2.subtract(measurement1);

    expect(result1.unit).toEqual(unit1);
    expect(result1.value).toBe(measurement1.squareCentimeters - measurement2.squareCentimeters);
    expect(result2.unit).toEqual(unit2);
    expect(result2.value).toBe(measurement2.squareMeters - measurement1.squareMeters);
  });

  it('multiply surfaces with same units should work', () => {
    const value1 = 42;
    const value2 = 24;
    const measurement1 = Surface.fromSquareCentimeters(value1);
    const measurement2 = Surface.fromSquareCentimeters(value2);

    const result = measurement1.multiply(measurement2);

    expect(result.unit).toEqual(units.surface.squareCentimeter);
    expect(result.value).toBe(math.multiply(value1, value2));
  });

  it('multiply surfaces with different units should work', () => {
    const value1 = 42;
    const value2 = 24;
    const unit1 = units.surface.squareCentimeter;
    const unit2 = units.surface.squareMeter;
    const measurement1 = new Surface(value1, unit1);
    const measurement2 = new Surface(value2, unit2);

    const result1 = measurement1.multiply(measurement2);
    const result2 = measurement2.multiply(measurement1);

    expect(result1.unit).toEqual(unit1);
    expect(result1.value).toBe(math.multiply(measurement1.squareCentimeters, measurement2.squareCentimeters));
    expect(result2.unit).toEqual(unit2);
    expect(result2.value).toBe(math.multiply(measurement2.squareMeters, measurement1.squareMeters));
  });

  it('divide surfaces with same units should work', () => {
    const value1 = 42;
    const value2 = 24;
    const measurement1 = Surface.fromSquareCentimeters(value1);
    const measurement2 = Surface.fromSquareCentimeters(value2);

    const result = measurement1.divide(measurement2);

    expect(result.unit).toEqual(units.surface.squareCentimeter);
    expect(result.value).toBe(math.divide(value1, value2));
  });

  it('divide surfaces with different units should work', () => {
    const value1 = 42;
    const value2 = 24;
    const unit1 = units.surface.squareCentimeter;
    const unit2 = units.surface.squareMeter;
    const measurement1 = new Surface(value1, unit1);
    const measurement2 = new Surface(value2, unit2);

    const result1 = measurement1.divide(measurement2);
    const result2 = measurement2.divide(measurement1);

    expect(result1.unit).toEqual(unit1);
    expect(result1.value).toBe(math.divide(measurement1.squareCentimeters, measurement2.squareCentimeters));
    expect(result2.unit).toEqual(unit2);
    expect(result2.value).toBe(math.divide(measurement2.squareMeters, measurement1.squareMeters));
  });

});
