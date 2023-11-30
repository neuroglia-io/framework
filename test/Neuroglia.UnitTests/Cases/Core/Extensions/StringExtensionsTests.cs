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

namespace Neuroglia.UnitTests.Cases.Core.Extensions;

public class StringExtensionsTests
{

    [Fact]
    public void ToCamelCase_Should_Work()
    {
        //arrange
        var input = "Hello, World!!!  Bye  !";
        var expectation = "helloWorldBye";

        //act
        var output = input.ToCamelCase();

        //assert
        output.Should().Be(expectation);
    }

    [Fact]
    public void ToKebabCase_Should_Work()
    {
        //arrange
        var input = "Hello, World!!!  Bye  !";
        var expectation = "hello-world-bye";

        //act
        var output = input.ToKebabCase();

        //assert
        output.Should().Be(expectation);
    }

    [Fact]
    public void ToKebabCase_WithAcronym_Should_Work()
    {
        //arrange
        var input = "Hello, World!!!  Bye  API!";
        var expectation = "hello-world-bye-api";

        //act
        var output = input.ToKebabCase();

        //assert
        output.Should().Be(expectation);
    }

    [Fact]
    public void ToSnakeCase_Should_Work()
    {
        //arrange
        var input = "Hello, World!!!  Bye  !";
        var expectation = "hello_world_bye";

        //act
        var output = input.ToSnakeCase();

        //assert
        output.Should().Be(expectation);
    }

    [Fact]
    public void SplitCamelCase_Should_Work()
    {
        //arrange
        var input = "helloWorldHowAreYou";
        var expecation = "hello world how are you";

        //act
        var output = input.SplitCamelCase();

        //assert
        output.Should().Be(expecation);
    }

    [Fact]
    public void SplitCamelCase_WithoutDefaultLowercase_Should_Work()
    {
        //arrange
        var input = "HelloWorldHowAreYou";
        var expecation = "Hello World How Are You";

        //act
        var output = input.SplitCamelCase(false);

        //assert
        output.Should().Be(expecation);
    }

    [Fact]
    public void SplitCamelCase_WithoutDefaultFirstLetterToLowercase_Should_Work()
    {
        //arrange
        var input = "helloWorldHowAreYou";
        var expecation = "hello world how are you";

        //act
        var output = input.SplitCamelCase(keepFirstLetterInUpercase: false);

        //assert
        output.Should().Be(expecation);
    }

    [Fact]
    public void Remove_Diacritics_Should_Work()
    {
        //arrange
        var input = "áàäâ éèëê íìïî óòöô úùüû ç";
        var expectation = "aaaa eeee iiii oooo uuuu c";

        //act
        var output = input.RemoveDiacritics();

        //assert
        output.Should().Be(expectation);
    }

}
