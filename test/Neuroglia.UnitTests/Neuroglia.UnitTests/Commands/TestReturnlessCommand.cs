using Neuroglia.Mediation;

namespace Neuroglia.UnitTests.Commands;

public class TestReturnlessCommand
    : Command
{

    

}

public class TestReturnlessCommandHandler
    : ICommandHandler<TestReturnlessCommand>
{

    public Task<IOperationResult> HandleAsync(TestReturnlessCommand request, CancellationToken cancellationToken = default) => Task.FromResult(this.Ok());

}
