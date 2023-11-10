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

public class UnitOfMeasurementTests
{

    [Fact]
    public void Create_UnitOfMeasurement_Should_Work()
    {
        //arrange
        var type = UnitOfMeasurementType.Length;
        var name = "fake-name";
        var symbol = "fake-symbol";
        var ratio = 5;

        //act
        var unit = new UnitOfMeasurement(type, name, symbol, ratio);

        //assert
        unit.Type.Should().Be(type);
        unit.Name.Should().Be(name);
        unit.Symbol.Should().Be(symbol);
        unit.Ratio.Should().Be(ratio);
    }

    [Fact]
    public void Create_UnitOfMeasurement_WithNullOrEmptyName_Should_Throw()
    {
        //arrange
        var createWithNullName = () => new UnitOfMeasurement(UnitOfMeasurementType.Length, null!, "fake-symbol", 69);
        var createWithEmptyName = () => new UnitOfMeasurement(UnitOfMeasurementType.Length, string.Empty, "fake-symbol", 69);

        //assert
        createWithNullName.Should().Throw<ArgumentNullException>();
        createWithEmptyName.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Create_UnitOfMeasurement_WithNullOrEmptySymbol_Should_Throw()
    {
        //arrange
        var createWithNullSymbol = () => new UnitOfMeasurement(UnitOfMeasurementType.Length, "fake-name", null!, 69);
        var createWithEmptySymbol = () => new UnitOfMeasurement(UnitOfMeasurementType.Length, "fake-name", string.Empty, 69);

        //assert
        createWithNullSymbol.Should().Throw<ArgumentNullException>();
        createWithEmptySymbol.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Compare_Measurements_Should_Work()
    {
        //arrange
        var unit1 = new UnitOfMeasurement(UnitOfMeasurementType.Surface, "fake-name", "fake", 2);
        var unit2 = new UnitOfMeasurement(UnitOfMeasurementType.Surface, "fake-name-1", "fake", 7);
        var unit3 = new UnitOfMeasurement(UnitOfMeasurementType.Volume, "fake-name-1", "fake", 7);

        //assert
        unit1.Should().Be(unit2);
        unit1.GetHashCode().Should().Be(unit2.GetHashCode());
        unit2.Equals((object)unit1).Should().BeTrue();
        unit2.Equals(unit1).Should().BeTrue();
        unit2.Equals(unit3).Should().BeFalse();
    }

    [Fact]
    public void Convert_Measurements_ToString_Should_Work()
    {
        //arrange
        var unit = Length.Units.Centimeter;

        //act
        var measurementString = unit.ToString();

        //act
        measurementString.Should().Be(unit.Symbol);
    }

}
