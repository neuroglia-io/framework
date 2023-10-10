using Neuroglia.Data;

namespace Neuroglia.UnitTests.Data.Events;

internal class UserCreatedEvent
    : DomainEvent<User, string>
{

    protected UserCreatedEvent() { }

    public UserCreatedEvent(string id, string firstName, string lastName, string email)
        : base(id)
    {
        this.FirstName = firstName;
        this.LastName = lastName;
        this.Email = email;
    }

    public string FirstName { get; protected set; } = null!;

    public string LastName { get; protected set; } = null!;

    public string Email { get; protected set; } = null!;

}
