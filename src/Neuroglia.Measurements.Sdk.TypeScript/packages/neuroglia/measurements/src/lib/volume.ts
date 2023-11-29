import { VolumeBase } from "./models/volume-base";
import { units } from './known-unit-of-measurements';
import { UnitOfMeasurement } from "./models/unit-of-measurement";

export class Volume extends VolumeBase {

  static override get unitOfReference(): UnitOfMeasurement {
    return units.volume.cubicMeter;
  }
  override get base(): number {
    return this.convertTo(Volume.unitOfReference).value;
  }

  get cubicMillimeters(): number {
    return this.convertTo(units.volume.cubicMillimeter).value;
  }

  get cubicCentimeters(): number {
    return this.convertTo(units.volume.cubicCentimeter).value;
  }

  get cubicDecimeters(): number {
    return this.convertTo(units.volume.cubicDecimeter).value;
  }

  get cubicMeters(): number {
    return this.convertTo(units.volume.cubicMeter).value;
  }

  get cubicDecameters(): number {
    return this.convertTo(units.volume.cubicDecameter).value;
  }

  get cubicHectometers(): number {
    return this.convertTo(units.volume.cubicHectometer).value;
  }

  get cubicKilometers(): number {
    return this.convertTo(units.volume.cubicKilometer).value;
  }

  get cubicInches(): number {
    return this.convertTo(units.volume.cubicInch).value;
  }

  get cubicFeet(): number {
    return this.convertTo(units.volume.cubicFoot).value;
  }

  get cubicYards(): number {
    return this.convertTo(units.volume.cubicYard).value;
  }

  get cubicMiles(): number {
    return this.convertTo(units.volume.cubicMile).value;
  }

  get fluidOunces(): number {
    return this.convertTo(units.volume.fluidOunce).value;
  }

  get imperialGallons(): number {
    return this.convertTo(units.volume.imperialGallon).value;
  }

  get USGallons(): number {
    return this.convertTo(units.volume.USGallon).value;
  }

  static fromCubicMillimeters(value: number): Volume {
    return new Volume(value, units.volume.cubicMillimeter);
  }

  static fromCubicCentimeters(value: number): Volume {
    return new Volume(value, units.volume.cubicCentimeter);
  }

  static fromCubicDecimeters(value: number): Volume {
    return new Volume(value, units.volume.cubicDecimeter);
  }

  static fromCubicMeters(value: number): Volume {
    return new Volume(value, units.volume.cubicMeter);
  }

  static fromCubicDecameters(value: number): Volume {
    return new Volume(value, units.volume.cubicDecameter);
  }

  static fromCubicHectometers(value: number): Volume {
    return new Volume(value, units.volume.cubicHectometer);
  }

  static fromCubicKilometers(value: number): Volume {
    return new Volume(value, units.volume.cubicKilometer);
  }

  static fromCubicInches(value: number): Volume {
    return new Volume(value, units.volume.cubicInch);
  }

  static fromCubicFeet(value: number): Volume {
    return new Volume(value, units.volume.cubicFoot);
  }

  static fromCubicYards(value: number): Volume {
    return new Volume(value, units.volume.cubicYard);
  }

  static fromCubicMiles(value: number): Volume {
    return new Volume(value, units.volume.cubicMile);
  }

  static fromFluidOunces(value: number): Volume {
    return new Volume(value, units.volume.fluidOunce);
  }

  static fromImperialGallons(value: number): Volume {
    return new Volume(value, units.volume.imperialGallon);
  }

  static fromUSGallons(value: number): Volume {
    return new Volume(value, units.volume.USGallon);
  }

}
