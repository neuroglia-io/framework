import { units } from "./known-unit-of-measurements";
import { Mass } from "./mass";
import { UnitOfMeasurementType } from "./enums";
import { create, all } from 'mathjs';

const math = create(all, {
  precision: 6,
  predictable: true
});

describe('Mass Tests', () => {

  it('create mass using parameters should work', () => {
    const value = 42;
    const unit = units.mass.gram;

    const measurement = new Mass(value, unit);

    expect(measurement).toBeDefined();
    expect(measurement.value).toBe(value);
    expect(measurement.unit).toEqual(unit);
  });

  it('create mass from object should work', () => {
    const value = 42;
    const unit = units.mass.gram;

    const measurement = new Mass({ value, unit });

    expect(measurement).toBeDefined();
    expect(measurement.value).toBe(value);
    expect(measurement.unit).toEqual(unit);
  });

  it('create mass energy should work', () => {
    const measurement = new Mass();

    expect(measurement).toBeDefined();
    expect(measurement.value).toBeUndefined();
    expect(measurement.unit.type).toEqual('mass');
  });

  it('create mass using fromMilligrams should work', () => {
    const value = 42;
    const milligrams = Mass.fromMilligrams(value);

    expect(milligrams).toBeDefined();
    expect(milligrams.value).toBe(value);
    expect(milligrams.unit).toEqual(units.mass.milligram);
  });

  it('create mass using fromCentigrams should work', () => {
    const value = 42;
    const centigrams = Mass.fromCentigrams(value);

    expect(centigrams).toBeDefined();
    expect(centigrams.value).toBe(value);
    expect(centigrams.unit).toEqual(units.mass.centigram);
  });

  it('create mass using fromDecigrams should work', () => {
    const value = 42;
    const decigrams = Mass.fromDecigrams(value);

    expect(decigrams).toBeDefined();
    expect(decigrams.value).toBe(value);
    expect(decigrams.unit).toEqual(units.mass.decigram);
  });

  it('create mass using fromGrams should work', () => {
    const value = 42;
    const grams = Mass.fromGrams(value);

    expect(grams).toBeDefined();
    expect(grams.value).toBe(value);
    expect(grams.unit).toEqual(units.mass.gram);
  });

  it('create mass using fromDecagrams should work', () => {
    const value = 42;
    const hectograms = Mass.fromDecagrams(value);

    expect(hectograms).toBeDefined();
    expect(hectograms.value).toBe(value);
    expect(hectograms.unit).toEqual(units.mass.decagram);
  });

  it('create mass using fromHectograms should work', () => {
    const value = 42;
    const hectograms = Mass.fromHectograms(value);

    expect(hectograms).toBeDefined();
    expect(hectograms.value).toBe(value);
    expect(hectograms.unit).toEqual(units.mass.hectogram);
  });

  it('create mass using fromKilograms should work', () => {
    const value = 42;
    const kilograms = Mass.fromKilograms(value);

    expect(kilograms).toBeDefined();
    expect(kilograms.value).toBe(value);
    expect(kilograms.unit).toEqual(units.mass.kilogram);
  });

  it('create mass using fromTons should work', () => {
    const value = 42;
    const usFluidDrams = Mass.fromTons(value);

    expect(usFluidDrams).toBeDefined();
    expect(usFluidDrams.value).toBe(value);
    expect(usFluidDrams.unit).toEqual(units.mass.ton);
  });

  it('create mass using fromKilotons should work', () => {
    const value = 42;
    const usFluidDrams = Mass.fromKilotons(value);

    expect(usFluidDrams).toBeDefined();
    expect(usFluidDrams.value).toBe(value);
    expect(usFluidDrams.unit).toEqual(units.mass.kiloton);
  });

  it('create mass using fromMegatons should work', () => {
    const value = 42;
    const usFluidDrams = Mass.fromMegatons(value);

    expect(usFluidDrams).toBeDefined();
    expect(usFluidDrams.value).toBe(value);
    expect(usFluidDrams.unit).toEqual(units.mass.megaton);
  });

  it('create mass using fromGrains should work', () => {
    const value = 42;
    const usFluidOunces = Mass.fromGrains(value);

    expect(usFluidOunces).toBeDefined();
    expect(usFluidOunces.value).toBe(value);
    expect(usFluidOunces.unit).toEqual(units.mass.grain);
  });

  it('create mass using fromDrams should work', () => {
    const value = 42;
    const usGills = Mass.fromDrams(value);

    expect(usGills).toBeDefined();
    expect(usGills.value).toBe(value);
    expect(usGills.unit).toEqual(units.mass.dram);
  });

  it('create mass using fromOunces should work', () => {
    const value = 42;
    const usPints = Mass.fromOunces(value);

    expect(usPints).toBeDefined();
    expect(usPints.value).toBe(value);
    expect(usPints.unit).toEqual(units.mass.ounce);
  });

  it('create mass using fromPounds should work', () => {
    const value = 42;
    const usPecks = Mass.fromPounds(value);

    expect(usPecks).toBeDefined();
    expect(usPecks.value).toBe(value);
    expect(usPecks.unit).toEqual(units.mass.pound);
  });

  it('create mass using fromStones should work', () => {
    const value = 42;
    const usBushels = Mass.fromStones(value);

    expect(usBushels).toBeDefined();
    expect(usBushels.value).toBe(value);
    expect(usBushels.unit).toEqual(units.mass.stone);
  });

  it('create mass with incompatible unit type should throw', () => {
    const value = 42;
    const unit = units.length.meter;

    expect(() => new Mass(value, unit)).toThrow(new Error(`Invalid unit of measurement type '${unit.type}', expected '${UnitOfMeasurementType.Mass}'.`));
  });

  it('access mass properties should work', () => {
    const value = 42;
    const unit = units.mass.gram;

    const measurement = new Mass(value, unit);

    expect(measurement.milligrams).toBe(math.chain(units.mass.milligram.ratio).divide(unit.ratio).multiply(value).done());
    expect(measurement.centigrams).toBe(math.chain(units.mass.centigram.ratio).divide(unit.ratio).multiply(value).done());
    expect(measurement.decigrams).toBe(math.chain(units.mass.decigram.ratio).divide(unit.ratio).multiply(value).done());
    expect(measurement.grams).toBe(value);
    expect(measurement.base).toBe(value);
    expect(measurement.decagrams).toBe(math.chain(units.mass.decagram.ratio).divide(unit.ratio).multiply(value).done());
    expect(measurement.hectograms).toBe(math.chain(units.mass.hectogram.ratio).divide(unit.ratio).multiply(value).done());
    expect(measurement.kilograms).toBe(math.chain(units.mass.kilogram.ratio).divide(unit.ratio).multiply(value).done());
    expect(measurement.tons).toBe(math.chain(units.mass.ton.ratio).divide(unit.ratio).multiply(value).done());
    expect(measurement.kilotons).toBe(math.chain(units.mass.kiloton.ratio).divide(unit.ratio).multiply(value).done());
    expect(measurement.megatons).toBe(math.chain(units.mass.megaton.ratio).divide(unit.ratio).multiply(value).done());
    expect(measurement.grains).toBe(math.chain(units.mass.grain.ratio).divide(unit.ratio).multiply(value).done());
    expect(measurement.drams).toBe(math.chain(units.mass.dram.ratio).divide(unit.ratio).multiply(value).done());
    expect(measurement.ounces).toBe(math.chain(units.mass.ounce.ratio).divide(unit.ratio).multiply(value).done());
    expect(measurement.pounds).toBe(math.chain(units.mass.pound.ratio).divide(unit.ratio).multiply(value).done());
    expect(measurement.stones).toBe(math.chain(units.mass.stone.ratio).divide(unit.ratio).multiply(value).done());
  });

  it('compare masses should work', () => {
    const measurement1 = Mass.fromCentigrams(250);
    const measurement2 = Mass.fromGrams(5);
    const measurement3 = Mass.fromDecigrams(50);

    expect(measurement1.compareTo(measurement2)).toBe(-1);
    expect(measurement2.compareTo(measurement1)).toBe(1);
    expect(measurement3.compareTo(measurement2)).toBe(0);
    expect(measurement3.compareTo(measurement3)).toBe(0);
    expect(measurement3.equals(measurement1)).toBeFalse();
    expect(measurement3.equals(measurement2)).toBeTrue();
    expect(measurement3.equals(measurement2, true)).toBeFalse();
    expect(measurement3.equals(measurement3, true)).toBeTrue();
  });

  it('convert mass should work', () => {
    const measurement = Mass.fromCentigrams(42);
    const convertionUnit = units.mass.decagram;

    const convertedMeasurement = measurement.convertTo(convertionUnit);

    expect(convertedMeasurement.unit).toEqual(convertionUnit);
    expect(convertedMeasurement.value).toBe(math.chain(convertionUnit.ratio).divide(measurement.unit.ratio).multiply(measurement.value).done());
  });

  it('convert mass to incompatible type should throw', () => {
    const measurement = Mass.fromCentigrams(42);
    const convertionUnit = units.length.meter;

    expect(() => measurement.convertTo(convertionUnit)).toThrow(new Error(`Incompatible measurement types '${measurement.unit.type}' and '${convertionUnit.type}'`));
  });

  it('sum masses with same units should work', () => {
    const value1 = 42;
    const value2 = 24;
    const measurement1 = Mass.fromCentigrams(value1);
    const measurement2 = Mass.fromCentigrams(value2);

    const result = measurement1.add(measurement2);

    expect(result.unit).toEqual(units.mass.centigram);
    expect(result.value).toBe(value1 + value2);
  });

  it('sum masses with different units should work', () => {
    const value1 = 42;
    const value2 = 24;
    const unit1 = units.mass.centigram;
    const unit2 = units.mass.gram;
    const measurement1 = new Mass(value1, unit1);
    const measurement2 = new Mass(value2, unit2);

    const result1 = measurement1.add(measurement2);
    const result2 = measurement2.add(measurement1);

    expect(result1.unit).toEqual(unit1);
    expect(result1.value).toBe(measurement1.centigrams + measurement2.centigrams);
    expect(result2.unit).toEqual(unit2);
    expect(result2.value).toBe(measurement2.grams + measurement1.grams);
  });

  it('subtract masses with same units should work', () => {
    const value1 = 42;
    const value2 = 24;
    const measurement1 = Mass.fromCentigrams(value1);
    const measurement2 = Mass.fromCentigrams(value2);

    const result = measurement1.subtract(measurement2);

    expect(result.unit).toEqual(units.mass.centigram);
    expect(result.value).toBe(value1 - value2);
  });

  it('subtract masses with different units should work', () => {
    const value1 = 42;
    const value2 = 24;
    const unit1 = units.mass.centigram;
    const unit2 = units.mass.gram;
    const measurement1 = new Mass(value1, unit1);
    const measurement2 = new Mass(value2, unit2);

    const result1 = measurement1.subtract(measurement2);
    const result2 = measurement2.subtract(measurement1);

    expect(result1.unit).toEqual(unit1);
    expect(result1.value).toBe(measurement1.centigrams - measurement2.centigrams);
    expect(result2.unit).toEqual(unit2);
    expect(result2.value).toBe(measurement2.grams - measurement1.grams);
  });

  it('multiply masses with same units should work', () => {
    const value1 = 42;
    const value2 = 24;
    const measurement1 = Mass.fromCentigrams(value1);
    const measurement2 = Mass.fromCentigrams(value2);

    const result = measurement1.multiply(measurement2);

    expect(result.unit).toEqual(units.mass.centigram);
    expect(result.value).toBe(math.multiply(value1, value2));
  });

  it('multiply masses with different units should work', () => {
    const value1 = 42;
    const value2 = 24;
    const unit1 = units.mass.centigram;
    const unit2 = units.mass.gram;
    const measurement1 = new Mass(value1, unit1);
    const measurement2 = new Mass(value2, unit2);

    const result1 = measurement1.multiply(measurement2);
    const result2 = measurement2.multiply(measurement1);

    expect(result1.unit).toEqual(unit1);
    expect(result1.value).toBe(math.multiply(measurement1.centigrams, measurement2.centigrams));
    expect(result2.unit).toEqual(unit2);
    expect(result2.value).toBe(math.multiply(measurement2.grams, measurement1.grams));
  });

  it('divide masses with same units should work', () => {
    const value1 = 42;
    const value2 = 24;
    const measurement1 = Mass.fromCentigrams(value1);
    const measurement2 = Mass.fromCentigrams(value2);

    const result = measurement1.divide(measurement2);

    expect(result.unit).toEqual(units.mass.centigram);
    expect(result.value).toBe(math.divide(value1, value2));
  });

  it('divide masses with different units should work', () => {
    const value1 = 42;
    const value2 = 24;
    const unit1 = units.mass.centigram;
    const unit2 = units.mass.gram;
    const measurement1 = new Mass(value1, unit1);
    const measurement2 = new Mass(value2, unit2);

    const result1 = measurement1.divide(measurement2);
    const result2 = measurement2.divide(measurement1);

    expect(result1.unit).toEqual(unit1);
    expect(result1.value).toBe(math.divide(measurement1.centigrams, measurement2.centigrams));
    expect(result2.unit).toEqual(unit2);
    expect(result2.value).toBe(math.divide(measurement2.grams, measurement1.grams));
  });

});
