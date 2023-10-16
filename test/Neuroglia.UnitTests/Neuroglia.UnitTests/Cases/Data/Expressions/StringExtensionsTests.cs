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

using Microsoft.Extensions.DependencyInjection;
using Neuroglia.Data.Expressions;
using Neuroglia.Data.Expressions.JQ;
using Neuroglia.Data.Expressions.Services;
using Neuroglia.Serialization;

namespace Neuroglia.UnitTests.Cases.Data.Expressions;

public class StringExtensionsTests
{

    [Fact]
    public void IsRuntimeExpression_Should_Work()
    {
        //arrange
        var validExpression = "${ .input }";
        var invalidExpression = ".input";

        //assert
        validExpression.IsRuntimeExpression().Should().BeTrue();
        invalidExpression.IsRuntimeExpression().Should().BeFalse();
    }

}