using Microsoft.Extensions.DependencyInjection;
using Neuroglia.Data.Infrastructure.EventSourcing.Memory;
using Neuroglia.UnitTests.Cases.Data.Infrastructure.EventSourcing;

namespace Neuroglia.UnitTests.Cases.Data.Infrastructure.EventSourcing.EventStores;

public class MemoryEventStoreTests
    : EventStoreTestsBase
{

    public MemoryEventStoreTests() : base(BuildServices()) { }

    public static IServiceCollection BuildServices()
    {
        var services = new ServiceCollection();
        services.AddMemoryCacheEventStore(_ => { });
        return services;
    }

}
