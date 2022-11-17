using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Neuroglia.Serialization;
using Neuroglia.UnitTests.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;
using System;
using System.Collections.Generic;
using System.Globalization;
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
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.CurrentUICulture = CultureInfo.CurrentCulture;
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.CurrentCulture;
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.CurrentCulture;
            ServiceCollection services = new();
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

        [Theory]
        [MemberData(nameof(SerializeJTokenTypes))]
        public async Task SerializeAndDeserialize_JTokenTypes_ShouldWork(JToken token, string expectedSerializedValue)
        {
            //arrange
            var toSerialize = token;

            //act
            var buffer = await this.Serializer.SerializeAsync(toSerialize);
            var deserialized = await this.Serializer.DeserializeAsync<JToken>(buffer);

            //assert
            deserialized.Should().NotBeNull();
            Assert.Equal(expectedSerializedValue.Trim(), buffer.Trim());
        }

        public static IEnumerable<object[]> SerializeJTokenTypes => new List<object[]>
        {
            new object[] { JObject.FromObject(new { }), "{}" },
            new object[] { JValue.CreateNull(), "--- ''" },
            new object[] { JToken.FromObject(Guid.Parse("864febab-99d4-49af-9fc8-a46c910bcc23")), "864febab-99d4-49af-9fc8-a46c910bcc23" },
            new object[] { JToken.FromObject(DateTimeOffset.Parse("2022-01-27T11:18:23.9397185+00:00")), DateTimeOffset.Parse("2022-01-27T11:18:23.9397185+00:00").ToString(CultureInfo.InvariantCulture) },
            new object[] { JToken.FromObject(DateTime.Parse("2022-01-27T11:18:23.9397185+00:00")), DateTime.Parse("2022-01-27T11:18:23.9397185+00:00").ToString(CultureInfo.InvariantCulture) },
            new object[] { JToken.FromObject(TimeSpan.FromSeconds(1)), "PT1S" },
            new object[] { JToken.FromObject(1.25F), "1.25" },
            new object[] { JToken.FromObject(1.25D), "1.25" },
            new object[] { new Uri("#/definitions/SchemaDefinitionPointer", UriKind.RelativeOrAbsolute), "'#/definitions/SchemaDefinitionPointer'" }
        };

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
            var toSerialize = JObject.FromObject(new 
            {
                FirstName = "Fake First Name", 
                LastName = "Fake Last Name"
            });

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
            if (value is not JSchema schema)
                return;
            string json = schema.ToString();
            JToken jtoken = JsonConvert.DeserializeObject<JToken>(json);
            this.WriteJToken(emitter, jtoken);
        }

    }

}
