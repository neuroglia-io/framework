import { ModelConstructor } from '@neuroglia/common';
import { UnitOfMeasurementType } from '../enums/unit-of-measurement-type';

/**
 * Represents a unit of measurement
 */
export class UnitOfMeasurement extends ModelConstructor {

  /** Gets/sets the unit of measure's type */
  type!: UnitOfMeasurementType;
  /** Gets/sets the unit of measure's name */
  name!: string;
  /** Gets/sets the unit of measure's symbol */
  symbol!: string;
  /** Gets/sets the unit of measure's ratio, compared to the reference unit */
  ratio!: number;

  constructor(model?: Partial<UnitOfMeasurement>) {
    super(model);
  }


}
