import { UnitOfMeasurementType } from '../enums';
import { UnitOfMeasurement } from './unit-of-measurement';
import { Measurement } from './measurement';

/**
 * Represents the measurement of a length
 */
export class Length extends Measurement {


  constructor(value?: number, unit?: UnitOfMeasurement);
  constructor(model?: Partial<Length>);
  constructor(...args: Array<number | UnitOfMeasurement | Partial<Length> | undefined>) {
    let model: Partial<Length> = {};
    if (args?.length === 1) {
      model = args[0] as Partial<Length>;
    }
    else if (args?.length == 2) {
    const [value, unit] = args as [number, UnitOfMeasurement];
    model = { value, unit };
    }
    super(model);
    if (!this.unit.type) this.unit.type = UnitOfMeasurementType.Length;
    if (this.unit.type !== UnitOfMeasurementType.Length) throw new Error(`Invalid unit of measurement type '${this.unit.type}', expected '${UnitOfMeasurementType.Length}'.`);
  }

}
