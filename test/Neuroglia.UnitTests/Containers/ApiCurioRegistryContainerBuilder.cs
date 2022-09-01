using DotNet.Testcontainers.Builders;
using System;

namespace Neuroglia.UnitTests.Containers
{
    public static class ApiCurioRegistryContainerBuilder
    {

        static ApiCurioRegistryContainer Container;

        public static ApiCurioRegistryContainer Build()
        {
            if (Container != null)
                return Container;
            Container = new TestcontainersBuilder<ApiCurioRegistryContainer>()
                .WithName($"apicurio-registry-{Guid.NewGuid():N}")
                .WithImage("apicurio/apicurio-registry-mem:latest-snapshot")
                .WithPortBinding(ApiCurioRegistryContainer.HostPort, true)
                .WithWaitStrategy(Wait
                    .ForUnixContainer()
                    .UntilPortIsAvailable(ApiCurioRegistryContainer.HostPort))
                .Build();
            Container.StartAsync().GetAwaiter().GetResult();
            return Container;
        }

    }

}
