import { Mass } from "./models/mass";
import { units } from './known-unit-of-measurements';

declare module "./models/mass" {
  export interface Mass {
    get milligrams(): number;
    get centigrams(): number;
    get decigrams(): number;
    get grams(): number;
    get decagrams(): number;
    get hectograms(): number;
    get kilograms(): number;
    get tons(): number;
    get kilotons(): number;
    get megatons(): number;
    get grains(): number;
    get drams(): number;
    get ounces(): number;
    get pounds(): number;
    get stones(): number;
  }
}

Object.defineProperty(Mass.prototype, 'milligrams', {
  get: function (this: Mass): number {
    return this.convertTo(units.mass.milligram).value;
  }
});

Object.defineProperty(Mass.prototype, 'centigrams', {
  get: function (this: Mass): number {
    return this.convertTo(units.mass.centigram).value;
  }
});

Object.defineProperty(Mass.prototype, 'decigrams', {
  get: function (this: Mass): number {
    return this.convertTo(units.mass.decigram).value;
  }
});

Object.defineProperty(Mass.prototype, 'grams', {
  get: function (this: Mass): number {
    return this.convertTo(units.mass.gram).value;
  }
});

Object.defineProperty(Mass.prototype, 'decagrams', {
  get: function (this: Mass): number {
    return this.convertTo(units.mass.decagram).value;
  }
});

Object.defineProperty(Mass.prototype, 'hectograms', {
  get: function (this: Mass): number {
    return this.convertTo(units.mass.hectogram).value;
  }
});

Object.defineProperty(Mass.prototype, 'kilograms', {
  get: function (this: Mass): number {
    return this.convertTo(units.mass.kilogram).value;
  }
});

Object.defineProperty(Mass.prototype, 'tons', {
  get: function (this: Mass): number {
    return this.convertTo(units.mass.ton).value;
  }
});

Object.defineProperty(Mass.prototype, 'kilotons', {
  get: function (this: Mass): number {
    return this.convertTo(units.mass.kiloton).value;
  }
});

Object.defineProperty(Mass.prototype, 'megatons', {
  get: function (this: Mass): number {
    return this.convertTo(units.mass.megaton).value;
  }
});

Object.defineProperty(Mass.prototype, 'grains', {
  get: function (this: Mass): number {
    return this.convertTo(units.mass.grain).value;
  }
});

Object.defineProperty(Mass.prototype, 'drams', {
  get: function (this: Mass): number {
    return this.convertTo(units.mass.dram).value;
  }
});

Object.defineProperty(Mass.prototype, 'ounces', {
  get: function (this: Mass): number {
    return this.convertTo(units.mass.ounce).value;
  }
});

Object.defineProperty(Mass.prototype, 'pounds', {
  get: function (this: Mass): number {
    return this.convertTo(units.mass.pound).value;
  }
});

Object.defineProperty(Mass.prototype, 'stones', {
  get: function (this: Mass): number {
    return this.convertTo(units.mass.stone).value;
  }
});
