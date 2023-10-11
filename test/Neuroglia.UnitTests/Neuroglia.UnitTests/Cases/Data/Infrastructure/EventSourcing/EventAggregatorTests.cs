using Neuroglia.Data.Infrastructure.EventSourcing.Services;

namespace Neuroglia.UnitTests.Cases.Data.Infrastructure.EventSourcing;

public class EventAggregatorTests
{

    [Fact]
    public void Aggregate_User_Events_Should_Work()
    {
        //arrange
        var aggregator = new EventAggregator<User>();
        var userCreatedEvent = new UserCreatedEvent(Guid.NewGuid().ToString("N")[..15], "John", "Doe");
        var userEmailValidatedEvent = new UserEmailValidatedEvent(userCreatedEvent.Id, "john.doe@email.com");

        //act
        var user = aggregator.Aggregate(new object[] { userCreatedEvent, userEmailValidatedEvent });

        //assert
        user.Id.Should().Be(userCreatedEvent.Id);
        user.FirstName.Should().Be(userCreatedEvent.FirstName);
        user.LastName.Should().Be(userCreatedEvent.LastName);
        user.Email.Should().Be(userEmailValidatedEvent.Email);
    }

    [Fact]
    public void Aggregate_User_Events_WithStateFactory_Should_Work()
    {
        //arrange
        var aggregator = new EventAggregator<UserWithNonDefaultConstructor>(stateFactory: () => new UserWithNonDefaultConstructor(Guid.NewGuid().ToString()));
        var userCreatedEvent = new UserCreatedEvent(Guid.NewGuid().ToString("N")[..15], "John", "Doe");
        var userEmailValidatedEvent = new UserEmailValidatedEvent(userCreatedEvent.Id, "john.doe@email.com");

        //act
        var user = aggregator.Aggregate(new object[] { userCreatedEvent, userEmailValidatedEvent });

        //assert
        user.Id.Should().Be(userCreatedEvent.Id);
        user.FirstName.Should().Be(userCreatedEvent.FirstName);
        user.LastName.Should().Be(userCreatedEvent.LastName);
        user.Email.Should().Be(userEmailValidatedEvent.Email);
    }

    [Fact]
    public void Aggregate_User_Events_WithNonDefaultReducers_Should_Work()
    {
        //arrange
        var aggregator = new EventAggregator<UserWithNonDefaultReducers>("Apply");
        var userCreatedEvent = new UserCreatedEvent(Guid.NewGuid().ToString("N")[..15], "John", "Doe");
        var userEmailValidatedEvent = new UserEmailValidatedEvent(userCreatedEvent.Id, "john.doe@email.com");

        //act
        var user = aggregator.Aggregate(new object[] { userCreatedEvent, userEmailValidatedEvent });

        //assert
        user.Id.Should().Be(userCreatedEvent.Id);
        user.FirstName.Should().Be(userCreatedEvent.FirstName);
        user.LastName.Should().Be(userCreatedEvent.LastName);
        user.Email.Should().Be(userEmailValidatedEvent.Email);
    }

    [Fact]
    public void Rehydrate_User_Events_Should_Work()
    {
        //arrange
        var aggregator = new EventAggregator<User>();
        var user = new User() { Id = Guid.NewGuid().ToString(), FirstName = "Jane", LastName = "Doe" };
        var userEmailValidatedEvent = new UserEmailValidatedEvent(user.Id, "john.doe@email.com");

        //act
        user = aggregator.Aggregate(new object[] { userEmailValidatedEvent }, user);

        //assert
        user.Email.Should().Be(userEmailValidatedEvent.Email);
    }

    record UserCreatedEvent(string Id, string FirstName, string LastName);

    record UserEmailValidatedEvent(string Id, string Email);

    class User
    {

        public string Id { get; set; } = null!;

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string Email { get; set; } = null!;

        void On(UserCreatedEvent e)
        {
            Id = e.Id;
            FirstName = e.FirstName;
            LastName = e.LastName;
        }

        void On(UserEmailValidatedEvent e)
        {
            Email = e.Email;
        }

    }

    class UserWithNonDefaultConstructor
    {

        public UserWithNonDefaultConstructor(string id)
        {
            Id = id;
        }

        public string Id { get; private set; } = null!;

        public string FirstName { get; private set; } = null!;

        public string LastName { get; private set; } = null!;

        public string Email { get; private set; } = null!;

        void On(UserCreatedEvent e)
        {
            Id = e.Id;
            FirstName = e.FirstName;
            LastName = e.LastName;
        }

        void On(UserEmailValidatedEvent e)
        {
            Email = e.Email;
        }

    }

    class UserWithNonDefaultReducers
    {

        public string Id { get; private set; } = null!;

        public string FirstName { get; private set; } = null!;

        public string LastName { get; private set; } = null!;

        public string Email { get; private set; } = null!;

        void Apply(UserCreatedEvent e)
        {
            Id = e.Id;
            FirstName = e.FirstName;
            LastName = e.LastName;
        }

        void Apply(UserEmailValidatedEvent e)
        {
            Email = e.Email;
        }

    }

}
