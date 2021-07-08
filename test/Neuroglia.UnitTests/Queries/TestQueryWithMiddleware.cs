using Neuroglia.Mediation;
using Neuroglia.UnitTests.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Neuroglia.UnitTests.Queries
{

    [PipelineMiddleware(typeof(TestPipelineMiddleware))]
    public class TestQueryWithMiddleware
         : Query<TestPerson>
    {

        public TestQueryWithMiddleware(TestPerson person)
        {
            this.Person = person;
        }

        public TestPerson Person { get; }

    }

    public class TestCommandWithMiddlewareHandler
        : IQueryHandler<TestQueryWithMiddleware, TestPerson>
    {

        public Task<IOperationResult<TestPerson>> HandleAsync(TestQueryWithMiddleware request, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(this.Ok(request.Person));
        }

    }

    public class TestPipelineMiddleware
        : IMiddleware<TestQueryWithMiddleware, IOperationResult<TestPerson>>
    {

        public async Task<IOperationResult<TestPerson>> HandleAsync(TestQueryWithMiddleware request, RequestHandlerDelegate<IOperationResult<TestPerson>> next, CancellationToken cancellationToken = default)
        {
            var result = (await next());
            result.Data.FirstName = $"Updated {request.Person.FirstName}";
            result.Data.LastName = $"Updated {request.Person.LastName}";
            return result;
        }

    }

}
