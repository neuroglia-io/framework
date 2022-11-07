using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Microsoft.Extensions.Logging;

namespace Neuroglia.UnitTests.Containers
{

    public sealed class EventStoreContainer
        : TestcontainerDatabase
    {

        public const int PublicPort1 = 1113;
        public const int PublicPort2 = 2113;

        internal EventStoreContainer(ITestcontainersConfiguration configuration, ILogger logger) 
            : base(configuration, logger)
        {

        }

        public override string ConnectionString => $"esdb://{this.Hostname}:{this.GetMappedPublicPort(PublicPort2)}?tls=false";

    }

}
