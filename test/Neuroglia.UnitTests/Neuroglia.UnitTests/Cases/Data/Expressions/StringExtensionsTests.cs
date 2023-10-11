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