import { units } from "./known-unit-of-measurements";
import { Measurement } from "./measurement";
import { create, all } from 'mathjs';

const math = create(all, {
  precision: 6,
  predictable: true
});

describe('Measurement Tests', () => {

  it('create measurement using parameters should work', () => {
    const value = 42;
    const unit = units.capacity.liter;

    const measurement = new Measurement(value, unit);

    expect(measurement).toBeDefined();
    expect(measurement.value).toBe(value);
    expect(measurement.unit).toEqual(unit);
  });

  it('create measurement from object should work', () => {
    const value = 42;
    const unit = units.capacity.liter;

    const measurement = new Measurement({ value, unit });

    expect(measurement).toBeDefined();
    expect(measurement.value).toBe(value);
    expect(measurement.unit).toEqual(unit);
  });

  it('access measurement base should throw', () => {
    const value = 42;
    const unit = units.capacity.liter;

    const measurement = new Measurement({ value, unit });

    expect(() => measurement.base).toThrow(new Error('Not implemented'));
  });

  it('access measurement unitOfReference should throw', () => {
    expect(() => Measurement.unitOfReference).toThrow(new Error('Not implemented'));
  });

  it('compare measurement with incompatible types should throw', () => {
    const value1 = 42;
    const value2 = 24;
    const unit1 = units.capacity.centiliter;
    const unit2 = units.length.meter;
    const measurement1 = new Measurement(value1, unit1);
    const measurement2 = new Measurement(value2, unit2);

    expect(() => measurement1.compareTo(measurement2)).toThrow(new Error(`Incompatible measurement types '${measurement1.unit.type}' and '${measurement2.unit.type}'`));
  });

  it('equals measurement with incompatible types should throw', () => {
    const value1 = 42;
    const value2 = 24;
    const unit1 = units.capacity.centiliter;
    const unit2 = units.length.meter;
    const measurement1 = new Measurement(value1, unit1);
    const measurement2 = new Measurement(value2, unit2);

    expect(() => measurement1.equals(measurement2)).toThrow(new Error(`Incompatible measurement types '${measurement1.unit.type}' and '${measurement2.unit.type}'`));
  });

  it('convert measurement with no unit should work', () => {
    const value = 42;
    const unit = units.capacity.centiliter;
    const measurement = new Measurement(value, unit);

    const result = measurement.convertTo(null!);

    expect(result).toBe(measurement);
  });

  it('sum measurement with no value should work', () => {
    const value = 42;
    const unit = units.capacity.centiliter;
    const measurement1 = new Measurement(value, unit);
    const measurement2 = new Measurement(0, unit);

    const result = measurement1.add(measurement2);

    expect(result).toBe(measurement1);
  });

  it('subtract measurement with no value should work', () => {
    const value = 42;
    const unit = units.capacity.centiliter;
    const measurement1 = new Measurement(value, unit);
    const measurement2 = new Measurement(0, unit);

    const result = measurement1.subtract(measurement2);

    expect(result).toBe(measurement1);
  });

  it('multiply measurement with no value should work', () => {
    const value = 42;
    const unit = units.capacity.centiliter;
    const measurement1 = new Measurement(value, unit);
    const measurement2 = new Measurement(0, unit);

    const result = measurement1.multiply(measurement2);

    expect(result).toBe(measurement1);
  });

  it('divide measurement with no value should work', () => {
    const value = 42;
    const unit = units.capacity.centiliter;
    const measurement1 = new Measurement(value, unit);
    const measurement2 = new Measurement(0, unit);

    const result = measurement1.divide(measurement2);

    expect(result).toBe(measurement1);
  });

  it('sum measurement with incompatible types should throw', () => {
    const value1 = 42;
    const value2 = 24;
    const unit1 = units.capacity.centiliter;
    const unit2 = units.length.meter;
    const measurement1 = new Measurement(value1, unit1);
    const measurement2 = new Measurement(value2, unit2);

    expect(() => measurement1.add(measurement2)).toThrow(new Error(`Incompatible measurement types '${measurement1.unit.type}' and '${measurement2.unit.type}'`));
  });

  it('subtract measurement with incompatible types should throw', () => {
    const value1 = 42;
    const value2 = 24;
    const unit1 = units.capacity.centiliter;
    const unit2 = units.length.meter;
    const measurement1 = new Measurement(value1, unit1);
    const measurement2 = new Measurement(value2, unit2);

    expect(() => measurement1.subtract(measurement2)).toThrow(new Error(`Incompatible measurement types '${measurement1.unit.type}' and '${measurement2.unit.type}'`));
  });

  it('multiplu measurement with incompatible types should throw', () => {
    const value1 = 42;
    const value2 = 24;
    const unit1 = units.capacity.centiliter;
    const unit2 = units.length.meter;
    const measurement1 = new Measurement(value1, unit1);
    const measurement2 = new Measurement(value2, unit2);

    expect(() => measurement1.multiply(measurement2)).toThrow(new Error(`Incompatible measurement types '${measurement1.unit.type}' and '${measurement2.unit.type}'`));
  });

  it('divide measurement with incompatible types should throw', () => {
    const value1 = 42;
    const value2 = 24;
    const unit1 = units.capacity.centiliter;
    const unit2 = units.length.meter;
    const measurement1 = new Measurement(value1, unit1);
    const measurement2 = new Measurement(value2, unit2);

    expect(() => measurement1.divide(measurement2)).toThrow(new Error(`Incompatible measurement types '${measurement1.unit.type}' and '${measurement2.unit.type}'`));
  });

});
