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

using Neuroglia.Serialization.Yaml;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace Neuroglia.UnitTests.Cases.Serialization;

public class YamlSerializerTests
{

    [Fact]
    public void Serialize_Deserialize_Equatable_List_Should_Work()
    {
        //arrange
        var toSerialize = new EquatableList<Fruit>()
        {
            new(){ Name = "apple" },
            new(){ Name = "banana" },
            new(){ Name = "orange" },
            new(){ Name = "cherry" }
        };

        //act
        var yaml = YamlSerializer.Default.Serialize(toSerialize);
        var deserialized = YamlSerializer.Default.Deserialize<EquatableList<Fruit>>(yaml);

        //assert
        deserialized.Should().Equal(toSerialize);
    }

    [Fact]
    public void Serialize_Deserialize_Equatable_Dictionary_Should_Work()
    {
        //arrange
        var toSerialize = new EquatableDictionary<string, Fruit>()
        {
            new("green", new(){ Name = "apple" }),
            new("yellow",new(){ Name = "banana" }),
            new("orange",new(){ Name = "orange" }),
            new("red",new(){ Name = "cherry" })
        };

        //act
        var yaml = YamlSerializer.Default.Serialize(toSerialize);
        var deserialized = YamlSerializer.Default.Deserialize<EquatableDictionary<string, Fruit>>(yaml);

        //assert
        deserialized.Should().Equal(toSerialize);
    }

    [Fact]
    public void Serialize_Deserialize_Equatable_List_With_ScalarType_should_Work()
    {
        //arrange
        var toSerialize = new EquatableList<Software>()
        {
            new() { Version = "1.0.0" },
            new() { Version = "1.2.0" },
            new() { Version = "1.2.3" }
        };

        //act
        var yaml = YamlSerializer.Default.Serialize(toSerialize);
        var deserialized = YamlSerializer.Default.Deserialize<EquatableList<Software>>(yaml);

        //assert
        deserialized.Should().Equal(toSerialize);
    }

    [Fact]
    public void Serialize_Deserialize_Equatable_Dictionary_With_ScalarType_should_Work()
    {
        //arrange
        var toSerialize = new EquatableDictionary<string, Software>()
        {
            new("fake-1", new() { Version = "1.0.0" }),
            new("fake-2", new() { Version = "1.2.0" }),
            new("fake-3", new() { Version = "1.2.3" })
        };

        //act
        var yaml = YamlSerializer.Default.Serialize(toSerialize);
        var deserialized = YamlSerializer.Default.Deserialize<EquatableDictionary<string, Software>>(yaml);

        //assert
        deserialized.Should().Equal(toSerialize);
    }

    record Fruit 
    {

        public required string Name {get; set;}

    }

    record Software
    {

        [YamlMember(ScalarStyle = ScalarStyle.SingleQuoted)]
        public required string Version { get; set; }

    }

}
