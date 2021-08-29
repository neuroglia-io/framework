using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Neuroglia.Data.EventSourcing;
using Neuroglia.Data.EventSourcing.Configuration;
using Neuroglia.Data.EventSourcing.Services;
using Neuroglia.Serialization;
using Neuroglia.UnitTests.Containers;
using Neuroglia.UnitTests.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Neuroglia.UnitTests.Cases.Data.Repositories
{

    [TestCaseOrderer("Xunit.PriorityTestCaseOrderer", "Neuroglia.Xunit")]
    public sealed class EventSourcingRepositoryTests
        : IDisposable
    {

        public EventSourcingRepositoryTests()
        {
            JsonConvert.DefaultSettings = () =>
            {
                JsonSerializerSettings settings = new();
                settings.ContractResolver = new NonPublicSetterContractResolver();
                return settings;
            };
            ServiceCollection services = new();
            services.AddLogging();
            services.AddNewtonsoftJsonSerializer(options =>
            {
                options.ContractResolver = new NonPublicSetterContractResolver();
            });
            services.AddEventStore(builder =>
                builder.ConfigureClient(client =>
                {
                    client.UseConnectionString(EventStoreContainerBuilder.Build().ConnectionString);
                }));
            services.AddEventSourcingRepository<TestPerson, Guid>();
            this.ServiceScope = services.BuildServiceProvider().CreateScope();
            this.EventStore = this.ServiceScope.ServiceProvider.GetRequiredService<IEventStore>();
            this.Repository = this.ServiceScope.ServiceProvider.GetRequiredService<EventSourcingRepository<TestPerson, Guid>>();
        }

        IServiceScope ServiceScope { get; }

        IEventStore EventStore { get; }

        EventSourcingRepository<TestPerson, Guid> Repository { get; }

        static Guid? AggregateId;

        [Fact, Priority(0)]
        public async Task AddAggregate()
        {
            //arrange
            var firstName = "Fake First Name";
            var lastName = "Fake Last Name";
            var aggregate = new TestPerson(firstName, lastName);

            //act
            aggregate = await this.Repository.AddAsync(aggregate);
            await this.Repository.SaveChangesAsync();
            AggregateId = aggregate?.Id;

            //assert
            aggregate.Should().NotBeNull();
            aggregate.FirstName.Should().Be(firstName);
            aggregate.LastName.Should().Be(lastName);
            aggregate = await this.Repository.FindAsync(AggregateId.Value);
            aggregate.Should().NotBeNull();
            aggregate.FirstName.Should().Be(firstName);
            aggregate.LastName.Should().Be(lastName);
        }

        [Fact, Priority(1)]
        public async Task ContainsAggregate()
        {
            //act
            bool exists = await this.Repository.ContainsAsync(AggregateId.Value);

            //assert
            exists.Should().BeTrue();
        }

        [Fact, Priority(2)]
        public async Task FindAggregate()
        {
            //act
            TestPerson aggregate = await this.Repository.FindAsync(AggregateId.Value);

            //assert
            aggregate.Should().NotBeNull();
        }

        [Fact, Priority(3)]
        public async Task UpdateAggregate()
        {
            //arrange
            var aggregate = await this.Repository.FindAsync(AggregateId.Value);
            var originalVersion = aggregate.Version;
            var newFirstName = "Updated Fake First Name";
            var newLastName = "Updated Fake Last Name";
            aggregate.SetFirstName(newFirstName);
            aggregate.SetLastName(newLastName);

            //act
            aggregate = await this.Repository.UpdateAsync(aggregate);

            //assert
            aggregate.Should().NotBeNull();
            aggregate.FirstName.Should().Be(newFirstName);
            aggregate.LastName.Should().Be(newLastName);
            aggregate.Version.Should().BeGreaterThan(originalVersion);
            aggregate = await this.Repository.FindAsync(AggregateId.Value);
            aggregate.Should().NotBeNull();
            aggregate.FirstName.Should().Be(newFirstName);
            aggregate.LastName.Should().Be(newLastName);
            aggregate.Version.Should().BeGreaterThan(originalVersion);
        }

        [Fact, Priority(4)]
        public async Task ListEntities()
        {
            //act
            Func<Task> act = () => this.Repository.ToListAsync();

            //assert
            await act.Should().ThrowAsync<NotSupportedException>();
        }

        [Fact, Priority(5)]
        public void GetQueryable()
        {
            //act
            Action act = () => this.Repository.AsQueryable();

            //assert
            act.Should().Throw<NotSupportedException>();
        }

        [Fact, Priority(6)]
        public async Task Snapshot()
        {
            //act
            var aggregate = await this.Repository.FindAsync(AggregateId.Value);
            for (int i = 0; i < EventSourcingRepositoryOptions.DefaultSnapshotFrequency / 2; i++)
            {
                aggregate.SetFirstName($"Fake First Name {i}");
                aggregate.SetLastName($"Fake Last Name {i}");
            }
            await this.Repository.UpdateAsync(aggregate);
            await this.Repository.SaveChangesAsync();
            var stream = await this.EventStore.GetStreamAsync($"{nameof(TestPerson).ToLower()}-snapshots-{AggregateId.Value.ToString().Replace("-", "")}");

            //assert
            stream.Should().NotBeNull();
            stream.Length.Should().Be(1);
        }

        [Fact, Priority(7)]
        public async Task RemoveAggregate()
        {
            //act
            await this.Repository.RemoveAsync(AggregateId.Value);
            await this.Repository.SaveChangesAsync();

            //assert
            TestPerson aggregate = await this.Repository.FindAsync(AggregateId.Value);
            aggregate.Should().BeNull();
        }

        public void Dispose()
        {
            this.ServiceScope.Dispose();
        }

    }

}
