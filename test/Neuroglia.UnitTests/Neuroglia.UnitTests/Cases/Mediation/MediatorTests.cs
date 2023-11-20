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

using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Neuroglia.Mediation;
using Neuroglia.UnitTests.Commands;
using Neuroglia.UnitTests.Data.Events;
using Neuroglia.UnitTests.Queries;
using System.Net;

namespace Neuroglia.UnitTests.Cases.Mediation;

public class MediatorTests
{

    public MediatorTests()
    {
        ServiceCollection services = new();
        services.AddValidatorsFromAssemblyContaining<MediatorTests>();
        services.AddMediator(builder => builder.ScanAssembly(typeof(MediatorTests).Assembly));
        this.Mediator = services.BuildServiceProvider().GetRequiredService<Mediator>();
    }

    protected Mediator Mediator { get; }

    [Fact]
    public async Task Command_NoReturnData_ShouldWork()
    {
        //arrange
        var command = new TestReturnlessCommand();

        //act
        var result = await this.Mediator.ExecuteAsync(command);

        //assert
        result.Should().NotBeNull();
        result.Status.Should().Be((int)HttpStatusCode.OK);
    }

    [Fact]
    public async Task Command_WithReturnData_ShouldWork()
    {
        //arrange
        var firstName = "Fake First Name";
        var lastName = "Fake Last Name";
        var person = new Person(firstName, lastName);
        var command = new TestCommandWithReturnData(person);

        //act
        var result = await this.Mediator.ExecuteAsync(command);

        //assert
        result.Should().NotBeNull();
        result.Status.Should().Be((int)HttpStatusCode.OK);
        result.Data.Should().NotBeNull();
        result.Data!.State.FirstName.Should().Be(firstName);
        result.Data.State.LastName.Should().Be(lastName);
    }

    [Fact]
    public async Task Command_NoHandler_ShouldFail()
    {
        //arrange
        var command = new TestHandlerlessCommand();

        //act
        await this.Mediator.Invoking(m => m.ExecuteAsync(command)).Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task Command_MultipleHandlers_ShouldFail()
    {
        //arrange
        var command = new TestCommandWithMultipleHandlers();

        //act
        await this.Mediator.Invoking(m => m.ExecuteAsync(command)).Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task Command_WithMiddleware_ShouldWork()
    {
        //arrange
        var firstName = "Fake First Name";
        var lastName = "Fake Last Name";
        var person = new Person(firstName, lastName);
        var command = new TestCommandWithMiddleware(person);

        //act
        var result = await this.Mediator.ExecuteAsync(command);

        //assert
        result.Should().NotBeNull();
        result.Status.Should().Be((int)HttpStatusCode.OK);
        result.Data.Should().NotBeNull();
        result.Data!.State.FirstName.Should().Be($"Updated {firstName}");
        result.Data.State.LastName.Should().Be($"Updated {lastName}");
    }

    [Fact]
    public async Task Command_WithGenericMiddleware_ShouldWork()
    {
        //arrange
        var firstName = "Fake First Name";
        var lastName = "Fake Last Name";
        var person = new Person(firstName, lastName);
        var command = new TestCommandWithGenericMiddleware(person);

        //act
        var result = await this.Mediator.ExecuteAsync(command);

        //assert
        result.Should().NotBeNull();
        result.Status.Should().Be((int)HttpStatusCode.OK);
    }

    [Fact]
    public async Task Command_WithGuardExceptionHandlingMiddleware_ShouldWork()
    {
        //arrange
        var firstName = "Fake First Name";
        var lastName = "Fake Last Name";
        var person = new Person(firstName, lastName);
        var command = new TestCommandWithGuardExceptionHandlingMiddleware(person);

        //act
        var result = await this.Mediator.ExecuteAsync(command);

        //arrange
        result.Should().NotBeNull();
        result.Status.Should().Be((int)HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Command_WithFluentValidationMiddleware_ShouldWork()
    {
        //arrange
        var command = new TestCommandWithGuardExceptionHandlingMiddleware(null!);

        //act
        var result = await this.Mediator.ExecuteAsync(command);

        //assert
        result.Should().NotBeNull();
        result.Status.Should().Be((int)HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Query_NoHandler_ShouldFail()
    {
        //arrange
        var query = new TestHandlerlessQuery();

        //act
        await this.Mediator.Invoking(m => m.ExecuteAsync(query)).Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task Query_WithPipeline_ShouldWork()
    {
        //arrange
        var firstName = "Fake First Name";
        var lastName = "Fake Last Name";
        var person = new Person(firstName, lastName);
        var command = new TestQueryWithMiddleware(person);

        //act
        var result = await this.Mediator.ExecuteAsync(command);

        //assert
        result.Should().NotBeNull();
        result.Status.Should().Be((int)HttpStatusCode.OK);
        result.Data.Should().NotBeNull();
        result.Data!.State.FirstName.Should().Be($"Updated {firstName}");
        result.Data.State.LastName.Should().Be($"Updated {lastName}");
    }

    [Fact]
    public async Task Notification_MultipleHandlers_ShouldWork()
    {
        //arrange
        var id = Guid.NewGuid();
        var firstName = "Fake First Name";
        var lastName = "Fake Last Name";
        var notification = new PersonCreatedDomainEvent(id, firstName, lastName);

        //act
        await this.Mediator.PublishAsync(notification);
        var e = await PersonCreatedDomainEventHandler.WaitForEventAsync();

        //assert
        e.Should().NotBeNull();
        e.AggregateId.Should().Be(id);
        e.FirstName.Should().Be(firstName);
        e.LastName.Should().Be(lastName);
    }

}