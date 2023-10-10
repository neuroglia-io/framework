using Neuroglia.Data;

namespace Neuroglia.UnitTests.Data.Events;

internal class UserLoggedInEvent
    : DomainEvent<User, string>
{

    protected UserLoggedInEvent() { }

    public UserLoggedInEvent(string id) : base(id) { }

}
