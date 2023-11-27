import { EnergyBase } from "./models/energy-base";
import { units } from './known-unit-of-measurements';
import { UnitOfMeasurement } from "./models/unit-of-measurement";

export class Energy extends EnergyBase {

  static override get unitOfReference(): UnitOfMeasurement {
    return units.energy.calorie;
  }
  override get base(): number {
    return this.convertTo(Energy.unitOfReference).value;
  }

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

  static fromMillicalories(value: number): Energy {
    return new Energy(value, units.energy.millicalorie);
  }

  static fromCenticalories(value: number): Energy {
    return new Energy(value, units.energy.centicalorie);
  }

  static fromDecicalories(value: number): Energy {
    return new Energy(value, units.energy.decicalorie);
  }

  static fromCalories(value: number): Energy {
    return new Energy(value, units.energy.calorie);
  }

  static fromDecacalories(value: number): Energy {
    return new Energy(value, units.energy.decacalorie);
  }

  static fromHectocalories(value: number): Energy {
    return new Energy(value, units.energy.hectocalorie);
  }

  static fromKilocalories(value: number): Energy {
    return new Energy(value, units.energy.kilocalorie);
  }

  static fromMillijoules(value: number): Energy {
    return new Energy(value, units.energy.millijoule);
  }

  static fromCentijoules(value: number): Energy {
    return new Energy(value, units.energy.centijoule);
  }

  static fromDecijoules(value: number): Energy {
    return new Energy(value, units.energy.decijoule);
  }

  static fromJoules(value: number): Energy {
    return new Energy(value, units.energy.joule);
  }

  static fromDecajoules(value: number): Energy {
    return new Energy(value, units.energy.decajoule);
  }

  static fromHectojoules(value: number): Energy {
    return new Energy(value, units.energy.hectojoule);
  }

  static fromKilojoules(value: number): Energy {
    return new Energy(value, units.energy.kilojoule);
  }

}
