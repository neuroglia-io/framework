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

public class MassTests
{

    [Fact]
    public void Create_Mass_Should_Work()
    {
        //arrange
        var value = 5;
        var unit = Mass.Units.Gram;

        //act
        var measurement = new Mass(value, unit);

        //assert
        measurement.Value.Should().Be(value);
        measurement.Unit.Should().Be(unit);
        measurement.Milligrams.Should().Be(value * (1000m / unit.Ratio));
        measurement.Centigrams.Should().Be(value * (100m / unit.Ratio));
        measurement.Decigrams.Should().Be(value * (10m / unit.Ratio));
        measurement.Grams.Should().Be(value);
        measurement.Decagrams.Should().Be(value * (0.1m / unit.Ratio));
        measurement.Hectograms.Should().Be(value * (0.01m / unit.Ratio));
        measurement.Kilograms.Should().Be(value * (0.001m / unit.Ratio));
        measurement.Tons.Should().Be(value * (0.000001m / unit.Ratio));
        measurement.Kilotons.Should().Be(value * (0.000000001m / unit.Ratio));
        measurement.Megatons.Should().Be(value * (0.000000000001m / unit.Ratio));
        measurement.Grains.Should().Be(value * (15.4323583529m / unit.Ratio));
        measurement.Drams.Should().Be(value * (0.31746031746031m / unit.Ratio));
        measurement.Ounces.Should().Be(value * (0.03527396195m / unit.Ratio));
        measurement.Pounds.Should().Be(value * (0.0022046226m / unit.Ratio));
        measurement.Stones.Should().Be(value * (0.000157473m / unit.Ratio));
    }

    [Fact]
    public void Create_Mass_WithNullUnit_Should_Fail()
    {
        //arrange
        var action = () => new Mass(69, null!);

        //assert
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Create_Mass_WithNonMassUnit_Should_Fail()
    {
        //arrange
        var action = () => new Mass(69, Volume.Units.CubicMillimeter);

        //assert
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Compare_Masss_Should_Work()
    {
        //arrange
        var mass1 = Mass.FromCentigrams(250);
        var mass2 = Mass.FromGrams(5);
        var mass3 = Mass.FromDecigrams(50);

        //assert
        mass1.CompareTo(mass2).Should().Be(-1);
        mass2.CompareTo(mass1).Should().Be(1);
        mass3.CompareTo(mass2).Should().Be(0);
        mass3.CompareTo(mass3).Should().Be(0);

        mass3.Equals(mass1).Should().BeFalse();
        mass3.Equals(mass2).Should().BeTrue();
        mass3.Equals(mass2, true).Should().BeFalse();
        mass3.Equals(mass3, true).Should().BeTrue();
    }

    [Fact]
    public void Convert_Mass_Should_Work()
    {
        //arrange
        var mass = Mass.FromCentigrams(500);
        var convertToUnit = Mass.Units.Gram;

        //act
        var convertedMass = mass.ConvertTo(convertToUnit);

        //assert
        convertedMass.Unit.Should().Be(convertToUnit);
        convertedMass.Value.Should().Be(mass.Value / 100m);
    }

    [Fact]
    public void Convert_Mass_WithNullUnit_Should_Throw()
    {
        //arrange
        var mass = Mass.FromDecagrams(3);
        var action = () => mass.ConvertTo(null!);

        //act
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Convert_Mass_ToNonMassUnit_Should_Throw()
    {
        //arrange
        var mass = Mass.FromDecagrams(3);
        var action = () => mass.ConvertTo(Volume.Units.CubicDecimeter);

        //act
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_Mass_FromMilligrams_Should_Work()
    {
        //act
        var value = 69;
        var mass = Mass.FromMilligrams(value);

        //assert
        mass.Value.Should().Be(value);
        mass.Unit.Should().Be(Mass.Units.Milligram);
    }

    [Fact]
    public void Create_Mass_FromCentigrams_Should_Work()
    {
        //act
        var value = 69;
        var mass = Mass.FromCentigrams(value);

        //assert
        mass.Value.Should().Be(value);
        mass.Unit.Should().Be(Mass.Units.Centigram);
    }

    [Fact]
    public void Create_Mass_FromDecigrams_Should_Work()
    {
        //act
        var value = 69;
        var mass = Mass.FromDecigrams(value);

        //assert
        mass.Value.Should().Be(value);
        mass.Unit.Should().Be(Mass.Units.Decigram);
    }

    [Fact]
    public void Create_Mass_FromGrams_Should_Work()
    {
        //act
        var value = 69;
        var mass = Mass.FromGrams(value);

        //assert
        mass.Value.Should().Be(value);
        mass.Unit.Should().Be(Mass.Units.Gram);
    }

    [Fact]
    public void Create_Mass_FromDecagrams_Should_Work()
    {
        //act
        var value = 69;
        var mass = Mass.FromDecagrams(value);

        //assert
        mass.Value.Should().Be(value);
        mass.Unit.Should().Be(Mass.Units.Decagram);
    }

    [Fact]
    public void Create_Mass_FromHectograms_Should_Work()
    {
        //act
        var value = 69;
        var mass = Mass.FromHectograms(value);

        //assert
        mass.Value.Should().Be(value);
        mass.Unit.Should().Be(Mass.Units.Hectogram);
    }

    [Fact]
    public void Create_Mass_FromKilograms_Should_Work()
    {
        //act
        var value = 69;
        var mass = Mass.FromKilograms(value);

        //assert
        mass.Value.Should().Be(value);
        mass.Unit.Should().Be(Mass.Units.Kilogram);
    }

    [Fact]
    public void Create_Mass_FromTons_Should_Work()
    {
        //act
        var value = 69;
        var mass = Mass.FromTons(value);

        //assert
        mass.Value.Should().Be(value);
        mass.Unit.Should().Be(Mass.Units.Ton);
    }

    [Fact]
    public void Create_Mass_FromKilotons_Should_Work()
    {
        //act
        var value = 69;
        var mass = Mass.FromKilotons(value);

        //assert
        mass.Value.Should().Be(value);
        mass.Unit.Should().Be(Mass.Units.Kiloton);
    }

    [Fact]
    public void Create_Mass_FromMegatons_Should_Work()
    {
        //act
        var value = 69;
        var mass = Mass.FromMegatons(value);

        //assert
        mass.Value.Should().Be(value);
        mass.Unit.Should().Be(Mass.Units.Megaton);
    }

    [Fact]
    public void Create_Mass_FromGrains_Should_Work()
    {
        //act
        var value = 69;
        var mass = Mass.FromGrains(value);

        //assert
        mass.Value.Should().Be(value);
        mass.Unit.Should().Be(Mass.Units.Grain);
    }

    [Fact]
    public void Create_Mass_FromDrams_Should_Work()
    {
        //act
        var value = 69;
        var mass = Mass.FromDrams(value);

        //assert
        mass.Value.Should().Be(value);
        mass.Unit.Should().Be(Mass.Units.Dram);
    }

    [Fact]
    public void Create_Mass_FromOunces_Should_Work()
    {
        //act
        var value = 69;
        var mass = Mass.FromOunces(value);

        //assert
        mass.Value.Should().Be(value);
        mass.Unit.Should().Be(Mass.Units.Ounce);
    }

    [Fact]
    public void Create_Mass_FromPounds_Should_Work()
    {
        //act
        var value = 69;
        var mass = Mass.FromPounds(value);

        //assert
        mass.Value.Should().Be(value);
        mass.Unit.Should().Be(Mass.Units.Pound);
    }

    [Fact]
    public void Create_Mass_FromStones_Should_Work()
    {
        //act
        var value = 69;
        var mass = Mass.FromStones(value);

        //assert
        mass.Value.Should().Be(value);
        mass.Unit.Should().Be(Mass.Units.Stone);
    }

    [Fact]
    public void Add_Mass_WithSameUnit_Should_Work()
    {
        //arrange
        var value1 = 69;
        var value2 = 3;
        var unit = Mass.Units.Centigram;
        var mass1 = new Mass(value1, unit);
        var mass2 = new Mass(value2, unit);

        //act
        var result = mass1.Add(mass2);

        //act
        result.Unit.Should().Be(unit);
        result.Value.Should().Be(value1 + value2);
    }

    [Fact]
    public void Add_Mass_WithDifferentUnit_Should_Work()
    {
        //arrange
        var value1 = 50;
        var value2 = 2;
        var unit1 = Mass.Units.Centigram;
        var unit2 = Mass.Units.Gram;
        var mass1 = new Mass(value1, unit1);
        var mass2 = new Mass(value2, unit2);

        //act
        var result = mass1.Add(mass2);

        //act
        result.Unit.Should().Be(unit1);
        result.Value.Should().Be(250);
    }

    [Fact]
    public void Add_Mass_UsingOperator_WithSameUnit_Should_Work()
    {
        //arrange
        var value1 = 69;
        var value2 = 3;
        var unit = Mass.Units.Centigram;
        var mass1 = new Mass(value1, unit);
        var mass2 = new Mass(value2, unit);

        //act
        var result = mass1 + mass2;

        //act
        result.Unit.Should().Be(unit);
        result.Value.Should().Be(value1 + value2);
    }

    [Fact]
    public void Add_Mass_UsingOperator_WithDifferentUnit_Should_Work()
    {
        //arrange
        var value1 = 50;
        var value2 = 2;
        var unit1 = Mass.Units.Centigram;
        var unit2 = Mass.Units.Gram;
        var mass1 = new Mass(value1, unit1);
        var mass2 = new Mass(value2, unit2);

        //act
        var result = mass1 + mass2;

        //act
        result.Unit.Should().Be(unit1);
        result.Value.Should().Be(250);
    }

    [Fact]
    public void Subtract_Mass_WithSameUnit_Should_Work()
    {
        //arrange
        var value1 = 69;
        var value2 = 3;
        var unit = Mass.Units.Centigram;
        var mass1 = new Mass(value1, unit);
        var mass2 = new Mass(value2, unit);

        //act
        var result = mass1.Subtract(mass2);

        //act
        result.Unit.Should().Be(unit);
        result.Value.Should().Be(value1 - value2);
    }

    [Fact]
    public void Subtract_Mass_WithDifferentUnit_Should_Work()
    {
        //arrange
        var value1 = 250;
        var value2 = 0.5m;
        var unit1 = Mass.Units.Centigram;
        var unit2 = Mass.Units.Gram;
        var mass1 = new Mass(value1, unit1);
        var mass2 = new Mass(value2, unit2);

        //act
        var result = mass1.Subtract(mass2);

        //act
        result.Unit.Should().Be(unit1);
        result.Value.Should().Be(200);
    }

    [Fact]
    public void Subtract_Mass_UsingOperator_WithSameUnit_Should_Work()
    {
        //arrange
        var value1 = 69;
        var value2 = 3;
        var unit = Mass.Units.Centigram;
        var mass1 = new Mass(value1, unit);
        var mass2 = new Mass(value2, unit);

        //act
        var result = mass1 - mass2;

        //act
        result.Unit.Should().Be(unit);
        result.Value.Should().Be(value1 - value2);
    }

    [Fact]
    public void Subtract_Mass_UsingOperator_WithDifferentUnit_Should_Work()
    {
        //arrange
        var value1 = 250;
        var value2 = 0.5m;
        var unit1 = Mass.Units.Centigram;
        var unit2 = Mass.Units.Gram;
        var mass1 = new Mass(value1, unit1);
        var mass2 = new Mass(value2, unit2);

        //act
        var result = mass1 - mass2;

        //act
        result.Unit.Should().Be(unit1);
        result.Value.Should().Be(200);
    }

    [Fact]
    public void Multiply_Mass_Should_Work()
    {
        //arrange
        var value = 10m;
        var multiplier = 2.5m;
        var mass = new Mass(value, Mass.Units.Gram);

        //act
        var result = mass * multiplier;

        //assert
        result.Value.Should().Be(value * multiplier);
        result.Unit.Should().Be(mass.Unit);
    }

    [Fact]
    public void Divide_Mass_Should_Work()
    {
        //arrange
        var value = 10m;
        var divider = 2.5m;
        var mass = new Mass(value, Mass.Units.Gram);

        //act
        var result = mass / divider;

        //assert
        result.Value.Should().Be(value / divider);
        result.Unit.Should().Be(mass.Unit);
    }

}