using DotNet.Testcontainers.Containers.Configurations;
using DotNet.Testcontainers.Containers.Modules.Abstractions;

namespace Neuroglia.UnitTests.Containers
{

    public class EventStoreContainer
        : TestcontainerDatabase
    {

        public const int PublicPort1 = 1113;
        public const int PublicPort2 = 2113;

        internal EventStoreContainer(ITestcontainersConfiguration configuration)
            : base(configuration)
        {
            
        }

        public override string ConnectionString => $"ConnectTo=tcp://admin:changeit@{this.Hostname}:{this.GetMappedPublicPort(PublicPort2)};";

    }

}
