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

  constructor(model?: Partial<Measurement>) {
    super(model);
    this.unit = new UnitOfMeasurement(model?.unit||{});
  }

}
