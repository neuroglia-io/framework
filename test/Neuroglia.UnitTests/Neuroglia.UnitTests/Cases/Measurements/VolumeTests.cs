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

public class VolumeTests
{

    [Fact]
    public void Create_Volume_Should_Work()
    {
        //arrange
        var value = 5;
        var unit = Volume.Units.CubicMeter;

        //act
        var measurement = new Volume(value, unit);

        //assert
        measurement.Value.Should().Be(value);
        measurement.Unit.Should().Be(unit);
        measurement.CubicMillimeters.Should().Be(value * (1000m / unit.Ratio));
        measurement.CubicCentimeters.Should().Be(value * (100m / unit.Ratio));
        measurement.CubicDecimeters.Should().Be(value * (10m / unit.Ratio));
        measurement.CubicMeters.Should().Be(value);
        measurement.CubicDecameters.Should().Be(value * (0.1m / unit.Ratio));
        measurement.CubicHectometers.Should().Be(value * (0.01m / unit.Ratio));
        measurement.CubicKilometers.Should().Be(value * (0.001m / unit.Ratio));

        measurement.CubicInches.Should().Be(value * (61023.7441m / unit.Ratio));
        measurement.CubicFeet.Should().Be(value * (35.314667m / unit.Ratio));
        measurement.CubicYards.Should().Be(value * (1.307951m / unit.Ratio));
        measurement.CubicMiles.Should().Be(value * (2.3991274853161m / unit.Ratio));

        measurement.FluidOunces.Should().Be(value * (33814.022702m / unit.Ratio));
        measurement.ImperialGallons.Should().Be(value * (219.96924829909m / unit.Ratio));
        measurement.USGallons.Should().Be(value * (264.172052m / unit.Ratio));
    }

    [Fact]
    public void Create_Volume_WithNullUnit_Should_Fail()
    {
        //arrange
        var action = () => new Volume(69, null!);

        //assert
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Create_Volume_WithNonVolumeUnit_Should_Fail()
    {
        //arrange
        var action = () => new Volume(69, Length.Units.Millimeter);

        //assert
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Compare_Volumes_Should_Work()
    {
        //arrange
        var volume1 = Volume.FromCubicCentimeters(250);
        var volume2 = Volume.FromCubicMeters(5);
        var volume3 = Volume.FromCubicDecimeters(50);

        //assert
        volume1.CompareTo(volume2).Should().Be(-1);
        volume2.CompareTo(volume1).Should().Be(1);
        volume3.CompareTo(volume2).Should().Be(0);
        volume3.CompareTo(volume3).Should().Be(0);

        volume3.Equals(volume1).Should().BeFalse();
        volume3.Equals(volume2).Should().BeTrue();
        volume3.Equals(volume2, true).Should().BeFalse();
        volume3.Equals(volume3, true).Should().BeTrue();
    }

    [Fact]
    public void Convert_Volume_Should_Work()
    {
        //arrange
        var volume = Volume.FromCubicCentimeters(500);
        var convertToUnit = Volume.Units.CubicMeter;

        //act
        var convertedVolume = volume.ConvertTo(convertToUnit);

        //assert
        convertedVolume.Unit.Should().Be(convertToUnit);
        convertedVolume.Value.Should().Be(volume.Value / 100m);
    }

    [Fact]
    public void Convert_Volume_WithNullUnit_Should_Throw()
    {
        //arrange
        var volume = Volume.FromCubicInches(3);
        var action = () => volume.ConvertTo(null!);

        //act
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Convert_Volume_ToNonVolumeUnit_Should_Throw()
    {
        //arrange
        var volume = Volume.FromCubicInches(3);
        var action = () => volume.ConvertTo(Length.Units.Centimeter);

        //act
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_Volume_FromCubicMillimeters_Should_Work()
    {
        //act
        var value = 69;
        var volume = Volume.FromCubicMillimeters(value);

        //assert
        volume.Value.Should().Be(value);
        volume.Unit.Should().Be(Volume.Units.CubicMillimeter);
    }

    [Fact]
    public void Create_Volume_FromCubicCentimeters_Should_Work()
    {
        //act
        var value = 69;
        var volume = Volume.FromCubicCentimeters(value);

        //assert
        volume.Value.Should().Be(value);
        volume.Unit.Should().Be(Volume.Units.CubicCentimeter);
    }

    [Fact]
    public void Create_Volume_FromCubicDecimeters_Should_Work()
    {
        //act
        var value = 69;
        var volume = Volume.FromCubicDecimeters(value);

        //assert
        volume.Value.Should().Be(value);
        volume.Unit.Should().Be(Volume.Units.CubicDecimeter);
    }

    [Fact]
    public void Create_Volume_FromCubicMeters_Should_Work()
    {
        //act
        var value = 69;
        var volume = Volume.FromCubicMeters(value);

        //assert
        volume.Value.Should().Be(value);
        volume.Unit.Should().Be(Volume.Units.CubicMeter);
    }

    [Fact]
    public void Create_Volume_FromCubicDecameters_Should_Work()
    {
        //act
        var value = 69;
        var volume = Volume.FromCubicDecameters(value);

        //assert
        volume.Value.Should().Be(value);
        volume.Unit.Should().Be(Volume.Units.CubicDecameter);
    }

    [Fact]
    public void Create_Volume_FromCubicHectometers_Should_Work()
    {
        //act
        var value = 69;
        var volume = Volume.FromCubicHectometers(value);

        //assert
        volume.Value.Should().Be(value);
        volume.Unit.Should().Be(Volume.Units.CubicHectometer);
    }

    [Fact]
    public void Create_Volume_FromCubicKilometers_Should_Work()
    {
        //act
        var value = 69;
        var volume = Volume.FromCubicKilometers(value);

        //assert
        volume.Value.Should().Be(value);
        volume.Unit.Should().Be(Volume.Units.CubicKilometer);
    }

    [Fact]
    public void Create_Volume_FromCubicInches_Should_Work()
    {
        //act
        var value = 69;
        var volume = Volume.FromCubicInches(value);

        //assert
        volume.Value.Should().Be(value);
        volume.Unit.Should().Be(Volume.Units.CubicInch);
    }

    [Fact]
    public void Create_Volume_FromCubicFeet_Should_Work()
    {
        //act
        var value = 69;
        var volume = Volume.FromCubicFeet(value);

        //assert
        volume.Value.Should().Be(value);
        volume.Unit.Should().Be(Volume.Units.CubicFoot);
    }

    [Fact]
    public void Create_Volume_FromCubicYards_Should_Work()
    {
        //act
        var value = 69;
        var volume = Volume.FromCubicYards(value);

        //assert
        volume.Value.Should().Be(value);
        volume.Unit.Should().Be(Volume.Units.CubicYard);
    }

    [Fact]
    public void Create_Volume_FromCubicMiles_Should_Work()
    {
        //act
        var value = 69;
        var volume = Volume.FromCubicMiles(value);

        //assert
        volume.Value.Should().Be(value);
        volume.Unit.Should().Be(Volume.Units.CubicMile);
    }

    [Fact]
    public void Create_Volume_FromFluidOunces_Should_Work()
    {
        //act
        var value = 69;
        var volume = Volume.FromFluidOunces(value);

        //assert
        volume.Value.Should().Be(value);
        volume.Unit.Should().Be(Volume.Units.FluidOunce);
    }

    [Fact]
    public void Create_Volume_FromImperialGallons_Should_Work()
    {
        //act
        var value = 69;
        var volume = Volume.FromImperialGallons(value);

        //assert
        volume.Value.Should().Be(value);
        volume.Unit.Should().Be(Volume.Units.ImperialGallon);
    }

    [Fact]
    public void Create_Volume_FromUSGallons_Should_Work()
    {
        //act
        var value = 69;
        var volume = Volume.FromUSGallons(value);

        //assert
        volume.Value.Should().Be(value);
        volume.Unit.Should().Be(Volume.Units.USGallon);
    }

    [Fact]
    public void Add_Volume_WithSameUnit_Should_Work()
    {
        //arrange
        var value1 = 69;
        var value2 = 3;
        var unit = Volume.Units.CubicCentimeter;
        var volume1 = new Volume(value1, unit);
        var volume2 = new Volume(value2, unit);

        //act
        var result = volume1.Add(volume2);

        //act
        result.Unit.Should().Be(unit);
        result.Value.Should().Be(value1 + value2);
    }

    [Fact]
    public void Add_Volume_WithDifferentUnit_Should_Work()
    {
        //arrange
        var value1 = 50;
        var value2 = 2;
        var unit1 = Volume.Units.CubicCentimeter;
        var unit2 = Volume.Units.CubicMeter;
        var volume1 = new Volume(value1, unit1);
        var volume2 = new Volume(value2, unit2);

        //act
        var result = volume1.Add(volume2);

        //act
        result.Unit.Should().Be(unit1);
        result.Value.Should().Be(250);
    }

    [Fact]
    public void Add_Volume_UsingOperator_WithSameUnit_Should_Work()
    {
        //arrange
        var value1 = 69;
        var value2 = 3;
        var unit = Volume.Units.CubicCentimeter;
        var volume1 = new Volume(value1, unit);
        var volume2 = new Volume(value2, unit);

        //act
        var result = volume1 + volume2;

        //act
        result.Unit.Should().Be(unit);
        result.Value.Should().Be(value1 + value2);
    }

    [Fact]
    public void Add_Volume_UsingOperator_WithDifferentUnit_Should_Work()
    {
        //arrange
        var value1 = 50;
        var value2 = 2;
        var unit1 = Volume.Units.CubicCentimeter;
        var unit2 = Volume.Units.CubicMeter;
        var volume1 = new Volume(value1, unit1);
        var volume2 = new Volume(value2, unit2);

        //act
        var result = volume1 + volume2;

        //act
        result.Unit.Should().Be(unit1);
        result.Value.Should().Be(250);
    }

    [Fact]
    public void Subtract_Volume_WithSameUnit_Should_Work()
    {
        //arrange
        var value1 = 69;
        var value2 = 3;
        var unit = Volume.Units.CubicCentimeter;
        var volume1 = new Volume(value1, unit);
        var volume2 = new Volume(value2, unit);

        //act
        var result = volume1.Subtract(volume2);

        //act
        result.Unit.Should().Be(unit);
        result.Value.Should().Be(value1 - value2);
    }

    [Fact]
    public void Subtract_Volume_WithDifferentUnit_Should_Work()
    {
        //arrange
        var value1 = 250;
        var value2 = 0.5m;
        var unit1 = Volume.Units.CubicCentimeter;
        var unit2 = Volume.Units.CubicMeter;
        var volume1 = new Volume(value1, unit1);
        var volume2 = new Volume(value2, unit2);

        //act
        var result = volume1.Subtract(volume2);

        //act
        result.Unit.Should().Be(unit1);
        result.Value.Should().Be(200);
    }

    [Fact]
    public void Subtract_Volume_UsingOperator_WithSameUnit_Should_Work()
    {
        //arrange
        var value1 = 69;
        var value2 = 3;
        var unit = Volume.Units.CubicCentimeter;
        var volume1 = new Volume(value1, unit);
        var volume2 = new Volume(value2, unit);

        //act
        var result = volume1 - volume2;

        //act
        result.Unit.Should().Be(unit);
        result.Value.Should().Be(value1 - value2);
    }

    [Fact]
    public void Subtract_Volume_UsingOperator_WithDifferentUnit_Should_Work()
    {
        //arrange
        var value1 = 250;
        var value2 = 0.5m;
        var unit1 = Volume.Units.CubicCentimeter;
        var unit2 = Volume.Units.CubicMeter;
        var volume1 = new Volume(value1, unit1);
        var volume2 = new Volume(value2, unit2);

        //act
        var result = volume1 - volume2;

        //act
        result.Unit.Should().Be(unit1);
        result.Value.Should().Be(200);
    }

    [Fact]
    public void Multiply_Volume_Should_Work()
    {
        //arrange
        var value = 10m;
        var multiplier = 2.5m;
        var volume = new Volume(value, Volume.Units.CubicMeter);

        //act
        var result = volume * multiplier;

        //assert
        result.Value.Should().Be(value * multiplier);
        result.Unit.Should().Be(volume.Unit);
    }

    [Fact]
    public void Divide_Volume_Should_Work()
    {
        //arrange
        var value = 10m;
        var divider = 2.5m;
        var volume = new Volume(value, Volume.Units.CubicMeter);

        //act
        var result = volume / divider;

        //assert
        result.Value.Should().Be(value / divider);
        result.Unit.Should().Be(volume.Unit);
    }

}
