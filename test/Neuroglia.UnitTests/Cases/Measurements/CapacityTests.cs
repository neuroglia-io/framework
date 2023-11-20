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

public class CapacityTests
{

    [Fact]
    public void Create_Capacity_Should_Work()
    {
        //arrange
        var value = 5;
        var unit = Capacity.Units.Liter;

        //act
        var measurement = new Capacity(value, unit);

        //assert
        measurement.Value.Should().Be(value);
        measurement.Unit.Should().Be(unit);
        measurement.Milliliters.Should().Be(value * (1000m / unit.Ratio));
        measurement.Centiliters.Should().Be(value * (100m / unit.Ratio));
        measurement.Deciliters.Should().Be(value * (10m / unit.Ratio));
        measurement.Liters.Should().Be(value);
        measurement.Decaliters.Should().Be(value * (0.1m / unit.Ratio));
        measurement.Hectoliters.Should().Be(value * (0.01m / unit.Ratio));
        measurement.Kiloliters.Should().Be(value * (0.001m / unit.Ratio));
    }

    [Fact]
    public void Create_Capacity_WithNullUnit_Should_Fail()
    {
        //arrange
        var action = () => new Capacity(69, null!);

        //assert
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Create_Capacity_WithNonCapacityUnit_Should_Fail()
    {
        //arrange
        var action = () => new Capacity(69, Volume.Units.CubicMillimeter);

        //assert
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Compare_Capacitys_Should_Work()
    {
        //arrange
        var capacity1 = Capacity.FromCentiliters(250);
        var capacity2 = Capacity.FromLiters(5);
        var capacity3 = Capacity.FromDeciliters(50);

        //assert
        capacity1.CompareTo(capacity2).Should().Be(-1);
        capacity2.CompareTo(capacity1).Should().Be(1);
        capacity3.CompareTo(capacity2).Should().Be(0);
        capacity3.CompareTo(capacity3).Should().Be(0);

        capacity3.Equals(capacity1).Should().BeFalse();
        capacity3.Equals(capacity2).Should().BeTrue();
        capacity3.Equals(capacity2, true).Should().BeFalse();
        capacity3.Equals(capacity3, true).Should().BeTrue();
    }

    [Fact]
    public void Convert_Capacity_Should_Work()
    {
        //arrange
        var capacity = Capacity.FromCentiliters(500);
        var convertToUnit = Capacity.Units.Liter;

        //act
        var convertedCapacity = capacity.ConvertTo(convertToUnit);

        //assert
        convertedCapacity.Unit.Should().Be(convertToUnit);
        convertedCapacity.Value.Should().Be(capacity.Value / 100m);
    }

    [Fact]
    public void Convert_Capacity_WithNullUnit_Should_Throw()
    {
        //arrange
        var capacity = Capacity.FromDecaliters(3);
        var action = () => capacity.ConvertTo(null!);

        //act
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Convert_Capacity_ToNonCapacityUnit_Should_Throw()
    {
        //arrange
        var capacity = Capacity.FromDecaliters(3);
        var action = () => capacity.ConvertTo(Volume.Units.CubicDecimeter);

        //act
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_Capacity_FromMilliliters_Should_Work()
    {
        //act
        var value = 69;
        var capacity = Capacity.FromMilliliters(value);

        //assert
        capacity.Value.Should().Be(value);
        capacity.Unit.Should().Be(Capacity.Units.Milliliter);
    }

    [Fact]
    public void Create_Capacity_FromCentiliters_Should_Work()
    {
        //act
        var value = 69;
        var capacity = Capacity.FromCentiliters(value);

        //assert
        capacity.Value.Should().Be(value);
        capacity.Unit.Should().Be(Capacity.Units.Centiliter);
    }

    [Fact]
    public void Create_Capacity_FromDeciliters_Should_Work()
    {
        //act
        var value = 69;
        var capacity = Capacity.FromDeciliters(value);

        //assert
        capacity.Value.Should().Be(value);
        capacity.Unit.Should().Be(Capacity.Units.Deciliter);
    }

    [Fact]
    public void Create_Capacity_FromLiters_Should_Work()
    {
        //act
        var value = 69;
        var capacity = Capacity.FromLiters(value);

        //assert
        capacity.Value.Should().Be(value);
        capacity.Unit.Should().Be(Capacity.Units.Liter);
    }

    [Fact]
    public void Create_Capacity_FromDecaliters_Should_Work()
    {
        //act
        var value = 69;
        var capacity = Capacity.FromDecaliters(value);

        //assert
        capacity.Value.Should().Be(value);
        capacity.Unit.Should().Be(Capacity.Units.Decaliter);
    }

    [Fact]
    public void Create_Capacity_FromHectoliters_Should_Work()
    {
        //act
        var value = 69;
        var capacity = Capacity.FromHectoliters(value);

        //assert
        capacity.Value.Should().Be(value);
        capacity.Unit.Should().Be(Capacity.Units.Hectoliter);
    }

    [Fact]
    public void Create_Capacity_FromKiloliters_Should_Work()
    {
        //act
        var value = 69;
        var capacity = Capacity.FromKiloliters(value);

        //assert
        capacity.Value.Should().Be(value);
        capacity.Unit.Should().Be(Capacity.Units.Kiloliter);
    }

    [Fact]
    public void Create_Capacity_FromUSFluidDrams_Should_Work()
    {
        //act
        var value = 69;
        var capacity = Capacity.FromUSFluidDrams(value);

        //assert
        capacity.Value.Should().Be(value);
        capacity.Unit.Should().Be(Capacity.Units.USFluidDram);
    }

    [Fact]
    public void Create_Capacity_FromUSFluidOunces_Should_Work()
    {
        //act
        var value = 69;
        var capacity = Capacity.FromUSFluidOunces(value);

        //assert
        capacity.Value.Should().Be(value);
        capacity.Unit.Should().Be(Capacity.Units.USFluidOunce);
    }

    [Fact]
    public void Create_Capacity_FromUSGills_Should_Work()
    {
        //act
        var value = 69;
        var capacity = Capacity.FromUSGills(value);

        //assert
        capacity.Value.Should().Be(value);
        capacity.Unit.Should().Be(Capacity.Units.USGill);
    }

    [Fact]
    public void Create_Capacity_FromUSPints_Should_Work()
    {
        //act
        var value = 69;
        var capacity = Capacity.FromUSPints(value);

        //assert
        capacity.Value.Should().Be(value);
        capacity.Unit.Should().Be(Capacity.Units.USPint);
    }

    [Fact]
    public void Create_Capacity_FromUSQuarts_Should_Work()
    {
        //act
        var value = 69;
        var capacity = Capacity.FromUSQuarts(value);

        //assert
        capacity.Value.Should().Be(value);
        capacity.Unit.Should().Be(Capacity.Units.USQuart);
    }

    [Fact]
    public void Create_Capacity_FromUSGallons_Should_Work()
    {
        //act
        var value = 69;
        var capacity = Capacity.FromUSGallons(value);

        //assert
        capacity.Value.Should().Be(value);
        capacity.Unit.Should().Be(Capacity.Units.USGallon);
    }

    [Fact]
    public void Create_Capacity_FromUSPecks_Should_Work()
    {
        //act
        var value = 69;
        var capacity = Capacity.FromUSPecks(value);

        //assert
        capacity.Value.Should().Be(value);
        capacity.Unit.Should().Be(Capacity.Units.USPeck);
    }

    [Fact]
    public void Create_Capacity_FromUSBushels_Should_Work()
    {
        //act
        var value = 69;
        var capacity = Capacity.FromUSBushels(value);

        //assert
        capacity.Value.Should().Be(value);
        capacity.Unit.Should().Be(Capacity.Units.USBushel);
    }

    [Fact]
    public void Add_Capacity_WithSameUnit_Should_Work()
    {
        //arrange
        var value1 = 69;
        var value2 = 3;
        var unit = Capacity.Units.Centiliter;
        var capacity1 = new Capacity(value1, unit);
        var capacity2 = new Capacity(value2, unit);

        //act
        var result = capacity1.Add(capacity2);

        //act
        result.Unit.Should().Be(unit);
        result.Value.Should().Be(value1 + value2);
    }

    [Fact]
    public void Add_Capacity_WithDifferentUnit_Should_Work()
    {
        //arrange
        var value1 = 50;
        var value2 = 2;
        var unit1 = Capacity.Units.Centiliter;
        var unit2 = Capacity.Units.Liter;
        var capacity1 = new Capacity(value1, unit1);
        var capacity2 = new Capacity(value2, unit2);

        //act
        var result = capacity1.Add(capacity2);

        //act
        result.Unit.Should().Be(unit1);
        result.Value.Should().Be(250);
    }

    [Fact]
    public void Add_Capacity_UsingOperator_WithSameUnit_Should_Work()
    {
        //arrange
        var value1 = 69;
        var value2 = 3;
        var unit = Capacity.Units.Centiliter;
        var capacity1 = new Capacity(value1, unit);
        var capacity2 = new Capacity(value2, unit);

        //act
        var result = capacity1 + capacity2;

        //act
        result.Unit.Should().Be(unit);
        result.Value.Should().Be(value1 + value2);
    }

    [Fact]
    public void Add_Capacity_UsingOperator_WithDifferentUnit_Should_Work()
    {
        //arrange
        var value1 = 50;
        var value2 = 2;
        var unit1 = Capacity.Units.Centiliter;
        var unit2 = Capacity.Units.Liter;
        var capacity1 = new Capacity(value1, unit1);
        var capacity2 = new Capacity(value2, unit2);

        //act
        var result = capacity1 + capacity2;

        //act
        result.Unit.Should().Be(unit1);
        result.Value.Should().Be(250);
    }

    [Fact]
    public void Subtract_Capacity_WithSameUnit_Should_Work()
    {
        //arrange
        var value1 = 69;
        var value2 = 3;
        var unit = Capacity.Units.Centiliter;
        var capacity1 = new Capacity(value1, unit);
        var capacity2 = new Capacity(value2, unit);

        //act
        var result = capacity1.Subtract(capacity2);

        //act
        result.Unit.Should().Be(unit);
        result.Value.Should().Be(value1 - value2);
    }

    [Fact]
    public void Subtract_Capacity_WithDifferentUnit_Should_Work()
    {
        //arrange
        var value1 = 250;
        var value2 = 0.5m;
        var unit1 = Capacity.Units.Centiliter;
        var unit2 = Capacity.Units.Liter;
        var capacity1 = new Capacity(value1, unit1);
        var capacity2 = new Capacity(value2, unit2);

        //act
        var result = capacity1.Subtract(capacity2);

        //act
        result.Unit.Should().Be(unit1);
        result.Value.Should().Be(200);
    }

    [Fact]
    public void Subtract_Capacity_UsingOperator_WithSameUnit_Should_Work()
    {
        //arrange
        var value1 = 69;
        var value2 = 3;
        var unit = Capacity.Units.Centiliter;
        var capacity1 = new Capacity(value1, unit);
        var capacity2 = new Capacity(value2, unit);

        //act
        var result = capacity1 - capacity2;

        //act
        result.Unit.Should().Be(unit);
        result.Value.Should().Be(value1 - value2);
    }

    [Fact]
    public void Subtract_Capacity_UsingOperator_WithDifferentUnit_Should_Work()
    {
        //arrange
        var value1 = 250;
        var value2 = 0.5m;
        var unit1 = Capacity.Units.Centiliter;
        var unit2 = Capacity.Units.Liter;
        var capacity1 = new Capacity(value1, unit1);
        var capacity2 = new Capacity(value2, unit2);

        //act
        var result = capacity1 - capacity2;

        //act
        result.Unit.Should().Be(unit1);
        result.Value.Should().Be(200);
    }

    [Fact]
    public void Multiply_Capacity_Should_Work()
    {
        //arrange
        var value = 10m;
        var multiplier = 2.5m;
        var capacity = new Capacity(value, Capacity.Units.Centiliter);

        //act
        var result = capacity * multiplier;

        //assert
        result.Value.Should().Be(value * multiplier);
        result.Unit.Should().Be(capacity.Unit);
    }

    [Fact]
    public void Divide_Capacity_Should_Work()
    {
        //arrange
        var value = 10m;
        var divider = 2.5m;
        var capacity = new Capacity(value, Capacity.Units.Centiliter);

        //act
        var result = capacity / divider;

        //assert
        result.Value.Should().Be(value / divider);
        result.Unit.Should().Be(capacity.Unit);
    }

}