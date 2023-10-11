using Json.Patch;
using Neuroglia.Data;
using Neuroglia.Data.PatchModel.Attributes;
using Neuroglia.UnitTests.Data.Events;

namespace Neuroglia.UnitTests.Data;

public class Person
    : AggregateRoot<Guid>
{
    protected Person() : base(Guid.NewGuid()) { }

    public Person(Guid id, string firstName, string lastName)
        : base(id)
    {
        this.On(this.RegisterEvent(new PersonCreatedDomainEvent(this.Id, firstName, lastName)));
    }

    public Person(string firstName, string lastName) : this(Guid.NewGuid(), firstName, lastName) { }

    public virtual string FirstName { get; internal protected set; } = null!;

    public virtual string LastName { get; internal protected set; } = null!;

    public virtual List<string> Addresses { get; } = new();

    public virtual List<Guid> FriendsIds { get; } = new();

    public virtual DateTime Birthday { get; protected set; }

    public virtual List<Contact> Contacts { get; protected set; } = new();

    public virtual List<PersonTrait> Traits { get; protected set; } = new();

    [JsonPatchOperation(OperationType.Replace, nameof(FirstName))]
    public virtual bool SetFirstName(string firstName)
    {
        if (this.FirstName == firstName) return false;
        this.On(this.RegisterEvent(new PersonFirstNameChangedDomainEvent(this.Id, firstName)));
        return true;
    }

    [JsonPatchOperation(OperationType.Replace, nameof(LastName))]
    public virtual bool SetLastName(string lastName)
    {
        if (this.LastName == lastName) return false;
        this.On(this.RegisterEvent(new PersonLastNameChangedDomainEvent(this.Id, lastName)));
        return true;
    }

    [JsonPatchOperation(OperationType.Add, nameof(Addresses))]
    public virtual void AddAddress(string address)
    {
        this.Addresses.Add(address);
    }

    [JsonPatchOperation(OperationType.Add, nameof(FriendsIds), ReferencedType = typeof(Person))]
    public virtual void AddFriend(Person friend)
    {
        this.FriendsIds.Add(friend.Id);
    }

    [JsonPatchOperation(OperationType.Remove, nameof(FriendsIds), ReferencedType = typeof(Person))]
    public virtual void RemoveFriend(Person friend)
    {
        this.FriendsIds.Remove(friend.Id);
    }

    [JsonPatchOperation(OperationType.Replace, nameof(Birthday))]
    public virtual void SetBirthday(DateTime birthDay)
    {
        this.Birthday= birthDay;
    }

    [JsonPatchOperation(OperationType.Add, nameof(Contacts))]
    public virtual void AddContact(Contact contact)
    {
        this.Contacts.Add(contact);
    }

    [JsonPatchOperation(OperationType.Replace, nameof(Contacts) + "/" + nameof(Contact.Tel))]
    public virtual void UpdateContactTelephoneNumber(Guid contactId, string tel)
    {
        var contact = this.Contacts.First(c => c.Id == contactId);
        contact.Tel = tel;
    }

    [JsonPatchOperation(OperationType.Replace, nameof(Traits) + "/" + nameof(PersonTrait.Name))]
    public virtual void SetPreferenceName(Guid id, string name)
    {
        var trait = this.Traits.First(t => t.Id == id);
        trait.Name = name;
    }

    [JsonPatchOperation(OperationType.Replace, nameof(Traits) + "/" + nameof(PersonTrait.Value))]
    public virtual void SetPreferenceValue(Guid id, decimal value)
    {
        var trait = this.Traits.First(t => t.Id == id);
        trait.Value = value;
    }

    protected void On(PersonCreatedDomainEvent e)
    {
        this.Id = e.AggregateId;
        this.CreatedAt = e.CreatedAt;
        this.LastModified = e.CreatedAt;
        this.FirstName = e.FirstName;
        this.LastName = e.LastName;
    }

    protected void On(PersonFirstNameChangedDomainEvent e)
    {
        this.LastModified = DateTimeOffset.Now;
        this.FirstName = e.FirstName;
    }

    protected void On(PersonLastNameChangedDomainEvent e)
    {
        this.LastModified = DateTimeOffset.Now;
        this.LastName = e.LastName;
    }

}