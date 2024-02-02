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

namespace Neuroglia.Data.Infrastructure.ResourceOriented;

/// <summary>
/// Defines extensions for <see cref="IResource"/>s
/// </summary>
public static class IResourceExtensions
{

    /// <summary>
    /// Gets the group the <see cref="IResource"/> belongs to
    /// </summary>
    /// <param name="resource">The extended <see cref="IResourceReference"/></param>
    /// <returns>The group the <see cref="IResource"/> belongs to</returns>
    public static string GetGroup(this IResource resource)
    {
        var components = resource.ApiVersion.Split('/');
        if (components.Length == 2) return components[0];
        else return string.Empty;
    }

    /// <summary>
    /// Gets the <see cref="IResource"/>'s version
    /// </summary>
    /// <param name="resource">The extended <see cref="IResource"/></param>
    /// <returns>The <see cref="IResource"/>'s version</returns>
    public static string GetVersion(this IResource resource)
    {
        var components = resource.ApiVersion.Split('/');
        if (components.Length == 2) return components[1];
        else return components[0];
    }

    /// <summary>
    /// Determines whether or not the <see cref="IResource"/> is namespaced
    /// </summary>
    /// <param name="resource">The <see cref="IResource"/> to check</param>
    /// <returns>A boolean indicating whether or not the <see cref="IResource"/> is namespaced</returns>
    public static bool IsNamespaced(this IResource resource) => !string.IsNullOrWhiteSpace(resource.GetNamespace());

    /// <summary>
    /// Gets the <see cref="IResource"/>'s name
    /// </summary>
    /// <param name="resource">The <see cref="IResource"/> to get the name of</param>
    /// <returns>The <see cref="IResource"/>'s name</returns>
    public static string GetName(this IResource resource) => resource.Metadata.Name!;

    /// <summary>
    /// Gets the <see cref="IResource"/>'s namespace
    /// </summary>
    /// <param name="resource">The <see cref="IResource"/> to get the namespace of</param>
    /// <returns>The <see cref="IResource"/>'s namespace</returns>
    public static string? GetNamespace(this IResource resource) => resource.Metadata.Namespace;

    /// <summary>
    /// Gets the <see cref="IResource"/>'s qualified name
    /// </summary>
    /// <param name="resource">The <see cref="IResource"/> to get the qualified name of</param>
    /// <returns>The <see cref="IResource"/>'s qualified name</returns>
    public static string GetQualifiedName(this IResource resource) => string.IsNullOrWhiteSpace(resource.GetNamespace()) ? resource.GetName() : $"{resource.GetName()}.{resource.GetNamespace()}";

    /// <summary>
    /// Determines whether or not the <see cref="IResource"/> is the specified type
    /// </summary>
    /// <param name="resource">The <see cref="IResource"/> to check</param>
    /// <param name="definition">The expected type of <see cref="IResource"/></param>
    /// <returns>A boolean indicating whether or not the <see cref="IResource"/> is of the specified type</returns>
    public static bool IsOfType(this IResource resource, ResourceDefinitionInfo definition)
    {
        if (resource.Definition != null) return resource.Definition == definition;
        ArgumentNullException.ThrowIfNull(definition);
        return resource.ApiVersion == definition.GetApiVersion() && resource.Kind == definition.Kind;
    }

    /// <summary>
    /// Determines whether or not the <see cref="IResource"/> is the specified type
    /// </summary>
    /// <typeparam name="TResource">The expected type of <see cref="IResource"/></typeparam>
    /// <param name="resource">The <see cref="IResource"/> to check</param>
    /// <returns>A boolean indicating whether or not the <see cref="IResource"/> is of the specified type</returns>
    public static bool IsOfType<TResource>(this IResource resource) where TResource : class, IResource, new() => resource.IsOfType(new TResource().Definition);

    /// <summary>
    /// Determines whether or not the <see cref="IResource"/> is an <see cref="IResourceDefinition"/>
    /// </summary>
    /// <param name="resource">The <see cref="IResource"/> to check</param>
    /// <returns>A boolean indicating whether or not the <see cref="IResource"/> is an <see cref="IResourceDefinition"/></returns>
    public static bool IsResourceDefinition(this IResource resource) => resource.GetGroup() == ResourceDefinition.ResourceGroup && resource.Kind == ResourceDefinition.ResourceKind;

}
