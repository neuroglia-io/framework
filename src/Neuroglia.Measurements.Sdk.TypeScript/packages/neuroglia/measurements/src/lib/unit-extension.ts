import { Unit } from "./models/unit";
import { units } from './known-unit-of-measurements';

declare module "./models/unit" {
  export interface Unit {
    get totalUnits(): number;
    get pairs(): number;
    get halfDozens(): number;
    get dozens(): number;
  }
}

Object.defineProperty(Unit.prototype, 'totalUnits', {
  get: function (this: Unit): number {
    return this.convertTo(units.unit.unit).value;
  }
});

Object.defineProperty(Unit.prototype, 'pairs', {
  get: function (this: Unit): number {
    return this.convertTo(units.unit.pair).value;
  }
});

Object.defineProperty(Unit.prototype, 'halfDozens', {
  get: function (this: Unit): number {
    return this.convertTo(units.unit.halfDozen).value;
  }
});

Object.defineProperty(Unit.prototype, 'dozens', {
  get: function (this: Unit): number {
    return this.convertTo(units.unit.dozen).value;
  }
});

