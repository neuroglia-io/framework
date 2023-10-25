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

using Neuroglia.Mediation;

namespace Neuroglia.UnitTests.Commands;

[PipelineMiddleware(typeof(TestPipelineMiddleware))]
public class TestCommandWithMiddleware
    : Command<Person>
{

    public TestCommandWithMiddleware(Person person)
    {
        this.Person = person;
    }

    public Person Person { get; }

}

public class TestCommandWithMiddlewareHandler
    : ICommandHandler<TestCommandWithMiddleware, Person>
{

    public Task<IOperationResult<Person>> HandleAsync(TestCommandWithMiddleware request, CancellationToken cancellationToken = default) => Task.FromResult(this.Ok(request.Person));

}

public class TestPipelineMiddleware
    : IMiddleware<TestCommandWithMiddleware, IOperationResult<Person>>
{

    public async Task<IOperationResult<Person>> HandleAsync(TestCommandWithMiddleware request, RequestHandlerDelegate<IOperationResult<Person>> next, CancellationToken cancellationToken = default)
    {
        var result = (await next());
        result.Data!.State.FirstName = $"Updated {request.Person.State.FirstName}";
        result.Data.State.LastName = $"Updated {request.Person.State.LastName}";
        return result;
    }

}
