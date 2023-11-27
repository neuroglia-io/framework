import { VolumeBase } from "./models/volume-base";
import { units } from './known-unit-of-measurements';

export class Volume extends VolumeBase {

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

}
