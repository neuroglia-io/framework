import { UnitOfMeasurementType } from '../enums';
import { UnitOfMeasurement } from './unit-of-measurement';
import { Measurement } from '../measurement';

/**
 * Represents the measurement of a capacity
 */
export class EnergyBase extends Measurement {


  constructor(value?: number, unit?: UnitOfMeasurement);
  constructor(model?: Partial<EnergyBase>);
  constructor(...args: Array<number | UnitOfMeasurement | Partial<EnergyBase> | undefined>) {
    let model: Partial<EnergyBase> = {};
    if (args?.length === 1) {
      model = args[0] as Partial<EnergyBase>;
    }
    else if (args?.length == 2) {
      const [value, unit] = args as [number, UnitOfMeasurement];
      model = { value, unit };
    }
    super(model);
    if (!this.unit.type) this.unit.type = UnitOfMeasurementType.Energy;
    if (this.unit.type !== UnitOfMeasurementType.Energy) throw new Error(`Invalid unit of measurement type '${this.unit.type}', expected '${UnitOfMeasurementType.Energy}'.`);
  }

}
