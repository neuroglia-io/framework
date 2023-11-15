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
using Neuroglia.Mediation;

namespace Neuroglia.UnitTests.Data.Events;

public class PersonCreatedDomainEvent
    : DomainEvent<Person, Guid>
{

    protected PersonCreatedDomainEvent() { }

    public PersonCreatedDomainEvent(Guid aggregateId, string firstName, string lastName)
        : base(aggregateId)
    {
        this.FirstName = firstName;
        this.LastName = lastName;
    }

    public string FirstName { get; protected set; } = null!;

    public string LastName { get; protected set; } = null!;

}

public class PersonCreatedDomainEventHandler
    : INotificationHandler<PersonCreatedDomainEvent>
{

    static readonly TaskCompletionSource<PersonCreatedDomainEvent> WaitForEventCompletionSource = new();

    public static Task<PersonCreatedDomainEvent> WaitForEventAsync() => WaitForEventCompletionSource.Task;

    public Task HandleAsync(PersonCreatedDomainEvent e, CancellationToken cancellationToken = default)
    {
        if (!WaitForEventCompletionSource.Task.IsCompleted) WaitForEventCompletionSource.SetResult(e);
        return Task.CompletedTask;
    }

}

public class PersonCreatedDomainEventHandler1
    : INotificationHandler<PersonCreatedDomainEvent>
{

    public Task HandleAsync(PersonCreatedDomainEvent e, CancellationToken cancellationToken = default) => Task.CompletedTask;

}

public class PersonCreatedDomainEventHandler2
    : INotificationHandler<PersonCreatedDomainEvent>
{

    public Task HandleAsync(PersonCreatedDomainEvent e, CancellationToken cancellationToken = default) => Task.CompletedTask;

}
