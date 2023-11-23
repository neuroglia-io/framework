import { Measurement } from './measurement';

/**
 * Represents the measurement of a temperature
 */
export class Temperature extends Measurement {


  constructor(model?: Partial<Temperature>) {
    super(model);
  }


}
