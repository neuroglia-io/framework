using Neuroglia.Data;
using Neuroglia.UnitTests.Data.Events;
using System;

namespace Neuroglia.UnitTests.Data
{

    public class TestPerson
        : AggregateRoot<Guid>
    {

        protected TestPerson()
            : base(Guid.NewGuid())
        {

        }

        public TestPerson(string firstName, string lastName)
          : this()
        {
            this.On(this.RegisterEvent(new TestPersonCreatedDomainEvent(this.Id, firstName, lastName)));
        }

        public virtual string FirstName { get; internal protected set; }

        public virtual string LastName { get; internal protected set; }

        protected void On(TestPersonCreatedDomainEvent e)
        {
            this.Id = e.AggregateId;
            this.CreatedAt = e.CreatedAt;
            this.LastModified = e.CreatedAt;
            this.FirstName = e.FirstName;
            this.LastName = e.LastName;
        }

    }

}
