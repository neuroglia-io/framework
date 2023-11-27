import { UnitOfMeasurementType } from '../enums';
import { UnitOfMeasurement } from './unit-of-measurement';
import { Measurement } from './measurement';

/**
 * Represents the measurement of a temperature
 */
export class Temperature extends Measurement {


  constructor(value?: number, unit?: UnitOfMeasurement);
  constructor(model?: Partial<Temperature>);
  constructor(...args: Array<number | UnitOfMeasurement | Partial<Temperature> | undefined>) {
    let model: Partial<Temperature> = {};
    if (args?.length === 1) {
      model = args[0] as Partial<Temperature>;
    }
    else if (args?.length == 2) {
    const [value, unit] = args as [number, UnitOfMeasurement];
    model = { value, unit };
    }
    super(model);
    if (!this.unit.type) this.unit.type = UnitOfMeasurementType.Temperature;
    if (this.unit.type !== UnitOfMeasurementType.Temperature) throw new Error(`Invalid unit of measurement type '${this.unit.type}', expected '${UnitOfMeasurementType.Temperature}'.`);
  }

}
