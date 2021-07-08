using Neuroglia.Mediation;
using System.Threading;
using System.Threading.Tasks;

namespace Neuroglia.UnitTests.Commands
{

    public class TestCommandWithMultipleHandlers
        : Command
    {



    }

    public class TestCommandWithMultipleHandlersHandler1
        : ICommandHandler<TestCommandWithMultipleHandlers>
    {

        public Task<IOperationResult> HandleAsync(TestCommandWithMultipleHandlers request, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(this.Ok());
        }

    }

    public class TestCommandWithMultipleHandlersHandler2
        : ICommandHandler<TestCommandWithMultipleHandlers>
    {

        public Task<IOperationResult> HandleAsync(TestCommandWithMultipleHandlers request, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(this.Ok());
        }

    }

}
