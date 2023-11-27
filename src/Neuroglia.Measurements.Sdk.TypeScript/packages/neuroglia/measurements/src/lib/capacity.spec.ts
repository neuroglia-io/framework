import { units } from "./known-unit-of-measurements";
import { Capacity } from "./capacity";
import { UnitOfMeasurementType } from "./enums";
import { create, all } from 'mathjs';

const math = create(all, {
  precision: 6,
  predictable: true
});

describe('Capacity Tests', () => {

  it('create capacity using parameters should work', () => {
    const value = 42;
    const unit = units.capacity.liter;

    const measurement = new Capacity(value, unit);

    expect(measurement).toBeDefined();
    expect(measurement.value).toBe(value);
    expect(measurement.unit).toEqual(unit);
  });

  it('create capacity from object should work', () => {
    const value = 42;
    const unit = units.capacity.liter;

    const measurement = new Capacity({ value, unit });

    expect(measurement).toBeDefined();
    expect(measurement.value).toBe(value);
    expect(measurement.unit).toEqual(unit);
  });

  it('create empty capacity should work', () => {
    const measurement = new Capacity();

    expect(measurement).toBeDefined();
    expect(measurement.value).toBeUndefined();
    expect(measurement.unit.type).toEqual('capacity');
  });

  it('create capacity using fromMilliliters should work', () => {
    const value = 42;
    const milliliters = Capacity.fromMilliliters(value);

    expect(milliliters).toBeDefined();
    expect(milliliters.value).toBe(value);
    expect(milliliters.unit).toEqual(units.capacity.milliliter);
  });

  it('create capacity using fromCentiliters should work', () => {
    const value = 42;
    const centiliters = Capacity.fromCentiliters(value);

    expect(centiliters).toBeDefined();
    expect(centiliters.value).toBe(value);
    expect(centiliters.unit).toEqual(units.capacity.centiliter);
  });

  it('create capacity using fromDeciliters should work', () => {
    const value = 42;
    const deciliters = Capacity.fromDeciliters(value);

    expect(deciliters).toBeDefined();
    expect(deciliters.value).toBe(value);
    expect(deciliters.unit).toEqual(units.capacity.deciliter);
  });

  it('create capacity using fromLiters should work', () => {
    const value = 42;
    const liters = Capacity.fromLiters(value);

    expect(liters).toBeDefined();
    expect(liters.value).toBe(value);
    expect(liters.unit).toEqual(units.capacity.liter);
  });

  it('create capacity using fromDecaliters should work', () => {
    const value = 42;
    const hectoliters = Capacity.fromDecaliters(value);

    expect(hectoliters).toBeDefined();
    expect(hectoliters.value).toBe(value);
    expect(hectoliters.unit).toEqual(units.capacity.decaliter);
  });

  it('create capacity using fromHectoliters should work', () => {
    const value = 42;
    const hectoliters = Capacity.fromHectoliters(value);

    expect(hectoliters).toBeDefined();
    expect(hectoliters.value).toBe(value);
    expect(hectoliters.unit).toEqual(units.capacity.hectoliter);
  });

  it('create capacity using fromKiloliters should work', () => {
    const value = 42;
    const kiloliters = Capacity.fromKiloliters(value);

    expect(kiloliters).toBeDefined();
    expect(kiloliters.value).toBe(value);
    expect(kiloliters.unit).toEqual(units.capacity.kiloliter);
  });

  it('create capacity using fromUSFluidDrams should work', () => {
    const value = 42;
    const usFluidDrams = Capacity.fromUSFluidDrams(value);

    expect(usFluidDrams).toBeDefined();
    expect(usFluidDrams.value).toBe(value);
    expect(usFluidDrams.unit).toEqual(units.capacity.USFluidDram);
  });

  it('create capacity using fromUSFluidOunces should work', () => {
    const value = 42;
    const usFluidOunces = Capacity.fromUSFluidOunces(value);

    expect(usFluidOunces).toBeDefined();
    expect(usFluidOunces.value).toBe(value);
    expect(usFluidOunces.unit).toEqual(units.capacity.USFluidOunce);
  });

  it('create capacity using fromUSGills should work', () => {
    const value = 42;
    const usGills = Capacity.fromUSGills(value);

    expect(usGills).toBeDefined();
    expect(usGills.value).toBe(value);
    expect(usGills.unit).toEqual(units.capacity.USGill);
  });

  it('create capacity using fromUSPints should work', () => {
    const value = 42;
    const usPints = Capacity.fromUSPints(value);

    expect(usPints).toBeDefined();
    expect(usPints.value).toBe(value);
    expect(usPints.unit).toEqual(units.capacity.USPint);
  });

  it('create capacity using fromUSQuarts should work', () => {
    const value = 42;
    const usQuarts = Capacity.fromUSQuarts(value);

    expect(usQuarts).toBeDefined();
    expect(usQuarts.value).toBe(value);
    expect(usQuarts.unit).toEqual(units.capacity.USQuart);
  });

  it('create capacity using fromUSGallons should work', () => {
    const value = 42;
    const usGallons = Capacity.fromUSGallons(value);

    expect(usGallons).toBeDefined();
    expect(usGallons.value).toBe(value);
    expect(usGallons.unit).toEqual(units.capacity.USGallon);
  });

  it('create capacity using fromUSPecks should work', () => {
    const value = 42;
    const usPecks = Capacity.fromUSPecks(value);

    expect(usPecks).toBeDefined();
    expect(usPecks.value).toBe(value);
    expect(usPecks.unit).toEqual(units.capacity.USPeck);
  });

  it('create capacity using fromUSBushels should work', () => {
    const value = 42;
    const usBushels = Capacity.fromUSBushels(value);

    expect(usBushels).toBeDefined();
    expect(usBushels.value).toBe(value);
    expect(usBushels.unit).toEqual(units.capacity.USBushel);
  });

  it('create capacity with incompatible unit type should throw', () => {
    const value = 42;
    const unit = units.length.meter;

    expect(() => new Capacity(value, unit)).toThrow(new Error(`Invalid unit of measurement type '${unit.type}', expected '${UnitOfMeasurementType.Capacity}'.`));
  });

  it('access capacity properties should work', () => {
    const value = 42;
    const unit = units.capacity.liter;

    const measurement = new Capacity(value, unit);

    expect(measurement.milliliters).toBe(math.chain(units.capacity.milliliter.ratio).divide(unit.ratio).multiply(value).done());
    expect(measurement.centiliters).toBe(math.chain(units.capacity.centiliter.ratio).divide(unit.ratio).multiply(value).done());
    expect(measurement.deciliters).toBe(math.chain(units.capacity.deciliter.ratio).divide(unit.ratio).multiply(value).done());
    expect(measurement.liters).toBe(value);
    expect(measurement.base).toBe(value);
    expect(measurement.decaliters).toBe(math.chain(units.capacity.decaliter.ratio).divide(unit.ratio).multiply(value).done());
    expect(measurement.hectoliters).toBe(math.chain(units.capacity.hectoliter.ratio).divide(unit.ratio).multiply(value).done());
    expect(measurement.kiloliters).toBe(math.chain(units.capacity.kiloliter.ratio).divide(unit.ratio).multiply(value).done());
    expect(measurement.USFluidDrams).toBe(math.chain(units.capacity.USFluidDram.ratio).divide(unit.ratio).multiply(value).done());
    expect(measurement.USFluidOunces).toBe(math.chain(units.capacity.USFluidOunce.ratio).divide(unit.ratio).multiply(value).done());
    expect(measurement.USGills).toBe(math.chain(units.capacity.USGill.ratio).divide(unit.ratio).multiply(value).done());
    expect(measurement.USPints).toBe(math.chain(units.capacity.USPint.ratio).divide(unit.ratio).multiply(value).done());
    expect(measurement.USQuarts).toBe(math.chain(units.capacity.USQuart.ratio).divide(unit.ratio).multiply(value).done());
    expect(measurement.USGallons).toBe(math.chain(units.capacity.USGallon.ratio).divide(unit.ratio).multiply(value).done());
    expect(measurement.USPecks).toBe(math.chain(units.capacity.USPeck.ratio).divide(unit.ratio).multiply(value).done());
    expect(measurement.USBushels).toBe(math.chain(units.capacity.USBushel.ratio).divide(unit.ratio).multiply(value).done());
  });

  it('compare capacities should work', () => {
    const measurement1 = Capacity.fromCentiliters(250);
    const measurement2 = Capacity.fromLiters(5);
    const measurement3 = Capacity.fromDeciliters(50);

    expect(measurement1.compareTo(measurement2)).toBe(-1);
    expect(measurement2.compareTo(measurement1)).toBe(1);
    expect(measurement3.compareTo(measurement2)).toBe(0);
    expect(measurement3.compareTo(measurement3)).toBe(0);
    expect(measurement3.equals(measurement1)).toBeFalse();
    expect(measurement3.equals(measurement2)).toBeTrue();
    expect(measurement3.equals(measurement2, true)).toBeFalse();
    expect(measurement3.equals(measurement3, true)).toBeTrue();
  });

  it('convert capacity should work', () => {
    const measurement = Capacity.fromCentiliters(42);
    const convertionUnit = units.capacity.decaliter;

    const convertedMeasurement = measurement.convertTo(convertionUnit);

    expect(convertedMeasurement.unit).toEqual(convertionUnit);
    expect(convertedMeasurement.value).toBe(math.chain(convertionUnit.ratio).divide(measurement.unit.ratio).multiply(measurement.value).done());
  });

  it('convert capacity to incompatible type should throw', () => {
    const measurement = Capacity.fromCentiliters(42);
    const convertionUnit = units.length.meter;

    expect(() => measurement.convertTo(convertionUnit)).toThrow(new Error(`Incompatible measurement types '${measurement.unit.type}' and '${convertionUnit.type}'`));
  });

  it('sum capacities with same units should work', () => {
    const value1 = 42;
    const value2 = 24;
    const measurement1 = Capacity.fromCentiliters(value1);
    const measurement2 = Capacity.fromCentiliters(value2);

    const result = measurement1.add(measurement2);

    expect(result.unit).toEqual(units.capacity.centiliter);
    expect(result.value).toBe(value1 + value2);
  });

  it('sum capacities with different units should work', () => {
    const value1 = 42;
    const value2 = 24;
    const unit1 = units.capacity.centiliter;
    const unit2 = units.capacity.liter;
    const measurement1 = new Capacity(value1, unit1);
    const measurement2 = new Capacity(value2, unit2);

    const result1 = measurement1.add(measurement2);
    const result2 = measurement2.add(measurement1);

    expect(result1.unit).toEqual(unit1);
    expect(result1.value).toBe(measurement1.centiliters + measurement2.centiliters);
    expect(result2.unit).toEqual(unit2);
    expect(result2.value).toBe(measurement2.liters + measurement1.liters);
  });

  it('subtract capacities with same units should work', () => {
    const value1 = 42;
    const value2 = 24;
    const measurement1 = Capacity.fromCentiliters(value1);
    const measurement2 = Capacity.fromCentiliters(value2);

    const result = measurement1.subtract(measurement2);

    expect(result.unit).toEqual(units.capacity.centiliter);
    expect(result.value).toBe(value1 - value2);
  });

  it('subtract capacities with different units should work', () => {
    const value1 = 42;
    const value2 = 24;
    const unit1 = units.capacity.centiliter;
    const unit2 = units.capacity.liter;
    const measurement1 = new Capacity(value1, unit1);
    const measurement2 = new Capacity(value2, unit2);

    const result1 = measurement1.subtract(measurement2);
    const result2 = measurement2.subtract(measurement1);

    expect(result1.unit).toEqual(unit1);
    expect(result1.value).toBe(measurement1.centiliters - measurement2.centiliters);
    expect(result2.unit).toEqual(unit2);
    expect(result2.value).toBe(measurement2.liters - measurement1.liters);
  });

  it('multiply capacities with same units should work', () => {
    const value1 = 42;
    const value2 = 24;
    const measurement1 = Capacity.fromCentiliters(value1);
    const measurement2 = Capacity.fromCentiliters(value2);

    const result = measurement1.multiply(measurement2);

    expect(result.unit).toEqual(units.capacity.centiliter);
    expect(result.value).toBe(math.multiply(value1, value2));
  });

  it('multiply capacities with different units should work', () => {
    const value1 = 42;
    const value2 = 24;
    const unit1 = units.capacity.centiliter;
    const unit2 = units.capacity.liter;
    const measurement1 = new Capacity(value1, unit1);
    const measurement2 = new Capacity(value2, unit2);

    const result1 = measurement1.multiply(measurement2);
    const result2 = measurement2.multiply(measurement1);

    expect(result1.unit).toEqual(unit1);
    expect(result1.value).toBe(math.multiply(measurement1.centiliters, measurement2.centiliters));
    expect(result2.unit).toEqual(unit2);
    expect(result2.value).toBe(math.multiply(measurement2.liters, measurement1.liters));
  });

  it('divide capacities with same units should work', () => {
    const value1 = 42;
    const value2 = 24;
    const measurement1 = Capacity.fromCentiliters(value1);
    const measurement2 = Capacity.fromCentiliters(value2);

    const result = measurement1.divide(measurement2);

    expect(result.unit).toEqual(units.capacity.centiliter);
    expect(result.value).toBe(math.divide(value1, value2));
  });

  it('divide capacities with different units should work', () => {
    const value1 = 42;
    const value2 = 24;
    const unit1 = units.capacity.centiliter;
    const unit2 = units.capacity.liter;
    const measurement1 = new Capacity(value1, unit1);
    const measurement2 = new Capacity(value2, unit2);

    const result1 = measurement1.divide(measurement2);
    const result2 = measurement2.divide(measurement1);

    expect(result1.unit).toEqual(unit1);
    expect(result1.value).toBe(math.divide(measurement1.centiliters, measurement2.centiliters));
    expect(result2.unit).toEqual(unit2);
    expect(result2.value).toBe(math.divide(measurement2.liters, measurement1.liters));
  });

});
