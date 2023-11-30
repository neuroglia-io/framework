import { MassBase } from "./models/mass-base";
import { units } from './known-unit-of-measurements';
import { UnitOfMeasurement } from "./models/unit-of-measurement";

export class Mass extends MassBase {

  static override get unitOfReference(): UnitOfMeasurement {
    return units.mass.gram;
  }
  override get base(): number {
    return this.convertTo(Mass.unitOfReference).value;
  }

  get milligrams(): number {
    return this.convertTo(units.mass.milligram).value;
  }

  get centigrams(): number {
    return this.convertTo(units.mass.centigram).value;
  }

  get decigrams(): number {
    return this.convertTo(units.mass.decigram).value;
  }

  get grams(): number {
    return this.convertTo(units.mass.gram).value;
  }

  get decagrams(): number {
    return this.convertTo(units.mass.decagram).value;
  }

  get hectograms(): number {
    return this.convertTo(units.mass.hectogram).value;
  }

  get kilograms(): number {
    return this.convertTo(units.mass.kilogram).value;
  }

  get tons(): number {
    return this.convertTo(units.mass.ton).value;
  }

  get kilotons(): number {
    return this.convertTo(units.mass.kiloton).value;
  }

  get megatons(): number {
    return this.convertTo(units.mass.megaton).value;
  }

  get grains(): number {
    return this.convertTo(units.mass.grain).value;
  }

  get drams(): number {
    return this.convertTo(units.mass.dram).value;
  }

  get ounces(): number {
    return this.convertTo(units.mass.ounce).value;
  }

  get pounds(): number {
    return this.convertTo(units.mass.pound).value;
  }

  get stones(): number {
    return this.convertTo(units.mass.stone).value;
  }

  static fromMilligrams(value: number): Mass {
    return new Mass(value, units.mass.milligram);
  }

  static fromCentigrams(value: number): Mass {
    return new Mass(value, units.mass.centigram);
  }

  static fromDecigrams(value: number): Mass {
    return new Mass(value, units.mass.decigram);
  }

  static fromGrams(value: number): Mass {
    return new Mass(value, units.mass.gram);
  }

  static fromDecagrams(value: number): Mass {
    return new Mass(value, units.mass.decagram);
  }

  static fromHectograms(value: number): Mass {
    return new Mass(value, units.mass.hectogram);
  }

  static fromKilograms(value: number): Mass {
    return new Mass(value, units.mass.kilogram);
  }

  static fromTons(value: number): Mass {
    return new Mass(value, units.mass.ton);
  }

  static fromKilotons(value: number): Mass {
    return new Mass(value, units.mass.kiloton);
  }

  static fromMegatons(value: number): Mass {
    return new Mass(value, units.mass.megaton);
  }

  static fromGrains(value: number): Mass {
    return new Mass(value, units.mass.grain);
  }

  static fromDrams(value: number): Mass {
    return new Mass(value, units.mass.dram);
  }

  static fromOunces(value: number): Mass {
    return new Mass(value, units.mass.ounce);
  }

  static fromPounds(value: number): Mass {
    return new Mass(value, units.mass.pound);
  }

  static fromStones(value: number): Mass {
    return new Mass(value, units.mass.stone);
  }

}
