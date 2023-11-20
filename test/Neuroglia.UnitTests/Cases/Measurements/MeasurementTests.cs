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

public class MeasurementTests
{

    [Fact]
    public void Create_Measurement_Should_Work()
    {
        //arrange
        var value = 69;
        var unit = Length.Units.Centimeter;

        //act
        var measurement = new Measurement(value, unit);

        //assert
        measurement.Value.Should().Be(value);
        measurement.Unit.Should().Be(unit);
    }

    [Fact]
    public void Create_Measurement_WithNullUnit_Should_Fail()
    {
        //arrange
        var action = () => new Measurement(69, null!);

        //assert
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Add_Measurement_WithSameUnit_Should_Work()
    {
        //arrange
        var value1 = 69;
        var value2 = 3;
        var unit = Length.Units.Centimeter;
        var measurement1 = new Measurement(value1, unit);
        var measurement2 = new Measurement(value2, unit);

        //act
        var result = measurement1.Add(measurement2);

        //act
        result.Unit.Should().Be(unit);
        result.Value.Should().Be(value1 + value2);
    }

    [Fact]
    public void Add_Measurement_WithDifferentUnit_Should_Work()
    {
        //arrange
        var value1 = 50;
        var value2 = 2;
        var unit1 = Length.Units.Centimeter;
        var unit2 = Length.Units.Meter;
        var measurement1 = new Measurement(value1, unit1);
        var measurement2 = new Measurement(value2, unit2);

        //act
        var result = measurement1.Add(measurement2);

        //act
        result.Unit.Should().Be(unit1);
        result.Value.Should().Be(250);
    }

    [Fact]
    public void Add_Measurement_UsingOperator_WithSameUnit_Should_Work()
    {
        //arrange
        var value1 = 69;
        var value2 = 3;
        var unit = Length.Units.Centimeter;
        var measurement1 = new Measurement(value1, unit);
        var measurement2 = new Measurement(value2, unit);

        //act
        var result = measurement1 + measurement2;

        //act
        result.Unit.Should().Be(unit);
        result.Value.Should().Be(value1 + value2);
    }

    [Fact]
    public void Add_Measurement_UsingOperator_WithDifferentUnit_Should_Work()
    {
        //arrange
        var value1 = 50;
        var value2 = 2;
        var unit1 = Length.Units.Centimeter;
        var unit2 = Length.Units.Meter;
        var measurement1 = new Measurement(value1, unit1);
        var measurement2 = new Measurement(value2, unit2);

        //act
        var result = measurement1 + measurement2;

        //act
        result.Unit.Should().Be(unit1);
        result.Value.Should().Be(250);
    }

    [Fact]
    public void Subtract_Measurement_WithSameUnit_Should_Work()
    {
        //arrange
        var value1 = 69;
        var value2 = 3;
        var unit = Length.Units.Centimeter;
        var measurement1 = new Measurement(value1, unit);
        var measurement2 = new Measurement(value2, unit);

        //act
        var result = measurement1.Subtract(measurement2);

        //act
        result.Unit.Should().Be(unit);
        result.Value.Should().Be(value1 - value2);
    }

    [Fact]
    public void Subtract_Measurement_WithDifferentUnit_Should_Work()
    {
        //arrange
        var value1 = 250;
        var value2 = 0.5m;
        var unit1 = Length.Units.Centimeter;
        var unit2 = Length.Units.Meter;
        var measurement1 = new Measurement(value1, unit1);
        var measurement2 = new Measurement(value2, unit2);

        //act
        var result = measurement1.Subtract(measurement2);

        //act
        result.Unit.Should().Be(unit1);
        result.Value.Should().Be(200);
    }

    [Fact]
    public void Subtract_Measurement_UsingOperator_WithSameUnit_Should_Work()
    {
        //arrange
        var value1 = 69;
        var value2 = 3;
        var unit = Length.Units.Centimeter;
        var measurement1 = new Measurement(value1, unit);
        var measurement2 = new Measurement(value2, unit);

        //act
        var result = measurement1 - measurement2;

        //act
        result.Unit.Should().Be(unit);
        result.Value.Should().Be(value1 - value2);
    }

    [Fact]
    public void Subtract_Measurement_UsingOperator_WithDifferentUnit_Should_Work()
    {
        //arrange
        var value1 = 250;
        var value2 = 0.5m;
        var unit1 = Length.Units.Centimeter;
        var unit2 = Length.Units.Meter;
        var measurement1 = new Measurement(value1, unit1);
        var measurement2 = new Measurement(value2, unit2);

        //act
        var result = measurement1 - measurement2;

        //act
        result.Unit.Should().Be(unit1);
        result.Value.Should().Be(200);
    }

    [Fact]
    public void Multiply_Measurement_Should_Work()
    {
        //arrange
        var value = 10m;
        var multiplier = 2.5m;
        var measurement = new Measurement(value, Mass.Units.Gram);

        //act
        var result = measurement * multiplier;

        //assert
        result.Value.Should().Be(value * multiplier);
        result.Unit.Should().Be(measurement.Unit);
    }

    [Fact]
    public void Divide_Measurement_Should_Work()
    {
        //arrange
        var value = 10m;
        var divider = 2.5m;
        var measurement = new Measurement(value, Mass.Units.Gram);

        //act
        var result = measurement / divider;

        //assert
        result.Value.Should().Be(value / divider);
        result.Unit.Should().Be(measurement.Unit);
    }

    [Fact]
    public void Compare_Measurements_Should_Work()
    {
        //arrange
        var measurement1 = new Measurement(250, Length.Units.Centimeter);
        var measurement2 = new Measurement(5, Length.Units.Meter);
        var measurement3 = new Measurement(50, Length.Units.Decimeter);

        //assert
        measurement1.CompareTo(measurement2).Should().Be(-1);
        measurement2.CompareTo(measurement1).Should().Be(1);
        measurement3.CompareTo(measurement2).Should().Be(0);
        measurement3.CompareTo(measurement3).Should().Be(0);
        measurement3.Equals(measurement1).Should().BeFalse();
        measurement3.Equals(measurement2).Should().BeTrue();
        measurement3.Equals((object)measurement2).Should().BeTrue();
        measurement3.GetHashCode().Should().Be(measurement2.GetHashCode());
        measurement3.Equals(measurement2, true).Should().BeFalse();
        measurement3.Equals(measurement3, true).Should().BeTrue();
    }

    [Fact]
    public void Convert_Measurement_ToString_Should_Work()
    {
        //arrange
        var value = 69;
        var unit = Length.Units.Centimeter;
        var measurement = new Measurement(value, unit);

        //act
        var measurementString = measurement.ToString();

        //act
        measurementString.Should().Be($"{value} {unit.Symbol}");
    }

}
