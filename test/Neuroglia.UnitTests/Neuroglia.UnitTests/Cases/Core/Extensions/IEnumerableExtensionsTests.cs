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
