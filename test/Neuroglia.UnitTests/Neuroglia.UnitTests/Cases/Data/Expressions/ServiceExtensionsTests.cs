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
