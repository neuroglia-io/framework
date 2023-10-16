// Copyright © 2021-Present Neuroglia SRL. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License"),
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

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