import { units } from "./known-unit-of-measurements";
import { Volume } from "./volume";
import { UnitOfMeasurementType } from "./enums";
import { create, all } from 'mathjs';

const math = create(all, {
  precision: 6,
  predictable: true
});

describe('Volume Tests', () => {

  it('create volume using parameters should work', () => {
    const value = 42;
    const unit = units.volume.cubicMeter;

    const measurement = new Volume(value, unit);

    expect(measurement).toBeDefined();
    expect(measurement.value).toBe(value);
    expect(measurement.unit).toEqual(unit);
  });

  it('create volume from object should work', () => {
    const value = 42;
    const unit = units.volume.cubicMeter;

    const measurement = new Volume({ value, unit });

    expect(measurement).toBeDefined();
    expect(measurement.value).toBe(value);
    expect(measurement.unit).toEqual(unit);
  });

  it('create volume energy should work', () => {
    const measurement = new Volume();

    expect(measurement).toBeDefined();
    expect(measurement.value).toBeUndefined();
    expect(measurement.unit.type).toEqual('volume');
  });

  it('create volume using fromCubicMillimeters should work', () => {
    const value = 42;
    const cubicMillimeters = Volume.fromCubicMillimeters(value);

    expect(cubicMillimeters).toBeDefined();
    expect(cubicMillimeters.value).toBe(value);
    expect(cubicMillimeters.unit).toEqual(units.volume.cubicMillimeter);
  });

  it('create volume using fromCubicCentimeters should work', () => {
    const value = 42;
    const cubicCentimeters = Volume.fromCubicCentimeters(value);

    expect(cubicCentimeters).toBeDefined();
    expect(cubicCentimeters.value).toBe(value);
    expect(cubicCentimeters.unit).toEqual(units.volume.cubicCentimeter);
  });

  it('create volume using fromCubicDecimeters should work', () => {
    const value = 42;
    const cubicDecimeters = Volume.fromCubicDecimeters(value);

    expect(cubicDecimeters).toBeDefined();
    expect(cubicDecimeters.value).toBe(value);
    expect(cubicDecimeters.unit).toEqual(units.volume.cubicDecimeter);
  });

  it('create volume using fromCubicMeters should work', () => {
    const value = 42;
    const squarMeters = Volume.fromCubicMeters(value);

    expect(squarMeters).toBeDefined();
    expect(squarMeters.value).toBe(value);
    expect(squarMeters.unit).toEqual(units.volume.cubicMeter);
  });

  it('create volume using fromCubicDecameters should work', () => {
    const value = 42;
    const cubicHectometers = Volume.fromCubicDecameters(value);

    expect(cubicHectometers).toBeDefined();
    expect(cubicHectometers.value).toBe(value);
    expect(cubicHectometers.unit).toEqual(units.volume.cubicDecameter);
  });

  it('create volume using fromCubicHectometers should work', () => {
    const value = 42;
    const cubicHectometers = Volume.fromCubicHectometers(value);

    expect(cubicHectometers).toBeDefined();
    expect(cubicHectometers.value).toBe(value);
    expect(cubicHectometers.unit).toEqual(units.volume.cubicHectometer);
  });

  it('create volume using fromCubicKilometers should work', () => {
    const value = 42;
    const cubicKilometers = Volume.fromCubicKilometers(value);

    expect(cubicKilometers).toBeDefined();
    expect(cubicKilometers.value).toBe(value);
    expect(cubicKilometers.unit).toEqual(units.volume.cubicKilometer);
  });

  it('create length using fromCubicInches should work', () => {
    const value = 42;
    const cubicInches = Volume.fromCubicInches(value);

    expect(cubicInches).toBeDefined();
    expect(cubicInches.value).toBe(value);
    expect(cubicInches.unit).toEqual(units.volume.cubicInch);
  });

  it('create length using fromCubicFeet should work', () => {
    const value = 42;
    const cubicFeet = Volume.fromCubicFeet(value);

    expect(cubicFeet).toBeDefined();
    expect(cubicFeet.value).toBe(value);
    expect(cubicFeet.unit).toEqual(units.volume.cubicFoot);
  });

  it('create length using fromCubicYards should work', () => {
    const value = 42;
    const cubicYards = Volume.fromCubicYards(value);

    expect(cubicYards).toBeDefined();
    expect(cubicYards.value).toBe(value);
    expect(cubicYards.unit).toEqual(units.volume.cubicYard);
  });

  it('create length using fromCubicMiles should work', () => {
    const value = 42;
    const cubicMiles = Volume.fromCubicMiles(value);

    expect(cubicMiles).toBeDefined();
    expect(cubicMiles.value).toBe(value);
    expect(cubicMiles.unit).toEqual(units.volume.cubicMile);
  });

  it('create length using fromFluidOunces should work', () => {
    const value = 42;
    const fluidOunces = Volume.fromFluidOunces(value);

    expect(fluidOunces).toBeDefined();
    expect(fluidOunces.value).toBe(value);
    expect(fluidOunces.unit).toEqual(units.volume.fluidOunce);
  });

  it('create length using fromImperialGallons should work', () => {
    const value = 42;
    const imperialGallons = Volume.fromImperialGallons(value);

    expect(imperialGallons).toBeDefined();
    expect(imperialGallons.value).toBe(value);
    expect(imperialGallons.unit).toEqual(units.volume.imperialGallon);
  });

  it('create length using fromUSGallons should work', () => {
    const value = 42;
    const usGallons = Volume.fromUSGallons(value);

    expect(usGallons).toBeDefined();
    expect(usGallons.value).toBe(value);
    expect(usGallons.unit).toEqual(units.volume.USGallon);
  });

  it('create volume with incompatible unit type should throw', () => {
    const value = 42;
    const unit = units.capacity.liter

    expect(() => new Volume(value, unit)).toThrow(new Error(`Invalid unit of measurement type '${unit.type}', expected '${UnitOfMeasurementType.Volume}'.`));
  });

  it('access volume properties should work', () => {
    const value = 42;
    const unit = units.volume.cubicMeter;

    const measurement = new Volume(value, unit);

    expect(measurement.cubicMillimeters).toBe(math.chain(units.volume.cubicMillimeter.ratio).divide(unit.ratio).multiply(value).done());
    expect(measurement.cubicCentimeters).toBe(math.chain(units.volume.cubicCentimeter.ratio).divide(unit.ratio).multiply(value).done());
    expect(measurement.cubicDecimeters).toBe(math.chain(units.volume.cubicDecimeter.ratio).divide(unit.ratio).multiply(value).done());
    expect(measurement.cubicMeters).toBe(value);
    expect(measurement.base).toBe(value);
    expect(measurement.cubicDecameters).toBe(math.chain(units.volume.cubicDecameter.ratio).divide(unit.ratio).multiply(value).done());
    expect(measurement.cubicHectometers).toBe(math.chain(units.volume.cubicHectometer.ratio).divide(unit.ratio).multiply(value).done());
    expect(measurement.cubicKilometers).toBe(math.chain(units.volume.cubicKilometer.ratio).divide(unit.ratio).multiply(value).done());
    expect(measurement.cubicInches).toBe(math.chain(units.volume.cubicInch.ratio).divide(unit.ratio).multiply(value).done());
    expect(measurement.cubicFeet).toBe(math.chain(units.volume.cubicFoot.ratio).divide(unit.ratio).multiply(value).done());
    expect(measurement.cubicYards).toBe(math.chain(units.volume.cubicYard.ratio).divide(unit.ratio).multiply(value).done());
    expect(measurement.cubicMiles).toBe(math.chain(units.volume.cubicMile.ratio).divide(unit.ratio).multiply(value).done());
    expect(measurement.fluidOunces).toBe(math.chain(units.volume.fluidOunce.ratio).divide(unit.ratio).multiply(value).done());
    expect(measurement.imperialGallons).toBe(math.chain(units.volume.imperialGallon.ratio).divide(unit.ratio).multiply(value).done());
    expect(measurement.USGallons).toBe(math.chain(units.volume.USGallon.ratio).divide(unit.ratio).multiply(value).done());
  });

  it('compare volumes should work', () => {
    const measurement1 = Volume.fromCubicCentimeters(250);
    const measurement2 = Volume.fromCubicMeters(5);
    const measurement3 = Volume.fromCubicDecimeters(50);

    expect(measurement1.compareTo(measurement2)).toBe(-1);
    expect(measurement2.compareTo(measurement1)).toBe(1);
    expect(measurement3.compareTo(measurement2)).toBe(0);
    expect(measurement3.compareTo(measurement3)).toBe(0);
    expect(measurement3.equals(measurement1)).toBeFalse();
    expect(measurement3.equals(measurement2)).toBeTrue();
    expect(measurement3.equals(measurement2, true)).toBeFalse();
    expect(measurement3.equals(measurement3, true)).toBeTrue();
  });

  it('convert volume should work', () => {
    const measurement = Volume.fromCubicCentimeters(42);
    const convertionUnit = units.volume.cubicDecameter;

    const convertedMeasurement = measurement.convertTo(convertionUnit);

    expect(convertedMeasurement.unit).toEqual(convertionUnit);
    expect(convertedMeasurement.value).toBe(math.chain(convertionUnit.ratio).divide(measurement.unit.ratio).multiply(measurement.value).done());
  });

  it('convert volume to incompatible type should throw', () => {
    const measurement = Volume.fromCubicCentimeters(42);
    const convertionUnit = units.capacity.liter;

    expect(() => measurement.convertTo(convertionUnit)).toThrow(new Error(`Incompatible measurement types '${measurement.unit.type}' and '${convertionUnit.type}'`));
  });

  it('sum volumes with same units should work', () => {
    const value1 = 42;
    const value2 = 24;
    const measurement1 = Volume.fromCubicCentimeters(value1);
    const measurement2 = Volume.fromCubicCentimeters(value2);

    const result = measurement1.add(measurement2);

    expect(result.unit).toEqual(units.volume.cubicCentimeter);
    expect(result.value).toBe(value1 + value2);
  });

  it('sum volumes with different units should work', () => {
    const value1 = 42;
    const value2 = 24;
    const unit1 = units.volume.cubicCentimeter;
    const unit2 = units.volume.cubicMeter;
    const measurement1 = new Volume(value1, unit1);
    const measurement2 = new Volume(value2, unit2);

    const result1 = measurement1.add(measurement2);
    const result2 = measurement2.add(measurement1);

    expect(result1.unit).toEqual(unit1);
    expect(result1.value).toBe(measurement1.cubicCentimeters + measurement2.cubicCentimeters);
    expect(result2.unit).toEqual(unit2);
    expect(result2.value).toBe(measurement2.cubicMeters + measurement1.cubicMeters);
  });

  it('subtract volumes with same units should work', () => {
    const value1 = 42;
    const value2 = 24;
    const measurement1 = Volume.fromCubicCentimeters(value1);
    const measurement2 = Volume.fromCubicCentimeters(value2);

    const result = measurement1.subtract(measurement2);

    expect(result.unit).toEqual(units.volume.cubicCentimeter);
    expect(result.value).toBe(value1 - value2);
  });

  it('subtract volumes with different units should work', () => {
    const value1 = 42;
    const value2 = 24;
    const unit1 = units.volume.cubicCentimeter;
    const unit2 = units.volume.cubicMeter;
    const measurement1 = new Volume(value1, unit1);
    const measurement2 = new Volume(value2, unit2);

    const result1 = measurement1.subtract(measurement2);
    const result2 = measurement2.subtract(measurement1);

    expect(result1.unit).toEqual(unit1);
    expect(result1.value).toBe(measurement1.cubicCentimeters - measurement2.cubicCentimeters);
    expect(result2.unit).toEqual(unit2);
    expect(result2.value).toBe(measurement2.cubicMeters - measurement1.cubicMeters);
  });

  it('multiply volumes with same units should work', () => {
    const value1 = 42;
    const value2 = 24;
    const measurement1 = Volume.fromCubicCentimeters(value1);
    const measurement2 = Volume.fromCubicCentimeters(value2);

    const result = measurement1.multiply(measurement2);

    expect(result.unit).toEqual(units.volume.cubicCentimeter);
    expect(result.value).toBe(math.multiply(value1, value2));
  });

  it('multiply volumes with different units should work', () => {
    const value1 = 42;
    const value2 = 24;
    const unit1 = units.volume.cubicCentimeter;
    const unit2 = units.volume.cubicMeter;
    const measurement1 = new Volume(value1, unit1);
    const measurement2 = new Volume(value2, unit2);

    const result1 = measurement1.multiply(measurement2);
    const result2 = measurement2.multiply(measurement1);

    expect(result1.unit).toEqual(unit1);
    expect(result1.value).toBe(math.multiply(measurement1.cubicCentimeters, measurement2.cubicCentimeters));
    expect(result2.unit).toEqual(unit2);
    expect(result2.value).toBe(math.multiply(measurement2.cubicMeters, measurement1.cubicMeters));
  });

  it('divide volumes with same units should work', () => {
    const value1 = 42;
    const value2 = 24;
    const measurement1 = Volume.fromCubicCentimeters(value1);
    const measurement2 = Volume.fromCubicCentimeters(value2);

    const result = measurement1.divide(measurement2);

    expect(result.unit).toEqual(units.volume.cubicCentimeter);
    expect(result.value).toBe(math.divide(value1, value2));
  });

  it('divide volumes with different units should work', () => {
    const value1 = 42;
    const value2 = 24;
    const unit1 = units.volume.cubicCentimeter;
    const unit2 = units.volume.cubicMeter;
    const measurement1 = new Volume(value1, unit1);
    const measurement2 = new Volume(value2, unit2);

    const result1 = measurement1.divide(measurement2);
    const result2 = measurement2.divide(measurement1);

    expect(result1.unit).toEqual(unit1);
    expect(result1.value).toBe(math.divide(measurement1.cubicCentimeters, measurement2.cubicCentimeters));
    expect(result2.unit).toEqual(unit2);
    expect(result2.value).toBe(math.divide(measurement2.cubicMeters, measurement1.cubicMeters));
  });

});
