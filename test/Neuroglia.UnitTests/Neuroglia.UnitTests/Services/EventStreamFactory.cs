using Neuroglia.Data.Infrastructure.EventSourcing;

namespace Neuroglia.UnitTests.Services;

internal static class EventStreamFactory
{

    internal static IEnumerable<IEventDescriptor> Create()
    {
        var userId = Guid.NewGuid();
        yield return new EventDescriptor("user-created", new UserCreatedEvent(userId, "John", "Doe", "john.doe@email.com"));
        yield return new EventDescriptor("user-email-confirmed", new UserEmailConfirmedEvent(userId));
        yield return new EventDescriptor("user-logged-in", new UserLoggedInEvent(userId));
        yield return new EventDescriptor("user-logged-out", new UserLoggedOutEvent(userId));
    }

    class UserCreatedEvent
    {


        protected UserCreatedEvent() { }

        public UserCreatedEvent(Guid id, string firstName, string lastName, string email)
        {
            this.Id = id;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Email = email;
        }

        public Guid Id { get; protected set; }

        public string FirstName { get; protected set; } = null!;

        public string LastName { get; protected set; } = null!;

        public string Email { get; protected set; } = null!;

    }

    class UserEmailConfirmedEvent
    {

        public UserEmailConfirmedEvent(Guid id)
        {
            this.Id = id;
        }
        public Guid Id { get; protected set; }

    }

    class UserLoggedInEvent
    {

        public UserLoggedInEvent(Guid id)
        {
            this.Id = id;
        }
        public Guid Id { get; protected set; }

    }

    class UserLoggedOutEvent
    {

        public UserLoggedOutEvent(Guid id)
        {
            this.Id = id;
        }
        public Guid Id { get; protected set; }

    }

}