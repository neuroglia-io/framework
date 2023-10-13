using Microsoft.Extensions.DependencyInjection;
using Neuroglia.Data.Infrastructure;
using Neuroglia.Mediation;

namespace Neuroglia.UnitTests.Cases.Data.Infrastructure.Repositories;

public class MemoryCacheRepositoryTests
    : QueryableRepositoryTestsBase
{

    public MemoryCacheRepositoryTests() : base(BuildServices()) { }

    static IServiceCollection BuildServices()
    {
        var services = new ServiceCollection();
        services.AddMediator();
        services.AddMemoryCacheRepository<User, string>();
        return services;
    }

}
