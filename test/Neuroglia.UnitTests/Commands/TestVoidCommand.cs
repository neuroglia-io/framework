using Neuroglia.Mediation;
using System.Threading;
using System.Threading.Tasks;

namespace Neuroglia.UnitTests.Commands
{

    public class TestVoidCommand
        : Command
    {

        

    }

    public class TestVoidCommandHandler
        : ICommandHandler<TestVoidCommand>
    {

        public Task<IOperationResult> HandleAsync(TestVoidCommand request, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(this.Ok());
        }

    }

}
