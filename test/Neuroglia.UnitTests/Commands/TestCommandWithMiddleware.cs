using Neuroglia.Mediation;
using Neuroglia.UnitTests.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Neuroglia.UnitTests.Commands
{

    [PipelineMiddleware(typeof(TestPipelineMiddleware))]
    public class TestCommandWithMiddleware
        : Command<TestPerson>
    {

        public TestCommandWithMiddleware(TestPerson person)
        {
            this.Person = person;
        }

        public TestPerson Person { get; }

    }

    public class TestCommandWithMiddlewareHandler
        : ICommandHandler<TestCommandWithMiddleware, TestPerson>
    {

        public Task<IOperationResult<TestPerson>> HandleAsync(TestCommandWithMiddleware request, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(this.Ok(request.Person));
        }

    }

    public class TestPipelineMiddleware
        : IMiddleware<TestCommandWithMiddleware, IOperationResult<TestPerson>>
    {

        public async Task<IOperationResult<TestPerson>> HandleAsync(TestCommandWithMiddleware request, RequestHandlerDelegate<IOperationResult<TestPerson>> next, CancellationToken cancellationToken = default)
        {
            var result = (await next());
            result.Data.FirstName = $"Updated {request.Person.FirstName}";
            result.Data.LastName = $"Updated {request.Person.LastName}";
            return result;
        }

    }

}
