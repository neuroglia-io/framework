namespace Neuroglia.Mapping;

/// <summary>
/// Defines the fundamentals of options used by a single map operation
/// </summary>
public interface IMappingOperationOptions
{

    /// <summary>
    /// Gets an <see cref="IDictionary{TKey, TValue}"/> containing the context items to be accessed at map time
    /// </summary>
    IDictionary<string, string> Items { get; }

}

/// <summary>
/// Defines the fundamentals of options used by a single map operation
/// </summary>
/// <typeparam name="TSource">The type to map from</typeparam>
/// <typeparam name="TDestination">The type to map to</typeparam>
public interface IMappingOperationOptions<TSource, TDestination>
    : IMappingOperationOptions
{



}
