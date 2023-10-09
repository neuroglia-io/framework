using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Neuroglia.Data.EventSourcing.DistributedCache.Services;

namespace Neuroglia.UnitTests.Cases.EventSourcing;

public class MemoryEventStoreTests
    : EventStoreTestsBase
{

    public MemoryEventStoreTests() : base(new MemoryCacheEventStore(new MemoryCache(Options.Create(new MemoryCacheOptions())))) { }

}