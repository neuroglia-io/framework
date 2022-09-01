using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Microsoft.Extensions.Logging;

namespace Neuroglia.UnitTests.Containers
{
    public class MongoDBContainer
        : TestcontainerDatabase
    {

        public const string DefaultDatabase = "test";
        public const string DefaultUsername = "test";
        public const string DefaultPassword = "test";
        public const int PublicPort = 27017;

        protected MongoDBContainer(ITestcontainersConfiguration configuration, ILogger<MongoDBContainer> logger) 
            : base(configuration, logger)
        {
            base.Database = DefaultDatabase;
            base.Username = DefaultUsername;
            base.Password = DefaultPassword;
        }

        public override string ConnectionString => $"mongodb://{Username}:{Password}@{this.Hostname}:{this.GetMappedPublicPort(PublicPort)}";

    }

}
