import { TemperatureBase } from "./models/temperature-base";
import { units } from './known-unit-of-measurements';
import { UnitOfMeasurement } from "./models/unit-of-measurement";

export class Temperature extends TemperatureBase {

  static override get unitOfReference(): UnitOfMeasurement {
    return units.temperature.kelvin;
  }
  override get base(): number {
    return this.convertTo(Temperature.unitOfReference).value;
  }

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

  static fromMillikelvins(value: number): Temperature {
    return new Temperature(value, units.temperature.millikelvin);
  }

  static fromCentikelvins(value: number): Temperature {
    return new Temperature(value, units.temperature.centikelvin);
  }

  static fromDecikelvins(value: number): Temperature {
    return new Temperature(value, units.temperature.decikelvin);
  }

  static fromKelvins(value: number): Temperature {
    return new Temperature(value, units.temperature.kelvin);
  }

  static fromDecakelvins(value: number): Temperature {
    return new Temperature(value, units.temperature.decakelvin);
  }

  static fromHectokelvins(value: number): Temperature {
    return new Temperature(value, units.temperature.hectokelvin);
  }

  static fromKilokelvins(value: number): Temperature {
    return new Temperature(value, units.temperature.kilokelvin);
  }

}
