using FluentAssertions;
using Microsoft.OpenApi.Models;
using Neuroglia.Data.Services;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Neuroglia.UnitTests.Cases.Data.Schemas
{
    public class OpenApiSchemaReaderTests
    {

        public OpenApiSchemaReaderTests()
        {
            schemaReader = OpenApiSchemaReader.Create();
        }

        ISchemaReader schemaReader;

        [Fact]
        public async Task GetDocument_ShouldWork()
        {
            //arrange
            var documentUri = new Uri("https://petstore.swagger.io/v2/swagger.json");

            //act
            var schema = await schemaReader.ReadFromAsync(documentUri);

            //assert
            schema.Should().NotBeNull();
            schema.Document.Should().BeOfType<OpenApiDocument>();
        }

    }

}
