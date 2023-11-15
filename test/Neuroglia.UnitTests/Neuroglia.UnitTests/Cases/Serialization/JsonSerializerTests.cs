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

using Neuroglia.Serialization.Json;

namespace Neuroglia.UnitTests.Cases.Serialization;

public class JsonSerializerTests
{

    [Fact]
    public void Serialize_Should_Work()
    {
        //arrange
        var dog = new Dog("Belgian Shepperd");

        //act
        var serialized = JsonSerializer.Default.SerializeToText(dog);
        var deserialized = JsonSerializer.Default.Deserialize<IDictionary<string, object>>(serialized)!;

        //assert
        deserialized.ElementAt(0).Key.Should().BeEquivalentTo(nameof(Mammal.Family));

    }

    abstract class Mammal
    {

        protected Mammal(string family)
        {
            this.Family = family;
        }

        public string Family { get; protected set; }

    }

    class Dog
        : Mammal
    {

        public Dog(string breed)
            : base("Canidae")
        {
            this.Breed = breed;
        }

        public string Breed { get; protected set; }

    }

}
