using System.Collections;

namespace Neuroglia.Mapping;

/// <summary>
/// Defines the fundamentals of a service used to map objects
/// </summary>
public interface IMapper
{

    /// <summary>
    /// Maps an object to the specified destination type
    /// </summary>
    /// <typeparam name="TDestination">The type to map the source to</typeparam>
    /// <param name="source">The object to map</param>
    /// <returns>The mapped destination object</returns>
    TDestination Map<TDestination>(object source);

    /// <summary>
    /// Maps an object to the specified destination type
    /// </summary>
    /// <typeparam name="TDestination">The type to map the source to</typeparam>
    /// <param name="source">The object to map</param>
    /// <param name="configuration">An <see cref="Action{T}"/> used to configure the <see cref="IMappingOperationOptions{TSource, TDestination}"/></param>
    /// <returns>The mapped destination object</returns>
    TDestination Map<TDestination>(object source, Action<IMappingOperationOptions> configuration);

    /// <summary>
    /// Maps an object to the specified destination type
    /// </summary>
    /// <typeparam name="TSource">The type to map from</typeparam>
    /// <typeparam name="TDestination">The type to map the source to</typeparam>
    /// <param name="source">The object to map</param>
    /// <returns>The mapped destination object</returns>
    TDestination Map<TSource, TDestination>(TSource source);

    /// <summary>
    /// Maps an object to the specified destination type
    /// </summary>
    /// <typeparam name="TSource">The type to map from</typeparam>
    /// <typeparam name="TDestination">The type to map the source to</typeparam>
    /// <param name="source">The object to map</param>
    /// <param name="configuration">An <see cref="Action{T}"/> used to configure the <see cref="IMappingOperationOptions{TSource, TDestination}"/></param>
    /// <returns>The mapped destination object</returns>
    TDestination Map<TSource, TDestination>(TSource source, Action<IMappingOperationOptions<TSource, TDestination>> configuration);

    /// <summary>
    /// Maps an object to the specified destination type
    /// </summary>
    /// <typeparam name="TSource">The type to map from</typeparam>
    /// <typeparam name="TDestination">The type to map the source to</typeparam>
    /// <param name="source">The object to map</param>
    /// <param name="destination">The object to map the source to</param>
    /// <returns>The mapped destination object</returns>
    TDestination Map<TSource, TDestination>(TSource source, TDestination destination);

    /// <summary>
    /// Maps an object to the specified destination type
    /// </summary>
    /// <typeparam name="TSource">The type to map from</typeparam>
    /// <typeparam name="TDestination">The type to map the source to</typeparam>
    /// <param name="source">The object to map</param>
    /// <param name="destination">The object to map the source to</param>
    /// <param name="configuration">An <see cref="Action{T}"/> used to configure the <see cref="IMappingOperationOptions{TSource, TDestination}"/></param>
    /// <returns>The mapped destination object</returns>
    TDestination Map<TSource, TDestination>(TSource source, TDestination destination, Action<IMappingOperationOptions<TSource, TDestination>> configuration);

    /// <summary>
    /// Maps an object to the specified destination type
    /// </summary>
    /// <param name="source">The object to map</param>
    /// <param name="sourceType">The type to map from</param>
    /// <param name="destinationType">The type to map to</param>
    /// <returns>The mapped destination object</returns>
    object Map(object source, Type sourceType, Type destinationType);

    /// <summary>
    /// Maps an object to the specified destination type
    /// </summary>
    /// <param name="source">The object to map</param>
    /// <param name="sourceType">The type to map from</param>
    /// <param name="destinationType">The type to map to</param>
    /// <param name="configuration">An <see cref="Action{T}"/> used to configure the <see cref="IMappingOperationOptions{TSource, TDestination}"/></param>
    /// <returns>The mapped destination object</returns>
    object Map(object source, Type sourceType, Type destinationType, Action<IMappingOperationOptions> configuration);

    /// <summary>
    /// Projects the elements of an <see cref="IEnumerable"/> to the specified destination type
    /// </summary>
    /// <typeparam name="TDestination">The type to map the elements to</typeparam>
    /// <param name="source">The <see cref="IEnumerable"/> to map the elements for</param>
    /// <returns>A new <see cref="IEnumerable"/> containing the mapped elements</returns>
    IEnumerable<TDestination> ProjectTo<TDestination>(IEnumerable source);

    /// <summary>
    /// Projects the elements of an <see cref="IEnumerable"/> to the specified destination type
    /// </summary>
    /// <typeparam name="TDestination">The type to map the elements to</typeparam>
    /// <param name="source">The <see cref="IEnumerable"/> to map the elements for</param>
    /// <param name="configuration">An <see cref="Action{T}"/> used to configure the <see cref="IMappingOperationOptions{TSource, TDestination}"/></param>
    /// <returns>A new <see cref="IEnumerable"/> containing the mapped elements</returns>
    IEnumerable<TDestination> ProjectTo<TDestination>(IEnumerable source, Action<IMappingOperationOptions> configuration);

    /// <summary>
    /// Projects the elements of an <see cref="IEnumerable"/> to the specified destination type
    /// </summary>
    /// <typeparam name="TSource">The type to map from</typeparam>
    /// <typeparam name="TDestination">The type to map the elements to</typeparam>
    /// <param name="source">The <see cref="IEnumerable"/> to map the elements for</param>
    /// <returns>A new <see cref="IEnumerable"/> containing the mapped elements</returns>
    IEnumerable<TDestination> ProjectTo<TSource, TDestination>(IEnumerable<TSource> source);

    /// <summary>
    /// Projects the elements of an <see cref="IEnumerable"/> to the specified destination type
    /// </summary>
    /// <typeparam name="TSource">The type to map from</typeparam>
    /// <typeparam name="TDestination">The type to map the elements to</typeparam>
    /// <param name="source">The <see cref="IEnumerable"/> to map the elements for</param>
    /// <param name="configuration">An <see cref="Action{T}"/> used to configure the <see cref="IMappingOperationOptions{TSource, TDestination}"/></param>
    /// <returns>A new <see cref="IEnumerable"/> containing the mapped elements</returns>
    IEnumerable<TDestination> ProjectTo<TSource, TDestination>(IEnumerable<TSource> source, Action<IMappingOperationOptions<TSource, TDestination>> configuration);

    /// <summary>
    /// Projects the elements of an <see cref="IEnumerable"/> to the specified destination type
    /// </summary>
    /// <param name="source">The <see cref="IEnumerable"/> to map the elements for</param>
    /// <param name="sourceType">The type to map from</param>
    /// <param name="destinationType">The type to map to</param>
    /// <returns>A new <see cref="IEnumerable"/> containing the mapped elements</returns>
    IEnumerable<object> ProjectTo(IEnumerable source, Type sourceType, Type destinationType);

    /// <summary>
    /// Projects the elements of an <see cref="IEnumerable"/> to the specified destination type
    /// </summary>
    /// <param name="source">The <see cref="IEnumerable"/> to map the elements for</param>
    /// <param name="sourceType">The type to map from</param>
    /// <param name="destinationType">The type to map to</param>
    /// <param name="configuration">An <see cref="Action{T}"/> used to configure the <see cref="IMappingOperationOptions{TSource, TDestination}"/></param>
    /// <returns>A new <see cref="IEnumerable"/> containing the mapped elements</returns>
    IEnumerable<object> ProjectTo(IEnumerable source, Type sourceType, Type destinationType, Action<IMappingOperationOptions> configuration);

}
