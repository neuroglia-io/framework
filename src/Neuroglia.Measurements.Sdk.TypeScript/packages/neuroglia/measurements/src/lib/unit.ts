import { UnitBase } from "./models/unit-base";
import { units } from './known-unit-of-measurements';
import { UnitOfMeasurement } from "./models/unit-of-measurement";

export class Unit extends UnitBase {

  static override get unitOfReference(): UnitOfMeasurement {
    return units.unit.unit;
  }
  override get base(): number {
    return this.convertTo(Unit.unitOfReference).value;
  }

  get totalUnits(): number {
    return this.convertTo(units.unit.unit).value;
  }

  get pairs(): number {
    return this.convertTo(units.unit.pair).value;
  }

  get halfDozens(): number {
    return this.convertTo(units.unit.halfDozen).value;
  }

  get dozens(): number {
    return this.convertTo(units.unit.dozen).value;
  }

  static fromUnits(value: number): Unit {
    return new Unit(value, units.unit.unit);
  }

  static fromPairs(value: number): Unit {
    return new Unit(value, units.unit.pair);
  }

  static fromHalfDozens(value: number): Unit {
    return new Unit(value, units.unit.halfDozen);
  }

  static fromDozens(value: number): Unit {
    return new Unit(value, units.unit.dozen);
  }

}
