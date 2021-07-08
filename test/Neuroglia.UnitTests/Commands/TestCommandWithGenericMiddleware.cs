using Neuroglia.Mediation;
using Neuroglia.UnitTests.Data;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Neuroglia.UnitTests.Commands
{

    [PipelineMiddleware(typeof(GenericTestPipelineMiddleware<,>))]
    public class TestCommandWithGenericMiddleware
        : Command<TestPerson>
    {

        public TestCommandWithGenericMiddleware(TestPerson person)
        {
            this.Person = person;
        }

        public TestPerson Person { get; }

    }

    public class TestCommandWithGenericMiddlewareHandler
        : ICommandHandler<TestCommandWithGenericMiddleware, TestPerson>
    {

        public Task<IOperationResult<TestPerson>> HandleAsync(TestCommandWithGenericMiddleware request, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(this.Invalid());
        }

    }

    public class GenericTestPipelineMiddleware<TRequest, TResult>
        : IMiddleware<TRequest, TResult>
        where TRequest : IRequest<TResult>
        where TResult : IOperationResult
    {

        public Task<TResult> HandleAsync(TRequest request, RequestHandlerDelegate<TResult> next, CancellationToken cancellationToken = default)
        {

            return Task.FromResult(this.CreateResult(OperationResultCode.Ok));
        }

        protected virtual TResult CreateResult(OperationResultCode resultCode)
        {
            Type responseType;
            if (typeof(TResult).IsGenericType)
                responseType = typeof(OperationResult<>).MakeGenericType(typeof(TResult).GetGenericArguments().First());
            else
                responseType = typeof(OperationResult);
            return (TResult)Activator.CreateInstance(responseType, resultCode.ToString(), Array.Empty<Error>());
        }

    }

}
