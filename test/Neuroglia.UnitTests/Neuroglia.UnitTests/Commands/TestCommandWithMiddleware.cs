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
        result.Data!.FirstName = $"Updated {request.Person.FirstName}";
        result.Data.LastName = $"Updated {request.Person.LastName}";
        return result;
    }

}
