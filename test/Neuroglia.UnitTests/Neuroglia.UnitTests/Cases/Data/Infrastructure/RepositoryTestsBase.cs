using Neuroglia.Data.Infrastructure.Services;

namespace Neuroglia.UnitTests.Cases.Data.Infrastructure;

public abstract class RepositoryTestsBase
{

    internal protected RepositoryTestsBase(IRepository<User, string> repository)
    {
        Repository = repository;
    }

    protected IRepository<User, string> Repository { get; }

    [Fact]
    public async Task Add_Should_Work()
    {
        //arrange
        var user = User.Create();

        //act
        var result = await Repository.AddAsync(user);
        await Repository.SaveChangesAsync();

        //assert
        result.Should().NotBeNull();
        result.Id.Should().Be(user.Id);
        result.FirstName.Should().Be(user.FirstName);
        result.LastName.Should().Be(user.LastName);
        result.Email.Should().Be(user.Email);
    }

    [Fact]
    public async Task Contains_Should_Work()
    {
        //arrange
        var user = await Repository.AddAsync(User.Create());
        await Repository.SaveChangesAsync();

        //act
        var result = await Repository.ContainsAsync(user.Id);

        //assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task Get_Should_Work()
    {
        //arrange
        var user = await Repository.AddAsync(User.Create());
        await Repository.SaveChangesAsync();

        //act
        var result = await Repository.GetAsync(user.Id);

        //assert
        result.Should().NotBeNull();
        result!.FirstName.Should().Be(user.FirstName);
        result.LastName.Should().Be(user.LastName);
        result.Email?.Should().Be(user.Email);
    }

    [Fact]
    public async Task Update_Should_Work()
    {
        //arrange
        var user = User.Create();
        await Repository.AddAsync(user);

        //act
        user.VerifyEmail();
        var result = await Repository.UpdateAsync(user);
        await Repository.SaveChangesAsync();

        //assert
        result.Should().NotBeNull();
        result.EmailVerified.Should().BeTrue();
    }

    [Fact]
    public async Task Remove_Should_Work()
    {
        //arrange
        var user = await Repository.AddAsync(User.Create());
        await Repository.SaveChangesAsync();

        //act
        var result = await Repository.RemoveAsync(user.Id);
        var contains = await Repository.ContainsAsync(user.Id);

        //assert
        result.Should().BeTrue();
        contains.Should().BeFalse();
    }

}
