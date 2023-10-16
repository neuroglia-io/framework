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
using System.Net;

namespace Neuroglia.UnitTests.Commands;

[PipelineMiddleware(typeof(GenericTestPipelineMiddleware<,>))]
public class TestCommandWithGenericMiddleware
    : Command<Person>
{

    public TestCommandWithGenericMiddleware(Person person)
    {
        this.Person = person;
    }

    public Person Person { get; }

}

public class TestCommandWithGenericMiddlewareHandler
    : ICommandHandler<TestCommandWithGenericMiddleware, Person>
{

    public Task<IOperationResult<Person>> HandleAsync(TestCommandWithGenericMiddleware request, CancellationToken cancellationToken = default) => Task.FromResult(this.Invalid());

}

public class GenericTestPipelineMiddleware<TRequest, TResult>
    : IMiddleware<TRequest, TResult>
    where TRequest : IRequest<TResult>
    where TResult : IOperationResult
{

    public Task<TResult> HandleAsync(TRequest request, RequestHandlerDelegate<TResult> next, CancellationToken cancellationToken = default)
    {

        return Task.FromResult(this.CreateResult((int)HttpStatusCode.OK));
    }

    protected virtual TResult CreateResult(int resultCode)
    {
        Type responseType;
        if (typeof(TResult).IsGenericType) responseType = typeof(OperationResult<>).MakeGenericType(typeof(TResult).GetGenericArguments().First());
        else responseType = typeof(OperationResult);
        return (TResult)Activator.CreateInstance(responseType, resultCode, Array.Empty<Error>())!;
    }

}
