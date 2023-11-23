import { Measurement } from './measurement';

/**
 * Represents the measurement of an amount of unfractable units
 */
export class Unit extends Measurement {


  constructor(model?: Partial<Unit>) {
    super(model);
  }


}
