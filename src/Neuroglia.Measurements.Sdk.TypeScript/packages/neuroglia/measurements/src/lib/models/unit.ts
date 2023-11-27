import { UnitOfMeasurementType } from '../enums';
import { UnitOfMeasurement } from './unit-of-measurement';
import { Measurement } from './measurement';

/**
 * Represents the measurement of an amount of unfractable units
 */
export class Unit extends Measurement {


  constructor(value?: number, unit?: UnitOfMeasurement);
  constructor(model?: Partial<Unit>);
  constructor(...args: Array<number | UnitOfMeasurement | Partial<Unit> | undefined>) {
    let model: Partial<Unit> = {};
    if (args?.length === 1) {
      model = args[0] as Partial<Unit>;
    }
    else if (args?.length == 2) {
    const [value, unit] = args as [number, UnitOfMeasurement];
    model = { value, unit };
    }
    super(model);
    if (!this.unit.type) this.unit.type = UnitOfMeasurementType.Unit;
    if (this.unit.type !== UnitOfMeasurementType.Unit) throw new Error(`Invalid unit of measurement type '${this.unit.type}', expected '${UnitOfMeasurementType.Unit}'.`);
  }

}
