using FluentAssertions;
using Xunit;

namespace Neuroglia.UnitTests.Cases.Core
{

    public class OperationResultTests
    {

        [Fact]
        public void GetData()
        {
            //arrange
            var originalData = "Hello, World";
            var result = (IOperationResult)new OperationResult<string>(OperationResultCode.Ok, originalData);

            //act
            var returnedData = result.GetData();

            //assert
            returnedData.Should().NotBeNull();
            returnedData.Should().Be(originalData);
        }

    }

}
