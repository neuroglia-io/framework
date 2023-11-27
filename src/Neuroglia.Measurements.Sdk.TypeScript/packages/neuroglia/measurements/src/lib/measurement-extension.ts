import { UnitOfMeasurement } from "./models";
import { Measurement } from "./models/measurement";
import { Decimal } from '@neuroglia/common';

declare module "./models/measurement" {
  export interface Measurement {
    add(measurement: Measurement): Measurement;
    substract(measurement: Measurement): Measurement;
    convertTo(unit: UnitOfMeasurement): Measurement;
  }
}

Measurement.prototype.add = function (measurement: Measurement): Measurement {
  if (!measurement?.value) {
    return this;
  }
  if (this.unit.type !== measurement.unit.type) {
    throw new Error(
      `Incompatible measurement types '${this.unit.type}' and '${measurement.unit.type}'`,
    );
  }
  const newMeasurement = Object.create(Object.getPrototypeOf(this));
  newMeasurement.unit = { ...this.unit };
  newMeasurement.value = Decimal.add(this.value, Decimal.mul(measurement.value, Decimal.div(this.unit.ratio, measurement.unit.ratio)));
  return newMeasurement;
};

Measurement.prototype.substract = function (measurement: Measurement): Measurement {
  if (!measurement?.value) {
    return this;
  }
  if (this.unit.type !== measurement.unit.type) {
    throw new Error(
      `Incompatible measurement types '${this.unit.type}' and '${measurement.unit.type}'`,
    );
  }
  const newMeasurement = Object.create(Object.getPrototypeOf(this));
  newMeasurement.unit = { ...this.unit };
  newMeasurement.value = Decimal.sub(this.value, Decimal.mul(measurement.value, Decimal.div(this.unit.ratio, measurement.unit.ratio)));
  return newMeasurement;
};

Measurement.prototype.convertTo = function (unit: UnitOfMeasurement): Measurement {
  if (!unit?.type) {
    return this;
  }
  if (this.unit.type !== unit.type) {
    throw new Error(
      `Incompatible measurement types '${this.unit.type}' and '${unit.type}'`,
    );
  }
  const newMeasurement = Object.create(Object.getPrototypeOf(this));
  newMeasurement.unit = { ...unit };
  newMeasurement.value = Decimal.mul(this.value, Decimal.div(unit.ratio, this.unit.ratio));
  return newMeasurement;
}
