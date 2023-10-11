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
