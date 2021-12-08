using Xunit;

namespace Neuroglia.UnitTests.Cases.Core
{

    public class ObjectExtensionsTests
    {

        [Fact]
        public void Merge()
        {
            //arrange
            var left = new
            {
                foo = new
                {
                    bar = "bar",
                    baz = "foo"
                },
                bar = "bar"
            };
            var right = new
            {
                foo = new
                {
                    baz = "baz"
                },
                bar = "foo",
                baz = new
                {
                    foo = new
                    {
                        baz = "bazfoobaz"
                    },
                    bar = "bazfoobar",
                }
            };

            //act
            var merged = left.Merge(right);
        }

    }

}
