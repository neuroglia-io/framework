// Copyright © 2021-Present Neuroglia SRL. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License"),
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Neuroglia.Measurements;

namespace Neuroglia.UnitTests.Cases.Measurements;

public class LengthTests
{

    [Fact]
    public void Create_Length_Should_Work()
    {
        //arrange
        var value = 5;
        var unit = Length.Units.Meter;

        //act
        var measurement = new Length(value, unit);

        //assert
        measurement.Value.Should().Be(value);
        measurement.Unit.Should().Be(unit);
        measurement.Millimeters.Should().Be(value * (1000m / unit.Ratio));
        measurement.Centimeters.Should().Be(value * (100m / unit.Ratio));
        measurement.Decimeters.Should().Be(value * (10m / unit.Ratio));
        measurement.Meters.Should().Be(value);
        measurement.Decameters.Should().Be(value * (0.1m / unit.Ratio));
        measurement.Hectometers.Should().Be(value * (0.01m / unit.Ratio));
        measurement.Kilometers.Should().Be(value * (0.001m / unit.Ratio));

        measurement.Inches.Should().Be(value * (39.3700787402m / unit.Ratio));
        measurement.Feet.Should().Be(value * (3.28084m / unit.Ratio));
        measurement.Yards.Should().Be(value * (1.09361m / unit.Ratio));
        measurement.Miles.Should().Be(value * (0.000621371m / unit.Ratio));
    }

    [Fact]
    public void Create_Length_WithNullUnit_Should_Fail()
    {
        //arrange
        var action = () => new Length(69, null!);

        //assert
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Create_Length_WithNonLengthUnit_Should_Fail()
    {
        //arrange
        var action = () => new Length(69, Volume.Units.CubicMillimeter);

        //assert
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Compare_Lengths_Should_Work()
    {
        //arrange
        var length1 = Length.FromCentimeters(250);
        var length2 = Length.FromMeters(5);
        var length3 = Length.FromDecimeters(50);

        //assert
        length1.CompareTo(length2).Should().Be(-1);
        length2.CompareTo(length1).Should().Be(1);
        length3.CompareTo(length2).Should().Be(0);
        length3.CompareTo(length3).Should().Be(0);

        length3.Equals(length1).Should().BeFalse();
        length3.Equals(length2).Should().BeTrue();
        length3.Equals(length2, true).Should().BeFalse();
        length3.Equals(length3, true).Should().BeTrue();
    }

    [Fact]
    public void Convert_Length_Should_Work()
    {
        //arrange
        var length = Length.FromCentimeters(500);
        var convertToUnit = Length.Units.Meter;

        //act
        var convertedLength = length.ConvertTo(convertToUnit);

        //assert
        convertedLength.Unit.Should().Be(convertToUnit);
        convertedLength.Value.Should().Be(length.Value / 100m);
    }

    [Fact]
    public void Convert_Length_WithNullUnit_Should_Throw()
    {
        //arrange
        var length = Length.FromInches(3);
        var action = () => length.ConvertTo(null!);

        //act
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Convert_Length_ToNonLengthUnit_Should_Throw()
    {
        //arrange
        var length = Length.FromInches(3);
        var action = () => length.ConvertTo(Volume.Units.CubicCentimeter);

        //act
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_Length_FromMillimeters_Should_Work()
    {
        //act
        var value = 69;
        var length = Length.FromMillimeters(value);

        //assert
        length.Value.Should().Be(value);
        length.Unit.Should().Be(Length.Units.Millimeter);
    }

    [Fact]
    public void Create_Length_FromCentimeters_Should_Work()
    {
        //act
        var value = 69;
        var length = Length.FromCentimeters(value);

        //assert
        length.Value.Should().Be(value);
        length.Unit.Should().Be(Length.Units.Centimeter);
    }

    [Fact]
    public void Create_Length_FromDecimeters_Should_Work()
    {
        //act
        var value = 69;
        var length = Length.FromDecimeters(value);

        //assert
        length.Value.Should().Be(value);
        length.Unit.Should().Be(Length.Units.Decimeter);
    }

    [Fact]
    public void Create_Length_FromMeters_Should_Work()
    {
        //act
        var value = 69;
        var length = Length.FromMeters(value);

        //assert
        length.Value.Should().Be(value);
        length.Unit.Should().Be(Length.Units.Meter);
    }

    [Fact]
    public void Create_Length_FromDecameters_Should_Work()
    {
        //act
        var value = 69;
        var length = Length.FromDecameters(value);

        //assert
        length.Value.Should().Be(value);
        length.Unit.Should().Be(Length.Units.Decameter);
    }

    [Fact]
    public void Create_Length_FromHectometers_Should_Work()
    {
        //act
        var value = 69;
        var length = Length.FromHectometers(value);

        //assert
        length.Value.Should().Be(value);
        length.Unit.Should().Be(Length.Units.Hectometer);
    }

    [Fact]
    public void Create_Length_FromKilometers_Should_Work()
    {
        //act
        var value = 69;
        var length = Length.FromKilometers(value);

        //assert
        length.Value.Should().Be(value);
        length.Unit.Should().Be(Length.Units.Kilometer);
    }

    [Fact]
    public void Create_Length_FromInches_Should_Work()
    {
        //act
        var value = 69;
        var length = Length.FromInches(value);

        //assert
        length.Value.Should().Be(value);
        length.Unit.Should().Be(Length.Units.Inch);
    }

    [Fact]
    public void Create_Length_FromFeet_Should_Work()
    {
        //act
        var value = 69;
        var length = Length.FromFeet(value);

        //assert
        length.Value.Should().Be(value);
        length.Unit.Should().Be(Length.Units.Foot);
    }

    [Fact]
    public void Create_Length_FromYards_Should_Work()
    {
        //act
        var value = 69;
        var length = Length.FromYards(value);

        //assert
        length.Value.Should().Be(value);
        length.Unit.Should().Be(Length.Units.Yard);
    }

    [Fact]
    public void Create_Length_FromMiles_Should_Work()
    {
        //act
        var value = 69;
        var length = Length.FromMiles(value);

        //assert
        length.Value.Should().Be(value);
        length.Unit.Should().Be(Length.Units.Mile);
    }

    [Fact]
    public void Add_Length_WithSameUnit_Should_Work()
    {
        //arrange
        var value1 = 69;
        var value2 = 3;
        var unit = Length.Units.Centimeter;
        var length1 = new Length(value1, unit);
        var length2 = new Length(value2, unit);

        //act
        var result = length1.Add(length2);

        //act
        result.Unit.Should().Be(unit);
        result.Value.Should().Be(value1 + value2);
    }

    [Fact]
    public void Add_Length_WithDifferentUnit_Should_Work()
    {
        //arrange
        var value1 = 50;
        var value2 = 2;
        var unit1 = Length.Units.Centimeter;
        var unit2 = Length.Units.Meter;
        var length1 = new Length(value1, unit1);
        var length2 = new Length(value2, unit2);

        //act
        var result = length1.Add(length2);

        //act
        result.Unit.Should().Be(unit1);
        result.Value.Should().Be(250);
    }

    [Fact]
    public void Add_Length_UsingOperator_WithSameUnit_Should_Work()
    {
        //arrange
        var value1 = 69;
        var value2 = 3;
        var unit = Length.Units.Centimeter;
        var length1 = new Length(value1, unit);
        var length2 = new Length(value2, unit);

        //act
        var result = length1 + length2;

        //act
        result.Unit.Should().Be(unit);
        result.Value.Should().Be(value1 + value2);
    }

    [Fact]
    public void Add_Length_UsingOperator_WithDifferentUnit_Should_Work()
    {
        //arrange
        var value1 = 50;
        var value2 = 2;
        var unit1 = Length.Units.Centimeter;
        var unit2 = Length.Units.Meter;
        var length1 = new Length(value1, unit1);
        var length2 = new Length(value2, unit2);

        //act
        var result = length1 + length2;

        //act
        result.Unit.Should().Be(unit1);
        result.Value.Should().Be(250);
    }

    [Fact]
    public void Subtract_Length_WithSameUnit_Should_Work()
    {
        //arrange
        var value1 = 69;
        var value2 = 3;
        var unit = Length.Units.Centimeter;
        var length1 = new Length(value1, unit);
        var length2 = new Length(value2, unit);

        //act
        var result = length1.Subtract(length2);

        //act
        result.Unit.Should().Be(unit);
        result.Value.Should().Be(value1 - value2);
    }

    [Fact]
    public void Subtract_Length_WithDifferentUnit_Should_Work()
    {
        //arrange
        var value1 = 250;
        var value2 = 0.5m;
        var unit1 = Length.Units.Centimeter;
        var unit2 = Length.Units.Meter;
        var length1 = new Length(value1, unit1);
        var length2 = new Length(value2, unit2);

        //act
        var result = length1.Subtract(length2);

        //act
        result.Unit.Should().Be(unit1);
        result.Value.Should().Be(200);
    }

    [Fact]
    public void Subtract_Length_UsingOperator_WithSameUnit_Should_Work()
    {
        //arrange
        var value1 = 69;
        var value2 = 3;
        var unit = Length.Units.Centimeter;
        var length1 = new Length(value1, unit);
        var length2 = new Length(value2, unit);

        //act
        var result = length1 - length2;

        //act
        result.Unit.Should().Be(unit);
        result.Value.Should().Be(value1 - value2);
    }

    [Fact]
    public void Subtract_Length_UsingOperator_WithDifferentUnit_Should_Work()
    {
        //arrange
        var value1 = 250;
        var value2 = 0.5m;
        var unit1 = Length.Units.Centimeter;
        var unit2 = Length.Units.Meter;
        var length1 = new Length(value1, unit1);
        var length2 = new Length(value2, unit2);

        //act
        var result = length1 - length2;

        //act
        result.Unit.Should().Be(unit1);
        result.Value.Should().Be(200);
    }

    [Fact]
    public void Multiply_Length_Should_Work()
    {
        //arrange
        var value = 10m;
        var multiplier = 2.5m;
        var length = new Length(value, Length.Units.Meter);

        //act
        var result = length * multiplier;

        //assert
        result.Value.Should().Be(value * multiplier);
        result.Unit.Should().Be(length.Unit);
    }

    [Fact]
    public void Divide_Length_Should_Work()
    {
        //arrange
        var value = 10m;
        var divider = 2.5m;
        var length = new Length(value, Length.Units.Meter);

        //act
        var result = length / divider;

        //assert
        result.Value.Should().Be(value / divider);
        result.Unit.Should().Be(length.Unit);
    }

}
