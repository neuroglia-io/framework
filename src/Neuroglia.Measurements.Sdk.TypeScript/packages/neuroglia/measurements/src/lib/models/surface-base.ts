import { UnitOfMeasurementType } from '../enums';
import { UnitOfMeasurement } from './unit-of-measurement';
import { Measurement } from '../measurement';

/**
 * Represents the measurement of a surface
 */
export class SurfaceBase extends Measurement {


  constructor(value?: number, unit?: UnitOfMeasurement);
  constructor(model?: Partial<SurfaceBase>);
  constructor(...args: Array<number | UnitOfMeasurement | Partial<SurfaceBase> | undefined>) {
    let model: Partial<SurfaceBase> = {};
    if (args?.length === 1) {
      model = args[0] as Partial<SurfaceBase>;
    }
    else if (args?.length == 2) {
      const [value, unit] = args as [number, UnitOfMeasurement];
      model = { value, unit };
    }
    super(model);
    if (!this.unit.type) this.unit.type = UnitOfMeasurementType.Surface;
    if (this.unit.type !== UnitOfMeasurementType.Surface) throw new Error(`Invalid unit of measurement type '${this.unit.type}', expected '${UnitOfMeasurementType.Surface}'.`);
  }

}
