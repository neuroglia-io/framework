using Neuroglia.Mediation;

namespace Neuroglia.UnitTests.Commands;

public class TestCommandWithMultipleHandlers
    : Command
{



}

public class TestCommandWithMultipleHandlersHandler1
    : ICommandHandler<TestCommandWithMultipleHandlers>
{

    public Task<IOperationResult> HandleAsync(TestCommandWithMultipleHandlers request, CancellationToken cancellationToken = default) => Task.FromResult(this.Ok());

}

public class TestCommandWithMultipleHandlersHandler2
    : ICommandHandler<TestCommandWithMultipleHandlers>
{

    public Task<IOperationResult> HandleAsync(TestCommandWithMultipleHandlers request, CancellationToken cancellationToken = default) => Task.FromResult(this.Ok());

}
