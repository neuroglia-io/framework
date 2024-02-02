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

using Json.Schema;
using Json.Schema.Generation;
using Neuroglia.Data.Schemas.Json;

namespace Neuroglia.UnitTests.Cases.Data.Schemas.Json;

public class JsonSchemaGeneratorTests
{

    [Fact]
    public void Generate_Schema_For_SelfContainingType_Should_Work()
    {
        //arrange
        var generator = new JsonSchemaBuilder();

        //act
        var schema = generator.FromType(typeof(Category), JsonSchemaGeneratorConfiguration.Default).Build();

        //assert
        schema.Should().NotBeNull();
    }

    class Category
    {

        public List<Category>? Children { get; set; }

    }

}
