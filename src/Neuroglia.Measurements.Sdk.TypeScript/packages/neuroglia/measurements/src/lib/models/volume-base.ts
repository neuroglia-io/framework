import { UnitOfMeasurementType } from '../enums';
import { UnitOfMeasurement } from './unit-of-measurement';
import { Measurement } from '../measurement';

/**
 * Represents the measurement of a volume
 */
export class VolumeBase extends Measurement {


  constructor(value?: number, unit?: UnitOfMeasurement);
  constructor(model?: Partial<VolumeBase>);
  constructor(...args: Array<number | UnitOfMeasurement | Partial<VolumeBase> | undefined>) {
    let model: Partial<VolumeBase> = {};
    if (args?.length === 1) {
      model = args[0] as Partial<VolumeBase>;
    }
    else if (args?.length == 2) {
      const [value, unit] = args as [number, UnitOfMeasurement];
      model = { value, unit };
    }
    super(model);
    if (!this.unit.type) this.unit.type = UnitOfMeasurementType.Volume;
    if (this.unit.type !== UnitOfMeasurementType.Volume) throw new Error(`Invalid unit of measurement type '${this.unit.type}', expected '${UnitOfMeasurementType.Volume}'.`);
  }

}
