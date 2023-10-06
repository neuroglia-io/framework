namespace Neuroglia.Mediation;

/// <summary>
/// Represents the default implementation of the <see cref="IQuery{TResult}"/> interface
/// </summary>
/// <typeparam name="T">The type of result wrapped by the resulting <see cref="IOperationResult{T}"/></typeparam>
public abstract class Query<T>
   : IQuery<IOperationResult<T>, T>
{



}
