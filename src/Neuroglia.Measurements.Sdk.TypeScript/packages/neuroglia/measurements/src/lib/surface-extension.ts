import { Surface } from "./models/surface";
import { units } from './known-unit-of-measurements';

declare module "./models/surface" {
  export interface Surface {
    get squareMillimeters(): number;
    get squareCentimeters(): number;
    get squareDecimeters(): number;
    get squareMeters(): number;
    get squareDecameters(): number;
    get squareHectometers(): number;
    get squareKilometers(): number;
    get squareInches(): number;
    get squareFeet(): number;
    get squareYards(): number;
    get squareMiles(): number;
  }
}

Object.defineProperty(Surface.prototype, 'squareMillimeters', {
  get: function (this: Surface): number {
    return this.convertTo(units.surface.squareMillimeter).value;
  }
});

Object.defineProperty(Surface.prototype, 'squareCentimeters', {
  get: function (this: Surface): number {
    return this.convertTo(units.surface.squareCentimeter).value;
  }
});

Object.defineProperty(Surface.prototype, 'squareDecimeters', {
  get: function (this: Surface): number {
    return this.convertTo(units.surface.squareDecimeter).value;
  }
});

Object.defineProperty(Surface.prototype, 'squareMeters', {
  get: function (this: Surface): number {
    return this.convertTo(units.surface.squareMeter).value;
  }
});

Object.defineProperty(Surface.prototype, 'squareDecameters', {
  get: function (this: Surface): number {
    return this.convertTo(units.surface.squareDecameter).value;
  }
});

Object.defineProperty(Surface.prototype, 'squareHectometers', {
  get: function (this: Surface): number {
    return this.convertTo(units.surface.squareHectometer).value;
  }
});

Object.defineProperty(Surface.prototype, 'squareKilometers', {
  get: function (this: Surface): number {
    return this.convertTo(units.surface.squareKilometer).value;
  }
});

Object.defineProperty(Surface.prototype, 'squareInches', {
  get: function (this: Surface): number {
    return this.convertTo(units.surface.squareInch).value;
  }
});

Object.defineProperty(Surface.prototype, 'squareFeet', {
  get: function (this: Surface): number {
    return this.convertTo(units.surface.squareFoot).value;
  }
});

Object.defineProperty(Surface.prototype, 'squareYards', {
  get: function (this: Surface): number {
    return this.convertTo(units.surface.squareYard).value;
  }
});

Object.defineProperty(Surface.prototype, 'squareMiles', {
  get: function (this: Surface): number {
    return this.convertTo(units.surface.squareMile).value;
  }
});
