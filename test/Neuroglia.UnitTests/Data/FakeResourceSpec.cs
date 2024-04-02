// Copyright © 2021-Present Neuroglia SRL. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License"),
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using YamlDotNet.Serialization;

namespace Neuroglia.UnitTests.Data;

[DataContract]
internal record FakeResourceSpec
{

    [DataMember(Order = 1, Name = "fakeProperty1"), JsonPropertyOrder(1), JsonPropertyName("fakeProperty1"), YamlMember(Order = 1, Alias = "fakeProperty1")]
    public string FakeProperty1 { get; set; } = "Fake Value";

    [DataMember(Order = 2, Name = "fakeProperty2"), JsonPropertyOrder(2), JsonPropertyName("fakeProperty2"), YamlMember(Order = 2, Alias = "fakeProperty2")]
    public long FakeProperty2 { get; set; } = 69;

}
