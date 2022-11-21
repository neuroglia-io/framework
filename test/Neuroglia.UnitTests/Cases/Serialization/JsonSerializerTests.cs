using FluentAssertions;
using Microsoft.Extensions.Options;
using Neuroglia.UnitTests.Data;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;
using JsonSerializer = Neuroglia.Serialization.JsonSerializer;

namespace Neuroglia.UnitTests.Cases.Serialization
{
    public class JsonSerializerTests
    {

        protected JsonSerializer Serializer { get; } = new JsonSerializer(Options.Create(new JsonSerializerOptions()));

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

        [Fact]
        public async Task SerializeAndDeserialize_AbstractClass_ShouldWork()
        {
            //arrange
            var toSerialize = new Concretion()
            {
                Value = "foo"
            };

            //act
            var buffer = await this.Serializer.SerializeAsync(toSerialize);
            var deserialized = await this.Serializer.DeserializeAsync<Abstraction>(buffer);

            //assert
            deserialized.Should().NotBeNull();
            deserialized.As<Concretion>().Value.Should().Be(toSerialize.Value);
        }

        [Fact]
        public async Task SerializeAndDeserialize_AbstractClass_DiscriminatedByDefault_ShouldWork()
        {
            //arrange
            var toSerialize = new ExtensionConcretion()
            {
                Value = "foo"
            };

            //act
            var buffer = await this.Serializer.SerializeAsync(toSerialize);
            var deserialized = await this.Serializer.DeserializeAsync<Abstraction>(buffer);

            //assert
            deserialized.Should().NotBeNull();
            deserialized.As<ExtensionConcretion>().Value.Should().Be(toSerialize.Value);
        }

    }

}
