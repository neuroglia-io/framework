using DotNet.Testcontainers.Containers;
using EventStore.Client;
using Microsoft.Extensions.DependencyInjection;
using Neuroglia.Data.Infrastructure.EventSourcing;
using Neuroglia.Serialization;
using Neuroglia.UnitTests.Cases.Data.Infrastructure.EventSourcing;
using Neuroglia.UnitTests.Containers;

namespace Neuroglia.UnitTests.Cases.Data.Infrastructure.EventSourcing.EventStores;

[TestCaseOrderer("Neuroglia.UnitTests.Services.PriorityTestCaseOrderer", "Neuroglia.UnitTests")]
public class ESEventStoreTests
    : EventStoreTestsBase
{

    public ESEventStoreTests() : base(BuildServices()) { }

    public static IServiceCollection BuildServices()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSerialization();
        services.AddSingleton(EventStoreContainerBuilder.Build());
        services.AddHostedService(provider => new ContainerBootstrapper(provider.GetRequiredService<IContainer>()));
        services.AddSingleton(provider => new EventStoreClient(EventStoreClientSettings.Create($"esdb://{provider.GetRequiredService<IContainer>().Hostname}:{provider.GetRequiredService<IContainer>().GetMappedPublicPort(EventStoreContainerBuilder.PublicPort2)}?tls=false")));
        services.AddESEventStore();
        return services;
    }

}