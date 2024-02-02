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
/// Describes the definition of a resource
/// </summary>
[DataContract]
public record ResourceDefinitionInfo
{

    /// <summary>
    /// Initializes a new <see cref="ResourceDefinitionInfo"/>
    /// </summary>
    public ResourceDefinitionInfo() { }

    /// <summary>
    /// Initializes a new <see cref="ResourceDefinitionInfo"/>
    /// </summary>
    /// <param name="group">The API group the resource definition belongs to</param>
    /// <param name="version">The resource definition's version</param>
    /// <param name="plural">The resource definition's plural name</param>
    /// <param name="kind">The resource definition's kind</param>
    public ResourceDefinitionInfo(string group, string version, string plural, string kind)
    {
        if (string.IsNullOrWhiteSpace(version)) throw new ArgumentNullException(nameof(version));
        if (string.IsNullOrWhiteSpace(plural)) throw new ArgumentNullException(nameof(plural));
        if (string.IsNullOrWhiteSpace(kind)) throw new ArgumentNullException(nameof(kind));
        this.Group = group;
        this.Version = version;
        this.Plural = plural;
        this.Kind = kind;
    }

    /// <summary>
    /// Gets/sets the group the resource definition belongs to
    /// </summary>
    [DataMember(Order = 1, Name = "group"), JsonPropertyOrder(1), JsonPropertyName("group"), YamlMember(Order = 1, Alias = "group")]
    public virtual string Group { get; set; } = null!;

    /// <summary>
    /// Gets/sets resource definition's version
    /// </summary>
    [DataMember(Order = 2, Name = "version"), JsonPropertyOrder(2), JsonPropertyName("version"), YamlMember(Order = 2, Alias = "version")]
    public virtual string Version { get; set; } = null!;

    /// <summary>
    /// Gets/sets the resource definition's plural name
    /// </summary>
    [DataMember(Order = 3, Name = "plural"), JsonPropertyOrder(3), JsonPropertyName("plural"), YamlMember(Order = 3, Alias = "plural")]
    public virtual string Plural { get; set; } = null!;

    /// <summary>
    /// Gets/sets the resource kind
    /// </summary>
    [DataMember(Order = 4, Name = "kind"), JsonPropertyOrder(4), JsonPropertyName("kind"), YamlMember(Order = 4, Alias = "kind")]
    public virtual string Kind { get; set; } = null!;

    /// <inheritdoc/>
    public override string ToString() => string.IsNullOrWhiteSpace(this.Group) ? this.Plural : $"{this.Plural}.{this.Group}";

    /// <summary>
    /// Implicitly converts the specified <see cref="ResourceDefinition"/> into a new <see cref="ResourceDefinitionInfo"/>
    /// </summary>
    /// <param name="resourceDefinition">The <see cref="ResourceDefinition"/> to convert</param>
    public static implicit operator ResourceDefinitionInfo?(ResourceDefinition? resourceDefinition)
    {
        if (resourceDefinition == null) return null;
        return new(resourceDefinition.Spec.Group, resourceDefinition.Spec.Versions.Last().Name, resourceDefinition.Spec.Names.Plural, resourceDefinition.Spec.Names.Kind);
    }

}
