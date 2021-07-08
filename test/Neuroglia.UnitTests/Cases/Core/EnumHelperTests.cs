using FluentAssertions;
using Neuroglia.UnitTests.Data;
using Xunit;

namespace Neuroglia.UnitTests.Cases.Core.Helpers
{

    public class EnumHelperTests
    {

        [Fact]
        public void StringifyAndParse_Enum_ShouldWork()
        {
            //arrange
            var value = TestEnum.Value3;

            //act
            var raw = EnumHelper.Stringify(value);
            var parsed = EnumHelper.Parse<TestEnum>(raw);

            //assert
            raw.Should().NotBeEmpty();
            raw.Should().Be("VALUE3");
            parsed.Should().Be(value);
        }

        [Fact]
        public void StringifyAndParse_Flags_ShouldWork()
        {
            //arrange
            var value = TestFlags.Value1 | TestFlags.Value3;

            //act
            var raw = EnumHelper.Stringify(value);
            var parsed = EnumHelper.Parse<TestFlags>(raw);

            //assert
            raw.Should().NotBeEmpty();
            raw.Should().Be("VALUE1 | VALUE3");
            parsed.Should().Be(value);
        }

    }

}
