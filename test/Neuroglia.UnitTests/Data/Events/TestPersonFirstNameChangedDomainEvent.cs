using Neuroglia.Data;
using System;

namespace Neuroglia.UnitTests.Data.Events
{
    public class TestPersonFirstNameChangedDomainEvent
        : DomainEvent<TestPerson, Guid>
    {

        protected TestPersonFirstNameChangedDomainEvent()
        {

        }

        public TestPersonFirstNameChangedDomainEvent(Guid personId, string firstName)
            : base(personId)
        {
            this.FirstName = firstName;
        }

        public string FirstName { get; protected set; }

    }

}
