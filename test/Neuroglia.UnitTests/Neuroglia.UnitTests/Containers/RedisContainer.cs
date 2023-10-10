using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;

namespace Neuroglia.UnitTests.Containers;

public static class RedisContainer
{

    static IContainer? Container;
    const int PublicPort = 6379;

    public static string ConnectionString => $"localhost:{Build().GetMappedPublicPort(PublicPort)}";

    public static IContainer Build()
    {
        if (Container != null) return Container;
        Container = new ContainerBuilder()
            .WithName($"redis-{Guid.NewGuid():N}")
            .WithImage("redis")
            .WithPortBinding(PublicPort, true)
            .WithWaitStrategy(Wait
                .ForUnixContainer()
                .UntilPortIsAvailable(PublicPort)
                .UntilMessageIsLogged("Ready to accept connections"))
            .Build();
        Container.StartAsync().GetAwaiter().GetResult();
        return Container;
    }

    public static async ValueTask DisposeAsync()
    {
        if (Container == null) return;
        await Container.DisposeAsync().ConfigureAwait(false);
        Container = null;
    }

}