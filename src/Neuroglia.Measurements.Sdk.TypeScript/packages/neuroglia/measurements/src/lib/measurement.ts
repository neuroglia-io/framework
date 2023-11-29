import { UnitOfMeasurement } from "./models/unit-of-measurement";
import { Measurement as MeasurementBase } from "./models/measurement-base";
import { create, all } from 'mathjs';

const math = create(all, {
  predictable: true
});

export class Measurement extends MeasurementBase {

  get base(): number {
    throw new Error('Not implemented');
  }

  static get unitOfReference(): UnitOfMeasurement {
    throw new Error('Not implemented');
  }

  add(measurement: Measurement): Measurement {
    if (!measurement?.value) {
      return this;
    }
    if (this.unit.type !== measurement.unit.type) {
      throw new Error(
        `Incompatible measurement types '${this.unit.type}' and '${measurement.unit.type}'`,
      );
    }
    const newMeasurement = Object.create(Object.getPrototypeOf(this));
    newMeasurement.unit = new UnitOfMeasurement(this.unit);
    newMeasurement.value = math.add(this.value, measurement.convertTo(this.unit).value);
    return newMeasurement;
  }

  subtract(measurement: Measurement): Measurement {
    if (!measurement?.value) {
      return this;
    }
    if (this.unit.type !== measurement.unit.type) {
      throw new Error(
        `Incompatible measurement types '${this.unit.type}' and '${measurement.unit.type}'`,
      );
    }
    const newMeasurement = Object.create(Object.getPrototypeOf(this));
    newMeasurement.unit = new UnitOfMeasurement(this.unit);
    newMeasurement.value = math.subtract(this.value, measurement.convertTo(this.unit).value);
    return newMeasurement;
  }

  multiply(measurement: Measurement): Measurement {
    if (!measurement?.value) {
      return this;
    }
    if (this.unit.type !== measurement.unit.type) {
      throw new Error(
        `Incompatible measurement types '${this.unit.type}' and '${measurement.unit.type}'`,
      );
    }
    const newMeasurement = Object.create(Object.getPrototypeOf(this));
    newMeasurement.unit = new UnitOfMeasurement(this.unit);
    newMeasurement.value = math.multiply(this.value, measurement.convertTo(this.unit).value);
    return newMeasurement;
  }

  divide(measurement: Measurement): Measurement {
    if (!measurement?.value) {
      return this;
    }
    if (this.unit.type !== measurement.unit.type) {
      throw new Error(
        `Incompatible measurement types '${this.unit.type}' and '${measurement.unit.type}'`,
      );
    }
    const newMeasurement = Object.create(Object.getPrototypeOf(this));
    newMeasurement.unit = new UnitOfMeasurement(this.unit);
    newMeasurement.value = math.divide(this.value, measurement.convertTo(this.unit).value);
    return newMeasurement;
  }

  convertTo(unit: UnitOfMeasurement): Measurement {
    if (!unit?.type) {
      return this;
    }
    if (this.unit.type !== unit.type) {
      throw new Error(
        `Incompatible measurement types '${this.unit.type}' and '${unit.type}'`,
      );
    }
    const newMeasurement = Object.create(Object.getPrototypeOf(this));
    newMeasurement.unit = new UnitOfMeasurement(unit);
    newMeasurement.value = math.multiply(this.value, math.divide(unit.ratio, this.unit.ratio));
    return newMeasurement;
  }

  equals(measurement: Measurement, strict: boolean = false): boolean {
    if (this.unit.type !== measurement?.unit?.type) {
      throw new Error(
        `Incompatible measurement types '${this.unit.type}' and '${measurement?.unit?.type}'`,
      );
    }
    if (strict && this.unit.name !== measurement.unit.name) return false;
    return this.base === measurement.base;
  }

  compareTo(measurement: Measurement): number {
    if (this.unit.type !== measurement?.unit?.type) {
      throw new Error(
        `Incompatible measurement types '${this.unit.type}' and '${measurement?.unit?.type}'`,
      );
    }
    if (this.base > measurement.base) return 1;
    if (this.base < measurement.base) return -1;
    return 0;
  }

}
