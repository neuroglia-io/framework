using Neuroglia.Mediation;
using Neuroglia.UnitTests.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Neuroglia.UnitTests.Commands
{

    public class TestCommandWithReturnData
        : Command<TestPerson>
    {

        public TestCommandWithReturnData(TestPerson person)
        {
            this.Person = person;
        }

        public TestPerson Person { get; }

    }

    public class TestCommandWithReturnDataHandler
        : ICommandHandler<TestCommandWithReturnData, TestPerson>
    {

        public Task<IOperationResult<TestPerson>> HandleAsync(TestCommandWithReturnData request, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(this.Ok(request.Person));
        }

    }

}
