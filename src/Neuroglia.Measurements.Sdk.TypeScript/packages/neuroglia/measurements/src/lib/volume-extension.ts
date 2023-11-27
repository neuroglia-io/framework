import { Volume } from "./models/volume";
import { units } from './known-unit-of-measurements';

declare module "./models/volume" {
  export interface Volume {
    get cubicMillimeters(): number;
    get cubicCentimeters(): number;
    get cubicDecimeters(): number;
    get cubicMeters(): number;
    get cubicDecameters(): number;
    get cubicHectometers(): number;
    get cubicKilometers(): number;
    get cubicInches(): number;
    get cubicFeet(): number;
    get cubicYards(): number;
    get cubicMiles(): number;
    get fluidOunces(): number;
    get imperialGallons(): number;
    get uSGallons(): number;
  }
}

Object.defineProperty(Volume.prototype, 'cubicMillimeters', {
  get: function (this: Volume): number {
    return this.convertTo(units.volume.cubicMillimeter).value;
  }
});

Object.defineProperty(Volume.prototype, 'cubicCentimeters', {
  get: function (this: Volume): number {
    return this.convertTo(units.volume.cubicCentimeter).value;
  }
});

Object.defineProperty(Volume.prototype, 'cubicDecimeters', {
  get: function (this: Volume): number {
    return this.convertTo(units.volume.cubicDecimeter).value;
  }
});

Object.defineProperty(Volume.prototype, 'cubicMeters', {
  get: function (this: Volume): number {
    return this.convertTo(units.volume.cubicMeter).value;
  }
});

Object.defineProperty(Volume.prototype, 'cubicDecameters', {
  get: function (this: Volume): number {
    return this.convertTo(units.volume.cubicDecameter).value;
  }
});

Object.defineProperty(Volume.prototype, 'cubicHectometers', {
  get: function (this: Volume): number {
    return this.convertTo(units.volume.cubicHectometer).value;
  }
});

Object.defineProperty(Volume.prototype, 'cubicKilometers', {
  get: function (this: Volume): number {
    return this.convertTo(units.volume.cubicKilometer).value;
  }
});

Object.defineProperty(Volume.prototype, 'cubicInches', {
  get: function (this: Volume): number {
    return this.convertTo(units.volume.cubicInch).value;
  }
});

Object.defineProperty(Volume.prototype, 'cubicFeet', {
  get: function (this: Volume): number {
    return this.convertTo(units.volume.cubicFoot).value;
  }
});

Object.defineProperty(Volume.prototype, 'cubicYards', {
  get: function (this: Volume): number {
    return this.convertTo(units.volume.cubicYard).value;
  }
});

Object.defineProperty(Volume.prototype, 'cubicMiles', {
  get: function (this: Volume): number {
    return this.convertTo(units.volume.cubicMile).value;
  }
});

Object.defineProperty(Volume.prototype, 'fluidOunces', {
  get: function (this: Volume): number {
    return this.convertTo(units.volume.fluidOunce).value;
  }
});

Object.defineProperty(Volume.prototype, 'imperialGallons', {
  get: function (this: Volume): number {
    return this.convertTo(units.volume.imperialGallon).value;
  }
});

Object.defineProperty(Volume.prototype, 'uSGallons', {
  get: function (this: Volume): number {
    return this.convertTo(units.volume.USGallon).value;
  }
});

