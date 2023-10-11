using Neuroglia.Data;
using Neuroglia.Mediation;

namespace Neuroglia.UnitTests.Data.Events;

public class PersonCreatedDomainEvent
    : DomainEvent<Person, Guid>
{

    protected PersonCreatedDomainEvent() { }

    public PersonCreatedDomainEvent(Guid personId, string firstName, string lastName)
        : base(personId)
    {
        this.FirstName = firstName;
        this.LastName = lastName;
    }

    public string FirstName { get; protected set; } = null!;

    public string LastName { get; protected set; } = null!;

}

public class PersonCreatedDomainEventHandler
    : INotificationHandler<PersonCreatedDomainEvent>
{

    static readonly TaskCompletionSource<PersonCreatedDomainEvent> WaitForEventCompletionSource = new();

    public static Task<PersonCreatedDomainEvent> WaitForEventAsync() => WaitForEventCompletionSource.Task;

    public Task HandleAsync(PersonCreatedDomainEvent e, CancellationToken cancellationToken = default)
    {
        if (!WaitForEventCompletionSource.Task.IsCompleted) WaitForEventCompletionSource.SetResult(e);
        return Task.CompletedTask;
    }

}

public class PersonCreatedDomainEventHandler1
    : INotificationHandler<PersonCreatedDomainEvent>
{

    public Task HandleAsync(PersonCreatedDomainEvent e, CancellationToken cancellationToken = default) => Task.CompletedTask;

}

public class PersonCreatedDomainEventHandler2
    : INotificationHandler<PersonCreatedDomainEvent>
{

    public Task HandleAsync(PersonCreatedDomainEvent e, CancellationToken cancellationToken = default) => Task.CompletedTask;

}
