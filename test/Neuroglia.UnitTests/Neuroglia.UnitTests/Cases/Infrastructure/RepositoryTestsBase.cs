using Neuroglia.Data.Infrastructure.Services;

namespace Neuroglia.UnitTests.Cases.Infrastructure;

public abstract class RepositoryTestsBase
{

    internal protected RepositoryTestsBase(IRepository<User, string> repository)
    {
        this.Repository = repository;
    }

    protected IRepository<User, string> Repository { get; }

    [Fact]
    public async Task Add_Should_Work()
    {
        //arrange
        var user = User.Create();

        //act
        var result = await this.Repository.AddAsync(user);
        await this.Repository.SaveChangesAsync();

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
        var user = await this.Repository.AddAsync(User.Create());
        await this.Repository.SaveChangesAsync();

        //act
        var result = await this.Repository.ContainsAsync(user.Id);

        //assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task Get_Should_Work()
    {
        //arrange
        var user = await this.Repository.AddAsync(User.Create());
        await this.Repository.SaveChangesAsync();

        //act
        var result = await this.Repository.GetAsync(user.Id);

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
        await this.Repository.AddAsync(user);

        //act
        user.VerifyEmail();
        var result = await this.Repository.UpdateAsync(user);
        await this.Repository.SaveChangesAsync();

        //assert
        result.Should().NotBeNull();
        result.EmailVerified.Should().BeTrue();
    }

    [Fact]
    public async Task Remove_Should_Work()
    {
        //arrange
        var user = await this.Repository.AddAsync(User.Create());
        await this.Repository.SaveChangesAsync();

        //act
        var result = await this.Repository.RemoveAsync(user.Id);
        var contains = await this.Repository.ContainsAsync(user.Id);

        //assert
        result.Should().BeTrue();
        contains.Should().BeFalse();
    }

}
