import { CapacityBase } from "./models/capacity-base";
import { units } from './known-unit-of-measurements';

export class Capacity extends CapacityBase {
  get milliliters(): number {
    return this.convertTo(units.capacity.milliliter).value;
  }

  get centiliters(): number {
    return this.convertTo(units.capacity.centiliter).value;
  }

  get deciliters(): number {
    return this.convertTo(units.capacity.deciliter).value;
  }

  get liters(): number {
    return this.convertTo(units.capacity.liter).value;
  }

  get decaliters(): number {
    return this.convertTo(units.capacity.decaliter).value;
  }

  get hectoliters(): number {
    return this.convertTo(units.capacity.hectoliter).value;
  }

  get kiloliters(): number {
    return this.convertTo(units.capacity.kiloliter).value;
  }

  get USFluidDrams(): number {
    return this.convertTo(units.capacity.USFluidDram).value;
  }

  get USFluidOunces(): number {
    return this.convertTo(units.capacity.USFluidOunce).value;
  }

  get USGills(): number {
    return this.convertTo(units.capacity.USGill).value;
  }

  get USPints(): number {
    return this.convertTo(units.capacity.USPint).value;
  }

  get USQuarts(): number {
    return this.convertTo(units.capacity.USQuart).value;
  }

  get USGallons(): number {
    return this.convertTo(units.capacity.USGallon).value;
  }

  get USPecks(): number {
    return this.convertTo(units.capacity.USPeck).value;
  }

  get USBushels(): number {
    return this.convertTo(units.capacity.USBushel).value;
  }

}
