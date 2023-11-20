
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

public class UnitTests
{

    [Fact]
    public void Create_Unit_Should_Work()
    {
        //arrange
        var value = 5;
        var unit = Unit.Units.Unit;

        //act
        var measurement = new Unit(value, unit);

        //assert
        measurement.Value.Should().Be(value);
        measurement.Unit.Should().Be(unit);
        measurement.TotalUnits.Should().Be(value);
        measurement.Pairs.Should().Be(value * (0.5m / unit.Ratio));
        measurement.HalfDozens.Should().Be(value * (1m / 6 / unit.Ratio));
        measurement.Dozens.Should().Be(value * (1m / 12 / unit.Ratio));
    }

    [Fact]
    public void Create_Unit_WithNullUnit_Should_Fail()
    {
        //arrange
        var action = () => new Unit(69, null!);

        //assert
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Create_Unit_WithNonUnitUnit_Should_Fail()
    {
        //arrange
        var action = () => new Unit(69, Volume.Units.CubicMillimeter);

        //assert
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Compare_Units_Should_Work()
    {
        //arrange
        var unit1 = Unit.FromUnits(10);
        var unit2 = Unit.FromUnits(20);
        var unit3 = Unit.FromPairs(10);

        //assert
        unit1.CompareTo(unit2).Should().Be(-1);
        unit2.CompareTo(unit1).Should().Be(1);
        unit3.CompareTo(unit2).Should().Be(0);
        unit3.CompareTo(unit3).Should().Be(0);

        unit3.Equals(unit1).Should().BeFalse();
        unit3.Equals(unit2).Should().BeTrue();
        unit3.Equals(unit2, true).Should().BeFalse();
        unit3.Equals(unit3, true).Should().BeTrue();
    }

    [Fact]
    public void Convert_Unit_Should_Work()
    {
        //arrange
        var unit = Unit.FromUnits(500);
        var convertToUnit = Unit.Units.Pair;

        //act
        var convertedUnit = unit.ConvertTo(convertToUnit);

        //assert
        convertedUnit.Unit.Should().Be(convertToUnit);
        convertedUnit.Value.Should().Be(unit.Value / 2m);
    }

    [Fact]
    public void Convert_Unit_WithNullUnit_Should_Throw()
    {
        //arrange
        var unit = Unit.FromPairs(10);
        var action = () => unit.ConvertTo(null!);

        //act
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Convert_Unit_ToNonUnitUnit_Should_Throw()
    {
        //arrange
        var unit = Unit.FromPairs(10);
        var action = () => unit.ConvertTo(Volume.Units.CubicDecimeter);

        //act
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_Unit_FromUnits_Should_Work()
    {
        //act
        var value = 69;
        var unit = Unit.FromUnits(value);

        //assert
        unit.Value.Should().Be(value);
        unit.Unit.Should().Be(Unit.Units.Unit);
    }

    [Fact]
    public void Create_Unit_FromPairs_Should_Work()
    {
        //act
        var value = 69;
        var unit = Unit.FromPairs(value);

        //assert
        unit.Value.Should().Be(value);
        unit.Unit.Should().Be(Unit.Units.Pair);
    }

    [Fact]
    public void Create_Unit_FromHalfDozens_Should_Work()
    {
        //act
        var value = 69;
        var unit = Unit.FromHalfDozens(value);

        //assert
        unit.Value.Should().Be(value);
        unit.Unit.Should().Be(Unit.Units.HalfDozen);
    }

    [Fact]
    public void Create_Unit_FromDozens_Should_Work()
    {
        //act
        var value = 69;
        var unit = Unit.FromDozens(value);

        //assert
        unit.Value.Should().Be(value);
        unit.Unit.Should().Be(Unit.Units.Dozen);
    }

    [Fact]
    public void Add_Unit_WithSameUnit_Should_Work()
    {
        //arrange
        var value1 = 69;
        var value2 = 3;
        var unit = Unit.Units.HalfDozen;
        var unit1 = new Unit(value1, unit);
        var unit2 = new Unit(value2, unit);

        //act
        var result = unit1.Add(unit2);

        //act
        result.Unit.Should().Be(unit);
        result.Value.Should().Be(value1 + value2);
    }

    [Fact]
    public void Add_Unit_WithDifferentUnit_Should_Work()
    {
        //arrange
        var value1 = 50;
        var value2 = 2;
        var unit1 = new Unit(value1, Unit.Units.Unit);
        var unit2 = new Unit(value2, Unit.Units.HalfDozen);

        //act
        var result = unit1.Add(unit2);

        //act
        result.Unit.Should().Be(unit1.Unit);
        result.Value.Should().Be(62);
    }

    [Fact]
    public void Add_Unit_UsingOperator_WithSameUnit_Should_Work()
    {
        //arrange
        var value1 = 69;
        var value2 = 3;
        var unit = Unit.Units.Pair;
        var unit1 = new Unit(value1, unit);
        var unit2 = new Unit(value2, unit);

        //act
        var result = unit1 + unit2;

        //act
        result.Unit.Should().Be(unit);
        result.Value.Should().Be(value1 + value2);
    }

    [Fact]
    public void Add_Unit_UsingOperator_WithDifferentUnit_Should_Work()
    {
        //arrange
        var value1 = 50;
        var value2 = 2;
        var unit1 = new Unit(value1, Unit.Units.Unit);
        var unit2 = new Unit(value2, Unit.Units.Pair);

        //act
        var result = unit1 + unit2;

        //act
        result.Unit.Should().Be(unit1.Unit);
        result.Value.Should().Be(54);
    }

    [Fact]
    public void Subtract_Unit_WithSameUnit_Should_Work()
    {
        //arrange
        var value1 = 69;
        var value2 = 3;
        var unit = Unit.Units.Unit;
        var unit1 = new Unit(value1, unit);
        var unit2 = new Unit(value2, unit);

        //act
        var result = unit1.Subtract(unit2);

        //act
        result.Unit.Should().Be(unit);
        result.Value.Should().Be(value1 - value2);
    }

    [Fact]
    public void Subtract_Unit_WithDifferentUnit_Should_Work()
    {
        //arrange
        var value1 = 250;
        var value2 = 25m;
        var unit1 = new Unit(value1, Unit.Units.Unit);
        var unit2 = new Unit(value2, Unit.Units.Pair);

        //act
        var result = unit1.Subtract(unit2);

        //act
        result.Unit.Should().Be(unit1.Unit);
        result.Value.Should().Be(200);
    }

    [Fact]
    public void Subtract_Unit_UsingOperator_WithSameUnit_Should_Work()
    {
        //arrange
        var value1 = 69;
        var value2 = 3;
        var unit = Unit.Units.Unit;
        var unit1 = new Unit(value1, unit);
        var unit2 = new Unit(value2, unit);

        //act
        var result = unit1 - unit2;

        //act
        result.Unit.Should().Be(unit);
        result.Value.Should().Be(value1 - value2);
    }

    [Fact]
    public void Subtract_Unit_UsingOperator_WithDifferentUnit_Should_Work()
    {
        //arrange
        var value1 = 250;
        var value2 = 25m;
        var unit1 = new Unit(value1, Unit.Units.Unit);
        var unit2 = new Unit(value2, Unit.Units.Pair);

        //act
        var result = unit1 - unit2;

        //act
        result.Unit.Should().Be(unit1.Unit);
        result.Value.Should().Be(200);
    }

    [Fact]
    public void Multiply_Unit_Should_Work()
    {
        //arrange
        var value = 10m;
        var multiplier = 2.5m;
        var unit = new Unit(value, Unit.Units.Unit);

        //act
        var result = unit * multiplier;

        //assert
        result.Value.Should().Be(value * multiplier);
        result.Unit.Should().Be(unit.Unit);
    }

    [Fact]
    public void Divide_Unit_Should_Work()
    {
        //arrange
        var value = 10m;
        var divider = 2.5m;
        var unit = new Unit(value, Unit.Units.Unit);

        //act
        var result = unit / divider;

        //assert
        result.Value.Should().Be(value / divider);
        result.Unit.Should().Be(unit.Unit);
    }

}