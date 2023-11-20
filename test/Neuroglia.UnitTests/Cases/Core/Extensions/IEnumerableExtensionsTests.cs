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

using System.Collections;

namespace Neuroglia.UnitTests.Cases.Core.Extensions;

public class IEnumerableExtensionsTests
{

    [Fact]
    public void WithValueSemantics_Should_Work()
    {
        //arrange
        var input = new List<string> { "red", "green", "blue" };
        var expectation = new EquatableList<string>(input);

        //act
        var output = input.WithValueSemantics();

        //assert
        output.As<object>().Should().Be(expectation);
        output.As<object>().Should().NotBe(input);
    }

    [Fact]
    public void OfType_Should_Work()
    {
        //arrange
        var input = (IEnumerable)new List<object>() { 1, 2, 3 };

        //act
        var output = input.OfType(typeof(int));

        //assert
        output.GetType().Should().BeAssignableTo(typeof(IEnumerable<int>));
        input.GetType().Should().NotBeAssignableTo(typeof(IEnumerable<int>));
    }

}
