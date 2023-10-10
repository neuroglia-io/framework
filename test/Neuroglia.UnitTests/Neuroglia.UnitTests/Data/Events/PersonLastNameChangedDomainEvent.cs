using Neuroglia.Data;

namespace Neuroglia.UnitTests.Data.Events;

public class PersonLastNameChangedDomainEvent
    : DomainEvent<Person, Guid>
{

    protected PersonLastNameChangedDomainEvent() { }

    public PersonLastNameChangedDomainEvent(Guid personId, string lastName)
        : base(personId)
    {
        this.LastName = lastName;
    }

    public string LastName { get; protected set; } = null!;

}
