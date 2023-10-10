using Neuroglia.Data;

namespace Neuroglia.UnitTests.Data.Events;

internal class UserEmailValidatedEvent
    : DomainEvent<User, string>
{

    protected UserEmailValidatedEvent() { }

    public UserEmailValidatedEvent(string id) : base(id) { }

}
