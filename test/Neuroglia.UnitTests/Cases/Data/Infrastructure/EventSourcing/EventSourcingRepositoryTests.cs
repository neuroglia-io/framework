// Copyright © 2021-Present Neuroglia SRL. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License"),
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Microsoft.Extensions.DependencyInjection;
using Neuroglia.Data.Infrastructure.EventSourcing.Memory;
using Neuroglia.Data.Infrastructure.EventSourcing;
using Neuroglia.Data.Infrastructure.EventSourcing.Services;
using Neuroglia.Mapping;
using Neuroglia.Mediation;
using Neuroglia.UnitTests.Data.Events;
using Neuroglia.Serialization;

namespace Neuroglia.UnitTests.Cases.Data.Infrastructure.EventSourcing;

public class EventSourcingRepositoryTests
{

    public EventSourcingRepositoryTests()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddMediator();
        serviceCollection.AddMapper(typeof(EventSourcingRepositoryTests).Assembly);
        serviceCollection.AddJsonSerializer();
        serviceCollection.AddMemoryEventStore();
        serviceCollection.AddEventSourcingRepository<User, string>();
        this.ServiceScope = serviceCollection.BuildServiceProvider().CreateScope();
    }

    protected IServiceScope ServiceScope { get; }

    protected IServiceProvider ServiceProvider => this.ServiceScope.ServiceProvider;

    protected IEventStore EventStore => this.ServiceProvider.GetRequiredService<IEventStore>();

    protected IEventMigrationManager MigrationManager => this.ServiceProvider.GetRequiredService<IEventMigrationManager>();

    protected EventSourcingRepository<User, string> Repository => this.ServiceProvider.GetRequiredService<EventSourcingRepository<User, string>>();

    [Fact]
    public async Task Migrate_UsingSingleMigration_Should_Work()
    {
        //arrange
        var user = User.Create();
        await this.Repository.AddAsync(user);
        await this.Repository.SaveChangesAsync();
        this.MigrationManager.RegisterEventMigration(typeof(UserCreatedEvent), (provider, e) => provider.GetRequiredService<IMapper>().Map<UserCreatedEventV2>(e));

        //act
        var result = await this.Repository.GetAsync(user.Id);

        //assert
        result.Should().NotBeNull();
        result!.State.IsV1.Should().BeFalse();
        result.State.IsV2.Should().BeTrue();
    }

    [Fact]
    public async Task Migrate_UsingChainMigration_Should_Work()
    {
        //arrange
        var user = User.Create();
        await this.Repository.AddAsync(user);
        await this.Repository.SaveChangesAsync();
        this.MigrationManager.RegisterEventMigration(typeof(UserCreatedEvent), (provider, e) => provider.GetRequiredService<IMapper>().Map<UserCreatedEventV2>(e));
        this.MigrationManager.RegisterEventMigration(typeof(UserCreatedEventV2), (provider, e) => provider.GetRequiredService<IMapper>().Map<UserCreatedEventV3>(e));

        //act
        var result = await this.Repository.GetAsync(user.Id);

        //assert
        result.Should().NotBeNull();
        result!.State.IsV1.Should().BeFalse();
        result.State.IsV2.Should().BeFalse();
        result.State.IsV3.Should().BeTrue();
    }

}
