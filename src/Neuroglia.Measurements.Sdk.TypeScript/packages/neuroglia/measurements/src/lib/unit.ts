import { UnitBase } from "./models/unit-base";
import { units } from './known-unit-of-measurements';

export class Unit extends UnitBase {

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

}
