﻿// Copyright © 2021-Present Neuroglia SRL. All rights reserved.
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

namespace Neuroglia.UnitTests.Data.Events;

internal record UserCreatedEventV2
    : DomainEvent<User, string>
{

    protected UserCreatedEventV2() { }

    public UserCreatedEventV2(string aggregateId, string firstName, string lastName, string email, Address address)
        : base(aggregateId)
    {
        this.FirstName = firstName;
        this.LastName = lastName;
        this.Email = email;
        this.Address = address;
    }

    public string FirstName { get; protected set; } = null!;

    public string LastName { get; protected set; } = null!;

    public string Email { get; protected set; } = null!;

    public Address Address { get; protected set; } = null!;

}
