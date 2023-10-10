using Neuroglia.Data;

namespace Neuroglia.UnitTests.Data.Events;

internal class UserCreatedEventV3
    : DomainEvent<User, string>
{

    protected UserCreatedEventV3() { }

    public UserCreatedEventV3(string id, string firstName, string lastName, string email, Address address, string phoneNumber)
        : base(id)
    {
        this.FirstName = firstName;
        this.LastName = lastName;
        this.Email = email;
        this.Address = address;
        this.PhoneNumber = phoneNumber;
    }

    public string FirstName { get; protected set; } = null!;

    public string LastName { get; protected set; } = null!;

    public string Email { get; protected set; } = null!;

    public Address Address { get; protected set; } = null!;

    public string PhoneNumber { get; protected set; } = null!;

}