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
using Neuroglia.Data.Infrastructure.EventSourcing;
using Neuroglia.UnitTests.Data.Events;

namespace Neuroglia.UnitTests.Data;

public class User
    : RestorableAggregateRoot<string, UserStateV1>,
    ISnapshotable<Snapshot<UserStateV1>>,
    IRestorable<Snapshot<UserStateV1>>,
    IVersionedState
{
    public ulong StateVersion { get => this.State.StateVersion; set => this.State.StateVersion = value; }

    public User() { }

    /// In real use-case, the constructor for the deprecated event should be removed
    public User(string firstName, string lastName, string email)
    {
        this.State.On(this.RegisterEvent(new UserCreatedEvent(Guid.NewGuid().ToString("N")[..15], firstName, lastName, email)));
    }

    public User(string firstName, string lastName, string email, Address address)
    {
        this.State.On(this.RegisterEvent(new UserCreatedEventV2(Guid.NewGuid().ToString("N")[..15], firstName, lastName, email, address)));
    }

    public void VerifyEmail() => this.State.On(this.RegisterEvent(new UserEmailValidatedEvent(this.Id)));

    public void LogIn() => this.State.On(this.RegisterEvent(new UserLoggedInEvent(this.Id)));

    public void LogOut() => this.State.On(this.RegisterEvent(new UserLoggedOutEvent(this.Id)));

    public Snapshot<UserStateV1> CreateSnapshot() => new(this.State, this.State.StateVersion);

    ISnapshot ISnapshotable.CreateSnapshot() => this.CreateSnapshot();

    internal static User Create() => new("John", "Doe", "john.doe@email.com");

    public void Restore(Snapshot<UserStateV1> snapshot)
    {
        this.State = snapshot.State;
    }

}
