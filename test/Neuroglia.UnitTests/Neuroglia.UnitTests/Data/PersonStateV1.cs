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

using Neuroglia.Data;
using Neuroglia.UnitTests.Data.Events;

namespace Neuroglia.UnitTests.Data;

public record PersonStateV1
    : AggregateState<Guid>
{

    public PersonStateV1() { }

    public virtual string FirstName { get; internal protected set; } = null!;

    public virtual string LastName { get; internal protected set; } = null!;

    public virtual List<string> Addresses { get; } = new();

    public virtual List<Guid> FriendsIds { get; } = new();

    public virtual DateTime Birthday { get; set; }

    public virtual List<Contact> Contacts { get; protected set; } = new();

    public virtual List<PersonTrait> Traits { get; protected set; } = new();

    internal void On(PersonCreatedDomainEvent e)
    {
        this.Id = e.AggregateId;
        this.CreatedAt = e.CreatedAt;
        this.LastModified = e.CreatedAt;
        this.FirstName = e.FirstName;
        this.LastName = e.LastName;
    }

    internal void On(PersonFirstNameChangedDomainEvent e)
    {
        this.LastModified = DateTimeOffset.Now;
        this.FirstName = e.FirstName;
    }

    internal void On(PersonLastNameChangedDomainEvent e)
    {
        this.LastModified = DateTimeOffset.Now;
        this.LastName = e.LastName;
    }

}