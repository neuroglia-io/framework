import { UnitOfMeasurementType } from '../enums';
import { UnitOfMeasurement } from './unit-of-measurement';
import { Measurement } from './measurement';

/**
 * Represents the measurement of a capacity
 */
export class Capacity extends Measurement {


  constructor(value?: number, unit?: UnitOfMeasurement);
  constructor(model?: Partial<Capacity>);
  constructor(...args: Array<number | UnitOfMeasurement | Partial<Capacity> | undefined>) {
    let model: Partial<Capacity> = {};
    if (args?.length === 1) {
      model = args[0] as Partial<Capacity>;
    }
    else if (args?.length == 2) {
    const [value, unit] = args as [number, UnitOfMeasurement];
    model = { value, unit };
    }
    super(model);
    if (!this.unit.type) this.unit.type = UnitOfMeasurementType.Capacity;
    if (this.unit.type !== UnitOfMeasurementType.Capacity) throw new Error(`Invalid unit of measurement type '${this.unit.type}', expected '${UnitOfMeasurementType.Capacity}'.`);
  }

}
