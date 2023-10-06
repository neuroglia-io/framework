using Neuroglia.Mediation;
using System.Net;

namespace Neuroglia.UnitTests.Commands;

[PipelineMiddleware(typeof(GenericTestPipelineMiddleware<,>))]
public class TestCommandWithGenericMiddleware
    : Command<Person>
{

    public TestCommandWithGenericMiddleware(Person person)
    {
        this.Person = person;
    }

    public Person Person { get; }

}

public class TestCommandWithGenericMiddlewareHandler
    : ICommandHandler<TestCommandWithGenericMiddleware, Person>
{

    public Task<IOperationResult<Person>> HandleAsync(TestCommandWithGenericMiddleware request, CancellationToken cancellationToken = default) => Task.FromResult(this.Invalid());

}

public class GenericTestPipelineMiddleware<TRequest, TResult>
    : IMiddleware<TRequest, TResult>
    where TRequest : IRequest<TResult>
    where TResult : IOperationResult
{

    public Task<TResult> HandleAsync(TRequest request, RequestHandlerDelegate<TResult> next, CancellationToken cancellationToken = default)
    {

        return Task.FromResult(this.CreateResult((int)HttpStatusCode.OK));
    }

    protected virtual TResult CreateResult(int resultCode)
    {
        Type responseType;
        if (typeof(TResult).IsGenericType) responseType = typeof(OperationResult<>).MakeGenericType(typeof(TResult).GetGenericArguments().First());
        else responseType = typeof(OperationResult);
        return (TResult)Activator.CreateInstance(responseType, resultCode, Array.Empty<Error>())!;
    }

}
