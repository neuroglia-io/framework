import { Length } from "./models/length";
import { units } from './known-unit-of-measurements';

declare module "./models/length" {
  export interface Length {
    get millimeters(): number;
    get centimeters(): number;
    get decimeters(): number;
    get meters(): number;
    get decameters(): number;
    get hectometers(): number;
    get kilometers(): number;
    get inches(): number;
    get feet(): number;
    get yards(): number;
    get miles(): number;
  }
}

Object.defineProperty(Length.prototype, 'millimeters', {
  get: function (this: Length): number {
    return this.convertTo(units.length.millimeter).value;
  }
});

Object.defineProperty(Length.prototype, 'centimeters', {
  get: function (this: Length): number {
    return this.convertTo(units.length.centimeter).value;
  }
});

Object.defineProperty(Length.prototype, 'decimeters', {
  get: function (this: Length): number {
    return this.convertTo(units.length.decimeter).value;
  }
});

Object.defineProperty(Length.prototype, 'meters', {
  get: function (this: Length): number {
    return this.convertTo(units.length.meter).value;
  }
});

Object.defineProperty(Length.prototype, 'decameters', {
  get: function (this: Length): number {
    return this.convertTo(units.length.decameter).value;
  }
});

Object.defineProperty(Length.prototype, 'hectometers', {
  get: function (this: Length): number {
    return this.convertTo(units.length.hectometer).value;
  }
});

Object.defineProperty(Length.prototype, 'kilometers', {
  get: function (this: Length): number {
    return this.convertTo(units.length.kilometer).value;
  }
});

Object.defineProperty(Length.prototype, 'inches', {
  get: function (this: Length): number {
    return this.convertTo(units.length.inch).value;
  }
});

Object.defineProperty(Length.prototype, 'feet', {
  get: function (this: Length): number {
    return this.convertTo(units.length.foot).value;
  }
});

Object.defineProperty(Length.prototype, 'yards', {
  get: function (this: Length): number {
    return this.convertTo(units.length.yard).value;
  }
});

Object.defineProperty(Length.prototype, 'miles', {
  get: function (this: Length): number {
    return this.convertTo(units.length.mile).value;
  }
});
