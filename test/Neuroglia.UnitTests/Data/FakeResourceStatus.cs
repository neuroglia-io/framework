using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using YamlDotNet.Serialization;

namespace Neuroglia.UnitTests.Data;

[DataContract]
internal record FakeResourceStatus
{

    [DataMember(Order = 1, Name = "fakeProperty1"), JsonPropertyOrder(1), JsonPropertyName("fakeProperty1"), YamlMember(Order = 1, Alias = "fakeProperty1")]
    public string FakeProperty1 { get; set; } = "Fake Value";

    [DataMember(Order = 2, Name = "fakeProperty2"), JsonPropertyOrder(2), JsonPropertyName("fakeProperty2"), YamlMember(Order = 2, Alias = "fakeProperty2")]
    public long? FakeProperty2 { get; set; }

}