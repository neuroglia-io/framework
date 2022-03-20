using FluentAssertions;
using Neuroglia.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;
using ProtoBuf;
using ProtoBuf.Meta;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
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
                },
                Dictionary = new() { { "fake-key", "fake-value" } },
                Uri = new ("http://test.com"),
                Dynamic = Dynamic.FromObject(new { foo = "bar", baz = "foobar" })
            };
            var dataToAssert = new List<TestData>();

            //act
            var protoValue = DynamicObject.FromObject(source);
            var bytes = await this.ProtoSerializer.SerializeAsync(protoValue);
            protoValue = await this.ProtoSerializer.DeserializeAsync<DynamicObject>(bytes);
            dataToAssert.Add(protoValue.ToObject<TestData>());

            var protoContainer = new TestContainer() { ProtoObject = Dynamic.FromObject(protoValue), ProtoArray = Dynamic.FromObject(source.DateTimes), ProtoValue = Dynamic.FromObject(source.TimeSpan), Boolean = false  };
            bytes = await this.ProtoSerializer.SerializeAsync(protoContainer);
            protoContainer = await this.ProtoSerializer.DeserializeAsync<TestContainer>(bytes);
            dataToAssert.Add(protoContainer.ProtoObject.ToObject<TestData>());

            //assert
            protoContainer.ProtoArray.ToObject<List<DateTime>>().Should().BeEquivalentTo(source.DateTimes);
            protoContainer.ProtoValue.ToObject<TimeSpan>().Should().Be(source.TimeSpan);
            protoContainer.Boolean.Should().BeFalse();
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
                data.Dictionary.Should().BeEquivalentTo(source.Dictionary);
                data.Uri.Should().Be(source.Uri);
                data.Dynamic.ToObject().Should().BeEquivalentTo(source.Dynamic.ToObject());
            }

        }

        [Fact]
        public async Task SerializeAndDeserialize_Array_ToFrom_Newtonsoft_ShouldWork()
        {
            var json = @"[{""id"":0,""name"":""Bread"",""description"":""Whole grain bread"",""releaseDate"":""1992-01-01T00:00:00Z"",""discontinuedDate"":null,""rating"":{},""price"":2.5},{""id"":1,""name"":""Milk"",""description"":""Low fat milk"",""releaseDate"":""1995-10-01T00:00:00Z"",""discontinuedDate"":null,""rating"":{},""price"":3.5},{""id"":2,""name"":""Vint soda"",""description"":""Americana Variety - Mix of 6 flavors"",""releaseDate"":""2000-10-01T00:00:00Z"",""discontinuedDate"":null,""rating"":{},""price"":20.9},{""id"":3,""name"":""Havina Cola"",""description"":""The Original Key Lime Cola"",""releaseDate"":""2005-10-01T00:00:00Z"",""discontinuedDate"":""2006-10-01T00:00:00Z"",""rating"":{},""price"":19.9},{""id"":4,""name"":""Fruit Punch"",""description"":""Mango flavor, 8.3 Ounce Cans (Pack of 24)"",""releaseDate"":""2003-01-05T00:00:00Z"",""discontinuedDate"":null,""rating"":{},""price"":22.99},{""id"":5,""name"":""Cranberry Juice"",""description"":""16-Ounce Plastic Bottles (Pack of 12)"",""releaseDate"":""2006-08-04T00:00:00Z"",""discontinuedDate"":null,""rating"":{},""price"":22.8},{""id"":6,""name"":""Pink Lemonade"",""description"":""36 Ounce Cans (Pack of 3)"",""releaseDate"":""2006-11-05T00:00:00Z"",""discontinuedDate"":null,""rating"":{},""price"":18.8},{""id"":7,""name"":""DVD Player"",""description"":""1080P Upconversion DVD Player"",""releaseDate"":""2006-11-15T00:00:00Z"",""discontinuedDate"":null,""rating"":{},""price"":35.88},{""id"":8,""name"":""LCD HDTV"",""description"":""42 inch 1080p LCD with Built-in Blu-ray Disc Player"",""releaseDate"":""2008-05-08T00:00:00Z"",""discontinuedDate"":null,""rating"":{},""price"":1088.8},{""id"":9,""name"":""Lemonade"",""description"":""Classic, refreshing lemonade (Single bottle)"",""releaseDate"":""1970-01-01T00:00:00Z"",""discontinuedDate"":null,""rating"":{},""price"":1.01},{""id"":10,""name"":""Coffee"",""description"":""Bulk size can of instant coffee"",""releaseDate"":""1982-12-31T00:00:00Z"",""discontinuedDate"":null,""rating"":{},""price"":6.99}]";
            var dyn = JsonConvert.DeserializeObject<Dynamic>(json);
            var obj = dyn.ToObject();
            var serialized = JsonConvert.SerializeObject(dyn);

            serialized.Should().Be(json);

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

            public TestContainer()
            {
                this.Boolean = true;
            }

            [ProtoMember(1)]
            public Dynamic ProtoObject { get; set; }

            [ProtoMember(2)]
            public Dynamic ProtoArray { get; set; }

            [ProtoMember(3)]
            public Dynamic ProtoValue { get; set; }

            [ProtoMember(4)]
            [DefaultValue(true)]
            public bool Boolean { get; set; }

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

            public Dictionary<string, string> Dictionary { get; set; }

            public Uri Uri { get; set; }

            public Dynamic Dynamic { get; set; }

        }

    }

}
