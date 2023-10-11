using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;

namespace Neuroglia.UnitTests.Containers;

public static class EventStoreContainerBuilder
{

    public const int PublicPort1 = 1113;
    public const int PublicPort2 = 2113;

    public static IContainer Build()
    {
        return new ContainerBuilder()
            .WithName($"event-store-{Guid.NewGuid():N}")
            .WithImage("eventstore/eventstore:latest")
            .WithPortBinding(PublicPort1, true)
            .WithPortBinding(PublicPort2, true)
            .WithEnvironment("EVENTSTORE_RUN_PROJECTIONS", "All")
            .WithEnvironment("EVENTSTORE_START_STANDARD_PROJECTIONS", "true")
            .WithEnvironment("EVENTSTORE_EXT_TCP_PORT", "1113")
            .WithEnvironment("EVENTSTORE_HTTP_PORT", "2113")
            .WithEnvironment("EVENTSTORE_INSECURE", "true")
            .WithEnvironment("EVENTSTORE_ENABLE_EXTERNAL_TCP", "true")
            .WithEnvironment("EVENTSTORE_ENABLE_ATOM_PUB_OVER_HTTP", "true")
            .WithWaitStrategy(Wait
                .ForUnixContainer()
                .UntilPortIsAvailable(PublicPort2))
            .Build();
    }

}