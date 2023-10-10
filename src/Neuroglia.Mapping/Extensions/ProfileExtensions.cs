using System.Reflection;

namespace Neuroglia.Mapping;

/// <summary>
/// Defines extensions for <see cref="AutoMapper.Profile"/>s
/// </summary>
public static class ProfileExtensions
{

    private static readonly MethodInfo GenericApplyConfigurationMethod = typeof(ProfileExtensions)
        .GetMethods(BindingFlags.Default | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
        .Where(m => m.Name == nameof(ApplyConfiguration) && m.IsGenericMethod)
        .First();

    /// <summary>
    /// Applies the specified configuration
    /// </summary>
    /// <typeparam name="TSource">The type to map from</typeparam>
    /// <typeparam name="TDestination">The type to map to</typeparam>
    /// <param name="profile">The <see cref="AutoMapper.Profile"/> to configure</param>
    /// <param name="configuration">The <see cref="IMappingConfiguration{TSource, TDestination}"/> to apply</param>
    public static void ApplyConfiguration<TSource, TDestination>(this AutoMapper.Profile profile, IMappingConfiguration<TSource, TDestination> configuration)
    {
        configuration.Configure(profile.CreateMap<TSource, TDestination>());
    }

    /// <summary>
    /// Applies the specified <see cref="IMappingConfiguration"/>
    /// </summary>
    /// <param name="profile">The <see cref="AutoMapper.Profile"/> to configure</param>
    /// <param name="configuration">The <see cref="IMappingConfiguration"/> to apply</param>
    public static void ApplyConfiguration(this AutoMapper.Profile profile, IMappingConfiguration configuration)
    {
        foreach (var configurationType in configuration.GetType()
            .GetInterfaces()
            .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMappingConfiguration<,>)))
        {
            var sourceType = configurationType.GetGenericArguments()[0];
            var destinationType = configurationType.GetGenericArguments()[1];
            GenericApplyConfigurationMethod.MakeGenericMethod(sourceType, destinationType).Invoke(null, new object[] { profile, configuration });
        }
    }

}
