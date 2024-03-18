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
/// Defines extensions for <see cref="IResourceDefinition"/>s
/// </summary>
public static class IResourceDefinitionExtensions
{

    /// <summary>
    /// Attempts to get the specified <see cref="ResourceDefinitionVersion"/>
    /// </summary>
    /// <param name="resourceDefinition">The extended <see cref="IResourceDefinition"/></param>
    /// <param name="version">The version to get</param>
    /// <param name="resourceDefinitionVersion">The specified <see cref="ResourceDefinitionVersion"/>, if any</param>
    /// <returns>A boolean indicating whether or not the specified <see cref="ResourceDefinitionVersion"/> exists</returns>
    public static bool TryGetVersion(this IResourceDefinition resourceDefinition, string version, out ResourceDefinitionVersion? resourceDefinitionVersion)
    {
        if (string.IsNullOrWhiteSpace(version)) throw new ArgumentNullException(nameof(version));
        resourceDefinitionVersion = resourceDefinition.Spec.Versions.SingleOrDefault(v => v.Name.Equals(version, StringComparison.Ordinal));
        return resourceDefinitionVersion != null;
    }

    /// <summary>
    /// Gets the <see cref="IResourceDefinition"/>'s storage version 
    /// </summary>
    /// <param name="resourceDefinition">The <see cref="IResourceDefinition"/> to get the storage version of</param>
    /// <returns>The <see cref="ResourceDefinitionVersion"/> used for storage</returns>
    public static ResourceDefinitionVersion GetStorageVersion(this IResourceDefinition resourceDefinition) => 
        resourceDefinition.Spec.Versions.SingleOrDefault(v => v.Storage) 
        ?? throw new Exception($"Failed to find the storage version of resource definition '{resourceDefinition}'");

}