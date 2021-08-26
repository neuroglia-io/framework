using DotNet.Testcontainers.Containers.Configurations;
using DotNet.Testcontainers.Containers.Modules.Abstractions;

namespace Neuroglia.UnitTests.Containers
{
    public class MongoDBContainer
        : TestcontainerDatabase
    {

        public const string DefaultDatabase = "test";
        public const string DefaultUsername = "test";
        public const string DefaultPassword = "test";
        public const int PublicPort = 27017;

        internal MongoDBContainer(ITestcontainersConfiguration configuration) 
            : base(configuration)
        {
            base.Database = DefaultDatabase;
            base.Username = DefaultUsername;
            base.Password = DefaultPassword;
        }

        public override string ConnectionString => $"mongodb://{Username}:{Password}@{this.Hostname}:{this.GetMappedPublicPort(PublicPort)}";

    }

}
