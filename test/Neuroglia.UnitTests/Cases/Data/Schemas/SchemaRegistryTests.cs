using FluentAssertions;
using Neuroglia.Data;
using Neuroglia.Data.Services;
using System.Threading.Tasks;
using Xunit;

namespace Neuroglia.UnitTests.Cases.Data.Schemas
{
    public class SchemaRegistryTests
    {

        public SchemaRegistryTests()
        {
            schemaRegistry = Neuroglia.Data.Services.SchemaRegistry.Create();
        }

        ISchemaRegistry schemaRegistry;

        [Fact]
        public async Task GetOpenApiSchema_ShouldWork()
        {
            //act
            var schema = await schemaRegistry.GetOpenApiSchemaAsync(new("https://petstore.swagger.io/v2/swagger.json"));

            //assert
            schema.Should().NotBeNull();
        }

        [Fact]
        public async Task GetProtoSchema_ShouldWork()
        {
            //act
            var schema = await schemaRegistry.GetProtoSchemaAsync(new("https://gist.githubusercontent.com/agreatfool/5e3a41052c6dd2f6d04b30901fc0269b/raw/b4e0f82f73843a79ecdbde31c1294952df3855c6/book.proto"));

            //assert
            schema.Should().NotBeNull();
        }

        [Fact]
        public async Task GetODataSchema_ShouldWork()
        {
            //act
            var schema = await schemaRegistry.GetODataSchemaAsync(new("https://services.odata.org/V3/(S(f3b5dp5wreeaiuzpgf0oj2ay))/OData/OData.svc/"));

            //assert
            schema.Should().NotBeNull();
        }

    }

}
