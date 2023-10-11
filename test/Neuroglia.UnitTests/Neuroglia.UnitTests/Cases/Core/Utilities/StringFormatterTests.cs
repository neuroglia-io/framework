namespace Neuroglia.UnitTests.Cases.Core.Utilities;

public class StringFormatterTests
{

    [Fact]
    public void Format_UsingDictionary_Should_Work()
    {
        //arrange
        var format = "Hello, {firstName} {lastName}!";
        var firstName = "John";
        var lastName = "Doe";
        var parameters = new Dictionary<string, object>()
        {
            { nameof(firstName), firstName },
            { "LAstName", lastName }
        };

        //act
        var result = StringFormatter.NamedFormat(format, parameters);

        //assert
        result.Should().Be($"Hello, {firstName} {lastName}!");
    }

    [Fact]
    public void Format_UsingObject_Should_Work()
    {
        //arrange
        var format = "Hello, {firstName} {lastName}!";
        var firstName = "John";
        var lastName = "Doe";
        var parameterObject = new { firstName, lastName };

        //act
        var result = StringFormatter.NamedFormat(format, parameterObject);

        //assert
        result.Should().Be($"Hello, {firstName} {lastName}!");
    }

    [Fact]
    public void Format_WithEmptyFormat_Should_Work()
    {
        //arrange
        var format = string.Empty;

        //act
        var firstName = "John";
        var lastName = "Doe";
        var parameters = new Dictionary<string, object>()
        {
            { nameof(firstName), firstName },
            { "LAstName", lastName }
        };

        //act
        var result = StringFormatter.NamedFormat(format, parameters);

        //assert
        result.Should().BeNullOrWhiteSpace();
    }

    [Fact]
    public void Format_WithNullDictionary_Should_Work()
    {
        //arrange
        var format = "Hello, {firstName} {lastName}!";

        //act
        var result = StringFormatter.NamedFormat(format, null!);

        //assert
        result.Should().Be(format);
    }

    [Fact]
    public void Format_WithEmptyDictionary_Should_Work()
    {
        //arrange
        var format = "Hello, {firstName} {lastName}!";
        var parameters = new Dictionary<string, object>();

        //act
        var result = StringFormatter.NamedFormat(format, parameters);

        //assert
        result.Should().Be(format);
    }

    [Fact]
    public void Format_WithNullObject_Should_Work()
    {
        //arrange
        var format = "Hello, {firstName} {lastName}!";

        //act
        var result = StringFormatter.NamedFormat(format, (object)null!);

        //assert
        result.Should().Be(format);
    }

}
