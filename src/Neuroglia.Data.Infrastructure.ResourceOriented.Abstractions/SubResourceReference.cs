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

using Neuroglia.Data.Guards;

namespace Neuroglia.Data.Infrastructure.ResourceOriented;

/// <summary>
/// Represents an object used to reference a sub resource
/// </summary>
[DataContract]
public record SubResourceReference
    : ResourceReference, ISubResourceReference
{

    /// <summary>
    /// Initializes a new <see cref="SubResourceReference"/>
    /// </summary>
    public SubResourceReference() { }

    /// <summary>
    /// Initializes a new <see cref="SubResourceReference"/>
    /// </summary>
    /// <param name="definition">The referenced resource's definition</param>
    /// <param name="name">The name of the referenced resource</param>
    /// <param name="subResource">The name of the referenced sub resource</param>
    /// <param name="namespace">The namespace the referenced resource belongs to, if any</param>
    public SubResourceReference(ResourceDefinitionReference definition, string name, string subResource, string? @namespace = null)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
        if (string.IsNullOrWhiteSpace(subResource)) throw new ArgumentNullException(nameof(subResource));
        this.Definition = Guard.AgainstArgument(definition).WhenNull().Value!;
        this.Name = Guard.AgainstArgument(name).WhenNullOrWhitespace().Value!;
        this.SubResource = Guard.AgainstArgument(subResource).WhenNullOrWhitespace().Value!;
        this.Namespace = @namespace;
    }

    /// <inheritdoc/>
    [Required]
    [DataMember(Order = 1, Name = "subResource", IsRequired = true), JsonPropertyOrder(1), JsonPropertyName("subResource"), YamlMember(Order = 1, Alias = "subResource")]
    public virtual string SubResource { get; set; } = null!;

    /// <inheritdoc/>
    public override string ToString() => $"{base.ToString()}/{this.SubResource}";

}