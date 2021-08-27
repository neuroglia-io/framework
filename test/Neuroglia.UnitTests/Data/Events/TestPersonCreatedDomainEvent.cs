using Neuroglia.Data;
using Neuroglia.Mediation;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Neuroglia.UnitTests.Data.Events
{

    public class TestPersonCreatedDomainEvent
        : DomainEvent<TestPerson, Guid>
    {

        protected TestPersonCreatedDomainEvent()
        {

        }

        public TestPersonCreatedDomainEvent(Guid personId, string firstName, string lastName)
            : base(personId)
        {
            this.FirstName = firstName;
            this.LastName = lastName;
        }

        public string FirstName { get; protected set; }

        public string LastName { get; protected set; }

    }

    public class TestPersonCreatedDomainEventHandler
        : INotificationHandler<TestPersonCreatedDomainEvent>
    {

        static TaskCompletionSource<TestPersonCreatedDomainEvent> WaitForEventCompletionSource = new();

        public static Task<TestPersonCreatedDomainEvent> WaitForEventAsync()
        {
            return WaitForEventCompletionSource.Task;
        }

        public Task HandleAsync(TestPersonCreatedDomainEvent e, CancellationToken cancellationToken = default)
        {
            WaitForEventCompletionSource.SetResult(e);
            return Task.CompletedTask;
        }

    }

    public class TestPersonCreatedDomainEventHandler1
        : INotificationHandler<TestPersonCreatedDomainEvent>
    {

        public Task HandleAsync(TestPersonCreatedDomainEvent e, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

    }

    public class TestPersonCreatedDomainEventHandler2
        : INotificationHandler<TestPersonCreatedDomainEvent>
    {

        public Task HandleAsync(TestPersonCreatedDomainEvent e, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

    }

}
