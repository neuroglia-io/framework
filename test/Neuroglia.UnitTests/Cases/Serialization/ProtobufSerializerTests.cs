using FluentAssertions;
using Neuroglia.Serialization;
using Neuroglia.UnitTests.Data;
using System.Threading.Tasks;
using Xunit;

namespace Neuroglia.UnitTests.Cases.Serialization
{

    public class ProtobufSerializerTests
    {

        protected ProtobufSerializer Serializer { get; } = new();

        [Fact]
        public async Task SerializeAndDeserialize_WithContact_ShouldWork()
        {
            //arrange
            var toSerialize = new TestAddress()
            {
                Street = "Fake Street",
                PostalCode = "Fake Postal Code",
                City = "Fake City",
                Country = "Fake Country"
            };

            //act
            var buffer = await this.Serializer.SerializeAsync(toSerialize);
            var deserialized = await this.Serializer.DeserializeAsync<TestAddress>(buffer);

            //assert
            deserialized.Should().NotBeNull();
            deserialized.Street.Should().Be(toSerialize.Street);
            deserialized.PostalCode.Should().Be(toSerialize.PostalCode);
            deserialized.City.Should().Be(toSerialize.City);
            deserialized.Country.Should().Be(toSerialize.Country);
        }

        [Fact]
        public async Task SerializeAndDeserialize_WithoutContact_ShouldWork()
        {
            //arrange
            var toSerialize = new
            {
                Street = "Fake Street",
                PostalCode = "Fake Postal Code",
                City = "Fake City",
                Country = "Fake Country"
            };

            //act
            var buffer = await this.Serializer.SerializeAsync(toSerialize);
            var deserialized = await this.Serializer.DeserializeAsync<ProtoObject>(buffer);

            //assert
            deserialized.Should().NotBeNull();
            var address = deserialized.ToObject<TestAddress>();
            address.Street.Should().Be(toSerialize.Street);
            address.PostalCode.Should().Be(toSerialize.PostalCode);
            address.City.Should().Be(toSerialize.City);
            address.Country.Should().Be(toSerialize.Country);
        }

    }

}
