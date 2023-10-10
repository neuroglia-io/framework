using AutoMapper;

namespace Neuroglia.Mapping;

/// <summary>
/// Defines the fundamentals of a mapping configuration
/// </summary>
public interface IMappingConfiguration
{



}

/// <summary>
/// Defines the fundamentals of a mapping configuration
/// </summary>
/// <typeparam name="TSource">The type to map from</typeparam>
/// <typeparam name="TDestination">The type to map to</typeparam>
public interface IMappingConfiguration<TSource, TDestination>
    : IMappingConfiguration
{

    /// <summary>
    /// Configures the specified mapping
    /// </summary>
    /// <param name="mapping">The <see cref="IMappingConfiguration{TSource, TDestination}"/> to configure</param>
    void Configure(IMappingExpression<TSource, TDestination> mapping);

}
