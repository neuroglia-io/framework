// Copyright © 2021-Present Neuroglia SRL. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License"),
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Neuroglia.Mapping;

/// <summary>
/// Defines extensions for <see cref="AutoMapper.Profile"/>s
/// </summary>
public static class ProfileExtensions
{

    static readonly MethodInfo GenericApplyConfigurationMethod = typeof(ProfileExtensions).GetMethods(BindingFlags.Default | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static).Where(m => m.Name == nameof(ApplyConfiguration) && m.IsGenericMethod).First();

    /// <summary>
    /// Applies the specified configuration
    /// </summary>
    /// <typeparam name="TSource">The type to map from</typeparam>
    /// <typeparam name="TDestination">The type to map to</typeparam>
    /// <param name="profile">The <see cref="AutoMapper.Profile"/> to configure</param>
    /// <param name="configuration">The <see cref="IMappingConfiguration{TSource, TDestination}"/> to apply</param>
    public static void ApplyConfiguration<TSource, TDestination>(this AutoMapper.Profile profile, IMappingConfiguration<TSource, TDestination> configuration) => configuration.Configure(profile.CreateMap<TSource, TDestination>());

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

    /// <summary>
    /// Applies all <see cref="IMappingConfiguration"/>s found in the specified assemblies and registers all type maps configured using <see cref="MapAttribute"/>s
    /// </summary>
    /// <param name="profile">The extended <see cref="AutoMapper.Profile"/></param>
    /// <param name="serviceProvider">The current <see cref="IServiceProvider"/></param>
    /// <param name="assemblies">An array containing the assemblies to search for <see cref="IMappingConfiguration"/>s. If null or empty, will search the calling <see cref="Assembly"/></param>
    public static void Configure(this AutoMapper.Profile profile, IServiceProvider serviceProvider, params Assembly[] assemblies)
    {
        if(assemblies == null || !assemblies.Any()) assemblies = new Assembly[] { Assembly.GetCallingAssembly() };
        var types = assemblies.SelectMany(a => a.GetTypes());
        foreach(var mappingConfigurationType in types.Where(t => !t.IsAbstract && !t.IsInterface && t.IsClass && typeof(IMappingConfiguration).IsAssignableFrom(t)))
        {
            profile.ApplyConfiguration((IMappingConfiguration)ActivatorUtilities.CreateInstance(serviceProvider, mappingConfigurationType));
        }
        foreach(var sourceType in types.Where(t => t.TryGetCustomAttribute<MapAttribute>(out _)))
        {
            var mapAttribute = sourceType.GetCustomAttribute<MapAttribute>()!;
            profile.CreateMap(sourceType, mapAttribute.DestinationType);
        }
    }

}
