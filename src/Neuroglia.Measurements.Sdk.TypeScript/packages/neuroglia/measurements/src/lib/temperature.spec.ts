import { units } from "./known-unit-of-measurements";
import { Temperature } from "./temperature";
import { UnitOfMeasurementType } from "./enums";
import { create, all } from 'mathjs';

const math = create(all, {
  precision: 6,
  predictable: true
});

describe('Temperature Tests', () => {

  it('create temperature using parameters should work', () => {
    const value = 42;
    const unit = units.temperature.kelvin;

    const measurement = new Temperature(value, unit);

    expect(measurement).toBeDefined();
    expect(measurement.value).toBe(value);
    expect(measurement.unit).toEqual(unit);
  });

  it('create temperature from object should work', () => {
    const value = 42;
    const unit = units.temperature.kelvin;

    const measurement = new Temperature({ value, unit });

    expect(measurement).toBeDefined();
    expect(measurement.value).toBe(value);
    expect(measurement.unit).toEqual(unit);
  });

  it('create temperature energy should work', () => {
    const measurement = new Temperature();

    expect(measurement).toBeDefined();
    expect(measurement.value).toBeUndefined();
    expect(measurement.unit.type).toEqual('temperature');
  });

  it('create temperature using fromMillikelvins should work', () => {
    const value = 42;
    const millikelvins = Temperature.fromMillikelvins(value);

    expect(millikelvins).toBeDefined();
    expect(millikelvins.value).toBe(value);
    expect(millikelvins.unit).toEqual(units.temperature.millikelvin);
  });

  it('create temperature using fromCentikelvins should work', () => {
    const value = 42;
    const centikelvins = Temperature.fromCentikelvins(value);

    expect(centikelvins).toBeDefined();
    expect(centikelvins.value).toBe(value);
    expect(centikelvins.unit).toEqual(units.temperature.centikelvin);
  });

  it('create temperature using fromDecikelvins should work', () => {
    const value = 42;
    const decikelvins = Temperature.fromDecikelvins(value);

    expect(decikelvins).toBeDefined();
    expect(decikelvins.value).toBe(value);
    expect(decikelvins.unit).toEqual(units.temperature.decikelvin);
  });

  it('create temperature using fromKelvins should work', () => {
    const value = 42;
    const kelvins = Temperature.fromKelvins(value);

    expect(kelvins).toBeDefined();
    expect(kelvins.value).toBe(value);
    expect(kelvins.unit).toEqual(units.temperature.kelvin);
  });

  it('create temperature using fromDecakelvins should work', () => {
    const value = 42;
    const hectokelvins = Temperature.fromDecakelvins(value);

    expect(hectokelvins).toBeDefined();
    expect(hectokelvins.value).toBe(value);
    expect(hectokelvins.unit).toEqual(units.temperature.decakelvin);
  });

  it('create temperature using fromHectokelvins should work', () => {
    const value = 42;
    const hectokelvins = Temperature.fromHectokelvins(value);

    expect(hectokelvins).toBeDefined();
    expect(hectokelvins.value).toBe(value);
    expect(hectokelvins.unit).toEqual(units.temperature.hectokelvin);
  });

  it('create temperature using fromKilokelvins should work', () => {
    const value = 42;
    const kilokelvins = Temperature.fromKilokelvins(value);

    expect(kilokelvins).toBeDefined();
    expect(kilokelvins.value).toBe(value);
    expect(kilokelvins.unit).toEqual(units.temperature.kilokelvin);
  });

  it('create temperature with incompatible unit type should throw', () => {
    const value = 42;
    const unit = units.length.meter;

    expect(() => new Temperature(value, unit)).toThrow(new Error(`Invalid unit of measurement type '${unit.type}', expected '${UnitOfMeasurementType.Temperature}'.`));
  });

  it('access temperature properties should work', () => {
    const value = 42;
    const unit = units.temperature.kelvin;

    const measurement = new Temperature(value, unit);

    expect(measurement.millikelvins).toBe(math.chain(units.temperature.millikelvin.ratio).divide(unit.ratio).multiply(value).done());
    expect(measurement.centikelvins).toBe(math.chain(units.temperature.centikelvin.ratio).divide(unit.ratio).multiply(value).done());
    expect(measurement.decikelvins).toBe(math.chain(units.temperature.decikelvin.ratio).divide(unit.ratio).multiply(value).done());
    expect(measurement.kelvins).toBe(value);
    expect(measurement.base).toBe(value);
    expect(measurement.decakelvins).toBe(math.chain(units.temperature.decakelvin.ratio).divide(unit.ratio).multiply(value).done());
    expect(measurement.hectokelvins).toBe(math.chain(units.temperature.hectokelvin.ratio).divide(unit.ratio).multiply(value).done());
    expect(measurement.kilokelvins).toBe(math.chain(units.temperature.kilokelvin.ratio).divide(unit.ratio).multiply(value).done());
  });

  it('compare temperatures should work', () => {
    const measurement1 = Temperature.fromCentikelvins(250);
    const measurement2 = Temperature.fromKelvins(5);
    const measurement3 = Temperature.fromDecikelvins(50);

    expect(measurement1.compareTo(measurement2)).toBe(-1);
    expect(measurement2.compareTo(measurement1)).toBe(1);
    expect(measurement3.compareTo(measurement2)).toBe(0);
    expect(measurement3.compareTo(measurement3)).toBe(0);
    expect(measurement3.equals(measurement1)).toBeFalse();
    expect(measurement3.equals(measurement2)).toBeTrue();
    expect(measurement3.equals(measurement2, true)).toBeFalse();
    expect(measurement3.equals(measurement3, true)).toBeTrue();
  });

  it('convert temperature should work', () => {
    const measurement = Temperature.fromCentikelvins(42);
    const convertionUnit = units.temperature.decakelvin;

    const convertedMeasurement = measurement.convertTo(convertionUnit);

    expect(convertedMeasurement.unit).toEqual(convertionUnit);
    expect(convertedMeasurement.value).toBe(math.chain(convertionUnit.ratio).divide(measurement.unit.ratio).multiply(measurement.value).done());
  });

  it('convert temperature to incompatible type should throw', () => {
    const measurement = Temperature.fromCentikelvins(42);
    const convertionUnit = units.length.meter;

    expect(() => measurement.convertTo(convertionUnit)).toThrow(new Error(`Incompatible measurement types '${measurement.unit.type}' and '${convertionUnit.type}'`));
  });

  it('sum temperatures with same units should work', () => {
    const value1 = 42;
    const value2 = 24;
    const measurement1 = Temperature.fromCentikelvins(value1);
    const measurement2 = Temperature.fromCentikelvins(value2);

    const result = measurement1.add(measurement2);

    expect(result.unit).toEqual(units.temperature.centikelvin);
    expect(result.value).toBe(value1 + value2);
  });

  it('sum temperatures with different units should work', () => {
    const value1 = 42;
    const value2 = 24;
    const unit1 = units.temperature.centikelvin;
    const unit2 = units.temperature.kelvin;
    const measurement1 = new Temperature(value1, unit1);
    const measurement2 = new Temperature(value2, unit2);

    const result1 = measurement1.add(measurement2);
    const result2 = measurement2.add(measurement1);

    expect(result1.unit).toEqual(unit1);
    expect(result1.value).toBe(measurement1.centikelvins + measurement2.centikelvins);
    expect(result2.unit).toEqual(unit2);
    expect(result2.value).toBe(measurement2.kelvins + measurement1.kelvins);
  });

  it('subtract temperatures with same units should work', () => {
    const value1 = 42;
    const value2 = 24;
    const measurement1 = Temperature.fromCentikelvins(value1);
    const measurement2 = Temperature.fromCentikelvins(value2);

    const result = measurement1.subtract(measurement2);

    expect(result.unit).toEqual(units.temperature.centikelvin);
    expect(result.value).toBe(value1 - value2);
  });

  it('subtract temperatures with different units should work', () => {
    const value1 = 42;
    const value2 = 24;
    const unit1 = units.temperature.centikelvin;
    const unit2 = units.temperature.kelvin;
    const measurement1 = new Temperature(value1, unit1);
    const measurement2 = new Temperature(value2, unit2);

    const result1 = measurement1.subtract(measurement2);
    const result2 = measurement2.subtract(measurement1);

    expect(result1.unit).toEqual(unit1);
    expect(result1.value).toBe(measurement1.centikelvins - measurement2.centikelvins);
    expect(result2.unit).toEqual(unit2);
    expect(result2.value).toBe(measurement2.kelvins - measurement1.kelvins);
  });

  it('multiply temperatures with same units should work', () => {
    const value1 = 42;
    const value2 = 24;
    const measurement1 = Temperature.fromCentikelvins(value1);
    const measurement2 = Temperature.fromCentikelvins(value2);

    const result = measurement1.multiply(measurement2);

    expect(result.unit).toEqual(units.temperature.centikelvin);
    expect(result.value).toBe(math.multiply(value1, value2));
  });

  it('multiply temperatures with different units should work', () => {
    const value1 = 42;
    const value2 = 24;
    const unit1 = units.temperature.centikelvin;
    const unit2 = units.temperature.kelvin;
    const measurement1 = new Temperature(value1, unit1);
    const measurement2 = new Temperature(value2, unit2);

    const result1 = measurement1.multiply(measurement2);
    const result2 = measurement2.multiply(measurement1);

    expect(result1.unit).toEqual(unit1);
    expect(result1.value).toBe(math.multiply(measurement1.centikelvins, measurement2.centikelvins));
    expect(result2.unit).toEqual(unit2);
    expect(result2.value).toBe(math.multiply(measurement2.kelvins, measurement1.kelvins));
  });

  it('divide temperatures with same units should work', () => {
    const value1 = 42;
    const value2 = 24;
    const measurement1 = Temperature.fromCentikelvins(value1);
    const measurement2 = Temperature.fromCentikelvins(value2);

    const result = measurement1.divide(measurement2);

    expect(result.unit).toEqual(units.temperature.centikelvin);
    expect(result.value).toBe(math.divide(value1, value2));
  });

  it('divide temperatures with different units should work', () => {
    const value1 = 42;
    const value2 = 24;
    const unit1 = units.temperature.centikelvin;
    const unit2 = units.temperature.kelvin;
    const measurement1 = new Temperature(value1, unit1);
    const measurement2 = new Temperature(value2, unit2);

    const result1 = measurement1.divide(measurement2);
    const result2 = measurement2.divide(measurement1);

    expect(result1.unit).toEqual(unit1);
    expect(result1.value).toBe(math.divide(measurement1.centikelvins, measurement2.centikelvins));
    expect(result2.unit).toEqual(unit2);
    expect(result2.value).toBe(math.divide(measurement2.kelvins, measurement1.kelvins));
  });

});
