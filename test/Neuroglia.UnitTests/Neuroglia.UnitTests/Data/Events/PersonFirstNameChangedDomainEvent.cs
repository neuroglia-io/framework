using Neuroglia.Data;

namespace Neuroglia.UnitTests.Data.Events;

public class PersonFirstNameChangedDomainEvent
    : DomainEvent<Person, Guid>
{

    protected PersonFirstNameChangedDomainEvent() { }
    public PersonFirstNameChangedDomainEvent(Guid personId, string firstName)
        : base(personId)
    {
        this.FirstName = firstName;
    }

    public string FirstName { get; protected set; } = null!;

}
