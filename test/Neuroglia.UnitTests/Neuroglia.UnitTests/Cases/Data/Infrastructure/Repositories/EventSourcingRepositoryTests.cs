using Microsoft.Extensions.DependencyInjection;
using Neuroglia.Data.Infrastructure.EventSourcing;
using Neuroglia.Data.Infrastructure.EventSourcing.Memory;
using Neuroglia.Mediation;

namespace Neuroglia.UnitTests.Cases.Data.Infrastructure.Repositories;

public class EventSourcingRepositoryTests
    : RepositoryTestsBase
{

    public EventSourcingRepositoryTests() : base(BuildServices()) { }

    static IServiceCollection BuildServices()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddMediator();
        serviceCollection.AddMemoryCacheEventStore(_ => { });
        serviceCollection.AddEventSourcingRepository<User, string>();
        return serviceCollection;
    }

}
