import { MassBase } from "./models/mass-base";
import { units } from './known-unit-of-measurements';

export class Mass extends MassBase {
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

}
