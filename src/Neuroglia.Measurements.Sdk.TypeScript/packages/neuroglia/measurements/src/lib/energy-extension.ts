import { Energy } from "./models/energy";
import { units } from './known-unit-of-measurements';

declare module "./models/energy" {
  export interface Energy {
    get millicalories(): number;
    get centicalories(): number;
    get decicalories(): number;
    get calories(): number;
    get decacalories(): number;
    get hectocalories(): number;
    get kilocalories(): number;
    get millijoules(): number;
    get centijoules(): number;
    get decijoules(): number;
    get joules(): number;
    get decajoules(): number;
    get hectojoules(): number;
    get kilojoules(): number;
  }
}

Object.defineProperty(Energy.prototype, 'millicalories', {
  get: function (this: Energy): number {
    return this.convertTo(units.energy.millicalorie).value;
  }
});

Object.defineProperty(Energy.prototype, 'centicalories', {
  get: function (this: Energy): number {
    return this.convertTo(units.energy.centicalorie).value;
  }
});

Object.defineProperty(Energy.prototype, 'decicalories', {
  get: function (this: Energy): number {
    return this.convertTo(units.energy.decicalorie).value;
  }
});

Object.defineProperty(Energy.prototype, 'calories', {
  get: function (this: Energy): number {
    return this.convertTo(units.energy.calorie).value;
  }
});

Object.defineProperty(Energy.prototype, 'decacalories', {
  get: function (this: Energy): number {
    return this.convertTo(units.energy.decacalorie).value;
  }
});

Object.defineProperty(Energy.prototype, 'hectocalories', {
  get: function (this: Energy): number {
    return this.convertTo(units.energy.hectocalorie).value;
  }
});

Object.defineProperty(Energy.prototype, 'kilocalories', {
  get: function (this: Energy): number {
    return this.convertTo(units.energy.kilocalorie).value;
  }
});

Object.defineProperty(Energy.prototype, 'millijoules', {
  get: function (this: Energy): number {
    return this.convertTo(units.energy.millijoule).value;
  }
});

Object.defineProperty(Energy.prototype, 'centijoules', {
  get: function (this: Energy): number {
    return this.convertTo(units.energy.centijoule).value;
  }
});

Object.defineProperty(Energy.prototype, 'decijoules', {
  get: function (this: Energy): number {
    return this.convertTo(units.energy.decijoule).value;
  }
});

Object.defineProperty(Energy.prototype, 'joules', {
  get: function (this: Energy): number {
    return this.convertTo(units.energy.joule).value;
  }
});

Object.defineProperty(Energy.prototype, 'decajoules', {
  get: function (this: Energy): number {
    return this.convertTo(units.energy.decajoule).value;
  }
});

Object.defineProperty(Energy.prototype, 'hectojoules', {
  get: function (this: Energy): number {
    return this.convertTo(units.energy.hectojoule).value;
  }
});

Object.defineProperty(Energy.prototype, 'kilojoules', {
  get: function (this: Energy): number {
    return this.convertTo(units.energy.kilojoule).value;
  }
});
