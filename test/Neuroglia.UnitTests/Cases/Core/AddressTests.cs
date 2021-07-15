using FluentAssertions;
using Xunit;

namespace Neuroglia.UnitTests.Cases.Core
{

    public class AddressTests
    {

        [Fact]
        public void Format_Default_Should_Work()
        {
            //arrange
            var street = "Foo";
            var streetNumber = "1C";
            var postalCode = "6969";
            var city = "Bar";
            var country = "Foobar";
            var address = new Address(street, streetNumber, postalCode, city, country);

            //act
            var formatted = address.ToString();

            //assert
            formatted.Should().NotBeEmpty();
            formatted.Should().Be($"{street} {streetNumber}, {postalCode} {city}, {country}");
        }

        [Fact]
        public void Format_Custom_Should_Work()
        {
            //arrange
            var street = "Foo";
            var streetNumber = "1C";
            var postalCode = "6969";
            var city = "Bar";
            var country = "Foobar";
            var address = new Address(street, streetNumber, postalCode, city, country);
            var format = "{streetNumber} {street}, {postalCode} {city}, {country}";

            //act
            var formatted = address.ToString(format);

            //assert
            formatted.Should().NotBeEmpty();
            formatted.Should().Be($"{streetNumber} {street}, {postalCode} {city}, {country}");
        }

    }

}
