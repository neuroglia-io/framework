using FluentAssertions;
using Neuroglia.Serialization;
using Neuroglia.UnitTests.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
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
        public async Task SerializeAndDeserialize_WithoutContact_ShouldWork()
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
            var buffer = await this.Serializer.SerializeAsync(ProtoObject.FromObject(toSerialize));
            var deserialized = await this.Serializer.DeserializeAsync<ProtoObject>(buffer);

            //assert
            deserialized.Should().NotBeNull();
            var address = deserialized.ToObject<TestAddress>();
            address.Street.Should().Be(toSerialize.Street);
            address.ZipCode.Should().Be(toSerialize.ZipCode);
            address.City.Should().Be(toSerialize.City);
            address.Country.Should().Be(toSerialize.Country);
        }

        [Fact]
        public async Task SerializeAndDeserialize_ProtoObject_ShouldWork()
        {
            //arrange
            var source = new TestData()
            {
                String = "StringPropertyValue",
                Bool = true,
                DateTime = DateTime.Now,
                DateTimeOffset = DateTimeOffset.Now,
                TimeSpan = TimeSpan.FromSeconds(3),
                Guid = Guid.NewGuid(),
                Strings = new()
                {
                    "Value1",
                    "Value2",
                    "Value3"
                },
                DateTimes = new()
                {
                    DateTime.Now,
                    DateTime.UtcNow
                },
                ComplexType = new()
                {
                    String = "HellowWorld",
                    Strings = new(){"1", "2", "3"}
                }
            };

            //act
            var proto = ProtoObject.FromObject(source);
            var bytes = await this.Serializer.SerializeAsync(proto);
            proto = await this.Serializer.DeserializeAsync<ProtoObject>(bytes);
            var deserialized = proto.ToObject<TestData>();

            //assert
            deserialized.String.Should().Be(source.String);
            deserialized.Bool.Should().Be(source.Bool);
            deserialized.DateTime.Should().Be(source.DateTime);
            deserialized.DateTimeOffset.Should().Be(source.DateTimeOffset);
            deserialized.TimeSpan.Should().Be(source.TimeSpan);
            deserialized.Guid.Should().Be(source.Guid);
            deserialized.ComplexType.Should().BeEquivalentTo(source.ComplexType);
            deserialized.Strings.Should().BeEquivalentTo(source.Strings);
            deserialized.DateTimes.Should().BeEquivalentTo(source.DateTimes);
            deserialized.ComplexTypes.Should().BeEquivalentTo(source.ComplexTypes);
        }

        [Fact]
        public async Task SerializeAndDeserialize_NewtonsoftJson_ProtoObject_ShouldWork()
        {
            //arrange
            var source = new TestData()
            {
                String = "StringPropertyValue",
                Bool = true,
                DateTime = DateTime.Now,
                DateTimeOffset = DateTimeOffset.Now,
                TimeSpan = TimeSpan.FromSeconds(3),
                Guid = Guid.NewGuid(),
                Strings = new()
                {
                    "Value1",
                    "Value2",
                    "Value3"
                },
                DateTimes = new()
                {
                    DateTime.Now,
                    DateTime.UtcNow
                },
                ComplexType = new()
                {
                    String = "HellowWorld",
                    Strings = new() { "1", "2", "3" }
                }
            };
            var proto = ProtoObject.FromObject(source);

            //act
            var json = JsonConvert.SerializeObject(proto);

            //assert
            var deserialized = JsonConvert.DeserializeObject<TestData>(json);
        }

        [Fact]
        public void ProtoObject_From_ExpandObject_Should_Work()
        {
            var json = JsonConvert.SerializeObject(new
            {
                id = 1,
                name = "asd",
                photoUrls = Array.Empty<object>(),
                tags = Array.Empty<object>(),
                status = "available"
            });
            var obj = JsonConvert.DeserializeObject<ExpandoObject>(json);
            var proto = ProtoObject.FromObject(obj);
            var res = proto.ToObject().ToExpandoObject();
        }

        class TestData
        {

            public string String { get; internal set; }

            public bool Bool { get; internal set; }

            public DateTime DateTime { get; internal set; }

            public DateTimeOffset DateTimeOffset { get; internal set; }

            public TimeSpan TimeSpan { get; internal set; }

            public Guid Guid { get; internal set; }

            public TestData ComplexType { get; internal set; }

            public List<string> Strings { get; internal set; }

            public List<DateTime> DateTimes { get; internal set; }

            public List<TestData> ComplexTypes { get; internal set; }

        }

    }

}
