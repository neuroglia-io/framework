import { EnergyBase } from "./models/energy-base";
import { units } from './known-unit-of-measurements';

export class Energy extends EnergyBase {

  get millicalories(): number {
    return this.convertTo(units.energy.millicalorie).value;
  }

  get centicalories(): number {
    return this.convertTo(units.energy.centicalorie).value;
  }

  get decicalories(): number {
    return this.convertTo(units.energy.decicalorie).value;
  }

  get calories(): number {
    return this.convertTo(units.energy.calorie).value;
  }

  get decacalories(): number {
    return this.convertTo(units.energy.decacalorie).value;
  }

  get hectocalories(): number {
    return this.convertTo(units.energy.hectocalorie).value;
  }

  get kilocalories(): number {
    return this.convertTo(units.energy.kilocalorie).value;
  }

  get millijoules(): number {
    return this.convertTo(units.energy.millijoule).value;
  }

  get centijoules(): number {
    return this.convertTo(units.energy.centijoule).value;
  }

  get decijoules(): number {
    return this.convertTo(units.energy.decijoule).value;
  }

  get joules(): number {
    return this.convertTo(units.energy.joule).value;
  }

  get decajoules(): number {
    return this.convertTo(units.energy.decajoule).value;
  }

  get hectojoules(): number {
    return this.convertTo(units.energy.hectojoule).value;
  }

  get kilojoules(): number {
    return this.convertTo(units.energy.kilojoule).value;
  }

}
