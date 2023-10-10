using Neuroglia.Data;

namespace Neuroglia.UnitTests.Data.Events;

internal class UserCreatedEventV2
    : DomainEvent<User, string>
{

    protected UserCreatedEventV2() { }

    public UserCreatedEventV2(string id, string firstName, string lastName, string email, Address address)
        : base(id)
    {
        this.FirstName = firstName;
        this.LastName = lastName;
        this.Email = email;
        this.Address = address;
    }

    public string FirstName { get; protected set; } = null!;

    public string LastName { get; protected set; } = null!;

    public string Email { get; protected set; } = null!;

    public Address Address { get; protected set; } = null!;

}
