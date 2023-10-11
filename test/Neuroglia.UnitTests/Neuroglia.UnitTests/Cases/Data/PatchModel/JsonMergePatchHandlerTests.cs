using Neuroglia.Data.PatchModel.Services;

namespace Neuroglia.UnitTests.Cases.Data.PatchModel;

public class JsonMergePatchHandlerTests
{

    [Fact]
    public async Task Apply_Patch_Should_Work()
    {
        //arrange
        var street = "36 Craven Street";
        var city = "London";
        var zipCode = "WC2N 5NF";
        var country = "UK";
        var patch = new { street, city, zipCode, country };
        var target = AddressFactory.Create();
        var patchHandler = new JsonMergePatchHandler();

        //act
        var patched = await patchHandler.ApplyPatchAsync(patch, target);

        //assert
        patched.Should().NotBeNull();
        patched!.Street.Should().Be(street);
        patched.City.Should().Be(city);
        patched.ZipCode.Should().Be(zipCode);
        patched.Country.Should().Be(country);
    }

}