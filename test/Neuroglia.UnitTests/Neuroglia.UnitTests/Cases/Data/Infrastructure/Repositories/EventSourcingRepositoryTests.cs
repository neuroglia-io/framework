using Microsoft.Extensions.DependencyInjection;
using Neuroglia.Data.Infrastructure.EventSourcing.MemoryCache;
using Neuroglia.Data.Infrastructure.EventSourcing;
using Neuroglia.Data.Infrastructure.Services;
using Neuroglia.Mediation;
using Neuroglia.UnitTests.Cases.Data.Infrastructure;

namespace Neuroglia.UnitTests.Cases.Data.Infrastructure.Repositories;

public class EventSourcingRepositoryTests
    : RepositoryTestsBase
{

    public EventSourcingRepositoryTests()
        : base(BuildRepository())
    {

    }

    static IRepository<User, string> BuildRepository()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddMediator();
        serviceCollection.AddMemoryCacheEventStore(_ => { });
        serviceCollection.AddEventSourcingRepository<User, string>();
        return serviceCollection.BuildServiceProvider().CreateScope().ServiceProvider.GetRequiredService<IRepository<User, string>>();
    }

}
