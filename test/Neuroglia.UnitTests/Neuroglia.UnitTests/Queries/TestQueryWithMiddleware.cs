using Neuroglia.Mediation;

namespace Neuroglia.UnitTests.Queries;

[PipelineMiddleware(typeof(TestPipelineMiddleware))]
public class TestQueryWithMiddleware
     : Query<Person>
{

    public TestQueryWithMiddleware(Person person) => this.Person = person;

    public Person Person { get; }

}

public class TestQueryWithMiddlewareHandler
    : IQueryHandler<TestQueryWithMiddleware, Person>
{

    public Task<IOperationResult<Person>> HandleAsync(TestQueryWithMiddleware request, CancellationToken cancellationToken = default) => Task.FromResult(this.Ok(request.Person));

}

public class TestPipelineMiddleware
    : IMiddleware<TestQueryWithMiddleware, IOperationResult<Person>>
{

    public async Task<IOperationResult<Person>> HandleAsync(TestQueryWithMiddleware request, RequestHandlerDelegate<IOperationResult<Person>> next, CancellationToken cancellationToken = default)
    {
        var result = (await next());
        result.Data!.FirstName = $"Updated {request.Person.FirstName}";
        result.Data.LastName = $"Updated {request.Person.LastName}";
        return result;
    }

}
