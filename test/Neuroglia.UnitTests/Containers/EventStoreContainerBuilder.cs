using DotNet.Testcontainers.Containers.Builders;
using DotNet.Testcontainers.Containers.WaitStrategies;
using System;

namespace Neuroglia.UnitTests.Containers
{
    public static class EventStoreContainerBuilder
    {

        static EventStoreContainer Container;

        public static EventStoreContainer Build()
        {
            if (Container != null)
                return Container;
            Container = new TestcontainersBuilder<EventStoreContainer>()
                .WithName($"event-store-{Guid.NewGuid().ToString("N")}")
                .WithImage("eventstore/eventstore:latest")
                .WithPortBinding(EventStoreContainer.PublicPort1, true)
                .WithPortBinding(EventStoreContainer.PublicPort2, true)
                .WithEnvironment("EVENTSTORE_RUN_PROJECTIONS", "All")
                .WithEnvironment("EVENTSTORE_START_STANDARD_PROJECTIONS", "true")
                .WithEnvironment("EVENTSTORE_EXT_TCP_PORT", "1113")
                .WithEnvironment("EVENTSTORE_HTTP_PORT", "2113")
                .WithEnvironment("EVENTSTORE_INSECURE", "true")
                .WithEnvironment("EVENTSTORE_ENABLE_EXTERNAL_TCP", "true")
                .WithEnvironment("EVENTSTORE_ENABLE_ATOM_PUB_OVER_HTTP", "true")
                .WithWaitStrategy(Wait
                    .ForUnixContainer()
                    .UntilPortIsAvailable(EventStoreContainer.PublicPort2))
                .Build();
            Container.StartAsync().GetAwaiter().GetResult();
            return Container;
        }

    }

}
