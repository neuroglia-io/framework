using Json.Patch;
using Json.Pointer;
using Neuroglia.Data.PatchModel.Services;

namespace Neuroglia.UnitTests.Cases.PatchModel;

public class JsonPatchHandlerTests
{

    [Fact]
    public async Task Apply_Patch_Should_Work()
    {
        //arrange
        var street = "36 Craven Street";
        var city = "London";
        var zipCode = "WC2N 5NF";
        var country = "UK";
        var patchOperations = new PatchOperation[]
        {
            PatchOperation.Replace(JsonPointer.Create<Address>(a => a.Street).ToCamelCase(), street),
            PatchOperation.Replace(JsonPointer.Create<Address>(a => a.City).ToCamelCase(), city),
            PatchOperation.Replace(JsonPointer.Create<Address>(a => a.ZipCode).ToCamelCase(), zipCode),
            PatchOperation.Replace(JsonPointer.Create<Address>(a => a.Country).ToCamelCase(), country),
        };
        var patch = new JsonPatch(patchOperations);
        var target = AddressFactory.Create();
        var patchHandler = new JsonPatchHandler();

        //act
        var patched = await patchHandler.ApplyPatchAsync(patch, target);

        //assert
        patched.Should().NotBeNull();
        patched.Street.Should().Be(street);
        patched.City.Should().Be(city);
        patched.ZipCode.Should().Be(zipCode);
        patched.Country.Should().Be(country);
    }

}
