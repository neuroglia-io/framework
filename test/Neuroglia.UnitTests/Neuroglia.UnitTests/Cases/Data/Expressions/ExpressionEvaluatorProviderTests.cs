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
using Neuroglia.Data.Expressions.JQ;
using Neuroglia.Data.Expressions.Services;
using Neuroglia.Serialization;

namespace Neuroglia.UnitTests.Cases.Data.Expressions;

public class ExpressionEvaluatorProviderTests
{

    public ExpressionEvaluatorProviderTests()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSerialization();
        services.AddJQExpressionEvaluator();
        var serviceProvider = services.BuildServiceProvider();
        this.ExpressionEvaluatorProvider = serviceProvider.GetRequiredService<IExpressionEvaluatorProvider>();
    }

    protected IExpressionEvaluatorProvider ExpressionEvaluatorProvider { get; }

    [Fact]
    public void GetEvaluator_Should_Work()
    {
        //arrange
        var language = "jq";

        //act
        var evaluator = this.ExpressionEvaluatorProvider.GetEvaluator(language);

        //assert
        evaluator.Should().NotBeNull();

        this.ExpressionEvaluatorProvider.GetEvaluator("unsupported").Should().BeNull();

        var action = () => this.ExpressionEvaluatorProvider.GetEvaluator(string.Empty);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void GetEvaluators_Should_Work()
    {
        //arrange
        var language = "jq";

        //act
        var evaluators = this.ExpressionEvaluatorProvider.GetEvaluators(language);

        //assert
        evaluators.Should().NotBeNull();
        evaluators.Should().ContainSingle();

        this.ExpressionEvaluatorProvider.GetEvaluators("unsupported").Should().BeEmpty();

        var action = () => this.ExpressionEvaluatorProvider.GetEvaluators(string.Empty);
        action.Should().Throw<ArgumentNullException>();
    }

}
