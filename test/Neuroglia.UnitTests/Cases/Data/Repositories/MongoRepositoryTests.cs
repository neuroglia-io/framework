using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Neuroglia.Data;
using Neuroglia.UnitTests.Containers;
using Neuroglia.UnitTests.Data;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Neuroglia.UnitTests.Cases.Data.Repositories
{

    [TestCaseOrderer("Xunit.PriorityTestCaseOrderer", "Neuroglia.Xunit")]
    public sealed class MongoRepositoryTests
        : IDisposable
    {

        public MongoRepositoryTests()
        {
            ServiceCollection services = new();
            services.AddLogging();
            services.AddMongoDbContext<TestMongoDbContext>(builder => builder
                .UseDatabase("test")
                .UseConnectionString(MongoDBContainerBuilder.Build().ConnectionString));
            services.AddMongoRepository<TestPerson, Guid, TestMongoDbContext>();
            this.ServiceScope = services.BuildServiceProvider().CreateScope();
            this.DbContext = this.ServiceScope.ServiceProvider.GetRequiredService<TestMongoDbContext>();
            this.Repository = this.ServiceScope.ServiceProvider.GetRequiredService<MongoRepository<TestPerson, Guid, TestMongoDbContext>>();
        }

        IServiceScope ServiceScope { get; }

        TestMongoDbContext DbContext { get; }

        MongoRepository<TestPerson, Guid, TestMongoDbContext> Repository { get; }

        static Guid? EntityId;

        [Fact, Priority(0)]
        public async Task AddEntity()
        {
            //arrange
            var firstName = "Fake First Name";
            var lastName = "Fake Last Name";
            var entity = new TestPerson(firstName, lastName);
            await this.DbContext.EnsureCreatedAsync();

            //act
            entity = await this.Repository.AddAsync(entity);
            await this.Repository.SaveChangesAsync();
            EntityId = entity?.Id;

            //assert
            entity.Should().NotBeNull();
            entity.FirstName.Should().Be(firstName);
            entity.LastName.Should().Be(lastName);
            entity = await this.DbContext.Collection<TestPerson>().AsQueryable().FirstOrDefaultAsync();
            entity.Should().NotBeNull();
            entity.FirstName.Should().Be(firstName);
            entity.LastName.Should().Be(lastName);
        }

        [Fact, Priority(1)]
        public async Task ContainsEntity()
        {
            //act
            bool exists = await this.Repository.ContainsAsync(EntityId.Value);

            //assert
            exists.Should().BeTrue();
        }

        [Fact, Priority(2)]
        public async Task FindEntity()
        {
            //act
            TestPerson entity = await this.Repository.FindAsync(EntityId.Value);

            //assert
            entity.Should().NotBeNull();
            TestPerson fromcontext = await this.DbContext.Collection<TestPerson>().AsQueryable().FirstOrDefaultAsync();
            entity.Should().NotBeNull();
            entity.Id.Should().Be(fromcontext.Id);
            entity.FirstName.Should().Be(fromcontext.FirstName);
            entity.LastName.Should().Be(fromcontext.LastName);
        }

        [Fact, Priority(3)]
        public async Task UpdateEntity()
        {
            //arrange
            var entity = await this.Repository.FindAsync(EntityId.Value);
            var newFirstName = "Updated Fake First Name";
            var newLastName = "Updated Fake Last Name";
            entity.FirstName = newFirstName;
            entity.LastName = newLastName;

            //act
            entity = await this.Repository.UpdateAsync(entity);

            //assert
            entity.Should().NotBeNull();
            entity.FirstName.Should().Be(newFirstName);
            entity.LastName.Should().Be(newLastName);
            entity = await this.DbContext.Collection<TestPerson>().AsQueryable().FirstOrDefaultAsync();
            entity.Should().NotBeNull();
            entity.FirstName.Should().Be(newFirstName);
            entity.LastName.Should().Be(newLastName);
        }

        [Fact, Priority(4)]
        public async Task ListEntities()
        {
            //act
            var entities = await this.Repository.ToListAsync();

            //assert
            entities.Should().NotBeEmpty();
        }

        [Fact, Priority(5)]
        public void GetQueryable()
        {
            //act
            var entities = this.Repository.AsQueryable().ToList();

            //assert
            entities.Should().NotBeEmpty();
        }

        [Fact, Priority(6)]
        public async Task RemoveEntity()
        {
            //act
            await this.Repository.RemoveAsync(EntityId.Value);
            await this.Repository.SaveChangesAsync();

            //assert
            TestPerson entity = await this.DbContext.Collection<TestPerson>().AsQueryable().FirstOrDefaultAsync();
            entity.Should().BeNull();
        }

        public void Dispose()
        {
            this.ServiceScope.Dispose();
        }

    }

}
