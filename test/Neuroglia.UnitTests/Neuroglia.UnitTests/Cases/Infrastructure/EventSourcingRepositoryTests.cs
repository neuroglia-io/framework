using Microsoft.Extensions.DependencyInjection;
using Neuroglia.Data.EventSourcing.MemoryCache;
using Neuroglia.Data.Infrastructure.EventSourcing;
using Neuroglia.Data.Infrastructure.Services;
using Neuroglia.Mediation;

namespace Neuroglia.UnitTests.Cases.Infrastructure;

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
