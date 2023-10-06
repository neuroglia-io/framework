using Neuroglia.Data;
using Neuroglia.Mediation;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Neuroglia.UnitTests.Data.Events
{

    public class PersonCreatedDomainEvent
        : DomainEvent<Person, Guid>
    {

        protected PersonCreatedDomainEvent()
        {

        }

        public PersonCreatedDomainEvent(Guid personId, string firstName, string lastName)
            : base(personId)
        {
            this.FirstName = firstName;
            this.LastName = lastName;
        }

        public string FirstName { get; protected set; }

        public string LastName { get; protected set; }

    }

    public class PersonCreatedDomainEventHandler
        : INotificationHandler<PersonCreatedDomainEvent>
    {

        static TaskCompletionSource<PersonCreatedDomainEvent> WaitForEventCompletionSource = new();

        public static Task<PersonCreatedDomainEvent> WaitForEventAsync()
        {
            return WaitForEventCompletionSource.Task;
        }

        public Task HandleAsync(PersonCreatedDomainEvent e, CancellationToken cancellationToken = default)
        {
            if (!WaitForEventCompletionSource.Task.IsCompleted)
                WaitForEventCompletionSource.SetResult(e);
            return Task.CompletedTask;
        }

    }

    public class PersonCreatedDomainEventHandler1
        : INotificationHandler<PersonCreatedDomainEvent>
    {

        public Task HandleAsync(PersonCreatedDomainEvent e, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

    }

    public class PersonCreatedDomainEventHandler2
        : INotificationHandler<PersonCreatedDomainEvent>
    {

        public Task HandleAsync(PersonCreatedDomainEvent e, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

    }

}
