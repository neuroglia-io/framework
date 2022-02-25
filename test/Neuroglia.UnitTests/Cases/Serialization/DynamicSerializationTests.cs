using FluentAssertions;
using Neuroglia.Serialization;
using Newtonsoft.Json;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Neuroglia.UnitTests.Cases.Serialization
{

    public class DynamicSerializationTests
    {

        protected ProtobufSerializer ProtoSerializer { get; } = new();

        [Theory]
        [MemberData(nameof(JsonTheoryData))]
        public void SerializeAndDeserialize_Dynamic_ToFrom_Newtonsoft_ShouldWork(string inputJson)
        {
            //act
            var input = JsonConvert.DeserializeObject<System.Dynamic.ExpandoObject>(inputJson);
            var dynDeserialized = JsonConvert.DeserializeObject<Dynamic>(inputJson);
            var dyn = DynamicObject.FromObject(input);
            var expando = dyn.ToObject().ToExpandoObject();
            var dynOutputJson = JsonConvert.SerializeObject(dyn, Formatting.None);
            var expandoOutputJson = JsonConvert.SerializeObject(expando, Formatting.None);

            //assert
            dynOutputJson.Should().Be(inputJson);
            expandoOutputJson.Should().Be(inputJson);
            dynDeserialized.ToObject().ToExpandoObject().Should().BeEquivalentTo(expando);
        }

        [Fact]
        public async Task SerializeAndDeserialize_Dynamic_ToFrom_Protobuf_ShouldWork()
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
            var dataToAssert = new List<TestData>();

            //act
            var protoValue = DynamicObject.FromObject(source);
            var bytes = await this.ProtoSerializer.SerializeAsync(protoValue);
            protoValue = await this.ProtoSerializer.DeserializeAsync<DynamicObject>(bytes);
            dataToAssert.Add(protoValue.ToObject<TestData>());

            var protoContainer = new TestContainer() { ProtoObject = Dynamic.FromObject(protoValue), ProtoArray = Dynamic.FromObject(source.DateTimes), ProtoValue = Dynamic.FromObject(source.TimeSpan)  };
            bytes = await this.ProtoSerializer.SerializeAsync(protoContainer);
            protoContainer = await this.ProtoSerializer.DeserializeAsync<TestContainer>(bytes);
            dataToAssert.Add(protoContainer.ProtoObject.ToObject<TestData>());

            //assert
            protoContainer.ProtoArray.ToObject<List<DateTime>>().Should().BeEquivalentTo(source.DateTimes);
            protoContainer.ProtoValue.ToObject<TimeSpan>().Should().Be(source.TimeSpan);
            foreach (var data in dataToAssert)
            {
                data.String.Should().Be(source.String);
                data.Bool.Should().Be(source.Bool);
                data.DateTime.Should().Be(source.DateTime);
                data.DateTimeOffset.Should().Be(source.DateTimeOffset);
                data.TimeSpan.Should().Be(source.TimeSpan);
                data.Guid.Should().Be(source.Guid);
                data.ComplexType.Should().BeEquivalentTo(source.ComplexType);
                data.Strings.Should().BeEquivalentTo(source.Strings);
                data.DateTimes.Should().BeEquivalentTo(source.DateTimes);
                data.ComplexTypes.Should().BeEquivalentTo(source.ComplexTypes);
            }

        }

        public static IEnumerable<object[]> JsonTheoryData
        {
            get
            {
                yield return new object[] { @"{""id"":1,""category"":{""id"":1,""name"":""asd""},""name"":""asd"",""photoUrls"":[],""tags"":[],""status"":""available""}" };
                yield return new object[] { @"{""id"":1,""name"":""parrot"",""photoUrls"":[""http://purplefieldstestimage1"",""http://purplefieldstestimage2""],""tags"":[],""status"":""available""}" };
                yield return new object[] { @"{""emptyArray"":[],""array"":[{""property1"":""22/02/2022 09:54"",""property2"":[""value1"",""value2"",""value3""],""property3"":{""subproperty1"":""23/02/2022 10:54"",""subproperty2"":[""value1"",""value2"",""value3""]}}],""string"":""Hello, world"",""emptyObj"":{},""obj"":{""index"":36,""name"":""Foo"",""address"":{""street"":""69 foo, 1968 bar, baz""}}}" };
            }
        }

        [ProtoContract]
        class TestContainer
        {

            [ProtoMember(1)]
            public Dynamic ProtoObject { get; set; }

            [ProtoMember(2)]
            public Dynamic ProtoArray { get; set; }

            [ProtoMember(3)]
            public Dynamic ProtoValue { get; set; }

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
