import { ModelConstructor } from '@neuroglia/common';
import { UnitOfMeasurement } from './unit-of-measurement';

/**
 * Represents a measure
 */
export class Measurement extends ModelConstructor {

  /** Gets/sets the measured value */
  value!: number;
  /** Gets/sets the unit the measure's value is expressed in */
  unit!: UnitOfMeasurement;

  constructor(value?: number, unit?: UnitOfMeasurement);
  constructor(model?: Partial<Measurement>);
  constructor(...args: Array<number | UnitOfMeasurement | Partial<Measurement> | undefined>) {
    let model: Partial<Measurement> = {};
    if (args?.length === 1) {
      model = args[0] as Partial<Measurement>;
    }
    else if (args?.length == 2) {
      const [value, unit] = args as [number, UnitOfMeasurement];
      model = { value, unit };
    }
    super(model);
    this.unit = new UnitOfMeasurement(model?.unit||{});
  }

}
