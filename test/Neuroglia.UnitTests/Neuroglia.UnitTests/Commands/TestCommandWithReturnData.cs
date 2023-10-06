using Neuroglia.Mediation;

namespace Neuroglia.UnitTests.Commands;

public class TestCommandWithReturnData
    : Command<Person>
{

    public TestCommandWithReturnData(Person person)
    {
        this.Person = person;
    }

    public Person Person { get; }

}

public class TestCommandWithReturnDataHandler
    : ICommandHandler<TestCommandWithReturnData, Person>
{

    public Task<IOperationResult<Person>> HandleAsync(TestCommandWithReturnData request, CancellationToken cancellationToken = default) => Task.FromResult(this.Ok(request.Person));

}
