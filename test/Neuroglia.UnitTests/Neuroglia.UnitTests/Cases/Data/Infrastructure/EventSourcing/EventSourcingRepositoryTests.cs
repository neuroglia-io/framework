using Microsoft.Extensions.DependencyInjection;
using Neuroglia.Data.Infrastructure.EventSourcing.Memory;
using Neuroglia.Data.Infrastructure.EventSourcing;
using Neuroglia.Data.Infrastructure.EventSourcing.Services;
using Neuroglia.Mapping;
using Neuroglia.Mediation;
using Neuroglia.UnitTests.Data.Events;

namespace Neuroglia.UnitTests.Cases.Data.Infrastructure.EventSourcing;

public class EventSourcingRepositoryTests
{

    public EventSourcingRepositoryTests()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddMediator();
        serviceCollection.AddMapper(typeof(EventSourcingRepositoryTests).Assembly);
        serviceCollection.AddMemoryCacheEventStore(_ => { });
        serviceCollection.AddEventSourcingRepository<User, string>();
        ServiceScope = serviceCollection.BuildServiceProvider().CreateScope();
    }

    protected IServiceScope ServiceScope { get; }

    protected IServiceProvider ServiceProvider => ServiceScope.ServiceProvider;

    protected IEventStore EventStore => ServiceProvider.GetRequiredService<IEventStore>();

    protected IEventMigrationManager MigrationManager => ServiceProvider.GetRequiredService<IEventMigrationManager>();

    protected EventSourcingRepository<User, string> Repository => ServiceProvider.GetRequiredService<EventSourcingRepository<User, string>>();

    [Fact]
    public async Task Migrate_UsingSingleMigration_Should_Work()
    {
        //arrange
        var user = User.Create();
        await Repository.AddAsync(user);
        await Repository.SaveChangesAsync();
        MigrationManager.RegisterEventMigration(typeof(UserCreatedEvent), (provider, e) => provider.GetRequiredService<IMapper>().Map<UserCreatedEventV2>(e));

        //act
        var result = await Repository.GetAsync(user.Id);

        //assert
        result.Should().NotBeNull();
        result!.IsV1.Should().BeFalse();
        result.IsV2.Should().BeTrue();
    }

    [Fact]
    public async Task Migrate_UsingChainMigration_Should_Work()
    {
        //arrange
        var user = User.Create();
        await Repository.AddAsync(user);
        await Repository.SaveChangesAsync();
        MigrationManager.RegisterEventMigration(typeof(UserCreatedEvent), (provider, e) => provider.GetRequiredService<IMapper>().Map<UserCreatedEventV2>(e));
        MigrationManager.RegisterEventMigration(typeof(UserCreatedEventV2), (provider, e) => provider.GetRequiredService<IMapper>().Map<UserCreatedEventV3>(e));

        //act
        var result = await Repository.GetAsync(user.Id);

        //assert
        result.Should().NotBeNull();
        result!.IsV1.Should().BeFalse();
        result.IsV2.Should().BeFalse();
        result.IsV3.Should().BeTrue();
    }

}
