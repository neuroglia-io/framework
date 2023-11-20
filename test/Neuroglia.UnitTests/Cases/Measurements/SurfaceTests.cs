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

public class SurfaceTests
{

    [Fact]
    public void Create_Surface_Should_Work()
    {
        //arrange
        var value = 5;
        var unit = Surface.Units.SquareMeter;

        //act
        var measurement = new Surface(value, unit);

        //assert
        measurement.Value.Should().Be(value);
        measurement.Unit.Should().Be(unit);
        measurement.SquareMillimeters.Should().Be(value * (1000m / unit.Ratio));
        measurement.SquareCentimeters.Should().Be(value * (100m / unit.Ratio));
        measurement.SquareDecimeters.Should().Be(value * (10m / unit.Ratio));
        measurement.SquareMeters.Should().Be(value);
        measurement.SquareDecameters.Should().Be(value * (0.1m / unit.Ratio));
        measurement.SquareHectometers.Should().Be(value * (0.01m / unit.Ratio));
        measurement.SquareKilometers.Should().Be(value * (0.001m / unit.Ratio));
        measurement.SquareInches.Should().Be(value * (1550.0031m / unit.Ratio));
        measurement.SquareFeet.Should().Be(value * (10.76391041671m / unit.Ratio));
        measurement.SquareYards.Should().Be(value * (1.1959900463011m / unit.Ratio));
        measurement.SquareMiles.Should().Be(value * (0.00000038610215855m / unit.Ratio));
    }

    [Fact]
    public void Create_Surface_WithNullUnit_Should_Fail()
    {
        //arrange
        var action = () => new Surface(69, null!);

        //assert
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Create_Surface_WithNonSurfaceUnit_Should_Fail()
    {
        //arrange
        var action = () => new Surface(69, Length.Units.Millimeter);

        //assert
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Compare_Surfaces_Should_Work()
    {
        //arrange
        var surface1 = Surface.FromSquareCentimeters(250);
        var surface2 = Surface.FromSquareMeters(5);
        var surface3 = Surface.FromSquareDecimeters(50);

        //assert
        surface1.CompareTo(surface2).Should().Be(-1);
        surface2.CompareTo(surface1).Should().Be(1);
        surface3.CompareTo(surface2).Should().Be(0);
        surface3.CompareTo(surface3).Should().Be(0);

        surface3.Equals(surface1).Should().BeFalse();
        surface3.Equals(surface2).Should().BeTrue();
        surface3.Equals(surface2, true).Should().BeFalse();
        surface3.Equals(surface3, true).Should().BeTrue();
    }

    [Fact]
    public void Convert_Surface_Should_Work()
    {
        //arrange
        var surface = Surface.FromSquareCentimeters(500);
        var convertToUnit = Surface.Units.SquareMeter;

        //act
        var convertedSurface = surface.ConvertTo(convertToUnit);

        //assert
        convertedSurface.Unit.Should().Be(convertToUnit);
        convertedSurface.Value.Should().Be(surface.Value / 100m);
    }

    [Fact]
    public void Convert_Surface_WithNullUnit_Should_Throw()
    {
        //arrange
        var surface = Surface.FromSquareInches(3);
        var action = () => surface.ConvertTo(null!);

        //act
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Convert_Surface_ToNonSurfaceUnit_Should_Throw()
    {
        //arrange
        var surface = Surface.FromSquareInches(3);
        var action = () => surface.ConvertTo(Length.Units.Centimeter);

        //act
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_Surface_FromSquareMillimeters_Should_Work()
    {
        //act
        var value = 69;
        var surface = Surface.FromSquareMillimeters(value);

        //assert
        surface.Value.Should().Be(value);
        surface.Unit.Should().Be(Surface.Units.SquareMillimeter);
    }

    [Fact]
    public void Create_Surface_FromSquareCentimeters_Should_Work()
    {
        //act
        var value = 69;
        var surface = Surface.FromSquareCentimeters(value);

        //assert
        surface.Value.Should().Be(value);
        surface.Unit.Should().Be(Surface.Units.SquareCentimeter);
    }

    [Fact]
    public void Create_Surface_FromSquareDecimeters_Should_Work()
    {
        //act
        var value = 69;
        var surface = Surface.FromSquareDecimeters(value);

        //assert
        surface.Value.Should().Be(value);
        surface.Unit.Should().Be(Surface.Units.SquareDecimeter);
    }

    [Fact]
    public void Create_Surface_FromSquareMeters_Should_Work()
    {
        //act
        var value = 69;
        var surface = Surface.FromSquareMeters(value);

        //assert
        surface.Value.Should().Be(value);
        surface.Unit.Should().Be(Surface.Units.SquareMeter);
    }

    [Fact]
    public void Create_Surface_FromSquareDecameters_Should_Work()
    {
        //act
        var value = 69;
        var surface = Surface.FromSquareDecameters(value);

        //assert
        surface.Value.Should().Be(value);
        surface.Unit.Should().Be(Surface.Units.SquareDecameter);
    }

    [Fact]
    public void Create_Surface_FromSquareHectometers_Should_Work()
    {
        //act
        var value = 69;
        var surface = Surface.FromSquareHectometers(value);

        //assert
        surface.Value.Should().Be(value);
        surface.Unit.Should().Be(Surface.Units.SquareHectometer);
    }

    [Fact]
    public void Create_Surface_FromSquareKilometers_Should_Work()
    {
        //act
        var value = 69;
        var surface = Surface.FromSquareKilometers(value);

        //assert
        surface.Value.Should().Be(value);
        surface.Unit.Should().Be(Surface.Units.SquareKilometer);
    }

    [Fact]
    public void Create_Surface_FromSquareInches_Should_Work()
    {
        //act
        var value = 69;
        var surface = Surface.FromSquareInches(value);

        //assert
        surface.Value.Should().Be(value);
        surface.Unit.Should().Be(Surface.Units.SquareInch);
    }

    [Fact]
    public void Create_Surface_FromSquareFeet_Should_Work()
    {
        //act
        var value = 69;
        var surface = Surface.FromSquareFeet(value);

        //assert
        surface.Value.Should().Be(value);
        surface.Unit.Should().Be(Surface.Units.SquareFoot);
    }

    [Fact]
    public void Create_Surface_FromSquareYards_Should_Work()
    {
        //act
        var value = 69;
        var surface = Surface.FromSquareYards(value);

        //assert
        surface.Value.Should().Be(value);
        surface.Unit.Should().Be(Surface.Units.SquareYard);
    }

    [Fact]
    public void Create_Surface_FromSquareMiles_Should_Work()
    {
        //act
        var value = 69;
        var surface = Surface.FromSquareMiles(value);

        //assert
        surface.Value.Should().Be(value);
        surface.Unit.Should().Be(Surface.Units.SquareMile);
    }

    [Fact]
    public void Add_Surface_WithSameUnit_Should_Work()
    {
        //arrange
        var value1 = 69;
        var value2 = 3;
        var unit = Surface.Units.SquareCentimeter;
        var surface1 = new Surface(value1, unit);
        var surface2 = new Surface(value2, unit);

        //act
        var result = surface1.Add(surface2);

        //act
        result.Unit.Should().Be(unit);
        result.Value.Should().Be(value1 + value2);
    }

    [Fact]
    public void Add_Surface_WithDifferentUnit_Should_Work()
    {
        //arrange
        var value1 = 50;
        var value2 = 2;
        var unit1 = Surface.Units.SquareCentimeter;
        var unit2 = Surface.Units.SquareMeter;
        var surface1 = new Surface(value1, unit1);
        var surface2 = new Surface(value2, unit2);

        //act
        var result = surface1.Add(surface2);

        //act
        result.Unit.Should().Be(unit1);
        result.Value.Should().Be(250);
    }

    [Fact]
    public void Add_Surface_UsingOperator_WithSameUnit_Should_Work()
    {
        //arrange
        var value1 = 69;
        var value2 = 3;
        var unit = Surface.Units.SquareCentimeter;
        var surface1 = new Surface(value1, unit);
        var surface2 = new Surface(value2, unit);

        //act
        var result = surface1 + surface2;

        //act
        result.Unit.Should().Be(unit);
        result.Value.Should().Be(value1 + value2);
    }

    [Fact]
    public void Add_Surface_UsingOperator_WithDifferentUnit_Should_Work()
    {
        //arrange
        var value1 = 50;
        var value2 = 2;
        var unit1 = Surface.Units.SquareCentimeter;
        var unit2 = Surface.Units.SquareMeter;
        var surface1 = new Surface(value1, unit1);
        var surface2 = new Surface(value2, unit2);

        //act
        var result = surface1 + surface2;

        //act
        result.Unit.Should().Be(unit1);
        result.Value.Should().Be(250);
    }

    [Fact]
    public void Subtract_Surface_WithSameUnit_Should_Work()
    {
        //arrange
        var value1 = 69;
        var value2 = 3;
        var unit = Surface.Units.SquareCentimeter;
        var surface1 = new Surface(value1, unit);
        var surface2 = new Surface(value2, unit);

        //act
        var result = surface1.Subtract(surface2);

        //act
        result.Unit.Should().Be(unit);
        result.Value.Should().Be(value1 - value2);
    }

    [Fact]
    public void Subtract_Surface_WithDifferentUnit_Should_Work()
    {
        //arrange
        var value1 = 250;
        var value2 = 0.5m;
        var unit1 = Surface.Units.SquareCentimeter;
        var unit2 = Surface.Units.SquareMeter;
        var surface1 = new Surface(value1, unit1);
        var surface2 = new Surface(value2, unit2);

        //act
        var result = surface1.Subtract(surface2);

        //act
        result.Unit.Should().Be(unit1);
        result.Value.Should().Be(200);
    }

    [Fact]
    public void Subtract_Surface_UsingOperator_WithSameUnit_Should_Work()
    {
        //arrange
        var value1 = 69;
        var value2 = 3;
        var unit = Surface.Units.SquareCentimeter;
        var surface1 = new Surface(value1, unit);
        var surface2 = new Surface(value2, unit);

        //act
        var result = surface1 - surface2;

        //act
        result.Unit.Should().Be(unit);
        result.Value.Should().Be(value1 - value2);
    }

    [Fact]
    public void Subtract_Surface_UsingOperator_WithDifferentUnit_Should_Work()
    {
        //arrange
        var value1 = 250;
        var value2 = 0.5m;
        var unit1 = Surface.Units.SquareCentimeter;
        var unit2 = Surface.Units.SquareMeter;
        var surface1 = new Surface(value1, unit1);
        var surface2 = new Surface(value2, unit2);

        //act
        var result = surface1 - surface2;

        //act
        result.Unit.Should().Be(unit1);
        result.Value.Should().Be(200);
    }

    [Fact]
    public void Multiply_Surface_Should_Work()
    {
        //arrange
        var value = 10m;
        var multiplier = 2.5m;
        var surface = new Surface(value, Surface.Units.SquareMeter);

        //act
        var result = surface * multiplier;

        //assert
        result.Value.Should().Be(value * multiplier);
        result.Unit.Should().Be(surface.Unit);
    }

    [Fact]
    public void Divide_Surface_Should_Work()
    {
        //arrange
        var value = 10m;
        var divider = 2.5m;
        var surface = new Surface(value, Surface.Units.SquareMeter);

        //act
        var result = surface / divider;

        //assert
        result.Value.Should().Be(value / divider);
        result.Unit.Should().Be(surface.Unit);
    }

}
