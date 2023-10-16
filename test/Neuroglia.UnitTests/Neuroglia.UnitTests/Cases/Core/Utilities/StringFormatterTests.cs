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

namespace Neuroglia.UnitTests.Cases.Core.Utilities;

public class StringFormatterTests
{

    [Fact]
    public void Format_UsingDictionary_Should_Work()
    {
        //arrange
        var format = "Hello, {firstName} {lastName}!";
        var firstName = "John";
        var lastName = "Doe";
        var parameters = new Dictionary<string, object>()
        {
            { nameof(firstName), firstName },
            { "LAstName", lastName }
        };

        //act
        var result = StringFormatter.NamedFormat(format, parameters);

        //assert
        result.Should().Be($"Hello, {firstName} {lastName}!");
    }

    [Fact]
    public void Format_UsingObject_Should_Work()
    {
        //arrange
        var format = "Hello, {firstName} {lastName}!";
        var firstName = "John";
        var lastName = "Doe";
        var parameterObject = new { firstName, lastName };

        //act
        var result = StringFormatter.NamedFormat(format, parameterObject);

        //assert
        result.Should().Be($"Hello, {firstName} {lastName}!");
    }

    [Fact]
    public void Format_WithEmptyFormat_Should_Work()
    {
        //arrange
        var format = string.Empty;

        //act
        var firstName = "John";
        var lastName = "Doe";
        var parameters = new Dictionary<string, object>()
        {
            { nameof(firstName), firstName },
            { "LAstName", lastName }
        };

        //act
        var result = StringFormatter.NamedFormat(format, parameters);

        //assert
        result.Should().BeNullOrWhiteSpace();
    }

    [Fact]
    public void Format_WithNullDictionary_Should_Work()
    {
        //arrange
        var format = "Hello, {firstName} {lastName}!";

        //act
        var result = StringFormatter.NamedFormat(format, null!);

        //assert
        result.Should().Be(format);
    }

    [Fact]
    public void Format_WithEmptyDictionary_Should_Work()
    {
        //arrange
        var format = "Hello, {firstName} {lastName}!";
        var parameters = new Dictionary<string, object>();

        //act
        var result = StringFormatter.NamedFormat(format, parameters);

        //assert
        result.Should().Be(format);
    }

    [Fact]
    public void Format_WithNullObject_Should_Work()
    {
        //arrange
        var format = "Hello, {firstName} {lastName}!";

        //act
        var result = StringFormatter.NamedFormat(format, (object)null!);

        //assert
        result.Should().Be(format);
    }

}
