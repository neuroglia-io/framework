using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Microsoft.Extensions.Logging;

namespace Neuroglia.UnitTests.Containers
{

    public class EventStoreContainer
        : TestcontainerDatabase
    {

        public const int PublicPort1 = 1113;
        public const int PublicPort2 = 2113;

        protected EventStoreContainer(ITestcontainersConfiguration configuration, ILogger<EventStoreContainer> logger) 
            : base(configuration, logger)
        {

        }

        public override string ConnectionString => $"esdb://{this.Hostname}:{this.GetMappedPublicPort(PublicPort2)}?tls=false";

    }

}
