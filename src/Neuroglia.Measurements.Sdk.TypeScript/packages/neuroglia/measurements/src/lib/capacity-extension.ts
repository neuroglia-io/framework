import { Capacity } from "./models/capacity";
import { units } from './known-unit-of-measurements';

declare module "./models/capacity" {
  export interface Capacity {
    get milliliters(): number;
    get centiliters(): number;
    get deciliters(): number;
    get liters(): number;
    get decaliters(): number;
    get hectoliters(): number;
    get kiloliters(): number;
    get USFluidDrams(): number;
    get USFluidOunces(): number;
    get USGills(): number;
    get USPints(): number;
    get USQuarts(): number;
    get USGallons(): number;
    get USPecks(): number;
    get USBushels(): number;
  }
}

Object.defineProperty(Capacity.prototype, 'milliliters', {
  get: function (this: Capacity): number {
    return this.convertTo(units.capacity.milliliter).value;
  }
});

Object.defineProperty(Capacity.prototype, 'centiliters', {
  get: function (this: Capacity): number {
    return this.convertTo(units.capacity.milliliter).value;
  }
});

Object.defineProperty(Capacity.prototype, 'deciliters', {
  get: function (this: Capacity): number {
    return this.convertTo(units.capacity.deciliter).value;
  }
});

Object.defineProperty(Capacity.prototype, 'liters', {
  get: function (this: Capacity): number {
    return this.convertTo(units.capacity.liter).value;
  }
});

Object.defineProperty(Capacity.prototype, 'decaliters', {
  get: function (this: Capacity): number {
    return this.convertTo(units.capacity.decaliter).value;
  }
});

Object.defineProperty(Capacity.prototype, 'hectoliters', {
  get: function (this: Capacity): number {
    return this.convertTo(units.capacity.hectoliter).value;
  }
});

Object.defineProperty(Capacity.prototype, 'kiloliters', {
  get: function (this: Capacity): number {
    return this.convertTo(units.capacity.kiloliter).value;
  }
});

Object.defineProperty(Capacity.prototype, 'USFluidDrams', {
  get: function (this: Capacity): number {
    return this.convertTo(units.capacity.USFluidDram).value;
  }
});

Object.defineProperty(Capacity.prototype, 'USFluidOunces', {
  get: function (this: Capacity): number {
    return this.convertTo(units.capacity.USFluidOunce).value;
  }
});

Object.defineProperty(Capacity.prototype, 'USGills', {
  get: function (this: Capacity): number {
    return this.convertTo(units.capacity.USGill).value;
  }
});

Object.defineProperty(Capacity.prototype, 'USPints', {
  get: function (this: Capacity): number {
    return this.convertTo(units.capacity.USPint).value;
  }
});

Object.defineProperty(Capacity.prototype, 'USQuarts', {
  get: function (this: Capacity): number {
    return this.convertTo(units.capacity.USQuart).value;
  }
});

Object.defineProperty(Capacity.prototype, 'USGallons', {
  get: function (this: Capacity): number {
    return this.convertTo(units.capacity.USGallon).value;
  }
});

Object.defineProperty(Capacity.prototype, 'USPecks', {
  get: function (this: Capacity): number {
    return this.convertTo(units.capacity.USPeck).value;
  }
});

Object.defineProperty(Capacity.prototype, 'USBushels', {
  get: function (this: Capacity): number {
    return this.convertTo(units.capacity.USBushel).value;
  }
});

