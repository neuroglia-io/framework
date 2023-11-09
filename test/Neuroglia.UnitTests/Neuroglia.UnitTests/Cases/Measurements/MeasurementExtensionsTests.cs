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

public class MeasurementExtensionsTests
{

    [Fact]
    public void Convert_Measurement_To_Capacity_Should_Work()
    {
        //arrange
        var value = 69;
        var unit = Capacity.Units.Liter;
        var measurement = new Measurement(value, unit);

        //act
        var volume = measurement.AsCapacity();

        //assert
        volume.Should().NotBeNull();
        volume!.Value.Should().Be(value);
        volume!.Unit.Should().Be(unit);
        new Measurement(value, Mass.Units.Gram).AsCapacity().Should().BeNull();
    }

    [Fact]
    public void Convert_Measurement_To_Length_Should_Work()
    {
        //arrange
        var value = 69;
        var unit = Length.Units.Centimeter;
        var measurement = new Measurement(value, unit);

        //act
        var length = measurement.AsLength();

        //assert
        length.Should().NotBeNull();
        length!.Value.Should().Be(value);
        length!.Unit.Should().Be(unit);
        new Measurement(value, Volume.Units.CubicCentimeter).AsLength().Should().BeNull();
    }

    [Fact]
    public void Convert_Measurement_To_Energy_Should_Work()
    {
        //arrange
        var value = 69;
        var unit = Energy.Units.Calorie;
        var measurement = new Measurement(value, unit);

        //act
        var energy = measurement.AsEnergy();

        //assert
        energy.Should().NotBeNull();
        energy!.Value.Should().Be(value);
        energy!.Unit.Should().Be(unit);
        new Measurement(value, Volume.Units.CubicCentimeter).AsEnergy().Should().BeNull();
    }

    [Fact]
    public void Convert_Measurement_To_Mass_Should_Work()
    {
        //arrange
        var value = 69;
        var unit = Mass.Units.Kilogram;
        var measurement = new Measurement(value, unit);

        //act
        var mass = measurement.AsMass();

        //assert
        mass.Should().NotBeNull();
        mass!.Value.Should().Be(value);
        mass!.Unit.Should().Be(unit);
        new Measurement(value, Volume.Units.CubicCentimeter).AsMass().Should().BeNull();
    }

    [Fact]
    public void Convert_Measurement_To_Surface_Should_Work()
    {
        //arrange
        var value = 69;
        var unit = Surface.Units.SquareMeter;
        var measurement = new Measurement(value, unit);

        //act
        var volume = measurement.AsSurface();

        //assert
        volume.Should().NotBeNull();
        volume!.Value.Should().Be(value);
        volume!.Unit.Should().Be(unit);
        new Measurement(value, Mass.Units.Gram).AsSurface().Should().BeNull();
    }

    [Fact]
    public void Convert_Measurement_To_Volume_Should_Work()
    {
        //arrange
        var value = 69;
        var unit = Volume.Units.CubicCentimeter;
        var measurement = new Measurement(value, unit);

        //act
        var volume = measurement.AsVolume();

        //assert
        volume.Should().NotBeNull();
        volume!.Value.Should().Be(value);
        volume!.Unit.Should().Be(unit);
        new Measurement(value, Mass.Units.Gram).AsVolume().Should().BeNull();
    }

}