using FluentAssertions;
using Neuroglia.Serialization;
using Neuroglia.UnitTests.Data;
using Newtonsoft.Json;
using System;
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
            var buffer = await this.Serializer.SerializeAsync(DynamicObject.FromObject(toSerialize));
            var deserialized = await this.Serializer.DeserializeAsync<DynamicObject>(buffer);

            //assert
            deserialized.Should().NotBeNull();
            var address = deserialized.ToObject<TestAddress>();
            address.Street.Should().Be(toSerialize.Street);
            address.ZipCode.Should().Be(toSerialize.ZipCode);
            address.City.Should().Be(toSerialize.City);
            address.Country.Should().Be(toSerialize.Country);
        }

        //[Fact]
        //public async Task SerializeAndDeserialize_NewtonsoftJson_ProtoObject_ShouldWork()
        //{
        //    //arrange
        //    var source = new TestData()
        //    {
        //        String = "StringPropertyValue",
        //        Bool = true,
        //        DateTime = DateTime.Now,
        //        DateTimeOffset = DateTimeOffset.Now,
        //        TimeSpan = TimeSpan.FromSeconds(3),
        //        Guid = Guid.NewGuid(),
        //        Strings = new()
        //        {
        //            "Value1",
        //            "Value2",
        //            "Value3"
        //        },
        //        DateTimes = new()
        //        {
        //            DateTime.Now,
        //            DateTime.UtcNow
        //        },
        //        ComplexType = new()
        //        {
        //            String = "HellowWorld",
        //            Strings = new() { "1", "2", "3" }
        //        }
        //    };
        //    var proto = DynamicObject.FromObject(source);

        //    //act
        //    var json = JsonConvert.SerializeObject(proto);

        //    //assert
        //    var deserialized = JsonConvert.DeserializeObject<TestData>(json);
        //}

        //[Fact]
        //public void ProtoObject_From_ExpandObject_Should_Work()
        //{
        //    var json = JsonConvert.SerializeObject(new
        //    {
        //        id = 1,
        //        name = "asd",
        //        photoUrls = Array.Empty<object>(),
        //        tags = Array.Empty<object>(),
        //        status = "available"
        //    });
        //    var obj = JsonConvert.DeserializeObject<ExpandoObject>(json);
        //    var proto = DynamicObject.FromObject(obj);
        //    var res = proto.ToObject().ToExpandoObject();

        //    json = @"{""id"":1,""category"":{""id"":1,""name"":""asd""},""name"":""asd"",""photoUrls"":[],""tags"":[],""status"":""available""}";
        //    var ex = JsonConvert.DeserializeObject<ExpandoObject>(json);
        //    proto = DynamicObject.FromObject(ex);
        //    res = proto.ToObject().ToExpandoObject();

        //    json = @"{""id"":1,""name"":""parrot"",""photoUrls"":[""http://purplefieldstestimage1"",""http://purplefieldstestimage2""],""tags"":[],""status"":""available""}";
        //    ex = JsonConvert.DeserializeObject<ExpandoObject>(json);
        //    proto = DynamicObject.FromObject(ex);
        //    res = proto.ToObject().ToExpandoObject();
        //}

    }

}
