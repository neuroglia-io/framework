import { units } from "./known-unit-of-measurements";
import { Capacity } from "./models";
import './extensions';

describe('Capacity Tests', () => {
  it('create capacity should work', () => {
    const value = 5;
    const unit = units.capacity.liter;

    const measurement = new Capacity(value, unit);

    expect(measurement).toBeDefined();
    expect(measurement.value).toBe(value);
    expect(measurement.unit).toEqual(unit);
  });
  it('access capacity properties should work', () => {
    const value = 5;
    const unit = units.capacity.liter;

    const measurement = new Capacity(value, unit);

    expect(measurement.milliliters).toBe(value * (units.capacity.milliliter.ratio / unit.ratio));
  });
});
