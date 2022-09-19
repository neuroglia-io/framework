using FluentAssertions;
using Google.Protobuf.Reflection;
using Neuroglia.Data.Services;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Neuroglia.UnitTests.Cases.Data.Schemas
{
    public class ProtoSchemaReaderTests
    {

        public ProtoSchemaReaderTests()
        {
            schemaReader = ProtoSchemaReader.Create();
        }

        ISchemaReader schemaReader;

        [Fact]
        public async Task GetDocument_ShouldWork()
        {
            //arrange
            var documentUri = new Uri("https://gist.githubusercontent.com/agreatfool/5e3a41052c6dd2f6d04b30901fc0269b/raw/b4e0f82f73843a79ecdbde31c1294952df3855c6/book.proto");

            //act
            var schema = await schemaReader.ReadFromAsync(documentUri);

            //assert
            schema.Should().NotBeNull();
        }

    }

}
