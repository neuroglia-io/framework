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
/// Represents the definition of resource namespaces
/// </summary>
[DataContract]
public record NamespaceDefinition
    : ResourceDefinition
{

    /// <summary>
    /// Gets/sets the group namespaces belong to
    /// </summary>
    public new static string ResourceGroup { get; set; } = DefaultResourceGroup;
    /// <summary>
    /// Gets/sets the resource version of namespaces
    /// </summary>
    public new static string ResourceVersion { get; set; } = "v1";
    /// <summary>
    /// Gets/sets the singular name of namespaces
    /// </summary>
    public static string ResourceSingular { get; set; } = "namespace";
    /// <summary>
    /// Gets/sets the plural name of namespaces
    /// </summary>
    public new static string ResourcePlural { get; set; } = "namespaces";
    /// <summary>
    /// Gets/sets the kind of namespaces
    /// </summary>
    public new static string ResourceKind { get; set; } = "Namespace";
    /// <summary>
    /// Gets/sets the short names of namespaces
    /// </summary>
    public static HashSet<string> ResourceShortNames { get; set; } = ["n", "ns"];

    /// <summary>
    /// Initializes a new <see cref="NamespaceDefinition"/>
    /// </summary>
    public NamespaceDefinition() : base(new ResourceDefinitionSpec(ResourceScope.Cluster, ResourceGroup, new(ResourceSingular, ResourcePlural, ResourceKind, ResourceShortNames), new ResourceDefinitionVersion(ResourceVersion, new()) { Served = true, Storage = true })) { }

}
