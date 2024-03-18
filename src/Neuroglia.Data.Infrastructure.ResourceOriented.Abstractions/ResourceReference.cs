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
/// Represents an object used to reference a resource
/// </summary>
[DataContract]
public record ResourceReference
    : IResourceReference
{

    /// <summary>
    /// Initializes a new <see cref="ResourceReference"/>
    /// </summary>
    public ResourceReference() { }

    /// <summary>
    /// Initializes a new <see cref="ResourceReference"/>
    /// </summary>
    /// <param name="definition">The referenced resource's definition</param>
    /// <param name="name">The name of the referenced resource</param>
    /// <param name="namespace">The namespace the referenced resource belongs to, if any</param>
    public ResourceReference(ResourceDefinitionReference definition, string name, string? @namespace = null)
    {
        if(string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
        this.Definition = definition ?? throw new ArgumentNullException(nameof(definition));
        this.Name = name;
        this.Namespace = @namespace;
    }

    /// <inheritdoc/>
    [Required]
    [DataMember(Order = 1, Name = "uid", IsRequired = true), JsonPropertyOrder(1), JsonPropertyName("uid"), YamlMember(Order = 1, Alias = "uid")]
    public virtual ResourceDefinitionReference Definition { get; set; } = null!;

    /// <inheritdoc/>
    [Required]
    [DataMember(Order = 2, Name = "name", IsRequired = true), JsonPropertyOrder(2), JsonPropertyName("name"), YamlMember(Order = 2, Alias = "name")]
    public virtual string Name { get; set; } = null!;

    /// <inheritdoc/>
    [DataMember(Order = 3, Name = "namespace"), JsonPropertyOrder(3), JsonPropertyName("namespace"), YamlMember(Order = 3, Alias = "namespace")]
    public virtual string? Namespace { get; set; }

    IResourceDefinitionReference IResourceReference.Definition => this.Definition;

    /// <inheritdoc/>
    public override string ToString() => string.IsNullOrWhiteSpace(this.Namespace) ? $"{this.Definition}/{this.Name}" : $"{this.Definition.Group}/{this.Definition.Version}/namespaces/{this.Namespace}/{this.Definition.Plural}/{this.Name}";

}

/// <summary>
/// Represents an object used to reference a resource
/// </summary>
/// <typeparam name="TResource">The type of the referenced resource</typeparam>
[DataContract]
public record ResourceReference<TResource>
    : ResourceReference
    where TResource : class, IResource, new()
{

    /// <summary>
    /// Initializes a new <see cref="ResourceReference{TResource}"/>
    /// </summary>
    public ResourceReference() { }

    /// <summary>
    /// Initializes a new <see cref="ResourceReference{TResource}"/>
    /// </summary>
    /// <param name="name">The name of the referenced resource</param>
    /// <param name="namespace">The namespace the referenced resource belongs to, if any</param>
    public ResourceReference(string name, string? @namespace = null)
        : base(DefinitionReference, name, @namespace)
    {

    }

    /// <summary>
    /// Gets a new <see cref="ResourceDefinitionReference"/> that describes the referenced resource's definition
    /// </summary>
    public static ResourceDefinitionReference DefinitionReference
    {
        get
        {
            var resource = new TResource();
            return new(resource.Definition.Group, resource.Definition.Version, resource.Definition.Plural);
        }
    }

}