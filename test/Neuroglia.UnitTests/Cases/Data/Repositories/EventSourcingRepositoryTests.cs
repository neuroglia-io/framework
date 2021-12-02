using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Neuroglia.Data.EventSourcing;
using Neuroglia.Data.EventSourcing.Configuration;
using Neuroglia.Data.EventSourcing.Services;
using Neuroglia.Mediation;
using Neuroglia.Serialization;
using Neuroglia.UnitTests.Containers;
using Neuroglia.UnitTests.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Diagnostics;
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
                settings.ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor;
                return settings;
            };
            ServiceCollection services = new();
            services.AddLogging();
            services.AddNewtonsoftJsonSerializer(options =>
            {
                options.ContractResolver = new NonPublicSetterContractResolver();
            });
            services.AddMediator(options =>
            {
                options.ScanAssembly(typeof(EventSourcingRepositoryTests).Assembly);
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
            var originalVersion = aggregate.StateVersion;
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
            aggregate.StateVersion.Should().BeGreaterThan(originalVersion);
            aggregate = await this.Repository.FindAsync(AggregateId.Value);
            aggregate.Should().NotBeNull();
            aggregate.FirstName.Should().Be(newFirstName);
            aggregate.LastName.Should().Be(newLastName);
            aggregate.StateVersion.Should().BeGreaterThan(originalVersion);
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
            //arrange
            var aggregate = await this.Repository.FindAsync(AggregateId.Value);
            var snapshotStreamId = $"{nameof(TestPerson).ToLower()}-snapshots-{AggregateId.Value.ToString().Replace("-", "")}";
            //act

            for (int i = 0; i < EventSourcingRepositoryOptions.DefaultSnapshotFrequency; i++)
            {
                aggregate.SetFirstName($"Fake First Name {i}");
                aggregate.SetLastName($"Fake Last Name {i}");
                await this.Repository.UpdateAsync(aggregate);
                await this.Repository.SaveChangesAsync();
            }

            var stream = await this.EventStore.GetStreamAsync(snapshotStreamId);

            //assert
            stream.Should().NotBeNull();
            stream.Length.Should().Be(2);
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

        [Fact, Priority(8)]
        public async Task BenchmarkSnapshotting()
        {
            //arrange
            var repositoryWithSnapshots = ActivatorUtilities.CreateInstance<EventSourcingRepository<TestPerson, Guid>>(this.ServiceScope.ServiceProvider);
            var aggregate = new TestPerson("Fake First Name ", "Fake Last Name");
            aggregate = await repositoryWithSnapshots.AddAsync(aggregate);
            await repositoryWithSnapshots.SaveChangesAsync();
            var aggregate1Id = aggregate.Id;
            for (int i = 0; i < 10; i++)
            {
                aggregate.SetFirstName($"Fake First Name {i}");
                aggregate = await repositoryWithSnapshots.UpdateAsync(aggregate);
                await repositoryWithSnapshots.SaveChangesAsync();
            }
            var repositoryWithoutSnapshots = ActivatorUtilities.CreateInstance<EventSourcingRepository<TestPerson, Guid>>(this.ServiceScope.ServiceProvider, Options.Create(new EventSourcingRepositoryOptions<TestPerson, Guid>() { SnapshotFrequency = null }));
            aggregate = new TestPerson("Fake First Name ", "Fake Last Name");
            aggregate = await repositoryWithoutSnapshots.AddAsync(aggregate);
            await repositoryWithoutSnapshots.SaveChangesAsync();
            var aggregate2Id = aggregate.Id;
            for (int i = 0; i < 10; i++)
            {
                aggregate.SetFirstName($"Fake First Name {i}");
                aggregate = await repositoryWithoutSnapshots.UpdateAsync(aggregate);
                await repositoryWithoutSnapshots.SaveChangesAsync();
            }

            //act
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            await repositoryWithSnapshots.FindAsync(aggregate1Id);
            stopwatch.Stop();
            var durationWithSnapshots = stopwatch.Elapsed;

            stopwatch.Restart();
            await repositoryWithoutSnapshots.FindAsync(aggregate2Id);
            stopwatch.Stop();
            var durationWithoutSnapshots = stopwatch.Elapsed;

            //assert
            durationWithSnapshots.Should().BeLessThan(durationWithoutSnapshots);
        }

        public void Dispose()
        {
            this.ServiceScope.Dispose();
        }

    }

}
