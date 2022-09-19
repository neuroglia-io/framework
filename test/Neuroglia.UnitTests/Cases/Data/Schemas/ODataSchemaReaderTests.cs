using FluentAssertions;
using Microsoft.Data.OData;
using Neuroglia.Data.Services;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Neuroglia.UnitTests.Cases.Data.Schemas
{

    public class ODataSchemaReaderTests
    {

        public ODataSchemaReaderTests()
        {
            schemaReader = ODataSchemaReader.Create();
        }

        ISchemaReader schemaReader;

        [Fact]
        public async Task GetDocument_ShouldWork()
        {
            //arrange
            var documentUri = new Uri("https://services.odata.org/V3/(S(f3b5dp5wreeaiuzpgf0oj2ay))/OData/OData.svc/");

            //act
            var schema = await schemaReader.ReadFromAsync(documentUri);

            //assert
            schema.Should().NotBeNull();
            schema.Document.Should().BeOfType<ODataWorkspace>();
            schema.Document.As<ODataWorkspace>().Collections.Should().HaveCount(7);
        }

    }

}
