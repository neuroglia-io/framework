import { SurfaceBase } from "./models/surface-base";
import { units } from './known-unit-of-measurements';
import { UnitOfMeasurement } from "./models/unit-of-measurement";

export class Surface extends SurfaceBase {

  static override get unitOfReference(): UnitOfMeasurement {
    return units.surface.squareMeter;
  }
  override get base(): number {
    return this.convertTo(Surface.unitOfReference).value;
  }

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

  static fromSquareMillimeters(value: number): Surface {
    return new Surface(value, units.surface.squareMillimeter);
  }

  static fromSquareCentimeters(value: number): Surface {
    return new Surface(value, units.surface.squareCentimeter);
  }

  static fromSquareDecimeters(value: number): Surface {
    return new Surface(value, units.surface.squareDecimeter);
  }

  static fromSquareMeters(value: number): Surface {
    return new Surface(value, units.surface.squareMeter);
  }

  static fromSquareDecameters(value: number): Surface {
    return new Surface(value, units.surface.squareDecameter);
  }

  static fromSquareHectometers(value: number): Surface {
    return new Surface(value, units.surface.squareHectometer);
  }

  static fromSquareKilometers(value: number): Surface {
    return new Surface(value, units.surface.squareKilometer);
  }

  static fromSquareInches(value: number): Surface {
    return new Surface(value, units.surface.squareInch);
  }

  static fromSquareFeet(value: number): Surface {
    return new Surface(value, units.surface.squareFoot);
  }

  static fromSquareYards(value: number): Surface {
    return new Surface(value, units.surface.squareYard);
  }

  static fromSquareMiles(value: number): Surface {
    return new Surface(value, units.surface.squareMile);
  }

}
