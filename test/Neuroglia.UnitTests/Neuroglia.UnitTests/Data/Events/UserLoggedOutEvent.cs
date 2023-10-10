using Neuroglia.Data;

namespace Neuroglia.UnitTests.Data.Events;

internal class UserLoggedOutEvent
    : DomainEvent<User, string>
{

    protected UserLoggedOutEvent() { }

    public UserLoggedOutEvent(string id) : base(id) { }

}