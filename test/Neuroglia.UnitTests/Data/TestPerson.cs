using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Neuroglia.Data;
using Neuroglia.UnitTests.Data.Events;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public virtual List<string> Addresses { get; } = new();

        public virtual List<Guid> FriendsIds { get; } = new();

        public virtual DateTime Birthday { get; protected set; }

        public virtual List<TestContact> Contacts { get; protected set; } = new();

        public virtual List<TestPersonTrait> Traits { get; protected set; } = new();

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

        [JsonPatchOperation(OperationType.Add, nameof(Addresses))]
        public virtual void AddAddress(string address)
        {
            this.Addresses.Add(address);
        }

        [JsonPatchOperation(OperationType.Add, nameof(FriendsIds), ReferencedType = typeof(TestPerson))]
        public virtual void AddFriend(TestPerson friend)
        {
            this.FriendsIds.Add(friend.Id);
        }

        [JsonPatchOperation(OperationType.Remove, nameof(FriendsIds), ReferencedType = typeof(TestPerson))]
        public virtual void RemoveFriend(TestPerson friend)
        {
            this.FriendsIds.Remove(friend.Id);
        }

        [JsonPatchOperation(OperationType.Replace, nameof(Birthday))]
        public virtual void SetBirthday(DateTime birthDay)
        {
            this.Birthday= birthDay;
        }

        [JsonPatchOperation(OperationType.Add, nameof(Contacts))]
        public virtual void AddContact(TestContact contact)
        {
            this.Contacts.Add(contact);
        }

        [JsonPatchOperation(OperationType.Replace, nameof(Contacts) + "/" + nameof(TestContact.Tel))]
        public virtual void UpdateContactTelephoneNumber(int at, string tel)
        {
            TestContact contact = this.Contacts.ElementAt(at);
            contact.Tel = tel;
        }

        [JsonPatchOperation(OperationType.Replace, nameof(Traits) + "/" + nameof(TestPersonTrait.Name))]
        public virtual void SetPreferenceName(Guid id, string name)
        {
            TestPersonTrait trait = this.Traits.First(t => t.Id == id);
            trait.Name = name;
        }

        [JsonPatchOperation(OperationType.Replace, nameof(Traits) + "/" + nameof(TestPersonTrait.Value))]
        public virtual void SetPreferenceValue(Guid id, decimal value)
        {
            TestPersonTrait trait = this.Traits.First(t => t.Id == id);
            trait.Value = value;
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

    public class TestPersonTrait
        : Entity<Guid>
    {

        public TestPersonTrait()
            : base(Guid.NewGuid())
        {

        }

        public virtual string Name { get; set; }

        public virtual decimal Value { get; set; }

    }

}
