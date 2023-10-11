using Microsoft.Extensions.DependencyInjection;
using Neuroglia.Data.Infrastructure.EventSourcing.MemoryCache;

namespace Neuroglia.UnitTests.Cases.EventSourcing;

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
