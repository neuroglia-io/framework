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
using Neuroglia.Data.Guards;
using Neuroglia.Mediation;
using Neuroglia.Mediation.Services;

namespace Neuroglia.UnitTests.Commands;

[PipelineMiddleware(typeof(GuardExceptionHandlingMiddleware<,>))]
[PipelineMiddleware(typeof(FluentValidationMiddleware<,>))]
public class TestCommandWithGuardExceptionHandlingMiddleware(Person person)
        : Command
{
    public Person Person { get; } = person;

}

public class TestCommandWithGuardExceptionHandlingMiddlewareValidator
    : AbstractValidator<TestCommandWithGuardExceptionHandlingMiddleware>
{

    public TestCommandWithGuardExceptionHandlingMiddlewareValidator() => this.RuleFor(c => c.Person).NotNull();

}

public class TestCommandWithGuardExceptionHandlingMiddlewareHandler
    : ICommandHandler<TestCommandWithGuardExceptionHandlingMiddleware>
{

    public Task<IOperationResult> HandleAsync(TestCommandWithGuardExceptionHandlingMiddleware request, CancellationToken cancellationToken = default) => Task.FromResult(Guard.Against<IOperationResult>(null).WhenNull().Value!);

}
