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

using Neuroglia.Data.Infrastructure.EventSourcing;

namespace Neuroglia.UnitTests.Services;

internal static class EventStreamFactory
{

    internal static IEnumerable<IEventDescriptor> Create()
    {
        var userId = Guid.NewGuid();
        yield return new EventDescriptor("user-created", new UserCreatedEvent(userId, "John", "Doe", "john.doe@email.com"));
        yield return new EventDescriptor("user-email-confirmed", new UserEmailConfirmedEvent(userId));
        yield return new EventDescriptor("user-logged-in", new UserLoggedInEvent(userId));
        yield return new EventDescriptor("user-logged-out", new UserLoggedOutEvent(userId));
    }

    class UserCreatedEvent
    {


        protected UserCreatedEvent() { }

        public UserCreatedEvent(Guid id, string firstName, string lastName, string email)
        {
            this.Id = id;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Email = email;
        }

        public Guid Id { get; protected set; }

        public string FirstName { get; protected set; } = null!;

        public string LastName { get; protected set; } = null!;

        public string Email { get; protected set; } = null!;

    }

    class UserEmailConfirmedEvent
    {

        public UserEmailConfirmedEvent(Guid id)
        {
            this.Id = id;
        }
        public Guid Id { get; protected set; }

    }

    class UserLoggedInEvent
    {

        public UserLoggedInEvent(Guid id)
        {
            this.Id = id;
        }
        public Guid Id { get; protected set; }

    }

    class UserLoggedOutEvent
    {

        public UserLoggedOutEvent(Guid id)
        {
            this.Id = id;
        }
        public Guid Id { get; protected set; }

    }

}