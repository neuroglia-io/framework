namespace Neuroglia.Mediation;

/// <summary>
/// Defines the fundamentals of a service used to handle queries
/// </summary>
public interface IQueryHandler
{



}

/// <summary>
/// Defines the fundamentals of a service used to handle queries
/// </summary>
/// <typeparam name="TQuery">The type of <see cref="IQuery"/> to handle</typeparam>
/// <typeparam name="T">The type of data wrapped by the <see cref="IOperationResult"/></typeparam>
public interface IQueryHandler<TQuery, T>
    : IQueryHandler, IRequestHandler<TQuery, IOperationResult<T>>
    where TQuery : class, IQuery<IOperationResult<T>>
{



}
