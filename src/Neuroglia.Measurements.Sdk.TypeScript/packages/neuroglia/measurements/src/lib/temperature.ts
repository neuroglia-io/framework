import { TemperatureBase } from "./models/temperature-base";
import { units } from './known-unit-of-measurements';

export class Temperature extends TemperatureBase {

  get millikelvins(): number {
    return this.convertTo(units.temperature.millikelvin).value;
  }

  get centikelvins(): number {
    return this.convertTo(units.temperature.centikelvin).value;
  }

  get decikelvins(): number {
    return this.convertTo(units.temperature.decikelvin).value;
  }

  get kelvins(): number {
    return this.convertTo(units.temperature.kelvin).value;
  }

  get decakelvins(): number {
    return this.convertTo(units.temperature.decakelvin).value;
  }

  get hectokelvins(): number {
    return this.convertTo(units.temperature.hectokelvin).value;
  }

  get kilokelvins(): number {
    return this.convertTo(units.temperature.kilokelvin).value;
  }

}
