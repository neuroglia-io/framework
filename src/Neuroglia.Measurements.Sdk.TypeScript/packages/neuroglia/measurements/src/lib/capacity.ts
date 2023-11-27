import { CapacityBase } from "./models/capacity-base";
import { units } from './known-unit-of-measurements';
import { UnitOfMeasurement } from "./models/unit-of-measurement";

export class Capacity extends CapacityBase {

  static override get unitOfReference(): UnitOfMeasurement {
    return units.capacity.liter;
  }
  override get base(): number {
    return this.convertTo(Capacity.unitOfReference).value;
  }

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

  static fromMilliliters(value: number): Capacity {
    return new Capacity(value, units.capacity.milliliter);
  }

  static fromCentiliters(value: number): Capacity {
    return new Capacity(value, units.capacity.centiliter);
  }

  static fromDeciliters(value: number): Capacity {
    return new Capacity(value, units.capacity.deciliter);
  }

  static fromLiters(value: number): Capacity {
    return new Capacity(value, units.capacity.liter);
  }

  static fromDecaliters(value: number): Capacity {
    return new Capacity(value, units.capacity.decaliter);
  }

  static fromHectoliters(value: number): Capacity {
    return new Capacity(value, units.capacity.hectoliter);
  }

  static fromKiloliters(value: number): Capacity {
    return new Capacity(value, units.capacity.kiloliter);
  }

  static fromUSFluidDrams(value: number): Capacity {
    return new Capacity(value, units.capacity.USFluidDram);
  }

  static fromUSFluidOunces(value: number): Capacity {
    return new Capacity(value, units.capacity.USFluidOunce);
  }

  static fromUSGills(value: number): Capacity {
    return new Capacity(value, units.capacity.USGill);
  }

  static fromUSPints(value: number): Capacity {
    return new Capacity(value, units.capacity.USPint);
  }

  static fromUSQuarts(value: number): Capacity {
    return new Capacity(value, units.capacity.USQuart);
  }

  static fromUSGallons(value: number): Capacity {
    return new Capacity(value, units.capacity.USGallon);
  }

  static fromUSPecks(value: number): Capacity {
    return new Capacity(value, units.capacity.USPeck);
  }

  static fromUSBushels(value: number): Capacity {
    return new Capacity(value, units.capacity.USBushel);
  }

}
