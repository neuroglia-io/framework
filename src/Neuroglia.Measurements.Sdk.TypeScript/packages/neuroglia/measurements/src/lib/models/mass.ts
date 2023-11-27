import { UnitOfMeasurementType } from '../enums';
import { UnitOfMeasurement } from './unit-of-measurement';
import { Measurement } from './measurement';

/**
 * Represents the measurement of a mass
 */
export class Mass extends Measurement {


  constructor(value?: number, unit?: UnitOfMeasurement);
  constructor(model?: Partial<Mass>);
  constructor(...args: Array<number | UnitOfMeasurement | Partial<Mass> | undefined>) {
    let model: Partial<Mass> = {};
    if (args?.length === 1) {
      model = args[0] as Partial<Mass>;
    }
    else if (args?.length == 2) {
    const [value, unit] = args as [number, UnitOfMeasurement];
    model = { value, unit };
    }
    super(model);
    if (!this.unit.type) this.unit.type = UnitOfMeasurementType.Mass;
    if (this.unit.type !== UnitOfMeasurementType.Mass) throw new Error(`Invalid unit of measurement type '${this.unit.type}', expected '${UnitOfMeasurementType.Mass}'.`);
  }

}
