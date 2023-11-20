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
using Neuroglia.Data.Expressions.Services;

namespace Neuroglia.UnitTests.Cases.Data.Expressions;

public class ServiceExtensionsTests
{

    [Fact]
    public void AddExpressionEvaluator_Should_Work()
    {
        //arrange
        var services = new ServiceCollection();

        //act
        services.AddExpressionEvaluator<FakeExpressionEvaluator>();
        var serviceProvider = services.BuildServiceProvider();

        //assert
        serviceProvider.GetServices<IExpressionEvaluator>().Should().ContainSingle();
        serviceProvider.GetServices<FakeExpressionEvaluator>().Should().ContainSingle();
    }

    class FakeExpressionEvaluator
        : IExpressionEvaluator
    {
        Task<object?> IExpressionEvaluator.EvaluateAsync(string expression, object input, IDictionary<string, object>? arguments, Type? expectedType, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        bool IExpressionEvaluator.Supports(string language)
        {
            throw new NotImplementedException();
        }

    }

}
