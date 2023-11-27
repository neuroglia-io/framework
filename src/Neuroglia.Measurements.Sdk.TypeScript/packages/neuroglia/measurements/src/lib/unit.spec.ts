import { units } from "./known-unit-of-measurements";
import { Unit } from "./unit";
import { UnitOfMeasurementType } from "./enums";
import { create, all } from 'mathjs';

const math = create(all, {
  precision: 6,
  predictable: true
});

describe('Unit Tests', () => {

  it('create unit using parameters should work', () => {
    const value = 42;
    const unit = units.unit.unit;

    const measurement = new Unit(value, unit);

    expect(measurement).toBeDefined();
    expect(measurement.value).toBe(value);
    expect(measurement.unit).toEqual(unit);
  });

  it('create unit from object should work', () => {
    const value = 42;
    const unit = units.unit.unit;

    const measurement = new Unit({ value, unit });

    expect(measurement).toBeDefined();
    expect(measurement.value).toBe(value);
    expect(measurement.unit).toEqual(unit);
  });

  it('create unit energy should work', () => {
    const measurement = new Unit();

    expect(measurement).toBeDefined();
    expect(measurement.value).toBeUndefined();
    expect(measurement.unit.type).toEqual('unit');
  });

  it('create unit using fromUnits should work', () => {
    const value = 42;
    const _units = Unit.fromUnits(value);

    expect(_units).toBeDefined();
    expect(_units.value).toBe(value);
    expect(_units.unit).toEqual(units.unit.unit);
  });

  it('create unit using fromPairs should work', () => {
    const value = 42;
    const pairs = Unit.fromPairs(value);

    expect(pairs).toBeDefined();
    expect(pairs.value).toBe(value);
    expect(pairs.unit).toEqual(units.unit.pair);
  });

  it('create unit using fromHalfDozens should work', () => {
    const value = 42;
    const halfDozens = Unit.fromHalfDozens(value);

    expect(halfDozens).toBeDefined();
    expect(halfDozens.value).toBe(value);
    expect(halfDozens.unit).toEqual(units.unit.halfDozen);
  });

  it('create unit using fromKilounits should work', () => {
    const value = 42;
    const dozens = Unit.fromDozens(value);

    expect(dozens).toBeDefined();
    expect(dozens.value).toBe(value);
    expect(dozens.unit).toEqual(units.unit.dozen);
  });

  it('create unit with incompatible unit type should throw', () => {
    const value = 42;
    const unit = units.length.meter;

    expect(() => new Unit(value, unit)).toThrow(new Error(`Invalid unit of measurement type '${unit.type}', expected '${UnitOfMeasurementType.Unit}'.`));
  });

  it('access unit properties should work', () => {
    const value = 42;
    const unit = units.unit.unit;

    const measurement = new Unit(value, unit);

    expect(measurement.totalUnits).toBe(value);
    expect(measurement.base).toBe(value);
    expect(measurement.pairs).toBe(math.chain(units.unit.pair.ratio).divide(unit.ratio).multiply(value).done());
    expect(measurement.halfDozens).toBe(math.chain(units.unit.halfDozen.ratio).divide(unit.ratio).multiply(value).done());
    expect(measurement.dozens).toBe(math.chain(units.unit.dozen.ratio).divide(unit.ratio).multiply(value).done());
  });

  it('compare units should work', () => {
    const measurement1 = Unit.fromPairs(250);
    const measurement2 = Unit.fromUnits(50 * 12);
    const measurement3 = Unit.fromDozens(50);

    expect(measurement1.compareTo(measurement2)).toBe(-1);
    expect(measurement2.compareTo(measurement1)).toBe(1);
    expect(measurement3.compareTo(measurement2)).toBe(0);
    expect(measurement3.compareTo(measurement3)).toBe(0);
    expect(measurement3.equals(measurement1)).toBeFalse();
    expect(measurement3.equals(measurement2)).toBeTrue();
    expect(measurement3.equals(measurement2, true)).toBeFalse();
    expect(measurement3.equals(measurement3, true)).toBeTrue();
  });

  it('convert unit should work', () => {
    const measurement = Unit.fromPairs(42);
    const convertionUnit = units.unit.dozen;

    const convertedMeasurement = measurement.convertTo(convertionUnit);

    expect(convertedMeasurement.unit).toEqual(convertionUnit);
    expect(convertedMeasurement.value).toBe(math.chain(convertionUnit.ratio).divide(measurement.unit.ratio).multiply(measurement.value).done());
  });

  it('convert unit to incompatible type should throw', () => {
    const measurement = Unit.fromPairs(42);
    const convertionUnit = units.length.meter;

    expect(() => measurement.convertTo(convertionUnit)).toThrow(new Error(`Incompatible measurement types '${measurement.unit.type}' and '${convertionUnit.type}'`));
  });

  it('sum units with same units should work', () => {
    const value1 = 42;
    const value2 = 24;
    const measurement1 = Unit.fromPairs(value1);
    const measurement2 = Unit.fromPairs(value2);

    const result = measurement1.add(measurement2);

    expect(result.unit).toEqual(units.unit.pair);
    expect(result.value).toBe(value1 + value2);
  });

  it('sum units with different units should work', () => {
    const value1 = 42;
    const value2 = 24;
    const unit1 = units.unit.pair;
    const unit2 = units.unit.halfDozen;
    const measurement1 = new Unit(value1, unit1);
    const measurement2 = new Unit(value2, unit2);

    const result1 = measurement1.add(measurement2);
    const result2 = measurement2.add(measurement1);

    expect(result1.unit).toEqual(unit1);
    expect(result1.value).toBe(measurement1.pairs + measurement2.pairs);
    expect(result2.unit).toEqual(unit2);
    expect(result2.value).toBe(measurement2.halfDozens + measurement1.halfDozens);
  });

  it('subtract units with same units should work', () => {
    const value1 = 42;
    const value2 = 24;
    const measurement1 = Unit.fromPairs(value1);
    const measurement2 = Unit.fromPairs(value2);

    const result = measurement1.subtract(measurement2);

    expect(result.unit).toEqual(units.unit.pair);
    expect(result.value).toBe(value1 - value2);
  });

  it('subtract units with different units should work', () => {
    const value1 = 42;
    const value2 = 24;
    const unit1 = units.unit.pair;
    const unit2 = units.unit.halfDozen;
    const measurement1 = new Unit(value1, unit1);
    const measurement2 = new Unit(value2, unit2);

    const result1 = measurement1.subtract(measurement2);
    const result2 = measurement2.subtract(measurement1);

    expect(result1.unit).toEqual(unit1);
    expect(result1.value).toBe(measurement1.pairs - measurement2.pairs);
    expect(result2.unit).toEqual(unit2);
    expect(result2.value).toBe(measurement2.halfDozens - measurement1.halfDozens);
  });

  it('multiply units with same units should work', () => {
    const value1 = 42;
    const value2 = 24;
    const measurement1 = Unit.fromPairs(value1);
    const measurement2 = Unit.fromPairs(value2);

    const result = measurement1.multiply(measurement2);

    expect(result.unit).toEqual(units.unit.pair);
    expect(result.value).toBe(math.multiply(value1, value2));
  });

  it('multiply units with different units should work', () => {
    const value1 = 42;
    const value2 = 24;
    const unit1 = units.unit.pair;
    const unit2 = units.unit.halfDozen;
    const measurement1 = new Unit(value1, unit1);
    const measurement2 = new Unit(value2, unit2);

    const result1 = measurement1.multiply(measurement2);
    const result2 = measurement2.multiply(measurement1);

    expect(result1.unit).toEqual(unit1);
    expect(result1.value).toBe(math.multiply(measurement1.pairs, measurement2.pairs));
    expect(result2.unit).toEqual(unit2);
    expect(result2.value).toBe(math.multiply(measurement2.halfDozens, measurement1.halfDozens));
  });

  it('divide units with same units should work', () => {
    const value1 = 42;
    const value2 = 24;
    const measurement1 = Unit.fromPairs(value1);
    const measurement2 = Unit.fromPairs(value2);

    const result = measurement1.divide(measurement2);

    expect(result.unit).toEqual(units.unit.pair);
    expect(result.value).toBe(math.divide(value1, value2));
  });

  it('divide units with different units should work', () => {
    const value1 = 42;
    const value2 = 24;
    const unit1 = units.unit.pair;
    const unit2 = units.unit.halfDozen;
    const measurement1 = new Unit(value1, unit1);
    const measurement2 = new Unit(value2, unit2);

    const result1 = measurement1.divide(measurement2);
    const result2 = measurement2.divide(measurement1);

    expect(result1.unit).toEqual(unit1);
    expect(result1.value).toBe(math.divide(measurement1.pairs, measurement2.pairs));
    expect(result2.unit).toEqual(unit2);
    expect(result2.value).toBe(math.divide(measurement2.halfDozens, measurement1.halfDozens));
  });

});
