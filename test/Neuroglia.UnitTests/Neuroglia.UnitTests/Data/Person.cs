// Copyright © 2021-Present Neuroglia SRL. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License"),
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Json.Patch;
using Neuroglia.Data;
using Neuroglia.Data.PatchModel;
using Neuroglia.Data.PatchModel.Services;
using Neuroglia.UnitTests.Data.Events;

namespace Neuroglia.UnitTests.Data;

public class Person
    : AggregateRoot<Guid, PersonStateV1>
{
    protected Person() { }

    public Person(Guid id, string firstName, string lastName)
    {
        this.State.On(this.RegisterEvent(new PersonCreatedDomainEvent(id, firstName, lastName)));
    }

    public Person(string firstName, string lastName) : this(Guid.NewGuid(), firstName, lastName) { }

    [JsonPatchOperation(JsonPatchOperationType.Replace, nameof(PersonStateV1.FirstName))]
    public virtual bool SetFirstName(string firstName)
    {
        if (this.State.FirstName == firstName) return false;
        this.State.On(this.RegisterEvent(new PersonFirstNameChangedDomainEvent(this.Id, firstName)));
        return true;
    }

    [JsonPatchOperation(JsonPatchOperationType.Replace, nameof(PersonStateV1.LastName))]
    public virtual bool SetLastName(string lastName)
    {
        if (this.State.LastName == lastName) return false;
        this.State.On(this.RegisterEvent(new PersonLastNameChangedDomainEvent(this.Id, lastName)));
        return true;
    }

    [JsonPatchOperation(JsonPatchOperationType.Add, nameof(PersonStateV1.Addresses))]
    public virtual void AddAddress(string address)
    {
        this.State.Addresses.Add(address);
    }

    [JsonPatchOperation(JsonPatchOperationType.Add, nameof(PersonStateV1.FriendsIds), ReferencedType = typeof(Person))]
    public virtual void AddFriend(Person friend)
    {
        this.State.FriendsIds.Add(friend.Id);
    }

    [JsonPatchOperation(JsonPatchOperationType.Remove, nameof(PersonStateV1.FriendsIds), ReferencedType = typeof(Person))]
    public virtual void RemoveFriend(Person friend)
    {
        this.State.FriendsIds.Remove(friend.Id);
    }

    [JsonPatchOperation(JsonPatchOperationType.Replace, nameof(PersonStateV1.Birthday))]
    public virtual void SetBirthday(DateTime birthDay)
    {
        this.State.Birthday = birthDay;
    }

    [JsonPatchOperation(JsonPatchOperationType.Add, nameof(PersonStateV1.Contacts))]
    public virtual void AddContact(Contact contact)
    {
        this.State.Contacts.Add(contact);
    }

    [JsonPatchOperation(JsonPatchOperationType.Replace, nameof(PersonStateV1.Contacts) + "/" + nameof(Contact.Tel))]
    public virtual void UpdateContactTelephoneNumber(Guid contactId, string tel)
    {
        var contact = this.State.Contacts.First(c => c.Id == contactId);
        contact.Tel = tel;
    }

    [JsonPatchOperation(JsonPatchOperationType.Replace, nameof(PersonStateV1.Traits) + "/" + nameof(PersonTrait.Name))]
    public virtual void SetPreferenceName(Guid id, string name)
    {
        var trait = this.State.Traits.First(t => t.Id == id);
        trait.Name = name;
    }

    [JsonPatchOperation(JsonPatchOperationType.Replace, nameof(PersonStateV1.Traits) + "/" + nameof(PersonTrait.Value))]
    public virtual void SetPreferenceValue(Guid id, decimal value)
    {
        var trait = this.State.Traits.First(t => t.Id == id);
        trait.Value = value;
    }

}
