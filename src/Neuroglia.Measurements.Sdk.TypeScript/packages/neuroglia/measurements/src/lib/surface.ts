import { SurfaceBase } from "./models/surface-base";
import { units } from './known-unit-of-measurements';

export class Surface extends SurfaceBase {

  get squareMillimeters(): number {
    return this.convertTo(units.surface.squareMillimeter).value;
  }

  get squareCentimeters(): number {
    return this.convertTo(units.surface.squareCentimeter).value;
  }

  get squareDecimeters(): number {
    return this.convertTo(units.surface.squareDecimeter).value;
  }

  get squareMeters(): number {
    return this.convertTo(units.surface.squareMeter).value;
  }

  get squareDecameters(): number {
    return this.convertTo(units.surface.squareDecameter).value;
  }

  get squareHectometers(): number {
    return this.convertTo(units.surface.squareHectometer).value;
  }

  get squareKilometers(): number {
    return this.convertTo(units.surface.squareKilometer).value;
  }

  get squareInches(): number {
    return this.convertTo(units.surface.squareInch).value;
  }

  get squareFeet(): number {
    return this.convertTo(units.surface.squareFoot).value;
  }

  get squareYards(): number {
    return this.convertTo(units.surface.squareYard).value;
  }

  get squareMiles(): number {
    return this.convertTo(units.surface.squareMile).value;
  }

}
