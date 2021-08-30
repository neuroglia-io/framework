using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
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

        [JsonPatchOperation(OperationType.Replace, nameof(FirstName))]
        public virtual bool SetFirstName(string firstName)
        {
            if (this.FirstName == firstName)
                return false;
            this.On(this.RegisterEvent(new TestPersonFirstNameChangedDomainEvent(this.Id, firstName)));
            return true;
        }

        [JsonPatchOperation(OperationType.Replace, nameof(LastName))]
        public virtual bool SetLastName(string lastName)
        {
            if (this.LastName == lastName)
                return false;
            this.On(this.RegisterEvent(new TestPersonLastNameChangedDomainEvent(this.Id, lastName)));
            return true;
        }

        protected void On(TestPersonCreatedDomainEvent e)
        {
            this.Id = e.AggregateId;
            this.CreatedAt = e.CreatedAt;
            this.LastModified = e.CreatedAt;
            this.FirstName = e.FirstName;
            this.LastName = e.LastName;
        }

        protected void On(TestPersonFirstNameChangedDomainEvent e)
        {
            this.LastModified = DateTimeOffset.Now;
            this.FirstName = e.FirstName;
        }

        protected void On(TestPersonLastNameChangedDomainEvent e)
        {
            this.LastModified = DateTimeOffset.Now;
            this.LastName = e.LastName;
        }

    }

}
