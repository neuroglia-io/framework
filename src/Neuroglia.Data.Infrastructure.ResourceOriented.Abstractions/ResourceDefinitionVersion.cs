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
/// Represents an object used to describe a version of a resource definition
/// </summary>
[DataContract]
public record ResourceDefinitionVersion
{

    /// <summary>
    /// Initializes a new <see cref="ResourceDefinitionVersion"/>
    /// </summary>
    public ResourceDefinitionVersion() { }

    /// <summary>
    /// Initializes a new <see cref="ResourceDefinitionVersion"/>
    /// </summary>
    /// <param name="name">The name of the described resource definition version</param>
    /// <param name="schema">The schema to validate defined resources</param>
    public ResourceDefinitionVersion(string name, ResourceDefinitionValidation schema)
    {
        if(string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
        this.Name = name;
        this.Schema = schema ?? throw new ArgumentNullException(nameof(schema));
    }

    /// <summary>
    /// Gets/sets the name of the described resource definition version
    /// </summary>
    [Required]
    [DataMember(Order = 1, Name = "name", IsRequired = true), JsonPropertyOrder(1), JsonPropertyName("name"), YamlMember(Order = 1, Alias = "name")]
    public virtual string Name { get; set; } = null!;

    /// <summary>
    /// Gets/sets the schema to validate defined resources
    /// </summary>
    [Required]
    [DataMember(Order = 2, Name = "schema", IsRequired = true), JsonPropertyOrder(2), JsonPropertyName("schema"), YamlMember(Order = 2, Alias = "schema")]
    public virtual ResourceDefinitionValidation Schema { get; set; } = null!;

    /// <summary>
    /// Gets/sets a boolean indicating whether or not the version is served by the API
    /// </summary>
    [DataMember(Order = 3, Name = "served", IsRequired = true), JsonPropertyOrder(3), JsonPropertyName("served"), YamlMember(Order = 3, Alias = "served")]
    public virtual bool Served { get; set; }

    /// <summary>
    /// Gets/sets a boolean indicating whether or not the version is the storage version
    /// </summary>
    [DataMember(Order = 4, Name = "storage", IsRequired = true), JsonPropertyOrder(4), JsonPropertyName("storage"), YamlMember(Order = 4, Alias = "storage")]
    public virtual bool Storage { get; set; }

    /// <summary>
    /// Gets/sets a boolean indicating whether or not the version is the storage version
    /// </summary>
    [DataMember(Order = 5, Name = "subresources", IsRequired = true), JsonPropertyOrder(5), JsonPropertyName("subresources"), YamlMember(Order = 5, Alias = "subresources")]
    public virtual IDictionary<string, object>? SubResources { get; set; }

}
