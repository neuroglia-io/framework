using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Neuroglia.Serialization;
using Neuroglia.UnitTests.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;
using System;
using System.Threading.Tasks;
using Xunit;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace Neuroglia.UnitTests.Cases.Serialization
{

    public class YamlDotNetSerializerTests
    {

        public YamlDotNetSerializerTests()
        {
            ServiceCollection services = new ServiceCollection();
            services.AddYamlDotNetSerializer();
            this.Serializer = services.BuildServiceProvider().GetRequiredService<YamlDotNetSerializer>();
        }

        protected YamlDotNetSerializer Serializer { get; }

        [Fact]
        public async Task SerializeAndDeserialize_ComplexObject_ShouldWork()
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
        public async Task SerializeAndDeserialize_Uri_ShouldWork()
        {
            //arrange
            var toSerialize = new Uri("http://fake.com");

            //act
            var buffer = await this.Serializer.SerializeAsync(toSerialize);
            var deserialized = await this.Serializer.DeserializeAsync<Uri>(buffer);

            //assert
            deserialized.Should().NotBeNull();
            deserialized.Should().Be(toSerialize);
        }

        [Fact]
        public async Task SerializeAndDeserialize_JObject_ShouldWork()
        {
            //arrange
            var toSerialize = JObject.FromObject(new { FirstName = "Fake First Name", LastName = "Fake Last Name" });

            //act
            var yaml = await this.Serializer.SerializeAsync(toSerialize);
            var deserialized = await this.Serializer.DeserializeAsync<JObject>(yaml);

            //assert
            deserialized.Should().NotBeNull();
            deserialized.Properties().Should().NotBeEmpty();
        }

        [Fact]
        public async Task SerializeAndDeserialize_JSchema_ShouldWork()
        {
            //arrange
            var toSerialize = new JSchemaGenerator().Generate(typeof(TestAddress));

            //act
            var yaml = await this.Serializer.SerializeAsync(toSerialize);
            var deserialized = await this.Serializer.DeserializeAsync<JSchema>(yaml);
        }

    }

    public class JSchemaTypeConverter
        : JTokenSerializer
    {

        public override bool Accepts(Type type)
        {
            return type == typeof(JSchema);
        }

        public override void WriteYaml(IEmitter emitter, object value, Type type)
        {
            JSchema schema = value as JSchema;
            if (schema == null)
                return;
            string json = schema.ToString();
            JToken jtoken = JsonConvert.DeserializeObject<JToken>(json);
            this.WriteJToken(emitter, jtoken);
        }

    }

    

}
