import { LengthBase } from "./models/length-base";
import { units } from './known-unit-of-measurements';
import { UnitOfMeasurement } from "./models/unit-of-measurement";

export class Length extends LengthBase {

  static override get unitOfReference(): UnitOfMeasurement {
    return units.length.meter;
  }
  override get base(): number {
    return this.convertTo(Length.unitOfReference).value;
  }

  get millimeters(): number {
    return this.convertTo(units.length.millimeter).value;
  }

  get centimeters(): number {
    return this.convertTo(units.length.centimeter).value;
  }

  get decimeters(): number {
    return this.convertTo(units.length.decimeter).value;
  }

  get meters(): number {
    return this.convertTo(units.length.meter).value;
  }

  get decameters(): number {
    return this.convertTo(units.length.decameter).value;
  }

  get hectometers(): number {
    return this.convertTo(units.length.hectometer).value;
  }

  get kilometers(): number {
    return this.convertTo(units.length.kilometer).value;
  }

  get inches(): number {
    return this.convertTo(units.length.inch).value;
  }

  get feet(): number {
    return this.convertTo(units.length.foot).value;
  }

  get yards(): number {
    return this.convertTo(units.length.yard).value;
  }

  get miles(): number {
    return this.convertTo(units.length.mile).value;
  }

  static fromMillimeters(value: number): Length {
    return new Length(value, units.length.millimeter);
  }

  static fromCentimeters(value: number): Length {
    return new Length(value, units.length.centimeter);
  }

  static fromDecimeters(value: number): Length {
    return new Length(value, units.length.decimeter);
  }

  static fromMeters(value: number): Length {
    return new Length(value, units.length.meter);
  }

  static fromDecameters(value: number): Length {
    return new Length(value, units.length.decameter);
  }

  static fromHectometers(value: number): Length {
    return new Length(value, units.length.hectometer);
  }

  static fromKilometers(value: number): Length {
    return new Length(value, units.length.kilometer);
  }

  static fromInches(value: number): Length {
    return new Length(value, units.length.inch);
  }

  static fromFeet(value: number): Length {
    return new Length(value, units.length.foot);
  }

  static fromYards(value: number): Length {
    return new Length(value, units.length.yard);
  }

  static fromMiles(value: number): Length {
    return new Length(value, units.length.mile);
  }

}
