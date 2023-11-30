import { units } from "./known-unit-of-measurements";
import { Energy } from "./energy";
import { UnitOfMeasurementType } from "./enums";
import { create, all } from 'mathjs';

const math = create(all, {
  precision: 6,
  predictable: true
});

describe('Energy Tests', () => {

  it('create energy using parameters should work', () => {
    const value = 42;
    const unit = units.energy.joule;

    const measurement = new Energy(value, unit);

    expect(measurement).toBeDefined();
    expect(measurement.value).toBe(value);
    expect(measurement.unit).toEqual(unit);
  });

  it('create energy from object should work', () => {
    const value = 42;
    const unit = units.energy.joule;

    const measurement = new Energy({ value, unit });

    expect(measurement).toBeDefined();
    expect(measurement.value).toBe(value);
    expect(measurement.unit).toEqual(unit);
  });

  it('create empty energy should work', () => {
    const measurement = new Energy();

    expect(measurement).toBeDefined();
    expect(measurement.value).toBeUndefined();
    expect(measurement.unit.type).toEqual('energy');
  });

  it('create energy using fromMillicalories should work', () => {
    const value = 42;
    const millicalories = Energy.fromMillicalories(value);

    expect(millicalories).toBeDefined();
    expect(millicalories.value).toBe(value);
    expect(millicalories.unit).toEqual(units.energy.millicalorie);
  });

  it('create energy using fromCenticalories should work', () => {
    const value = 42;
    const centicalories = Energy.fromCenticalories(value);

    expect(centicalories).toBeDefined();
    expect(centicalories.value).toBe(value);
    expect(centicalories.unit).toEqual(units.energy.centicalorie);
  });

  it('create energy using fromDecicalories should work', () => {
    const value = 42;
    const decicalories = Energy.fromDecicalories(value);

    expect(decicalories).toBeDefined();
    expect(decicalories.value).toBe(value);
    expect(decicalories.unit).toEqual(units.energy.decicalorie);
  });

  it('create energy using fromCalories should work', () => {
    const value = 42;
    const calories = Energy.fromCalories(value);

    expect(calories).toBeDefined();
    expect(calories.value).toBe(value);
    expect(calories.unit).toEqual(units.energy.calorie);
  });

  it('create energy using fromDecacalories should work', () => {
    const value = 42;
    const hectocalories = Energy.fromDecacalories(value);

    expect(hectocalories).toBeDefined();
    expect(hectocalories.value).toBe(value);
    expect(hectocalories.unit).toEqual(units.energy.decacalorie);
  });

  it('create energy using fromHectocalories should work', () => {
    const value = 42;
    const hectocalories = Energy.fromHectocalories(value);

    expect(hectocalories).toBeDefined();
    expect(hectocalories.value).toBe(value);
    expect(hectocalories.unit).toEqual(units.energy.hectocalorie);
  });

  it('create energy using fromKilocalories should work', () => {
    const value = 42;
    const kilocalories = Energy.fromKilocalories(value);

    expect(kilocalories).toBeDefined();
    expect(kilocalories.value).toBe(value);
    expect(kilocalories.unit).toEqual(units.energy.kilocalorie);
  });

  it('create energy using fromMillijoules should work', () => {
    const value = 42;
    const usFluidDrams = Energy.fromMillijoules(value);

    expect(usFluidDrams).toBeDefined();
    expect(usFluidDrams.value).toBe(value);
    expect(usFluidDrams.unit).toEqual(units.energy.millijoule);
  });

  it('create energy using fromCentijoules should work', () => {
    const value = 42;
    const usFluidOunces = Energy.fromCentijoules(value);

    expect(usFluidOunces).toBeDefined();
    expect(usFluidOunces.value).toBe(value);
    expect(usFluidOunces.unit).toEqual(units.energy.centijoule);
  });

  it('create energy using fromDecijoules should work', () => {
    const value = 42;
    const usGills = Energy.fromDecijoules(value);

    expect(usGills).toBeDefined();
    expect(usGills.value).toBe(value);
    expect(usGills.unit).toEqual(units.energy.decijoule);
  });

  it('create energy using fromJoules should work', () => {
    const value = 42;
    const usPints = Energy.fromJoules(value);

    expect(usPints).toBeDefined();
    expect(usPints.value).toBe(value);
    expect(usPints.unit).toEqual(units.energy.joule);
  });

  it('create energy using fromDecajoules should work', () => {
    const value = 42;
    const usPecks = Energy.fromDecajoules(value);

    expect(usPecks).toBeDefined();
    expect(usPecks.value).toBe(value);
    expect(usPecks.unit).toEqual(units.energy.decajoule);
  });

  it('create energy using fromHectojoules should work', () => {
    const value = 42;
    const usBushels = Energy.fromHectojoules(value);

    expect(usBushels).toBeDefined();
    expect(usBushels.value).toBe(value);
    expect(usBushels.unit).toEqual(units.energy.hectojoule);
  });

  it('create energy using fromKilojoules should work', () => {
    const value = 42;
    const usBushels = Energy.fromKilojoules(value);

    expect(usBushels).toBeDefined();
    expect(usBushels.value).toBe(value);
    expect(usBushels.unit).toEqual(units.energy.kilojoule);
  });

  it('create energy with incompatible unit type should throw', () => {
    const value = 42;
    const unit = units.length.meter;

    expect(() => new Energy(value, unit)).toThrow(new Error(`Invalid unit of measurement type '${unit.type}', expected '${UnitOfMeasurementType.Energy}'.`));
  });

  it('access energy properties should work', () => {
    const value = 42;
    const unit = units.energy.calorie;

    const measurement = new Energy(value, unit);

    expect(measurement.millicalories).toBe(math.chain(units.energy.millicalorie.ratio).divide(unit.ratio).multiply(value).done());
    expect(measurement.centicalories).toBe(math.chain(units.energy.centicalorie.ratio).divide(unit.ratio).multiply(value).done());
    expect(measurement.decicalories).toBe(math.chain(units.energy.decicalorie.ratio).divide(unit.ratio).multiply(value).done());
    expect(measurement.calories).toBe(value);
    expect(measurement.base).toBe(value);
    expect(measurement.decacalories).toBe(math.chain(units.energy.decacalorie.ratio).divide(unit.ratio).multiply(value).done());
    expect(measurement.hectocalories).toBe(math.chain(units.energy.hectocalorie.ratio).divide(unit.ratio).multiply(value).done());
    expect(measurement.kilocalories).toBe(math.chain(units.energy.kilocalorie.ratio).divide(unit.ratio).multiply(value).done());
    expect(measurement.millijoules).toBe(math.chain(units.energy.millijoule.ratio).divide(unit.ratio).multiply(value).done());
    expect(measurement.centijoules).toBe(math.chain(units.energy.centijoule.ratio).divide(unit.ratio).multiply(value).done());
    expect(measurement.decijoules).toBe(math.chain(units.energy.decijoule.ratio).divide(unit.ratio).multiply(value).done());
    expect(measurement.joules).toBe(math.chain(units.energy.joule.ratio).divide(unit.ratio).multiply(value).done());
    expect(measurement.decajoules).toBe(math.chain(units.energy.decajoule.ratio).divide(unit.ratio).multiply(value).done());
    expect(measurement.hectojoules).toBe(math.chain(units.energy.hectojoule.ratio).divide(unit.ratio).multiply(value).done());
    expect(measurement.kilojoules).toBe(math.chain(units.energy.kilojoule.ratio).divide(unit.ratio).multiply(value).done());
  });

  it('compare energies should work', () => {
    const measurement1 = Energy.fromCenticalories(250);
    const measurement2 = Energy.fromCalories(5);
    const measurement3 = Energy.fromDecicalories(50);

    expect(measurement1.compareTo(measurement2)).toBe(-1);
    expect(measurement2.compareTo(measurement1)).toBe(1);
    expect(measurement3.compareTo(measurement2)).toBe(0);
    expect(measurement3.compareTo(measurement3)).toBe(0);
    expect(measurement3.equals(measurement1)).toBeFalse();
    expect(measurement3.equals(measurement2)).toBeTrue();
    expect(measurement3.equals(measurement2, true)).toBeFalse();
    expect(measurement3.equals(measurement3, true)).toBeTrue();
  });

  it('convert energy should work', () => {
    const measurement = Energy.fromCenticalories(42);
    const convertionUnit = units.energy.decacalorie;

    const convertedMeasurement = measurement.convertTo(convertionUnit);

    expect(convertedMeasurement.unit).toEqual(convertionUnit);
    expect(convertedMeasurement.value).toBe(math.chain(convertionUnit.ratio).divide(measurement.unit.ratio).multiply(measurement.value).done());
  });

  it('convert energy to incompatible type should throw', () => {
    const measurement = Energy.fromCenticalories(42);
    const convertionUnit = units.length.meter;

    expect(() => measurement.convertTo(convertionUnit)).toThrow(new Error(`Incompatible measurement types '${measurement.unit.type}' and '${convertionUnit.type}'`));
  });

  it('sum energies with same units should work', () => {
    const value1 = 42;
    const value2 = 24;
    const measurement1 = Energy.fromCenticalories(value1);
    const measurement2 = Energy.fromCenticalories(value2);

    const result = measurement1.add(measurement2);

    expect(result.unit).toEqual(units.energy.centicalorie);
    expect(result.value).toBe(value1 + value2);
  });

  it('sum energies with different units should work', () => {
    const value1 = 42;
    const value2 = 24;
    const unit1 = units.energy.centicalorie;
    const unit2 = units.energy.calorie;
    const measurement1 = new Energy(value1, unit1);
    const measurement2 = new Energy(value2, unit2);

    const result1 = measurement1.add(measurement2);
    const result2 = measurement2.add(measurement1);

    expect(result1.unit).toEqual(unit1);
    expect(result1.value).toBe(measurement1.centicalories + measurement2.centicalories);
    expect(result2.unit).toEqual(unit2);
    expect(result2.value).toBe(measurement2.calories + measurement1.calories);
  });

  it('subtract energies with same units should work', () => {
    const value1 = 42;
    const value2 = 24;
    const measurement1 = Energy.fromCenticalories(value1);
    const measurement2 = Energy.fromCenticalories(value2);

    const result = measurement1.subtract(measurement2);

    expect(result.unit).toEqual(units.energy.centicalorie);
    expect(result.value).toBe(value1 - value2);
  });

  it('subtract energies with different units should work', () => {
    const value1 = 42;
    const value2 = 24;
    const unit1 = units.energy.centicalorie;
    const unit2 = units.energy.calorie;
    const measurement1 = new Energy(value1, unit1);
    const measurement2 = new Energy(value2, unit2);

    const result1 = measurement1.subtract(measurement2);
    const result2 = measurement2.subtract(measurement1);

    expect(result1.unit).toEqual(unit1);
    expect(result1.value).toBe(measurement1.centicalories - measurement2.centicalories);
    expect(result2.unit).toEqual(unit2);
    expect(result2.value).toBe(measurement2.calories - measurement1.calories);
  });

  it('multiply energies with same units should work', () => {
    const value1 = 42;
    const value2 = 24;
    const measurement1 = Energy.fromCenticalories(value1);
    const measurement2 = Energy.fromCenticalories(value2);

    const result = measurement1.multiply(measurement2);

    expect(result.unit).toEqual(units.energy.centicalorie);
    expect(result.value).toBe(math.multiply(value1, value2));
  });

  it('multiply energies with different units should work', () => {
    const value1 = 42;
    const value2 = 24;
    const unit1 = units.energy.centicalorie;
    const unit2 = units.energy.calorie;
    const measurement1 = new Energy(value1, unit1);
    const measurement2 = new Energy(value2, unit2);

    const result1 = measurement1.multiply(measurement2);
    const result2 = measurement2.multiply(measurement1);

    expect(result1.unit).toEqual(unit1);
    expect(result1.value).toBe(math.multiply(measurement1.centicalories, measurement2.centicalories));
    expect(result2.unit).toEqual(unit2);
    expect(result2.value).toBe(math.multiply(measurement2.calories, measurement1.calories));
  });

  it('divide energies with same units should work', () => {
    const value1 = 42;
    const value2 = 24;
    const measurement1 = Energy.fromCenticalories(value1);
    const measurement2 = Energy.fromCenticalories(value2);

    const result = measurement1.divide(measurement2);

    expect(result.unit).toEqual(units.energy.centicalorie);
    expect(result.value).toBe(math.divide(value1, value2));
  });

  it('divide energies with different units should work', () => {
    const value1 = 42;
    const value2 = 24;
    const unit1 = units.energy.centicalorie;
    const unit2 = units.energy.calorie;
    const measurement1 = new Energy(value1, unit1);
    const measurement2 = new Energy(value2, unit2);

    const result1 = measurement1.divide(measurement2);
    const result2 = measurement2.divide(measurement1);

    expect(result1.unit).toEqual(unit1);
    expect(result1.value).toBe(math.divide(measurement1.centicalories, measurement2.centicalories));
    expect(result2.unit).toEqual(unit2);
    expect(result2.value).toBe(math.divide(measurement2.calories, measurement1.calories));
  });

});
