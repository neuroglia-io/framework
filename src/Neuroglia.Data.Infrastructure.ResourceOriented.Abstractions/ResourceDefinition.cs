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

using Neuroglia.Serialization.Yaml;

namespace Neuroglia.Data.Infrastructure.ResourceOriented;

/// <summary>
/// Represents the definition of a resource type
/// </summary>
[DataContract]
public record ResourceDefinition
    : Resource<ResourceDefinitionSpec>, IResourceDefinition
{

    static ResourceDefinition()
    {
        using var stream = typeof(ResourceDefinition).Assembly.GetManifestResourceStream($"{typeof(ResourceDefinition).Namespace}.resource-definition.yaml")!;
        using var streamReader = new StreamReader(stream);
        Instance = YamlSerializer.Default.Deserialize<ResourceDefinition>(streamReader.ReadToEnd())!;
    }

    /// <summary>
    /// Gets/sets the group resource definitions belong to
    /// </summary>
    public static string ResourceGroup { get; set; } = DefaultResourceGroup;
    /// <summary>
    /// Gets/sets the resource version of resource definitions
    /// </summary>
    public static string ResourceVersion { get; set; } = "v1";
    /// <summary>
    /// Gets/sets the plural name of resource definitions
    /// </summary>
    public static string ResourcePlural { get; set; } = "resource-definitions";
    /// <summary>
    /// Gets/sets the kind of resource definitions
    /// </summary>
    public static string ResourceKind { get; set; } = "ResourceDefinition";

    /// <summary>
    /// Gets the definition of <see cref="ResourceDefinition"/>s
    /// </summary>
    public static ResourceDefinition Instance { get; set; }

    /// <summary>
    /// Initializes a new <see cref="ResourceDefinition"/>
    /// </summary>
    public ResourceDefinition() : base(new ResourceDefinitionInfo(ResourceGroup, ResourceVersion, ResourcePlural, ResourceKind)) { }

    /// <summary>
    /// Initializes a new <see cref="ResourceDefinition"/>
    /// </summary>
    /// <param name="spec">The resource definition's spec</param>
    public ResourceDefinition(ResourceDefinitionSpec spec) : base(new(ResourceGroup, ResourceVersion, ResourcePlural, ResourceKind), new($"{spec.Names.Plural}.{spec.Group}"), spec) { }

}
