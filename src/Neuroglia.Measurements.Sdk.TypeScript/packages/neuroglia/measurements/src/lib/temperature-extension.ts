import { Temperature } from "./models/temperature";
import { units } from './known-unit-of-measurements';

declare module "./models/temperature" {
  export interface Temperature {
    get millikelvins(): number;
    get centikelvins(): number;
    get decikelvins(): number;
    get kelvins(): number;
    get decakelvins(): number;
    get hectokelvins(): number;
    get kilokelvins(): number;
  }
}

Object.defineProperty(Temperature.prototype, 'millikelvins', {
  get: function (this: Temperature): number {
    return this.convertTo(units.temperature.millikelvin).value;
  }
});

Object.defineProperty(Temperature.prototype, 'centikelvins', {
  get: function (this: Temperature): number {
    return this.convertTo(units.temperature.centikelvin).value;
  }
});

Object.defineProperty(Temperature.prototype, 'decikelvins', {
  get: function (this: Temperature): number {
    return this.convertTo(units.temperature.decikelvin).value;
  }
});

Object.defineProperty(Temperature.prototype, 'kelvins', {
  get: function (this: Temperature): number {
    return this.convertTo(units.temperature.kelvin).value;
  }
});

Object.defineProperty(Temperature.prototype, 'decakelvins', {
  get: function (this: Temperature): number {
    return this.convertTo(units.temperature.decakelvin).value;
  }
});

Object.defineProperty(Temperature.prototype, 'hectokelvins', {
  get: function (this: Temperature): number {
    return this.convertTo(units.temperature.hectokelvin).value;
  }
});

Object.defineProperty(Temperature.prototype, 'kilokelvins', {
  get: function (this: Temperature): number {
    return this.convertTo(units.temperature.kilokelvin).value;
  }
});

