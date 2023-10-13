using Microsoft.Extensions.DependencyInjection;
using Neuroglia.Data.Infrastructure.Services;

namespace Neuroglia.UnitTests.Cases.Data.Infrastructure;

public abstract class QueryableRepositoryTestsBase
    : RepositoryTestsBase
{

    protected QueryableRepositoryTestsBase(IServiceCollection services) : base(services) { }

    protected override IQueryableRepository<User, string> Repository => (IQueryableRepository<User, string>)base.Repository;

    [Fact, Priority(6)]
    public async Task Query_Should_Work()
    {
        //arrange
        var user = await Repository.AddAsync(User.Create());
        await Repository.SaveChangesAsync();

        //assert
        this.Repository.AsQueryable()
            .Where(u => u.FirstName == "John" && u.LastName == "Doe")
            .ToList()
            .Should()
            .NotBeNullOrEmpty();
    }

}