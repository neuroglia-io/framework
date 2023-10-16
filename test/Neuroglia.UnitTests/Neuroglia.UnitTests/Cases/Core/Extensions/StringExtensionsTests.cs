namespace Neuroglia.UnitTests.Cases.Core.Extensions;

public class StringExtensionsTests
{

    [Fact]
    public void ToCamelCase_Should_Work()
    {
        //arrange
        var input = "Hello, World!!!  Bye  !";
        var expectation = "helloWorldBye";

        //act
        var output = input.ToCamelCase();

        //assert
        output.Should().Be(expectation);
    }

    [Fact]
    public void ToKebabCase_Should_Work()
    {
        //arrange
        var input = "Hello, World!!!  Bye  !";
        var expectation = "hello-world-bye";

        //act
        var output = input.ToKebabCase();

        //assert
        output.Should().Be(expectation);
    }

    [Fact]
    public void ToSnakeCase_Should_Work()
    {
        //arrange
        var input = "Hello, World!!!  Bye  !";
        var expectation = "hello_world_bye";

        //act
        var output = input.ToSnakeCase();

        //assert
        output.Should().Be(expectation);
    }

    [Fact]
    public void SplitCamelCase_Should_Work()
    {
        //arrange
        var input = "helloWorldHowAreYou";
        var expecation = "hello world how are you";

        //act
        var output = input.SplitCamelCase();

        //assert
        output.Should().Be(expecation);
    }

    [Fact]
    public void SplitCamelCase_WithoutDefaultLowercase_Should_Work()
    {
        //arrange
        var input = "HelloWorldHowAreYou";
        var expecation = "Hello World How Are You";

        //act
        var output = input.SplitCamelCase(false);

        //assert
        output.Should().Be(expecation);
    }

    [Fact]
    public void SplitCamelCase_WithoutDefaultFirstLetterToLowercase_Should_Work()
    {
        //arrange
        var input = "helloWorldHowAreYou";
        var expecation = "hello world how are you";

        //act
        var output = input.SplitCamelCase(keepFirstLetterInUpercase: false);

        //assert
        output.Should().Be(expecation);
    }

    [Fact]
    public void Remove_Diacritics_Should_Work()
    {
        //arrange
        var input = "áàäâ éèëê íìïî óòöô úùüû ç";
        var expectation = "aaaa eeee iiii oooo uuuu c";

        //act
        var output = input.RemoveDiacritics();

        //assert
        output.Should().Be(expectation);
    }

}
