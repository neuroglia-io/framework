using FluentAssertions;
using System.Linq;
using System.Reflection;
using Xunit;

namespace Neuroglia.UnitTests.Cases.Core.Helpers
{
    public class XmlDocumentationHelperTests
    {

        [Fact]
        public void GetDocumentation_Type_ShouldWork()
        {
            //arrange
            var type = typeof(Error);

            //act
            var summary = type.GetDocumentationSummary();

            //assert
            summary.Should().NotBeEmpty();
        }

        [Fact]
        public void GetDocumentation_Constructor_ShouldWork()
        {
            //arrange
            var constructor = typeof(Error).GetConstructors().First();

            //act
            var summary = constructor.GetDocumentationSummary();

            //assert
            summary.Should().NotBeEmpty();
        }

        [Fact]
        public void GetDocumentation_Field_ShouldWork()
        {
            //arrange
            var field = typeof(OperationResultCode).GetFields(BindingFlags.Default | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy).First();

            //act
            var summary = field.GetDocumentationSummary();

            //assert
            summary.Should().NotBeEmpty();
        }

        [Fact]
        public void GetDocumentation_Property_ShouldWork()
        {
            //arrange
            var property = typeof(Error).GetProperty(nameof(Error.Message));

            //act
            var summary = property.GetDocumentationSummary();

            //assert
            summary.Should().NotBeEmpty();
        }

        [Fact]
        public void GetDocumentation_Method_ShouldWork()
        {
            //arrange
            var method = typeof(EnumHelper).GetMethods().First();

            //act
            var summary = method.GetDocumentationSummary();

            //assert
            summary.Should().NotBeEmpty();
        }

        [Fact]
        public void GetDocumentation_Parameter_ShouldWork()
        {
            //arrange
            var param = typeof(EnumHelper).GetMethods().First(m => m.GetParameters().Length > 0).GetParameters().First();

            //act
            var summary = param.GetDocumentationSummary();

            //assert
            summary.Should().NotBeEmpty();
        }

    }

}
