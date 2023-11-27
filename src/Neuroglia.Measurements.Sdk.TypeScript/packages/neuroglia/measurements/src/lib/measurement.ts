import { UnitOfMeasurement } from "./models/unit-of-measurement";
import { Measurement as MeasurementBase } from "./models/measurement-base";
import { Decimal } from '@neuroglia/common';

export class Measurement extends MeasurementBase {

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
    newMeasurement.unit = { ...this.unit };
    newMeasurement.value = Decimal.add(this.value, Decimal.mul(measurement.value, Decimal.div(this.unit.ratio, measurement.unit.ratio)));
    return newMeasurement;
  }

  sub(measurement: Measurement): Measurement {
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
    newMeasurement.unit = { ...unit };
    newMeasurement.value = Decimal.mul(this.value, Decimal.div(unit.ratio, this.unit.ratio));
    return newMeasurement;
  }

}
