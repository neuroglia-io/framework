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

public class EnergyTests
{

    [Fact]
    public void Create_Energy_Should_Work()
    {
        //arrange
        var value = 5;
        var unit = Energy.Units.Calorie;

        //act
        var measurement = new Energy(value, unit);

        //assert
        measurement.Value.Should().Be(value);
        measurement.Unit.Should().Be(unit);
        measurement.Millicalories.Should().Be(value * (1000m / unit.Ratio));
        measurement.Centicalories.Should().Be(value * (100m / unit.Ratio));
        measurement.Decicalories.Should().Be(value * (10m / unit.Ratio));
        measurement.Calories.Should().Be(value);
        measurement.Decacalories.Should().Be(value * (0.1m / unit.Ratio));
        measurement.Hectocalories.Should().Be(value * (0.01m / unit.Ratio));
        measurement.Kilocalories.Should().Be(value * (0.001m / unit.Ratio));

        measurement.Millijoules.Should().Be(value * (4184m / unit.Ratio));
        measurement.Centijoules.Should().Be(value * (418.4m / unit.Ratio));
        measurement.Decijoules.Should().Be(value * (41.84m / unit.Ratio));
        measurement.Joules.Should().Be(value * (4.184m / unit.Ratio));
        measurement.Decajoules.Should().Be(value * (0.4184m / unit.Ratio));
        measurement.Hectojoules.Should().Be(value * (0.04184m / unit.Ratio));
        measurement.Kilojoules.Should().Be(value * (0.004184m / unit.Ratio));
    }

    [Fact]
    public void Create_Energy_WithNullUnit_Should_Fail()
    {
        //arrange
        var action = () => new Energy(69, null!);

        //assert
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Create_Energy_WithNonEnergyUnit_Should_Fail()
    {
        //arrange
        var action = () => new Energy(69, Volume.Units.CubicMillimeter);

        //assert
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Compare_Energys_Should_Work()
    {
        //arrange
        var energy1 = Energy.FromCenticalories(250);
        var energy2 = Energy.FromCalories(5);
        var energy3 = Energy.FromDecicalories(50);

        //assert
        energy1.CompareTo(energy2).Should().Be(-1);
        energy2.CompareTo(energy1).Should().Be(1);
        energy3.CompareTo(energy2).Should().Be(0);
        energy3.CompareTo(energy3).Should().Be(0);

        energy3.Equals(energy1).Should().BeFalse();
        energy3.Equals(energy2).Should().BeTrue();
        energy3.Equals(energy2, true).Should().BeFalse();
        energy3.Equals(energy3, true).Should().BeTrue();
    }

    [Fact]
    public void Convert_Energy_Should_Work()
    {
        //arrange
        var energy = Energy.FromCenticalories(500);
        var convertToUnit = Energy.Units.Calorie;

        //act
        var convertedEnergy = energy.ConvertTo(convertToUnit);

        //assert
        convertedEnergy.Unit.Should().Be(convertToUnit);
        convertedEnergy.Value.Should().Be(energy.Value / 100m);
    }

    [Fact]
    public void Convert_Energy_WithNullUnit_Should_Throw()
    {
        //arrange
        var energy = Energy.FromCalories(3);
        var action = () => energy.ConvertTo(null!);

        //act
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Convert_Energy_ToNonEnergyUnit_Should_Throw()
    {
        //arrange
        var energy = Energy.FromCalories(3);
        var action = () => energy.ConvertTo(Volume.Units.CubicCentimeter);

        //act
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_Energy_FromMillicalories_Should_Work()
    {
        //act
        var value = 69;
        var energy = Energy.FromMillicalories(value);

        //assert
        energy.Value.Should().Be(value);
        energy.Unit.Should().Be(Energy.Units.Millicalorie);
    }

    [Fact]
    public void Create_Energy_FromCenticalories_Should_Work()
    {
        //act
        var value = 69;
        var energy = Energy.FromCenticalories(value);

        //assert
        energy.Value.Should().Be(value);
        energy.Unit.Should().Be(Energy.Units.Centicalorie);
    }

    [Fact]
    public void Create_Energy_FromDecicalories_Should_Work()
    {
        //act
        var value = 69;
        var energy = Energy.FromDecicalories(value);

        //assert
        energy.Value.Should().Be(value);
        energy.Unit.Should().Be(Energy.Units.Decicalorie);
    }

    [Fact]
    public void Create_Energy_FromCalories_Should_Work()
    {
        //act
        var value = 69;
        var energy = Energy.FromCalories(value);

        //assert
        energy.Value.Should().Be(value);
        energy.Unit.Should().Be(Energy.Units.Calorie);
    }

    [Fact]
    public void Create_Energy_FromDecacalories_Should_Work()
    {
        //act
        var value = 69;
        var energy = Energy.FromDecacalories(value);

        //assert
        energy.Value.Should().Be(value);
        energy.Unit.Should().Be(Energy.Units.Decacalorie);
    }

    [Fact]
    public void Create_Energy_FromHectocalories_Should_Work()
    {
        //act
        var value = 69;
        var energy = Energy.FromHectocalories(value);

        //assert
        energy.Value.Should().Be(value);
        energy.Unit.Should().Be(Energy.Units.Hectocalorie);
    }

    [Fact]
    public void Create_Energy_FromKilocalories_Should_Work()
    {
        //act
        var value = 69;
        var energy = Energy.FromKilocalories(value);

        //assert
        energy.Value.Should().Be(value);
        energy.Unit.Should().Be(Energy.Units.Kilocalorie);
    }

    [Fact]
    public void Create_Energy_FromMillijoules_Should_Work()
    {
        //act
        var value = 69;
        var energy = Energy.FromMillijoules(value);

        //assert
        energy.Value.Should().Be(value);
        energy.Unit.Should().Be(Energy.Units.Millijoule);
    }

    [Fact]
    public void Create_Energy_FromCentijoules_Should_Work()
    {
        //act
        var value = 69;
        var energy = Energy.FromCentijoules(value);

        //assert
        energy.Value.Should().Be(value);
        energy.Unit.Should().Be(Energy.Units.Centijoule);
    }

    [Fact]
    public void Create_Energy_FromDecijoules_Should_Work()
    {
        //act
        var value = 69;
        var energy = Energy.FromDecijoules(value);

        //assert
        energy.Value.Should().Be(value);
        energy.Unit.Should().Be(Energy.Units.Decijoule);
    }

    [Fact]
    public void Create_Energy_FromJoules_Should_Work()
    {
        //act
        var value = 69;
        var energy = Energy.FromJoules(value);

        //assert
        energy.Value.Should().Be(value);
        energy.Unit.Should().Be(Energy.Units.Joule);
    }

    [Fact]
    public void Create_Energy_FromDecajoules_Should_Work()
    {
        //act
        var value = 69;
        var energy = Energy.FromDecajoules(value);

        //assert
        energy.Value.Should().Be(value);
        energy.Unit.Should().Be(Energy.Units.Decajoule);
    }

    [Fact]
    public void Create_Energy_FromHectojoules_Should_Work()
    {
        //act
        var value = 69;
        var energy = Energy.FromHectojoules(value);

        //assert
        energy.Value.Should().Be(value);
        energy.Unit.Should().Be(Energy.Units.Hectojoule);
    }

    [Fact]
    public void Create_Energy_FromKilojoules_Should_Work()
    {
        //act
        var value = 69;
        var energy = Energy.FromKilojoules(value);

        //assert
        energy.Value.Should().Be(value);
        energy.Unit.Should().Be(Energy.Units.Kilojoule);
    }

    [Fact]
    public void Add_Energy_WithSameUnit_Should_Work()
    {
        //arrange
        var value1 = 69;
        var value2 = 3;
        var unit = Energy.Units.Centicalorie;
        var energy1 = new Energy(value1, unit);
        var energy2 = new Energy(value2, unit);

        //act
        var result = energy1.Add(energy2);

        //act
        result.Unit.Should().Be(unit);
        result.Value.Should().Be(value1 + value2);
    }

    [Fact]
    public void Add_Energy_WithDifferentUnit_Should_Work()
    {
        //arrange
        var value1 = 50;
        var value2 = 2;
        var unit1 = Energy.Units.Centicalorie;
        var unit2 = Energy.Units.Calorie;
        var energy1 = new Energy(value1, unit1);
        var energy2 = new Energy(value2, unit2);

        //act
        var result = energy1.Add(energy2);

        //act
        result.Unit.Should().Be(unit1);
        result.Value.Should().Be(250);
    }

    [Fact]
    public void Add_Energy_UsingOperator_WithSameUnit_Should_Work()
    {
        //arrange
        var value1 = 69;
        var value2 = 3;
        var unit = Energy.Units.Centicalorie;
        var energy1 = new Energy(value1, unit);
        var energy2 = new Energy(value2, unit);

        //act
        var result = energy1 + energy2;

        //act
        result.Unit.Should().Be(unit);
        result.Value.Should().Be(value1 + value2);
    }

    [Fact]
    public void Add_Energy_UsingOperator_WithDifferentUnit_Should_Work()
    {
        //arrange
        var value1 = 50;
        var value2 = 2;
        var unit1 = Energy.Units.Centicalorie;
        var unit2 = Energy.Units.Calorie;
        var energy1 = new Energy(value1, unit1);
        var energy2 = new Energy(value2, unit2);

        //act
        var result = energy1 + energy2;

        //act
        result.Unit.Should().Be(unit1);
        result.Value.Should().Be(250);
    }

    [Fact]
    public void Subtract_Energy_WithSameUnit_Should_Work()
    {
        //arrange
        var value1 = 69;
        var value2 = 3;
        var unit = Energy.Units.Centicalorie;
        var energy1 = new Energy(value1, unit);
        var energy2 = new Energy(value2, unit);

        //act
        var result = energy1.Subtract(energy2);

        //act
        result.Unit.Should().Be(unit);
        result.Value.Should().Be(value1 - value2);
    }

    [Fact]
    public void Subtract_Energy_WithDifferentUnit_Should_Work()
    {
        //arrange
        var value1 = 250;
        var value2 = 0.5m;
        var unit1 = Energy.Units.Centicalorie;
        var unit2 = Energy.Units.Calorie;
        var energy1 = new Energy(value1, unit1);
        var energy2 = new Energy(value2, unit2);

        //act
        var result = energy1.Subtract(energy2);

        //act
        result.Unit.Should().Be(unit1);
        result.Value.Should().Be(200);
    }

    [Fact]
    public void Subtract_Energy_UsingOperator_WithSameUnit_Should_Work()
    {
        //arrange
        var value1 = 69;
        var value2 = 3;
        var unit = Energy.Units.Centicalorie;
        var energy1 = new Energy(value1, unit);
        var energy2 = new Energy(value2, unit);

        //act
        var result = energy1 - energy2;

        //act
        result.Unit.Should().Be(unit);
        result.Value.Should().Be(value1 - value2);
    }

    [Fact]
    public void Subtract_Energy_UsingOperator_WithDifferentUnit_Should_Work()
    {
        //arrange
        var value1 = 250;
        var value2 = 0.5m;
        var unit1 = Energy.Units.Centicalorie;
        var unit2 = Energy.Units.Calorie;
        var energy1 = new Energy(value1, unit1);
        var energy2 = new Energy(value2, unit2);

        //act
        var result = energy1 - energy2;

        //act
        result.Unit.Should().Be(unit1);
        result.Value.Should().Be(200);
    }

    [Fact]
    public void Multiply_Energy_Should_Work()
    {
        //arrange
        var value = 10m;
        var multiplier = 2.5m;
        var energy = new Energy(value, Energy.Units.Calorie);

        //act
        var result = energy * multiplier;

        //assert
        result.Value.Should().Be(value * multiplier);
        result.Unit.Should().Be(energy.Unit);
    }

    [Fact]
    public void Divide_Energy_Should_Work()
    {
        //arrange
        var value = 10m;
        var divider = 2.5m;
        var energy = new Energy(value, Energy.Units.Calorie);

        //act
        var result = energy / divider;

        //assert
        result.Value.Should().Be(value / divider);
        result.Unit.Should().Be(energy.Unit);
    }

}
