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

public class User
    : AggregateRoot<string>
{

    public User() { }

    /// In real use-case, the constructor for the deprecated event should be removed
    public User(string firstName, string lastName, string email)
        : base(Guid.NewGuid().ToString("N")[..15])
    {
        this.On(this.RegisterEvent(new UserCreatedEvent(this.Id, firstName, lastName, email)));
    }

    public User(string firstName, string lastName, string email, Address address)
        : base(Guid.NewGuid().ToString("N")[..15])
    {
        this.On(this.RegisterEvent(new UserCreatedEventV2(this.Id, firstName, lastName, email, address)));
    }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public bool EmailVerified { get; set; }

    public bool IsLoggedIn { get; set; }

    public DateTimeOffset? LastOnline { get; set; }

    public Address? Address { get; set; }

    public string? PhoneNumber { get; set; }

    public bool IsV1 { get; set; }

    public bool IsV2 { get; set; }

    public bool IsV3 { get; set; }

    public void VerifyEmail()
    {
        this.On(this.RegisterEvent(new UserEmailValidatedEvent(this.Id)));
    }

    public void LogIn()
    {
        this.On(this.RegisterEvent(new UserLoggedInEvent(this.Id)));
    }

    public void LogOut()
    {
        this.On(this.RegisterEvent(new UserLoggedOutEvent(this.Id)));
    }

    /// In real use-case, the reducer for the deprecated event should be removed
    void On(UserCreatedEvent e)
    {
        this.CreatedAt = e.CreatedAt;
        this.FirstName = e.FirstName;
        this.LastName = e.LastName;
        this.Email = e.Email;
        this.IsV1 = true;
    }

    void On(UserCreatedEventV2 e)
    {
        this.CreatedAt = e.CreatedAt;
        this.FirstName = e.FirstName;
        this.LastName = e.LastName;
        this.Email = e.Email;
        this.Address = e.Address;
        this.IsV2 = true;
    }

#pragma warning disable IDE0051 // Remove unused private members
    void On(UserCreatedEventV3 e)
#pragma warning restore IDE0051 // Remove unused private members
    {
        this.CreatedAt = e.CreatedAt;
        this.FirstName = e.FirstName;
        this.LastName = e.LastName;
        this.Email = e.Email;
        this.Address = e.Address;
        this.PhoneNumber = e.PhoneNumber;
        this.IsV3 = true;
    }

    void On(UserEmailValidatedEvent e)
    {
        this.LastModified = e.CreatedAt;
        this.EmailVerified = true;
    }

    void On(UserLoggedInEvent e)
    {
        this.LastModified = e.CreatedAt;
        this.IsLoggedIn = true;
    }

    void On(UserLoggedOutEvent e)
    {
        this.LastModified = e.CreatedAt;
        this.IsLoggedIn = false;
        this.LastOnline = e.CreatedAt;
    }

    internal static User Create() => new("John", "Doe", "john.doe@email.com");

}
