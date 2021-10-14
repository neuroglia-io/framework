using FluentAssertions;
using Neuroglia.Serialization;
using Neuroglia.UnitTests.Data;
using System.Threading.Tasks;
using Xunit;

namespace Neuroglia.UnitTests.Cases.Serialization
{
    public class AvroSerializerTests
    {

        protected AvroSerializer Serializer { get; } = new AvroSerializer();

        [Fact]
        public async Task SerializeAndDeserialize_ShouldWork()
        {
            //arrange
            var toSerialize = new TestAddress()
            {
                Street = "Fake Street",
                ZipCode = "Fake Postal Code",
                City = "Fake City",
                Country = "Fake Country"
            };

            //act
            var buffer = await this.Serializer.SerializeAsync(toSerialize);
            var deserialized = await this.Serializer.DeserializeAsync<TestAddress>(buffer);

            //assert
            deserialized.Should().NotBeNull();
            deserialized.Street.Should().Be(toSerialize.Street);
            deserialized.ZipCode.Should().Be(toSerialize.ZipCode);
            deserialized.City.Should().Be(toSerialize.City);
            deserialized.Country.Should().Be(toSerialize.Country);
        }

    }

}
