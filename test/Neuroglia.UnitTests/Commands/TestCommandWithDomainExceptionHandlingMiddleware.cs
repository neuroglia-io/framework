using FluentValidation;
using Neuroglia.Data;
using Neuroglia.Mediation;
using Neuroglia.UnitTests.Data;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Neuroglia.UnitTests.Commands
{

    [PipelineMiddleware(typeof(DomainExceptionHandlingMiddleware<,>))]
    [PipelineMiddleware(typeof(FluentValidationMiddleware<,>))]
    public class TestCommandWithDomainExceptionHandlingMiddleware
        : Command
    {

        public TestCommandWithDomainExceptionHandlingMiddleware(TestPerson person)
        {
            this.Person = person;
        }

        public TestPerson Person { get; }

    }

    public class TestCommandWithDomainExceptionHandlingMiddlewareValidator
        : AbstractValidator<TestCommandWithDomainExceptionHandlingMiddleware>
    {

        public TestCommandWithDomainExceptionHandlingMiddlewareValidator()
        {
            this.RuleFor(c => c.Person)
                .NotNull();
        }

    }

    public class TestCommandWithDomainExceptionHandlingMiddlewareHandler
        : ICommandHandler<TestCommandWithDomainExceptionHandlingMiddleware>
    {

        public Task<IOperationResult> HandleAsync(TestCommandWithDomainExceptionHandlingMiddleware request, CancellationToken cancellationToken = default)
        {
            throw DomainException.NullReference(typeof(object), Guid.NewGuid().ToString());
        }

    }
}
