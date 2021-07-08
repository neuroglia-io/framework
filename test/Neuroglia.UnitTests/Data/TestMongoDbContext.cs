using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Neuroglia.Data;

namespace Neuroglia.UnitTests.Data
{

    public class TestMongoDbContext
        : MongoDbContext
    {

        public TestMongoDbContext(ILoggerFactory loggerFactory, IOptions<MongoDbContextOptions<TestMongoDbContext>> options, IPluralizer pluralizer) 
            : base(loggerFactory, options, pluralizer)
        {

        }

    }

}
