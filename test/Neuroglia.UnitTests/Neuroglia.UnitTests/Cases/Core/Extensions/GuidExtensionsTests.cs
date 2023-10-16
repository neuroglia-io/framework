namespace Neuroglia.UnitTests.Cases.Core.Extensions;

public class GuidExtensionsTests
{

    [Fact]
    public void ToShortString_Should_Work()
    {
        //arrange
        var input = Guid.NewGuid();
        var expectation = input.ToString("N")[..15];

        //act
        var output = input.ToShortString();

        //assert
        output.Should().Be(expectation);
    }

}