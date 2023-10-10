namespace Neuroglia.Mapping;

/// <summary>
/// Represents the default implementation of the <see cref="IMappingOperationOptions{TSource, TDestination}"/> interface
/// </summary>
public class MappingOperationOptions
    : IMappingOperationOptions
{

    /// <summary>
    /// Initializes a new <see cref="MappingOperationOptions{TSource, TDestination}"/>
    /// </summary>
    public MappingOperationOptions()
    {
        this.Items = new Dictionary<string, string>();
    }

    /// <inheritdoc/>
    public IDictionary<string, string> Items { get; }

}

/// <summary>
/// Represents the default implementation of the <see cref="IMappingOperationOptions{TSource, TDestination}"/> interface
/// </summary>
/// <typeparam name="TSource">The type to map from</typeparam>
/// <typeparam name="TDestination">The type to map to</typeparam>
public class MappingOperationOptions<TSource, TDestination>
    : MappingOperationOptions, IMappingOperationOptions<TSource, TDestination>
{



}
