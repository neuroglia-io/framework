using DotNet.Testcontainers.Containers;
using Microsoft.Extensions.DependencyInjection;
using Neuroglia.Data.Infrastructure.EventSourcing.Redis;
using Neuroglia.Serialization;
using Neuroglia.UnitTests.Containers;
using StackExchange.Redis;

namespace Neuroglia.UnitTests.Cases.Data.Infrastructure.EventSourcing.EventStores;

[TestCaseOrderer("Neuroglia.UnitTests.Services.PriorityTestCaseOrderer", "Neuroglia.UnitTests")]
public class RedisEventStoreTests
    : EventStoreTestsBase
{

    public RedisEventStoreTests() : base(BuildServices()) { }

    public static IServiceCollection BuildServices()
    {
        var services = new ServiceCollection();
        services.AddSerialization();
        services.AddSingleton(provider => RedisContainerBuilder.Build());
        services.AddHostedService(provider => new ContainerBootstrapper(provider.GetRequiredService<IContainer>()));
        services.AddSingleton<IConnectionMultiplexer>(provider => ConnectionMultiplexer.Connect($"localhost:{provider.GetRequiredService<IContainer>().GetMappedPublicPort(RedisContainerBuilder.PublicPort)}"));
        services.AddRedisEventStore(_ => { });
        return services;
    }

}
