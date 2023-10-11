using Microsoft.Extensions.DependencyInjection;
using Neuroglia.Data.Expressions;
using Neuroglia.Data.Expressions.JQ;
using Neuroglia.Data.Expressions.Services;
using Neuroglia.Serialization;

namespace Neuroglia.UnitTests.Cases.Data.Expressions.Evaluators;

public class JQExpressionEvaluatorTests
{

    public JQExpressionEvaluatorTests()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSerialization();
        services.AddJQExpressionEvaluator();
        var serviceProvider = services.BuildServiceProvider();
        ExpressionEvaluator = serviceProvider.GetRequiredService<IExpressionEvaluator>();
    }

    protected IExpressionEvaluator ExpressionEvaluator { get; }

    [Fact]
    public void Supports_Should_Work()
    {
        //arrange
        var supported = "jq";
        var unsupported = "js";

        //assert
        ExpressionEvaluator.Supports(supported).Should().BeTrue();
        ExpressionEvaluator.Supports(unsupported).Should().BeFalse();
    }

    [Fact]
    public async Task Interpolate_String_Should_Work()
    {
        //arrange
        var expression = @$"""\($GREETINGS) \(.firstName) \(.lastName)""";
        var firstName = "John";
        var lastName = "Doe";
        var greetings = "Hello,";
        var input = new { firstName, lastName };
        var args = new Dictionary<string, object>()
        {
            { "GREETINGS", greetings }
        };

        //act
        var result = await ExpressionEvaluator.EvaluateAsync<string>(expression, input, args);

        //assert
        result.Should().Be($"{greetings} {firstName} {lastName}");
    }

    [Fact]
    public async Task Evaluate_StringExpression_WithArrayInput_Should_Work()
    {
        //arrange
        var input = new User[] { new() { FirstName = "John", LastName = "Doe" }, new() { FirstName = "Jane", LastName = "Doe" } };
        var expression = "${ . }";

        //act
        var results = (await ExpressionEvaluator.EvaluateAsync<List<User>>(expression, input))!;

        //assert
        results.Should().HaveCount(input.Length);
    }

    [Fact]
    public async Task Evaluate_ObjectExpression_WithObjectInput_Should_Work()
    {
        //arrange
        var firstName = "John";
        var lastName = "Doe";
        var input = new { firstName, lastName };
        var expression = new { firstName = "${ .firstName }", lastName = "${ .lastName }" };

        //act
        var result = (await ExpressionEvaluator.EvaluateAsync(expression, input))!;
        var mapping = result as IDictionary<string, object>;

        //assert
        mapping.Should().NotBeNullOrEmpty();
        mapping.Should().Contain(nameof(firstName), firstName);
        mapping.Should().Contain(nameof(lastName), lastName);
    }

    [Fact]
    public async Task Evaluate_ObjectExpression_WithObjectInput_Generic_Should_Work()
    {
        //arrange
        var firstName = "John";
        var lastName = "Doe";
        var input = new { firstName, lastName };
        var expression = new { firstName = "${ .firstName }", lastName = "${ .lastName }" };

        //act
        var result = (await ExpressionEvaluator.EvaluateAsync<User>(expression, input))!;

        //assert
        result.FirstName.Should().Be(firstName);
        result.LastName.Should().Be(lastName);
    }

    [Fact]
    public async Task Evaluate_Condition_Should_Work()
    {
        //arrange
        var expression = ".age >= $AGE_LIMIT";
        var age = 19;
        var input = new { age };
        var args = new Dictionary<string, object>()
        {
            { "AGE_LIMIT", 18 }
        };

        //act
        var result = await ExpressionEvaluator.EvaluateConditionAsync(expression, input, args);

        //assert
        result.Should().BeTrue();
    }

}