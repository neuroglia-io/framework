using Microsoft.Extensions.DependencyInjection;
using Neuroglia.Data.EventSourcing.Redis;
using Neuroglia.Data.EventSourcing.Services;
using Neuroglia.Data.Infrastructure.EventSourcing.Services;
using Neuroglia.Serialization;
using Neuroglia.UnitTests.Containers;
using StackExchange.Redis;

namespace Neuroglia.UnitTests.Cases.EventSourcing;

[TestCaseOrderer("Neuroglia.UnitTests.Services.PriorityTestCaseOrderer", "Neuroglia.UnitTests")]
public class RedisEventStoreTests
    : EventStoreTestsBase
{

    public RedisEventStoreTests() : base(BuildEventStore()) { }

    protected override void Dispose(bool disposing)
    {
        if (!disposing) return;
        base.Dispose(disposing);
        RedisContainer.DisposeAsync().GetAwaiter().GetResult();
    }

    public static IEventStore BuildEventStore()
    {
        var services = new ServiceCollection();
        services.AddSerialization();
        services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(RedisContainer.ConnectionString));
        services.AddRedisEventStore(_ => { });
        return services.BuildServiceProvider().CreateScope().ServiceProvider.GetRequiredService<RedisEventStore>();
    }

}