using Neuroglia.Data;
using System;

namespace Neuroglia.UnitTests.Data.Events
{
    public class TestPersonLastNameChangedDomainEvent
        : DomainEvent<TestPerson, Guid>
    {

        protected TestPersonLastNameChangedDomainEvent()
        {

        }

        public TestPersonLastNameChangedDomainEvent(Guid personId, string lastName)
            : base(personId)
        {
            this.LastName = lastName;
        }

        public string LastName { get; protected set; }

    }

}
