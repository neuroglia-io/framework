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
/// Represents a namespace
/// </summary>
[DataContract]
public record Namespace
    : Resource
{

    /// <summary>
    /// Gets the name of the default namespace
    /// </summary>
    public static string DefaultNamespaceName { get; set; } = "default";
    /// <summary>
    /// Gets/sets the group namespaces belong to
    /// </summary>
    public static string ResourceGroup => NamespaceDefinition.ResourceGroup;
    /// <summary>
    /// Gets/sets the resource version of namespaces
    /// </summary>
    public static string ResourceVersion => NamespaceDefinition.ResourceVersion;
    /// <summary>
    /// Gets/sets the plural name of namespaces
    /// </summary>
    public static string ResourcePlural => NamespaceDefinition.ResourcePlural;
    /// <summary>
    /// Gets/sets the kind of namespaces
    /// </summary>
    public static string ResourceKind => NamespaceDefinition.ResourceKind;

    /// <summary>
    /// Gets the <see cref="Namespace"/>'s resource type.
    /// </summary>
    public static readonly ResourceDefinitionInfo ResourceDefinition = new(ResourceGroup, ResourceVersion, ResourcePlural, ResourceKind);

    /// <summary>
    /// Initializes a new <see cref="Namespace"/>
    /// </summary>
    public Namespace() : base(ResourceDefinition) { }

    /// <summary>
    /// Initializes a new <see cref="Namespace"/>
    /// </summary>
    /// <param name="name">The name of the namespace to create</param>
    public Namespace(string name) : base(ResourceDefinition, new(name)) { }

}