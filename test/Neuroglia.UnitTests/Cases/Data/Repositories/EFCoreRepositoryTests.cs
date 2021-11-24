using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Neuroglia.Data;
using Neuroglia.UnitTests.Containers;
using Neuroglia.UnitTests.Data;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Neuroglia.UnitTests.Cases.Data.Repositories
{

    [TestCaseOrderer("Xunit.PriorityTestCaseOrderer", "Neuroglia.Xunit")]
    public sealed class EFCoreRepositoryTests
        : IDisposable
    {

        public EFCoreRepositoryTests()
        {
            ServiceCollection services = new();
            services.AddLogging();
            services.AddDbContext<TestDbContext>(options => options.UseNpgsql(PostgreSQLContainerBuilder.Build().ConnectionString));
            services.AddEFCoreRepositories<TestDbContext>();
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            this.ServiceScope = services.BuildServiceProvider().CreateScope();
            this.DbContext = this.ServiceScope.ServiceProvider.GetRequiredService<TestDbContext>();
            this.Repository = this.ServiceScope.ServiceProvider.GetRequiredService<EFCoreRepository<TestPerson, Guid, TestDbContext>>();
        }

        IServiceScope ServiceScope { get; }

        TestDbContext DbContext { get; }

        EFCoreRepository<TestPerson, Guid, TestDbContext> Repository { get; }

        static Guid? EntityId;

        [Fact, Priority(0)]
        public async Task AddEntity()
        {
            //arrange
            var firstName = "Fake First Name";
            var lastName = "Fake Last Name";
            var entity = new TestPerson(firstName, lastName);
            await this.DbContext.Database.EnsureCreatedAsync();

            //act
            entity = await this.Repository.AddAsync(entity);
            await this.Repository.SaveChangesAsync();
            EntityId = entity?.Id;

            //assert
            entity.Should().NotBeNull();
            entity.FirstName.Should().Be(firstName);
            entity.LastName.Should().Be(lastName);
            entity = await this.DbContext.Set<TestPerson>().FirstOrDefaultAsync();
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
            entity.Should().Be(await this.DbContext.Set<TestPerson>().FirstOrDefaultAsync());
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
            entity = await this.DbContext.Set<TestPerson>().FirstOrDefaultAsync();
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
        public async Task GetQueryable()
        {
            //act
            var entities = await this.Repository.AsQueryable().ToListAsync();

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
            TestPerson entity = await this.DbContext.Set<TestPerson>().FirstOrDefaultAsync();
            entity.Should().BeNull();
        }

        public void Dispose()
        {
            this.ServiceScope.Dispose();
        }

    }

}
