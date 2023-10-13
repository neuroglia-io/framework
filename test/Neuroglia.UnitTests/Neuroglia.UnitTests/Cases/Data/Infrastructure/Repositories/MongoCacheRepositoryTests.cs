using DotNet.Testcontainers.Containers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Neuroglia.Data.Infrastructure;
using Neuroglia.Mediation;
using Neuroglia.UnitTests.Containers;

namespace Neuroglia.UnitTests.Cases.Data.Infrastructure.Repositories;

[TestCaseOrderer("Neuroglia.UnitTests.Services.PriorityTestCaseOrderer", "Neuroglia.UnitTests")]
public class MongoCacheRepositoryTests
    : QueryableRepositoryTestsBase
{

    public MongoCacheRepositoryTests() : base(BuildServices()) { }

    static IServiceCollection BuildServices()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton<IConfiguration>(new ConfigurationBuilder().Build());
        services.AddSingleton(provider => MongoContainerBuilder.Build());
        services.AddHostedService(provider => new ContainerBootstrapper(provider.GetRequiredService<IContainer>()));
        services.AddMediator();
        services.AddSingleton<IMongoClient>(provider => new MongoClient(MongoClientSettings.FromConnectionString($"mongodb://{MongoContainerBuilder.DefaultUserName}:{MongoContainerBuilder.DefaultPassword}@localhost:{provider.GetRequiredService<IContainer>().GetMappedPublicPort(MongoContainerBuilder.PublicPort)}")));
        services.AddMongoDatabase("test");
        services.AddMongoRepository<User, string>();
        return services;
    }

}
