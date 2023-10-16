using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Neuroglia.UnitTests.Cases.Core.Extensions;

public class MemberInfoExtensionsTests
{

    [Fact]
    public void TryGetCustomAttribute_Should_Work()
    {
        //arrange
        var memberInfo = typeof(Address).GetMember(nameof(Address.Street)).First();

        //act
        var isRequired = memberInfo.TryGetCustomAttribute<RequiredAttribute>(out var requiredAttribute);
        var hasJsonPropertyAttribute = memberInfo.TryGetCustomAttribute<JsonPropertyNameAttribute>(out _);

        //assert
        isRequired.Should().BeTrue();
        requiredAttribute.Should().NotBeNull();
        hasJsonPropertyAttribute.Should().BeFalse();
    }

}