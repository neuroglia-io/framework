import { LengthBase } from "./models/length-base";
import { units } from './known-unit-of-measurements';

export class Length extends LengthBase {

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

}
