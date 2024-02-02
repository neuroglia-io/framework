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
/// Represents a reference to a resource definition
/// </summary>
[DataContract]
public record ResourceDefinitionReference
    : IResourceDefinitionReference
{

    /// <summary>
    /// Initializes a new <see cref="ResourceDefinitionReference"/>
    /// </summary>
    public ResourceDefinitionReference() { }

    /// <summary>
    /// Initializes a new <see cref="ResourceDefinitionReference"/>
    /// </summary>
    /// <param name="group">The API group resources of the described definition belong to</param>
    /// <param name="version">The version of the API group resources of the described definition belong to</param>
    /// <param name="plural">The plural name resources of the described definition belong to</param>
    public ResourceDefinitionReference(string group, string version, string plural)
    {
        if (string.IsNullOrWhiteSpace(version)) throw new ArgumentNullException(nameof(version));
        if (string.IsNullOrWhiteSpace(plural)) throw new ArgumentNullException(nameof(plural));
        this.Group = group;
        this.Version = version;
        this.Plural = plural;
    }

    /// <inheritdoc/>
    [Required]
    [DataMember(Order = 1, Name = "group", IsRequired = true), JsonPropertyOrder(1), JsonPropertyName("group"), YamlMember(Order = 1, Alias = "group")]
    public virtual string Group { get; set; } = null!;

    /// <inheritdoc/>
    [Required]
    [DataMember(Order = 2, Name = "version", IsRequired = true), JsonPropertyOrder(2), JsonPropertyName("version"), YamlMember(Order = 2, Alias = "version")]
    public virtual string Version { get; set; } = null!;

    /// <inheritdoc/>
    [Required]
    [DataMember(Order = 3, Name = "plural", IsRequired = true), JsonPropertyOrder(3), JsonPropertyName("plural"), YamlMember(Order = 3, Alias = "plural")]
    public virtual string Plural { get; set; } = null!;

    /// <inheritdoc/>
    public override string ToString() => $"{this.Group}/{this.Version}/{this.Plural}";

    /// <summary>
    /// Implicitly converts the <see cref="ResourceDefinition"/> into a new <see cref="ResourceDefinitionReference"/>
    /// </summary>
    /// <param name="definition">The <see cref="ResourceDefinition"/> to convert</param>
    public static implicit operator ResourceDefinitionReference(ResourceDefinition definition) => definition == null ? throw new ArgumentNullException(nameof(definition)) : new(definition.Spec.Group, definition.Spec.Versions.Last().Name, definition.Spec.Names.Plural);

}