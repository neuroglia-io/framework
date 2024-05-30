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
using System.Text.Json.Serialization;

namespace Neuroglia.UnitTests.Data;

public record UserStateV1
    : AggregateState<string>
{

    [JsonInclude]
    public string FirstName { get; set; } = null!;

    [JsonInclude]
    public string LastName { get; set; } = null!;

    [JsonInclude]
    public string Email { get; set; } = null!;

    [JsonInclude]
    public bool EmailVerified { get; protected set; }

    [JsonInclude]
    public bool IsLoggedIn { get; protected set; }

    [JsonInclude]
    public DateTimeOffset? LastOnline { get; protected set; }

    [JsonInclude]
    public Address? Address { get; protected set; }

    [JsonInclude]
    public string? PhoneNumber { get; protected set; }

    [JsonInclude]
    public bool IsV1 { get; protected set; }

    [JsonInclude]
    public bool IsV2 { get; protected set; }

    [JsonInclude]
    public bool IsV3 { get; protected set; }

    /// In real use-case, the reducer for the deprecated event should be removed
    internal void On(UserCreatedEvent e)
    {
        this.Id = e.AggregateId;
        this.CreatedAt = e.CreatedAt;
        this.FirstName = e.FirstName;
        this.LastName = e.LastName;
        this.Email = e.Email;
        this.IsV1 = true;
    }

    internal void On(UserCreatedEventV2 e)
    {
        this.Id = e.AggregateId;
        this.CreatedAt = e.CreatedAt;
        this.FirstName = e.FirstName;
        this.LastName = e.LastName;
        this.Email = e.Email;
        this.Address = e.Address;
        this.IsV2 = true;
    }

    internal void On(UserCreatedEventV3 e)
    {
        this.Id = e.AggregateId;
        this.CreatedAt = e.CreatedAt;
        this.FirstName = e.FirstName;
        this.LastName = e.LastName;
        this.Email = e.Email;
        this.Address = e.Address;
        this.PhoneNumber = e.PhoneNumber;
        this.IsV3 = true;
    }

    internal void On(UserEmailValidatedEvent e)
    {
        this.LastModified = e.CreatedAt;
        this.EmailVerified = true;
    }

    internal void On(UserLoggedInEvent e)
    {
        this.LastModified = e.CreatedAt;
        this.IsLoggedIn = true;
    }

    internal void On(UserLoggedOutEvent e)
    {
        this.LastModified = e.CreatedAt;
        this.IsLoggedIn = false;
        this.LastOnline = e.CreatedAt;
    }

}